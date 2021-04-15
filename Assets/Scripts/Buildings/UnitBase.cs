using System;
using Mirror;
using Rts.Combat;
using UnityEngine;

namespace Rts.Buildings
{
    public class UnitBase : NetworkBehaviour
    {
        [SerializeField] private Health health;

        public static event Action<int> ServerOnPlayerDie;
        public static event Action<UnitBase> ServerOnBaseSpawned;
        public static event Action<UnitBase> ServerOnBaseDespawned;

        #region Server

        public override void OnStartServer()
        {
            health.ServerOnDie += ServerHandleDie;
            
            OnServerOnBaseSpawned(this);
        }

        public override void OnStopServer()
        {
            OnServerOnBaseDespawned(this);
            
            health.ServerOnDie -= ServerHandleDie;
        }

        [Server]
        private void ServerHandleDie()
        {
            OnServerOnPlayerDie(connectionToClient.connectionId);
            
            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #region Client

        

        #endregion

        private static void OnServerOnBaseSpawned(UnitBase unitBase)
        {
            ServerOnBaseSpawned?.Invoke(unitBase);
        }

        private static void OnServerOnBaseDespawned(UnitBase unitBase)
        {
            ServerOnBaseDespawned?.Invoke(unitBase);
        }

        private static void OnServerOnPlayerDie(int obj)
        {
            ServerOnPlayerDie?.Invoke(obj);
        }
    }
}