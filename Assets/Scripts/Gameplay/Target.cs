using System;
using System.Collections.Generic;
using Events.ScriptableObjects;
using Tests.Gameplay;
using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// The target is the game object that booms worth needs to hit in order to complete the level,
    /// on being hit it will explode and emit an event that its been hit
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public class Target : HittableObstacle
    {
        public override Collider2D Collider
        {
            get => m_circleCollider2D;
        }
        private CircleCollider2D m_circleCollider2D;

        public event Action OnTargetHit;

        protected override void Awake()
        {
            m_circleCollider2D = GetComponent<CircleCollider2D>();
            base.Awake();
        }

        protected override void OnHit(BoomsWorth boomsWorth)
        {
            //At this point Boomsworth should have their velocity set to 0 so they don't move after hitting the target
            boomsWorth.Rigidbody2D.velocity = Vector2.zero;
            OnTargetHit?.Invoke();
            //TODO Add explosion effect
        }
    }
}