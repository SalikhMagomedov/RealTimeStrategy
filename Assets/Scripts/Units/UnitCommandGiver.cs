using Rts.Buildings;
using Rts.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rts.Units
{
    public class UnitCommandGiver : MonoBehaviour
    {
        [SerializeField] private UnitSelectionHandler unitSelectionHandler;
        [SerializeField] private LayerMask layerMask;

        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;

            GameOverHandler.ClientOnGameOver += ClienHandleGameOver;
        }

        private void OnDestroy()
        {
            GameOverHandler.ClientOnGameOver -= ClienHandleGameOver;
        }

        private void Update()
        {
            if (!Mouse.current.rightButton.wasPressedThisFrame) return;

            var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask)) return;

            if (hit.collider.TryGetComponent<Targetable>(out var target))
            {
                if (target.hasAuthority)
                {
                    TryMove(hit.point);
                    return;
                }

                TryTarget(target);
                return;
            }
            TryMove(hit.point);
        }

        private void TryTarget(Targetable target)
        {
            foreach (var unit in unitSelectionHandler.SelectedUnits)
            {
                unit.Targeter.CmdSetTarget(target.gameObject);
            }
        }

        private void TryMove(Vector3 point)
        {
            foreach (var unit in unitSelectionHandler.SelectedUnits)
            {
                unit.UnitMovement.CmdMove(point);
            }
        }

        private void ClienHandleGameOver(string obj)
        {
            enabled = false;
        }
    }
}