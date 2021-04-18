using Mirror;
using Rts.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Rts.Menus
{
    public class LobbyMenu : MonoBehaviour
    {
        [SerializeField] private GameObject lobbyUi;
        [SerializeField] private Button startGameButton;
        [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
        
        private void Start()
        {
            RtsNetworkManager.ClientOnConnected += HandleClientConnected;
            RtsPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
            RtsPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
        }

        private void OnDestroy()
        {
            RtsNetworkManager.ClientOnConnected -= HandleClientConnected;
            RtsPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
            RtsPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
        }

        private void ClientHandleInfoUpdated()
        {
            var players = ((RtsNetworkManager) NetworkManager.singleton).Players;

            for (var i = 0; i < players.Count; i++)
            {
                playerNameTexts[i].text = players[i].DisplayName;
            }

            for (var i = players.Count; i < playerNameTexts.Length; i++)
            {
                playerNameTexts[i].text = "Waiting for Player...";
            }

            startGameButton.interactable = players.Count > 1;
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