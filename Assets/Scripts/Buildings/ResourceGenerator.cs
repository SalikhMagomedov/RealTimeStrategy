using System;
using Mirror;
using Rts.Combat;
using Rts.Networking;
using UnityEngine;

namespace Rts.Buildings
{
    public class ResourceGenerator : NetworkBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private int resourcesPerInterval = 10;
        [SerializeField] private float interval = 2f;

        private float _timer;
        private RtsPlayer _player;

        public override void OnStartServer()
        {
            _timer = interval;
            _player = connectionToClient.identity.GetComponent<RtsPlayer>();

            health.ServerOnDie += ServerHandleDie;
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= ServerHandleDie;
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

        [ServerCallback]
        private void Update()
        {
            _timer -= Time.deltaTime;

            if (!(_timer <= 0)) return;
            
            _player.SetResources(_player.Resources + resourcesPerInterval);
                
            _timer += interval;
        }

        private void ServerHandleDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        private void ServerHandleGameOver()
        {
            enabled = false;
        }
    }
}