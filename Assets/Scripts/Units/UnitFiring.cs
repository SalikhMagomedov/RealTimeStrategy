using Mirror;
using Rts.Combat;
using UnityEngine;

namespace Rts.Units
{
    public class UnitFiring : NetworkBehaviour
    {
        [SerializeField] private Targeter targeter;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private float fireRange = 5f;
        [SerializeField] private float fireRate = 1f;
        [SerializeField] private float rotationSpeed = 20f;

        private float _lastFireTime;

        [ServerCallback]
        private void Update()
        {
            var target = targeter.Target;
            
            if (target == null) return;
            if (!CanFireAtTarget()) return;

            var targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (!(Time.time > 1 / fireRate + _lastFireTime)) return;

            var position = projectileSpawnPoint.position;
            var projectileRotation = Quaternion.LookRotation(target.AimAtPoint.position - position);
            var projectileInstance = Instantiate(projectilePrefab, position, projectileRotation);

            NetworkServer.Spawn(projectileInstance, connectionToClient);

            _lastFireTime = Time.time;
        }

        [Server]
        private bool CanFireAtTarget()
        {
            return (targeter.Target.transform.position - transform.position).sqrMagnitude <= fireRange * fireRange;
        }
    }
}