using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rts.Combat
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private GameObject healthBarParent;
        [SerializeField] private Image healthBarImage;

        private void Awake()
        {
            health.ClientOnHealthUpdated += HandleHealthUpdated;
        }

        private void OnDestroy()
        {
            health.ClientOnHealthUpdated -= HandleHealthUpdated;
        }

        private void HandleHealthUpdated(int currentHealth, int maxHealth)
        {
            healthBarImage.fillAmount = (float)currentHealth / maxHealth;
        }

        private void OnMouseEnter()
        {
            healthBarParent.SetActive(true);
        }

        private void OnMouseExit()
        {
            healthBarParent.SetActive(false);
        }
    }
}