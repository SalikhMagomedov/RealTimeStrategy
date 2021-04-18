using Mirror;
using Rts.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rts.Menus
{
    public class JoinLobbyMenu : MonoBehaviour
    {
        [SerializeField] private GameObject landingPagePanel;
        [SerializeField] private TMP_InputField addressInput;
        [SerializeField] private Button joinButton;

        private void OnEnable()
        {
            RtsNetworkManager.ClientOnConnected += HandleClientConnected;
            RtsNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
        }

        private void OnDisable()
        {
            RtsNetworkManager.ClientOnConnected -= HandleClientConnected;
            RtsNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
        }

        public void Join()
        {
            var address = addressInput.text;

            NetworkManager.singleton.networkAddress = address;
            NetworkManager.singleton.StartClient();

            joinButton.interactable = false;
        }

        private void HandleClientConnected()
        {
            joinButton.interactable = true;
            
            gameObject.SetActive(false);
            landingPagePanel.SetActive(false);
        }

        private void HandleClientDisconnected()
        {
            joinButton.interactable = true;
        }
    }
}