using System;
using System.Collections.Generic;
using Mirror;

namespace Rts.Buildings
{
    public class GameOverHandler : NetworkBehaviour
    {
        public static event Action ServerOnGameOver;
        public static event Action<string> ClientOnGameOver;
        
        private List<UnitBase> bases = new List<UnitBase>();
        
        #region Server

        public override void OnStartServer()
        {
            UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
            UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
        }

        public override void OnStopServer()
        {
            UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
            UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
        }

        [Server]
        private void ServerHandleBaseSpawned(UnitBase unitBase)
        {
            bases.Add(unitBase);
        }
        
        [Server]
        private void ServerHandleBaseDespawned(UnitBase unitBase)
        {
            bases.Remove(unitBase);

            if (bases.Count != 1) return;

            var playerId = bases[0].connectionToClient.connectionId;
            
            RpcGameOver($"Player {playerId}");
            
            OnServerOnGameOver();
        }

        #endregion

        #region Client

        [ClientRpc]
        private void RpcGameOver(string winner)
        {
            OnClientOnGameOver(winner);
        }

        #endregion

        private static void OnClientOnGameOver(string obj)
        {
            ClientOnGameOver?.Invoke(obj);
        }

        private static void OnServerOnGameOver()
        {
            ServerOnGameOver?.Invoke();
        }
    }
}