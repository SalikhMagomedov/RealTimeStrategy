﻿using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace Rts.Units
{
    public class Unit : NetworkBehaviour
    {
        [SerializeField] private UnityEvent onSelected;
        [SerializeField] private UnityEvent onDeselected;

        #region Client

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
    }
}