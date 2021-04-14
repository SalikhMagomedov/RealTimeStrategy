using Mirror;
using UnityEngine;

namespace Rts.Combat
{
    public class Targeter : NetworkBehaviour
    {
        public Targetable Target { get; private set; }

        [Command]
        public void CmdSetTarget(GameObject targetGameObject)
        {
            if (!targetGameObject.TryGetComponent<Targetable>(out var target)) return;

            Target = target;
        }

        [Server]
        public void ClearTarget()
        {
            Target = null;
        }
    }
}