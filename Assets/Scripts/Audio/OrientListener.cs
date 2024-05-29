using RuntimeAnchors;
using UnityEngine;

namespace Audio
{
    public class OrientListener : MonoBehaviour
    {
        // Reference to the camera transform determine listener orientation
        [SerializeField] private TransformAnchor _cameraTransform;

        void LateUpdate()
        {
            if(_cameraTransform.isSet)
                transform.forward = _cameraTransform.Value.forward;
        }
    }
}