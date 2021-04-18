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
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Building[] buildings = new Building[0];
        [SerializeField] private LayerMask buildingBlockLayer;
        [SerializeField] private float buildingRangeLimit = 5f;

        private Color _teamColor;
        private readonly List<Unit> _myUnits = new List<Unit>();
        private readonly List<Building> _myBuildings = new List<Building>();

        public event Action<int> ClientOnResourcesUpdated;
        
        public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
        public static event Action ClientOnInfoUpdated;

        public Transform CameraTransform => cameraTransform;
        public IEnumerable<Unit> MyUnits => _myUnits;
        public IEnumerable<Building> MyBuildings => _myBuildings;
        
        [field: SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
        public int Resources { get; private set; } = 500;

        [field: SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
        public bool IsPartyOwner { get; private set; }
        
        [field: SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
        public string DisplayName { get; private set; }
        
        [Server]
        public void SetResources(int newResources)
        {
            Resources = newResources;
        }

        [Server]
        public void SetDisplayName(string displayName)
        {
            DisplayName = displayName;
        }

        public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
        {
            return !Physics.CheckBox(point + buildingCollider.center, buildingCollider.size / 2, Quaternion.identity,
                buildingBlockLayer) && _myBuildings.Any(building =>
                (point - building.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit);
        }

        #region Server

        public override void OnStartServer()
        {
            Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
            Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
            Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
            
            DontDestroyOnLoad(gameObject);
        }

        public override void OnStopServer()
        {
            Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
            Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
            Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
        }

        [Server]
        public void SetPartyOwner(bool state)
        {
            IsPartyOwner = state;
        }
        
        [Server]
        public Color GetTeamColor()
        {
            return _teamColor;
        }

        [Server]
        public void SetTeamColor(Color color)
        {
            _teamColor = color;
        }

        [Command]
        public void CmdStartGame()
        {
            if (!IsPartyOwner) return;
            
            ((RtsNetworkManager)NetworkManager.singleton).StartGame();
        }

        [Command]
        public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
        {
            var buildingToPlace = buildings.FirstOrDefault(building => building.ID == buildingId);

            if (buildingToPlace == null) return;
            if (Resources < buildingToPlace.Price) return;

            var buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

            if (!CanPlaceBuilding(buildingCollider, point)) return;

            var buildingInstance = Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);
            
            NetworkServer.Spawn(buildingInstance, connectionToClient);
            
            SetResources(Resources - buildingToPlace.Price);
        }

        private void ServerHandleUnitSpawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
            
            _myUnits.Add(unit);
        }

        private void ServerHandleUnitDespawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
            
            _myUnits.Remove(unit);
        }
        
        private void ServerHandleBuildingSpawned(Building building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;
            
            _myBuildings.Add(building);
        }

        private void ServerHandleBuildingDespawned(Building building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;
            
            _myBuildings.Remove(building);
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

        public override void OnStartClient()
        {
            if(NetworkServer.active) return;

            DontDestroyOnLoad(gameObject);
            ((RtsNetworkManager) NetworkManager.singleton).Players.Add(this);
        }

        public override void OnStopClient()
        {
            OnClientOnInfoUpdated();
            
            if (!isClientOnly) return;
            
            ((RtsNetworkManager) NetworkManager.singleton).Players.Remove(this);

            if (!hasAuthority) return;
            
            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
            Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
            Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
        }

        private void AuthorityHandleUnitSpawned(Unit unit)
        {
            _myUnits.Add(unit);
        }

        private void AuthorityHandleUnitDespawned(Unit unit)
        {
            _myUnits.Remove(unit);
        }
        
        private void AuthorityHandleBuildingSpawned(Building building)
        {
            _myBuildings.Add(building);
        }

        private void AuthorityHandleBuildingDespawned(Building building)
        {
            _myBuildings.Remove(building);
        }

        private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
        {
            if(!hasAuthority) return;

            OnAuthorityOnPartyOwnerStateUpdated(newState);
        }

        private void ClientHandleResourcesUpdated(int oldValue, int newValue)
        {
            OnClientOnResourcesUpdated(newValue);
        }

        private void ClientHandleDisplayNameUpdated(string oldName, string newName)
        {
            OnClientOnInfoUpdated();
        }

        #endregion

        protected virtual void OnClientOnResourcesUpdated(int obj)
        {
            ClientOnResourcesUpdated?.Invoke(obj);
        }

        protected virtual void OnAuthorityOnPartyOwnerStateUpdated(bool obj)
        {
            AuthorityOnPartyOwnerStateUpdated?.Invoke(obj);
        }

        private static void OnClientOnInfoUpdated()
        {
            ClientOnInfoUpdated?.Invoke();
        }
    }
}