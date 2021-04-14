using System;
using Mirror;
using Rts.Combat;
using UnityEngine;
using UnityEngine.Events;

namespace Rts.Units
{
    public class Unit : NetworkBehaviour
    {
        [SerializeField] private UnitMovement unitMovement;
        [SerializeField] private Targeter targeter;
        [SerializeField] private UnityEvent onSelected;
        [SerializeField] private UnityEvent onDeselected;

        public static event Action<Unit> ServerOnUnitSpawned;
        public static event Action<Unit> ServerOnUnitDespawned;
        public static event Action<Unit> AuthorityOnUnitSpawned;
        public static event Action<Unit> AuthorityOnUnitDespawned;
        
        public UnitMovement UnitMovement => unitMovement;
        public Targeter Targeter => targeter;

        #region Server

        public override void OnStartServer()
        {
            OnServerOnUnitSpawned(this);
        }

        public override void OnStopServer()
        {
            OnServerOnUnitDespawned(this);
        }

        #endregion

        #region Client

        public override void OnStartClient()
        {
            if (!isClientOnly || !hasAuthority) return;
            
            OnAuthorityOnUnitSpawned(this);
        }

        public override void OnStopClient()
        {
            if (!isClientOnly || !hasAuthority) return;
            
            OnAuthorityOnUnitDespawned(this);
        }

        [Client]
        public void Select()
        {
            if (!hasAuthority) return;
            
            onSelected?.Invoke();
        }

        [Client]
        public void Deselect()
        {
            if (!hasAuthority) return;
            
            onDeselected?.Invoke();
        }

        #endregion

        private static void OnServerOnUnitSpawned(Unit unit)
        {
            ServerOnUnitSpawned?.Invoke(unit);
        }

        private static void OnServerOnUnitDespawned(Unit unit)
        {
            ServerOnUnitDespawned?.Invoke(unit);
        }

        private static void OnAuthorityOnUnitSpawned(Unit unit)
        {
            AuthorityOnUnitSpawned?.Invoke(unit);
        }

        private static void OnAuthorityOnUnitDespawned(Unit unit)
        {
            AuthorityOnUnitDespawned?.Invoke(unit);
        }
    }
}