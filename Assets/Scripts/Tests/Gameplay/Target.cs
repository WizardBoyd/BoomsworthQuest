using System;
using System.Collections.Generic;
using Events.ScriptableObjects;
using UnityEngine;

namespace Tests.Gameplay
{
    /// <summary>
    /// The target is the game object that booms worth needs to hit in order to complete the level,
    /// on being hit it will explode and emit an event that its been hit
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public class Target : MonoBehaviour
    {
        [Header("Broadcasting On")] 
        [SerializeField]
        private VoidEventChannelSO TargetHitChannel = default;

        private SpriteRenderer renderer;
        
        private List<SpriteRenderer> m_spriteRenderers;
        private List<Rigidbody2D> m_rigidbodys2d;

        private void Awake()
        {
            m_spriteRenderers = new List<SpriteRenderer>();
            m_rigidbodys2d = new List<Rigidbody2D>();
            renderer = GetComponent<SpriteRenderer>();
            GetChildren();
        }

        private void GetChildren()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                Rigidbody2D rigidbody2D = child.GetComponent<Rigidbody2D>();
                m_spriteRenderers.Add(spriteRenderer);
                m_rigidbodys2d.Add(rigidbody2D);
                child.SetActive(false);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent<BoomsWorth>(out _))
            {
                TargetHitChannel.RaiseEvent();
                Scatter();
            }
        }

        private void Scatter()
        {
            renderer.enabled = false;
            foreach (Rigidbody2D rigidbody2D1 in m_rigidbodys2d)
            {
                rigidbody2D1.gameObject.SetActive(true);
            }
        }
    }
}