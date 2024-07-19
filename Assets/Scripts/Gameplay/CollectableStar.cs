using System;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class CollectableStar : MonoBehaviour
    {
        public CircleCollider2D CollectableStarCollider { get; private set; }
        
        public event Action<CollectableStar> OnStarCollected; 
        
        private void Awake()
        {
            CollectableStarCollider = GetComponent<CircleCollider2D>();
            CollectableStarCollider.isTrigger = true;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                OnStarCollected?.Invoke(this);
            }
        }
    }
}