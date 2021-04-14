using System;
using Mirror;
using UnityEngine;

namespace Rts.Combat
{
    public class Health : NetworkBehaviour
    {
        [SerializeField] private int maxHealth = 100;

        [SyncVar]
        private int _currentHealth;

        public event Action ServerOnDie;

        #region Server

        public override void OnStartServer()
        {
            _currentHealth = maxHealth;
        }

        [Server]
        public void DealDamage(int damageAmount)
        {
            if (_currentHealth == 0) return;

            _currentHealth = Mathf.Max(_currentHealth - damageAmount, 0);
            
            if (_currentHealth != 0) return;
            
            OnServerOnDie();

            Debug.Log("We Died");
        }

        #endregion

        #region Client

        

        #endregion

        protected virtual void OnServerOnDie()
        {
            ServerOnDie?.Invoke();
        }
    }
}