using System;
using System.Collections.Generic;
using Mirror;
using Rts.Buildings;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Rts.Networking
{
    public class RtsNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject unitBasePrefab;
        [SerializeField] private GameOverHandler gameOverHandlerPrefab;

        public static event Action ClientOnConnected;
        public static event Action ClientOnDisconnected;

        private bool _isGameInProgress;

        public List<RtsPlayer> Players { get; } = new List<RtsPlayer>();

        #region Server

        public override void OnServerConnect(NetworkConnection conn)
        {
            if(!_isGameInProgress) return;
            
            conn.Disconnect();
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            var player = conn.identity.GetComponent<RtsPlayer>();

            Players.Remove(player);
            
            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            Players.Clear();

            _isGameInProgress = false;
        }

        public void StartGame()
        {
            if(Players.Count < 2) return;

            _isGameInProgress = true;

            ServerChangeScene("Scene_Map_01");
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            var player = conn.identity.GetComponent<RtsPlayer>();
            
            Players.Add(player);
            
            player.SetTeamColor(new Color(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f)));
            
            player.SetPartyOwner(Players.Count == 1);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (!SceneManager.GetActiveScene().name.StartsWith("Scene_Map")) return;
            
            var gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
                
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

            foreach (var player in Players)
            {
                var baseInstance = Instantiate(unitBasePrefab, GetStartPosition().position, Quaternion.identity);
                
                NetworkServer.Spawn(baseInstance, player.connectionToClient);
            }
        }

        #endregion

        #region Client
        
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            
            OnClientOnConnected();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            
            OnClientOnDisconnected();
        }

        public override void OnStopClient()
        {
            Players.Clear();
        }

        #endregion

        private static void OnClientOnConnected()
        {
            ClientOnConnected?.Invoke();
        }

        private static void OnClientOnDisconnected()
        {
            ClientOnDisconnected?.Invoke();
        }
    }
}