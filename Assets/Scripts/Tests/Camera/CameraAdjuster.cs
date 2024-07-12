using System;
using UnityEngine;

namespace Tests.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraAdjuster : MonoBehaviour
    {
        [SerializeField] 
        private int targetWidth = 20;
        [SerializeField] 
        private int TargetHeight = 20;

        private UnityEngine.Camera m_camera;

        private void Awake()
        {
            m_camera = GetComponent<UnityEngine.Camera>();
        }

        private void Start()
        {
            AdjustCameraSize();
        }

        private void LateUpdate()
        {
            AdjustCameraSize();
        }

        void AdjustCameraSize()
        {
            m_camera.transform.position = new Vector3(m_camera.transform.position.x, (float)TargetHeight / 2, m_camera.transform.position.z);
            float targetAspect = (float)targetWidth / TargetHeight;
            float windowAspect = (float)Screen.width / (float)Screen.height;
            float scaleHeight = windowAspect / targetAspect;

            if (scaleHeight < 1.0f)
            {
                m_camera.orthographicSize = (float)TargetHeight / 2.0f;
            }
            else
            {
                float scaleWidth = 1.0f / scaleHeight;
                m_camera.orthographicSize = ((float)TargetHeight / 2) * scaleWidth;
            }
        }
    }
}