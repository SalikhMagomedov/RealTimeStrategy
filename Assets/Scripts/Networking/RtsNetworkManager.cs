using Mirror;
using Rts.Buildings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rts.Networking
{
    public class RtsNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject unitSpawnerPrefab;
        [SerializeField] private GameOverHandler gameOverHandlerPrefab;
        
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            var connectionTransform = conn.identity.transform;
            var unitSpawnerInstance = Instantiate(unitSpawnerPrefab,
                connectionTransform.position,
                connectionTransform.rotation);
            
            NetworkServer.Spawn(unitSpawnerInstance, conn);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (!SceneManager.GetActiveScene().name.StartsWith("Scene_Map")) return;
            
            var gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
                
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
        }
    }
}