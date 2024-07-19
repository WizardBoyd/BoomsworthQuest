using System;
using Tests.Gameplay;
using UnityEngine;

namespace Gameplay
{
    public abstract class  HittableObstacle : MonoBehaviour
    {
        public abstract Collider2D Collider { get;}

        protected virtual void Awake()
        {
            Collider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.TryGetComponent<BoomsWorth>(out BoomsWorth boomsWorth))
            {
                OnHit(boomsWorth);
            }
        }
        
        protected abstract void OnHit(BoomsWorth boomsWorth);
    }
}