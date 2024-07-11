using System;
using UnityEngine;

namespace Tests.Gameplay
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class WorldBorder : MonoBehaviour
    {
        private BoxCollider2D m_worldBorder;

        private void Awake()
        {
            m_worldBorder = GetComponent<BoxCollider2D>();
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            //An Object leaves the world borders
            Debug.Log("Something has left border");
            Destroy(other.gameObject, 1.0f);
        }
    }
}