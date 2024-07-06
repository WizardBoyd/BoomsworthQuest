using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tests.Input
{
    /// <summary>
    /// DragRotate rotates the transform of the game-object In 2D space
    /// </summary>
    public class DragRotateHandler2D : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private Camera m_camera;
        
        public bool IsDragging { get; private set; }

        private void Awake()
        {
            m_camera = Camera.main;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 pos = m_camera.ScreenToWorldPoint(eventData.position);
            Vector2 direction = pos - new Vector2(transform.position.x, transform.position.y);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0,0,angle);
        }
        
        public void OnBeginDrag(PointerEventData eventData) => IsDragging = true;

        public void OnEndDrag(PointerEventData eventData) => IsDragging = false;
    }
}