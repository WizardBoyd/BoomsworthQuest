using System;
using Events.ScriptableObjects;
using Tests.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tests.Gameplay
{
    [RequireComponent(typeof(LineRenderer))]
    public class Canon : MonoBehaviour
    {
        private LineRenderer m_lineRenderer;
        private GameObject m_currentGhostCanon = null;
        
        [SerializeField] private Animator Animator;

        [Header("Configuration")] 
        [SerializeField]
        private DragRotateHandler2D CanonRotationHandler;
        [SerializeField] 
        private Transform CanonBallChamber;
        
        [Header("Listening To")] 
        [SerializeField]
        private VoidEventChannelSO FuseLitChannel = default;

        private void Awake()
        {
            m_lineRenderer = GetComponent<LineRenderer>();
        }

        private void OnEnable()
        {
            FuseLitChannel.OnEventRaised += Fire;

        }

        private void OnDisable()
        {
            FuseLitChannel.OnEventRaised -= Fire;
        }
        
        private void Update()
        {
            if (CanonRotationHandler.IsDragging)
            {
                TrajectorySimulationRequestNoObject request = new TrajectorySimulationRequestNoObject();
                request.Type = Collider2DType.Circle;
                request.OriginPoint = CanonBallChamber.transform.position;
                request.Renderer = m_lineRenderer;
                request.SimulationSteps = 100;
                request.Force = CanonBallChamber.right * 100;
                PhysicsSimulationManager.Instance.SimulateTrajectory(request);
            }
        }
        
        public void Fire()
        {
            Animator.SetTrigger("Fire");
        }

        public void On_Fired()
        {
            Debug.Log("The Canon Ball Is Ready To Fire");
        }
    }
}