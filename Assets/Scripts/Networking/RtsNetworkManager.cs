using Mirror;
using UnityEngine;

namespace Rts.Networking
{
    public class RtsNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject unitSpawnerPrefab;

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            var unitSpawnerInstance = Instantiate(unitSpawnerPrefab, conn.identity.transform);
            
            NetworkServer.Spawn(unitSpawnerInstance, conn);
        }
    }
}