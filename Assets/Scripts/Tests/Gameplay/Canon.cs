using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tests.Gameplay
{
    public class Canon : MonoBehaviour, IPointerClickHandler,IBeginDragHandler, IDragHandler, IEndDragHandler
    {

        private Vector2 pos;
        private Vector2 direction;
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log($"Drag has begun {eventData.position}");
        }

        public void OnDrag(PointerEventData eventData)
        {
            pos = Camera.main.ScreenToWorldPoint(eventData.position);
            direction = pos - new Vector2(transform.position.x, transform.position.y);
            float angle = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
            Debug.Log($"The angle is {angle}");
            Quaternion roation = Quaternion.Euler(0,0,angle);
            transform.rotation = roation;

            //transform.LookAt(new Vector3(pos.x,pos.y, transform.position.z));
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log($"Drag has Ended {eventData.position}");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"Pointer has been clicked");
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y),
                    new Vector2(pos.x, transform.position.y));
                Gizmos.color = Color.red;
                Gizmos.DrawLine(new Vector2(pos.x, transform.position.y), new Vector2(pos.x, pos.y));
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, direction);
            }
        }
    }
}