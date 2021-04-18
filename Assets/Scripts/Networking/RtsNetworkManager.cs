using System;
using Mirror;
using Rts.Buildings;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Rts.Networking
{
    public class RtsNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject unitSpawnerPrefab;
        [SerializeField] private GameOverHandler gameOverHandlerPrefab;

        public static event Action ClientOnConnected;
        public static event Action ClientOnDisconnected;

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

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            var player = conn.identity.GetComponent<RtsPlayer>();
            
            player.SetTeamColor(new Color(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f)));
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (!SceneManager.GetActiveScene().name.StartsWith("Scene_Map")) return;
            
            var gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
                
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
        }

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