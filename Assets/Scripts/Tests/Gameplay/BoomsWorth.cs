using UnityEngine;

namespace Tests.Gameplay
{
    public class BoomsWorth : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D m_Rb;

        public void SendFlying(Vector2 velocity)
        {
            m_Rb.AddForce(velocity, ForceMode2D.Impulse);
        }
    }
}