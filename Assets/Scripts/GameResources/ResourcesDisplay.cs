using Mirror;
using Rts.Networking;
using TMPro;
using UnityEngine;

namespace Rts.GameResources
{
    public class ResourcesDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text resourcesText;

        private RtsPlayer _player;

        private void Start()
        {
            _player = NetworkClient.connection.identity.GetComponent<RtsPlayer>();
            
            ClientHandleResourcesUpdated(_player.Resources);
                    
            _player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
        }

        private void OnDestroy()
        {
            _player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
        }

        private void ClientHandleResourcesUpdated(int obj)
        {
            resourcesText.text = $"Resources: {obj}";
        }
    }
}