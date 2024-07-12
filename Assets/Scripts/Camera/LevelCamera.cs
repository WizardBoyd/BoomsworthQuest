using System;
using DependencyInjection.attributes;
using Events.ScriptableObjects;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Camera
{
    //TODO: for editor playtime I need to handle what happens if the simulator is changed to a different device during runtime
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class LevelCamera : MonoBehaviour
    {
        [Header("Listening To")]
        [SerializeField] private VoidEventChannelSO m_onLevelReadyEvent;
        
        [SerializeField]
        private Collider2D m_collider;
        private UnityEngine.Camera m_camera;

       [Inject]
       private TouchInputReader m_touchInputReader;

        [SerializeField] [Range(0, 1)] 
        private float m_zoomSpeed = 0.1f;
        
        [SerializeField] [Min(0.1f)]
        private float m_minZoominValue = 0.1f;
        
        private void Awake()
        {
            m_camera = GetComponent<UnityEngine.Camera>();
            //m_collider = GetComponent<Collider2D>();
        }

        private void OnEnable()
        {
            m_onLevelReadyEvent.OnEventRaised += OnLevelReady;
        }
        
        private void OnDisable()
        {
            m_onLevelReadyEvent.OnEventRaised -= OnLevelReady;
            if(m_touchInputReader != null)
                m_touchInputReader.OnTouchUpdate -= Process;
        }

        

        private void Process()
        {
            if(!EnsurePreconditions())
                return;
            if(m_touchInputReader.IsTouchingInteractable && !m_touchInputReader.IsAppCurrentlyInteractable)
                return;
            ProcessNavigation();
            ProcessTouchZoom();
        }

        private bool EnsurePreconditions()
        {
            if(m_touchInputReader == null)
                return false;
            if (m_camera == null)
                return false;
            if (m_collider == null)
                return false;
            return true;
        }

        private void ProcessNavigation()
        {
            if (m_touchInputReader.PrimaryFingerPhase != TouchPhase.Moved)
                return;
            Vector2 worldDelta = m_camera.ScreenToWorldPoint(m_touchInputReader.DeltaPrimaryFingerPosition) - m_camera.ScreenToWorldPoint(Vector2.zero);
            Vector3 oldCameraPosition = m_camera.transform.position;
            m_camera.transform.position += new Vector3(worldDelta.x, worldDelta.y, 0);
            if (!IsCameraInBounds())
            {
                Debug.Log("Reverting To Old Position");
                m_camera.transform.position = oldCameraPosition;
            }
        }
        
        private void ProcessTouchZoom()
        {
            if (m_touchInputReader.PrimaryFingerPhase == TouchPhase.Ended || m_touchInputReader.PrimaryFingerPhase == TouchPhase.Canceled
                                                     || m_touchInputReader.SecondaryFingerPhase == TouchPhase.Ended ||
                                                     m_touchInputReader.SecondaryFingerPhase == TouchPhase.Canceled)
            {
                // Do nothing
                //One or both touches have ended
                return;
            }
            //apply the zoom
            if (m_camera.orthographic)
            {
                float oldCameraOrthographicSize = m_camera.orthographicSize;
                m_camera.orthographicSize -= m_touchInputReader.DeltaDistanceBetweenFingers * m_zoomSpeed * Time.deltaTime;
                m_camera.orthographicSize = Mathf.Max(m_camera.orthographicSize, m_minZoominValue);
                if (!IsCameraInBounds())
                {
                    m_camera.orthographicSize = oldCameraOrthographicSize;
                }
            }
        }

        public bool IsCameraInBounds()
        {
            //Get the Min and max position of the camera
            Vector2 cameraMin = GetCameraMinPosition();
            Vector2 cameraMax = GetCameraMaxPosition();
            
            return m_collider.OverlapPoint(cameraMin) && m_collider.OverlapPoint(cameraMax);
        }

        public Vector2 GetCameraMaxPosition()
        {
            Vector2 topRight = m_camera.ScreenToWorldPoint(new Vector3(m_camera.pixelWidth, m_camera.pixelHeight, m_camera.farClipPlane));
            return topRight;
        }

        public Vector2 GetCameraMinPosition()
        {
            Vector2 bottomLeft = m_camera.ScreenToWorldPoint(new Vector3(0, 0, m_camera.farClipPlane));
            return bottomLeft;
        }

        public float GetWidthOfCameraInWorldUnits()
        {
            float screenAspect = (float)Screen.width / (float)Screen.height;
            float cameraWidth = m_camera.orthographicSize * 2 * screenAspect;
            return cameraWidth;
        }

        public float GetHeightOfCameraInWorldUnits()
        {
            return m_camera.orthographicSize * 2;
        }
        
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(!EnsurePreconditions())
                return;
            Vector2 cameraMin = GetCameraMinPosition();
            Vector2 cameraMax = GetCameraMaxPosition();
            
            Gizmos.color = m_collider.OverlapPoint(cameraMin) ? Color.green : Color.red;
            Gizmos.DrawWireCube(cameraMin, Vector3.one);
            Gizmos.color = m_collider.OverlapPoint(cameraMax) ? Color.green : Color.red;
            Gizmos.DrawWireCube(cameraMax, Vector3.one);
            Gizmos.color = m_collider.OverlapPoint(cameraMin) && m_collider.OverlapPoint(cameraMax) ? Color.green : Color.red;
            Gizmos.DrawWireCube(m_camera.transform.position, new Vector3(cameraMax.x - cameraMin.x, cameraMax.y - cameraMin.y, 1));
        }
#endif
        
        private void OnLevelReady()
        {
            if(m_touchInputReader != null)
                m_touchInputReader.OnTouchUpdate += Process;
        }

        // private void ProcessTouch()
        // {
        //     int fingerCount = Touch.activeFingers.Count;
        //     if(fingerCount <= 0)
        //         return;
        //     Touch firstTouch = Touch.activeFingers[0].currentTouch;
        //     Touch secondTouch;
        //     if (fingerCount == 1)
        //     {
        //         //We know there is only one finger on screen so process the movement
        //         ProcessTouchNavigate(firstTouch);
        //     }
        //     else if (fingerCount == 2)
        //     {
        //         Debug.Log("Doing Zoom");
        //         secondTouch = Touch.activeFingers[1].currentTouch;
        //         ProcessTouchZoom(firstTouch, secondTouch);
        //     }
        // }
        //
        //
        // private void KeepCameraInBounds()
        // {
        //     //First thing to do is to perform an AABB check to see if the camera bounds has left the world bounds
        //     //If it has, we need to move the camera back into the world bounds
        //     
        // }
        //
        // private Vector2 GetVectorToClosestWorldBoundPoint(Vector2 point)
        // {
        //     return m_collider.ClosestPoint(point);
        // }
        //
        // private void ProcessTouchNavigate(Touch primaryTouch)
        // {
        //     if (primaryTouch.phase == TouchPhase.Began)
        //     {
        //         m_firstTouchPreviousPosition = primaryTouch.screenPosition;
        //     }
        //     else if (primaryTouch.phase == TouchPhase.Moved)
        //     {
        //         Vector2 touchPosition = primaryTouch.screenPosition;
        //         Vector2 deltaPosition = primaryTouch.screenPosition - m_firstTouchPreviousPosition;
        //         
        //         
        //         //Convert screen delta to world delta
        //         Vector2 worldDelta = m_camera.ScreenToWorldPoint(deltaPosition) - m_camera.ScreenToWorldPoint(Vector2.zero);
        //         m_camera.transform.Translate(-worldDelta);
        //         m_firstTouchPreviousPosition = touchPosition;
        //     }
        // }
        //
        // private void ProcessTouchZoom(Touch firstTouch, Touch secondTouch)
        // {
        //     if (firstTouch.phase == TouchPhase.Ended || firstTouch.phase == TouchPhase.Canceled
        //                                              || secondTouch.phase == TouchPhase.Ended ||
        //                                              secondTouch.phase == TouchPhase.Ended)
        //     {
        //         // Do nothing
        //         //One or both touches have ended
        //         return;
        //     }
        //     
        //     if(firstTouch.phase == TouchPhase.Began || secondTouch.phase == TouchPhase.Began)
        //     {
        //         m_firstTouchPreviousPosition = firstTouch.screenPosition;
        //         m_SecondTouchPreviousPosition = secondTouch.screenPosition;
        //         m_previousDistanceBetweenTouches =
        //             Vector2.Distance(m_firstTouchPreviousPosition, m_SecondTouchPreviousPosition);
        //         return; //skip this frame to avoid jump in zoom
        //     }
        //     
        //     Vector2 firstTouchCurrentPosition = firstTouch.screenPosition;
        //     Vector2 secondTouchCurrentPosition = secondTouch.screenPosition;
        //     
        //     float currentDistanceBetweenTouches = Vector2.Distance(firstTouchCurrentPosition, secondTouchCurrentPosition);
        //     float deltaDistance = currentDistanceBetweenTouches - m_previousDistanceBetweenTouches;
        //     
        //     //apply the zoom
        //     if (m_camera.orthographic)
        //     {
        //         m_camera.orthographicSize -= deltaDistance * m_zoomSpeed * Time.deltaTime;
        //         m_camera.orthographicSize = Mathf.Max(m_camera.orthographicSize, m_minZoominValue);
        //     }
        // }
        //
        //
        // private bool CheckIfCameraLeavesCollider(Vector3 newCameraPosition)
        // {
        //     // Calculate the camera's position
        //     Vector3 cameraPosition = m_camera.transform.position;
        //     Vector3 cameraSize = new Vector3(GetWidthOfCameraInWorldUnits(), GetHeightOfCameraInWorldUnits(), 0);
        //     Vector3 cameraMin = cameraPosition - cameraSize / 2;
        //     Vector3 cameraMax = cameraPosition + cameraSize / 2;
        //     
        //     // Calculate the collider's position
        //     Vector3 colliderPosition = m_collider.bounds.center;
        //     Vector3 colliderSize = m_collider.bounds.size;
        //     Vector3 colliderMin = colliderPosition - colliderSize / 2;
        //     Vector3 colliderMax = colliderPosition + colliderSize / 2;
        //     
        //     // Check if the camera is outside the collider
        //     if (cameraMin.x < colliderMin.x || cameraMax.x > colliderMax.x ||
        //         cameraMin.y < colliderMin.y || cameraMax.y > colliderMax.y)
        //     {
        //         return true;
        //     }
        //
        //     return false;
        // }
        //
        // private void PositionCameraForLevel()
        // {
        //     // Calculate the the position of the camera
        //     float cameraZPosition = m_camera.transform.position.z;
        //     m_camera.transform.position = 
        //         new Vector3(m_collider.bounds.center.x, 
        //             m_collider.bounds.center.y, 
        //             cameraZPosition);
        // }
        //
        // private float GetHeightOfCameraInWorldUnits()
        // {
        //     return m_camera.orthographicSize * 2;
        // }
        //
        // private float GetWidthOfCameraInWorldUnits()
        // {
        //     float screenAspect = (float)Screen.width / Screen.height;
        //     float cameraWidth = m_camera.orthographicSize * 2 * screenAspect;
        //     return cameraWidth;
        // }
        //
        // private void AdjustCameraToBestFitBounds()
        // {
        //     //Calculate the size of the collider in world units
        //     Vector3 colliderSize = m_collider.bounds.size;
        //     
        //     // Determine the aspect ratio of the camera
        //     float screenAspect = (float)Screen.width / Screen.height;
        //
        //     // Calculate the size of the collider in terms of the camera's view
        //     float colliderHorizontalSize = colliderSize.x;
        //     float colliderVerticalSize = colliderSize.y;
        //
        //     // Adjust the orthographic size based on the larger dimension of the collider relative to the camera's aspect ratio
        //     if ((colliderHorizontalSize / screenAspect) > colliderVerticalSize)
        //     {
        //         // If the collider is wider than it is tall, adjust based on the width
        //         m_camera.orthographicSize = colliderHorizontalSize / screenAspect / 2;
        //     }
        //     else
        //     {
        //         // If the collider is taller than it is wide, adjust based on the height
        //         m_camera.orthographicSize = colliderVerticalSize / 2;
        //     }
        // }
        //
        // private Vector3 GetCameraMinPosition()
        // {
        //     float cameraHeight = GetHeightOfCameraInWorldUnits();
        //     float cameraWidth = GetWidthOfCameraInWorldUnits();
        //     
        //     Vector3 bottomLeft = m_camera.transform.position - new Vector3(cameraWidth/2, cameraHeight/2, 0);
        //     bottomLeft.z = m_camera.transform.position.z;
        //     return bottomLeft;
        // }
        //
        // private Vector3 GetCameraMaxPosition()
        // {
        //     float cameraHeight = GetHeightOfCameraInWorldUnits();
        //     float cameraWidth = GetWidthOfCameraInWorldUnits();
        //     
        //     Vector3 topRight = m_camera.transform.position + new Vector3(cameraWidth/2, cameraHeight/2, 0);
        //     topRight.z = m_camera.transform.position.z;
        //     return topRight;
        // }
    }
}