﻿using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rts.Buildings
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private Transform unitSpawnPoint;

        #region Server

        [Command]
        private void CmdSpawnUnit()
        {
            var unitInstance = Instantiate(unitPrefab, unitSpawnPoint);
            
            NetworkServer.Spawn(unitInstance, connectionToClient);
        }

        #endregion

        #region Client

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (!hasAuthority) return;
            
            CmdSpawnUnit();
        }

        #endregion
    }
}