using System;
using Mirror;
using Rts.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rts.Menus
{
    public class LobbyMenu : MonoBehaviour
    {
        [SerializeField] private GameObject lobbyUi;

        private void Start()
        {
            RtsNetworkManager.ClientOnConnected += HandleClientConnected;
        }

        private void OnDestroy()
        {
            RtsNetworkManager.ClientOnConnected -= HandleClientConnected;
        }

        private void HandleClientConnected()
        {
            lobbyUi.SetActive(true);
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