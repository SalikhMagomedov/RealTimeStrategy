using Mirror;
using UnityEngine;

namespace Rts.Menus
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject landingPagePanel;

        public void HostLobby()
        {
            landingPagePanel.SetActive(false);
            
            NetworkManager.singleton.StartHost();
        }
    }
}