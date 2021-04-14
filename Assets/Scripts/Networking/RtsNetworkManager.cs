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

            var connectionTransform = conn.identity.transform;
            var unitSpawnerInstance = Instantiate(unitSpawnerPrefab,
                connectionTransform.position,
                connectionTransform.rotation);
            
            NetworkServer.Spawn(unitSpawnerInstance, conn);
        }
    }
}