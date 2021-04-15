using System;
using Mirror;
using Rts.Buildings;
using UnityEngine;

namespace Rts.Combat
{
    public class Health : NetworkBehaviour
    {
        [SerializeField] private int maxHealth = 100;

        [SyncVar(hook = nameof(HandleHealthUpdated))]
        private int _currentHealth;

        public event Action ServerOnDie;
        public event Action<int, int> ClientOnHealthUpdated;

        #region Server

        public override void OnStartServer()
        {
            _currentHealth = maxHealth;
            UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
        }

        public override void OnStopServer()
        {
            UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
        }

        [Server]
        public void DealDamage(int damageAmount)
        {
            if (_currentHealth == 0) return;

            _currentHealth = Mathf.Max(_currentHealth - damageAmount, 0);
            
            if (_currentHealth != 0) return;
            
            OnServerOnDie();

            Debug.Log("We Died");
        }

        [Server]
        private void ServerHandlePlayerDie(int obj)
        {
            if (connectionToClient.connectionId != obj) return;
            
            DealDamage(_currentHealth);
        }

        #endregion

        #region Client

        private void HandleHealthUpdated(int oldHealth, int newHealth)
        {
            OnClientOnHealthUpdated(newHealth, maxHealth);
        }

        #endregion

        protected virtual void OnServerOnDie()
        {
            ServerOnDie?.Invoke();
        }

        protected virtual void OnClientOnHealthUpdated(int newHealth, int max)
        {
            ClientOnHealthUpdated?.Invoke(newHealth, max);
        }
    }
}