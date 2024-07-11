using System;
using UnityEngine;

namespace Tests.Camera
{
    public class AdjustPlaySpaceToCamera : MonoBehaviour
    {
        [SerializeField]
        private Collider2D m_worldBounds;

        private UnityEngine.Camera m_camera;

        private void Awake()
        {
            m_camera = UnityEngine.Camera.main;
        }

        private void Start()
        {
            //UpdateSize();
        }


        void UpdateSize()
        {
            transform.position = new Vector3(m_camera.transform.position.x, m_camera.transform.position.y, 0);

            Vector3 bottomLeft = m_camera.ViewportToWorldPoint(Vector3.zero);
            Vector3 topRight = m_camera.ViewportToWorldPoint(new Vector3(m_camera.rect.width, m_camera.rect.height));

            Vector3 screenSize = topRight - bottomLeft;
            
            float screenRatio = screenSize.x / screenSize.y;
            float targetRatio = transform.localScale.x / transform.localScale.y;

            if (screenRatio > targetRatio)
            {
                transform.localScale =
                    new Vector3(screenSize.y * targetRatio, screenSize.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(screenSize.x, screenSize.x / targetRatio, transform.localScale.z);
            }
        }
    }
}