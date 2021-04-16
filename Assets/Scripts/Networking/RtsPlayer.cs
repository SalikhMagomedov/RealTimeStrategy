using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Rts.Buildings;
using Rts.Units;
using UnityEngine;

namespace Rts.Networking
{
    public class RtsPlayer : NetworkBehaviour
    {
        [SerializeField] private Building[] buildings = new Building[0];
        [SerializeField] private List<Unit> myUnits = new List<Unit>();
        [SerializeField] private List<Building> myBuildings = new List<Building>();

        public event Action<int> ClientOnResourcesUpdated; 
        
        public IEnumerable<Unit> MyUnits => myUnits;
        public IEnumerable<Building> MyBuildings => myBuildings;
        
        [field: SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
        public int Resources { get; private set; } = 500;

        [Server]
        public void SetResources(int newResources)
        {
            Resources = newResources;
        }

        #region Server

        public override void OnStartServer()
        {
            Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
            Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
            Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
        }

        public override void OnStopServer()
        {
            Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
            Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
            Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
        }

        [Command]
        public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
        {
            var buildingToPlace = buildings.FirstOrDefault(building => building.ID == buildingId);

            if (buildingToPlace == null) return;

            var buildingInstance = Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);
            
            NetworkServer.Spawn(buildingInstance, connectionToClient);
        }

        private void ServerHandleUnitSpawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
            
            myUnits.Add(unit);
        }

        private void ServerHandleUnitDespawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
            
            myUnits.Remove(unit);
        }
        
        private void ServerHandleBuildingSpawned(Building building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;
            
            myBuildings.Add(building);
        }

        private void ServerHandleBuildingDespawned(Building building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;
            
            myBuildings.Remove(building);
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            if (NetworkServer.active) return;

            Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
            Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
            Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
        }

        public override void OnStopClient()
        {
            if (!isClientOnly || !hasAuthority) return;
            
            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
            Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
            Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
        }

        private void AuthorityHandleUnitSpawned(Unit unit)
        {
            myUnits.Add(unit);
        }

        private void AuthorityHandleUnitDespawned(Unit unit)
        {
            myUnits.Remove(unit);
        }
        
        private void AuthorityHandleBuildingSpawned(Building building)
        {
            myBuildings.Add(building);
        }

        private void AuthorityHandleBuildingDespawned(Building building)
        {
            myBuildings.Remove(building);
        }

        private void ClientHandleResourcesUpdated(int oldValue, int newValue)
        {
            OnClientOnResourcesUpdated(newValue);
        }

        #endregion

        protected virtual void OnClientOnResourcesUpdated(int obj)
        {
            ClientOnResourcesUpdated?.Invoke(obj);
        }
    }
}