using Mirror;
using Rts.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Rts.Menus
{
    public class LobbyMenu : MonoBehaviour
    {
        [SerializeField] private GameObject lobbyUi;
        [SerializeField] private Button startGameButton;
        
        private void Start()
        {
            RtsNetworkManager.ClientOnConnected += HandleClientConnected;
            RtsPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        }

        private void OnDestroy()
        {
            RtsNetworkManager.ClientOnConnected -= HandleClientConnected;
            RtsPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        }

        private void AuthorityHandlePartyOwnerStateUpdated(bool obj)
        {
            startGameButton.gameObject.SetActive(obj);
        }

        private void HandleClientConnected()
        {
            lobbyUi.SetActive(true);
        }

        public void StartGame()
        {
            NetworkClient.connection.identity.GetComponent<RtsPlayer>().CmdStartGame();
        }

        public void LeaveLobby()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StartHost();
            }
            else
            {
                NetworkManager.singleton.StartClient();
                
                SceneManager.LoadScene(0);
            }
        }
    }
}