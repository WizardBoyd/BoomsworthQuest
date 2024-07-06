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
        [SerializeField] 
        private VoidEventChannelSO OnSceneReadyChannel = default;

        private void Awake()
        {
            m_lineRenderer = GetComponent<LineRenderer>();
        }

        private void OnEnable()
        {
            FuseLitChannel.OnEventRaised += Fire;
            OnSceneReadyChannel.OnEventRaised += OnSceneReady;
        }

        private void OnDisable()
        {
            FuseLitChannel.OnEventRaised -= Fire;
            OnSceneReadyChannel.OnEventRaised -= OnSceneReady;
            // PhysicsSimulationManager.Instance.RemoveActionToPerformEachStep(UpdateCanonSimulation);
            // PhysicsSimulationManager.Instance.On_PhysicStepPerformed -= UpdateTrajectoryPath;
        }

        private void OnSceneReady()
        {
            // PhysicsSimulationManager.Instance.AddActionToPerformEachStep(UpdateCanonSimulation);
            // PhysicsSimulationManager.Instance.On_PhysicStepPerformed += UpdateTrajectoryPath;
        }


        private void Update()
        {
            if (CanonRotationHandler.IsDragging)
            {
                PhysicsSimulationManager.Instance.SimulateTrajectory(
                    CanonBallChamber.transform.position,
                    CanonBallChamber.right * 10,
                    m_lineRenderer);
            }
        }

        private void UpdateTrajectoryPath(int step)
        {
            if (m_currentGhostCanon != null)
            {
                // m_lineRenderer.positionCount = PhysicsSimulationManager.Instance.SimulationStepsPerFrame;
                // m_lineRenderer.SetPosition(step, m_currentGhostCanon.transform.position);
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