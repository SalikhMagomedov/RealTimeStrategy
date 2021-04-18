﻿using Mirror;
using Rts.Networking;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Rts.Cameras
{
    public class Minimap : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        [SerializeField] private RectTransform minimapRect;
        [SerializeField] private float mapScale = 20f;
        [SerializeField] private float offset = -6f;

        private Transform _playerCameraTransform;

        private void Update()
        {
            if(_playerCameraTransform != null) return;
            if(NetworkClient.connection.identity == null) return;
            _playerCameraTransform = NetworkClient.connection.identity.GetComponent<RtsPlayer>().CameraTransform;
        }

        private void MoveCamera()
        {
            var mousePos = Mouse.current.position.ReadValue();

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect, mousePos, null,
                out var localPoint)) return;

            var rect = minimapRect.rect;
            var lerp = new Vector2((localPoint.x - rect.x) / rect.width, (localPoint.y - rect.y) / rect.height);
            var newCameraPos = new Vector3(Mathf.Lerp(-mapScale, mapScale, lerp.x), _playerCameraTransform.position.y,
                Mathf.Lerp(-mapScale, mapScale, lerp.y));

            _playerCameraTransform.position = newCameraPos + new Vector3(0, 0, offset);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            MoveCamera();
        }

        public void OnDrag(PointerEventData eventData)
        {
            MoveCamera();
        }
    }
}