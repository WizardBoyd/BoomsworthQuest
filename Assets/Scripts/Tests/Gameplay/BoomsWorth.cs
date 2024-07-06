using System;
using Events.ScriptableObjects;
using UnityEngine;

namespace Tests.Gameplay
{
    public class BoomsWorth : MonoBehaviour
    {
        public Rigidbody2D Rigidbody2D { get; private set; }
        public CircleCollider2D Collider2D { get; private set; }

        public bool HasLaunched { get; set; }
        public VoidEventChannelSO OnVelocityStopped
        {
            get => m_OnVelocityStopped;
        }

        [Header("Configurations")] 
        [SerializeField][Range(0, 1)]
        private float stopThreashold = .1f;

        [Header("Broadcasting On")] 
        [SerializeField]
        private VoidEventChannelSO m_OnVelocityStopped = default;
        
        private void Awake()
        {
            Rigidbody2D = GetComponent<Rigidbody2D>();
            Collider2D = GetComponent<CircleCollider2D>();
            HasLaunched = false;
        }

        private void Update()
        {
            if(!HasLaunched)
                return;
            
        }

        private void CheckIfStopped()
        {
            if (Rigidbody2D.velocity.magnitude < stopThreashold && !HasLaunched)
            {
                m_OnVelocityStopped.RaiseEvent();
                Destroy(gameObject);
            }
        }
    }
}