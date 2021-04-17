using Mirror;
using UnityEngine;

namespace Rts.Networking
{
    public class TeamColorSetter : NetworkBehaviour
    {
        [SerializeField] private Renderer[] colorRenderers = new Renderer[0];

        [SyncVar(hook = nameof(HandleTeamColorUpdated))] private Color _teamColor = new Color();
        
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        #region Server

        public override void OnStartServer()
        {
            var player = connectionToClient.identity.GetComponent<RtsPlayer>();

            _teamColor = player.GetTeamColor();
        }

        #endregion

        #region Client

        private void HandleTeamColorUpdated(Color oldValue, Color newValue)
        {
            foreach (var colorRenderer in colorRenderers)
            {
                colorRenderer.material.SetColor(BaseColor, newValue);
            }
        }

        #endregion
    }
}