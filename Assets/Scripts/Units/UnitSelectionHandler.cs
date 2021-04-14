using System.Collections.Generic;
using Mirror;
using Rts.Networking;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rts.Units
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform unitSelectionArea;
        [SerializeField] private LayerMask layerMask;

        private Vector2 _startPosition;
        private RtsPlayer _player;
        private Camera _mainCamera;
        public List<Unit> SelectedUnits { get; } = new List<Unit>();

        private void Start()
        {
            _mainCamera = Camera.main;
            _player = NetworkClient.connection.identity.GetComponent<RtsPlayer>();
        }

        private void Update()
        {
            if (_player == null) _player = NetworkClient.connection.identity.GetComponent<RtsPlayer>();

            if (Mouse.current.leftButton.wasPressedThisFrame)
                StartSelectionArea();
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
                ClearSelectionArea();
            else if (Mouse.current.leftButton.isPressed) UpdateSelectionArea();
        }

        private void StartSelectionArea()
        {
            if (!Keyboard.current.leftShiftKey.isPressed)
            {
                foreach (var selectedUnit in SelectedUnits) selectedUnit.Deselect();

                SelectedUnits.Clear();
            }

            unitSelectionArea.gameObject.SetActive(true);
            _startPosition = Mouse.current.position.ReadValue();

            UpdateSelectionArea();
        }

        private void UpdateSelectionArea()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var areaWidth = mousePosition.x - _startPosition.x;
            var areaHeight = mousePosition.y - _startPosition.y;

            unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
            unitSelectionArea.anchoredPosition = _startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
        }

        private void ClearSelectionArea()
        {
            unitSelectionArea.gameObject.SetActive(false);

            if (unitSelectionArea.sizeDelta.sqrMagnitude == 0)
            {
                var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask)) return;
                if (!hit.collider.TryGetComponent<Unit>(out var unit)) return;
                if (!unit.hasAuthority) return;

                SelectedUnits.Add(unit);

                foreach (var selectedUnit in SelectedUnits) selectedUnit.Select();

                return;
            }

            var min = unitSelectionArea.anchoredPosition - unitSelectionArea.sizeDelta / 2;
            var max = unitSelectionArea.anchoredPosition + unitSelectionArea.sizeDelta / 2;

            foreach (var unit in _player.MyUnits)
            {
                if (SelectedUnits.Contains(unit)) continue;

                var screenPosition = _mainCamera.WorldToScreenPoint(unit.transform.position);
                if (!(screenPosition.x > min.x) || !(screenPosition.x < max.x) || !(screenPosition.y > min.y) ||
                    !(screenPosition.y < max.y)) continue;
                SelectedUnits.Add(unit);
                unit.Select();
            }
        }
    }
}