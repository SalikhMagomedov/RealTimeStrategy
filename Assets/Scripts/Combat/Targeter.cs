using Mirror;
using Rts.Buildings;
using UnityEngine;

namespace Rts.Combat
{
    public class Targeter : NetworkBehaviour
    {
        public Targetable Target { get; private set; }

        public override void OnStartServer()
        {
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        public override void OnStopServer()
        {
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

        [Server]
        private void ServerHandleGameOver()
        {
            ClearTarget();
        }

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