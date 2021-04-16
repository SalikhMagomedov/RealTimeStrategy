using System;
using Mirror;
using UnityEngine;

namespace Rts.Buildings
{
    public class Building : NetworkBehaviour
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private int id = -1;
        [SerializeField] private int price = 100;

        public static event Action<Building> ServerOnBuildingSpawned;
        public static event Action<Building> ServerOnBuildingDespawned;
        public static event Action<Building> AuthorityOnBuildingSpawned;
        public static event Action<Building> AuthorityOnBuildingDespawned;

        public Sprite Icon => icon;

        public int ID => id;

        public int Price => price;

        #region Server

        public override void OnStartServer()
        {
            OnServerOnBuildingSpawned(this);
        }

        public override void OnStopServer()
        {
            OnServerOnBuildingDespawned(this);
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            OnAuthorityOnBuildingSpawned(this);
        }
        
        public override void OnStopClient()
        {
            if (!hasAuthority) return;
            
            OnAuthorityOnBuildingDespawned(this);
        }

        #endregion

        private static void OnServerOnBuildingSpawned(Building obj)
        {
            ServerOnBuildingSpawned?.Invoke(obj);
        }

        private static void OnServerOnBuildingDespawned(Building obj)
        {
            ServerOnBuildingDespawned?.Invoke(obj);
        }

        private static void OnAuthorityOnBuildingSpawned(Building obj)
        {
            AuthorityOnBuildingSpawned?.Invoke(obj);
        }

        private static void OnAuthorityOnBuildingDespawned(Building obj)
        {
            AuthorityOnBuildingDespawned?.Invoke(obj);
        }
    }
}