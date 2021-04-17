using Mirror;
using Rts.Combat;
using Rts.Networking;
using Rts.Units;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rts.Buildings
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField] private Health health;
        [SerializeField] private Unit unitPrefab;
        [SerializeField] private Transform unitSpawnPoint;
        [SerializeField] private TMP_Text remainingUnitsText;
        [SerializeField] private Image unitProgressImage;
        [SerializeField] private int maxUnitQueue = 5;
        [SerializeField] private float spawnMoveRange = 7;
        [SerializeField] private float unitSpawnDuration = 5f;

        [SyncVar(hook = nameof(ClientHandleQueuedUnitUpdated))]
        private int _queuedUnits;
        [SyncVar]
        private float _unitTimer;
        
        private float _progressImageVelocity;

        private void Update()
        {
            if (isServer)
            {
                ProduceUnits();
            }

            if (isClient)
            {
                UpdateTimerDisplay();
            }
        }

        #region Server

        public override void OnStartServer()
        {
            health.ServerOnDie += ServerHandleDie;
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= ServerHandleDie;
        }

        [Server]
        private void ProduceUnits()
        {
            if (_queuedUnits == 0) return;

            _unitTimer += Time.deltaTime;
            
            if(_unitTimer < unitSpawnDuration) return;

            var position = unitSpawnPoint.position;
            var unitInstance = Instantiate(unitPrefab.gameObject, position, unitSpawnPoint.rotation);
            
            NetworkServer.Spawn(unitInstance, connectionToClient);

            var spawnOffset = Random.insideUnitSphere * spawnMoveRange;

            spawnOffset.y = position.y;

            var unitMovement = unitInstance.GetComponent<UnitMovement>();
            
            unitMovement.ServerMove(position + spawnOffset);

            _queuedUnits--;
            _unitTimer = 0;
        }

        [Server]
        private void ServerHandleDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        [Command]
        private void CmdSpawnUnit()
        {
            if (_queuedUnits == maxUnitQueue) return;

            var player = connectionToClient.identity.GetComponent<RtsPlayer>();
            
            if (player.Resources < unitPrefab.ResourceCost) return;

            _queuedUnits++;
            
            player.SetResources(player.Resources - unitPrefab.ResourceCost);
        }

        private void ClientHandleQueuedUnitUpdated(int oldValue, int newValue)
        {
            remainingUnitsText.text = newValue.ToString();
        }

        #endregion

        #region Client

        private void UpdateTimerDisplay()
        {
            var newProgress = _unitTimer / unitSpawnDuration;

            if (newProgress < unitProgressImage.fillAmount)
            {
                unitProgressImage.fillAmount = newProgress;
            }
            else
            {
                unitProgressImage.fillAmount = Mathf.SmoothDamp(unitProgressImage.fillAmount, newProgress,
                    ref _progressImageVelocity, .1f);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (!hasAuthority) return;
            
            CmdSpawnUnit();
        }

        #endregion
    }
}