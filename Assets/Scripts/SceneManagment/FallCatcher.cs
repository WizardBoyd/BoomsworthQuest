using RuntimeAnchors;
using SceneManagment.ScriptableObjects;
using UnityEngine;

namespace SceneManagment
{
    //Meant to catch a player if they fall off the world
    public class FallCatcher : MonoBehaviour
    {
        [SerializeField] private PathSO _leadsToPath = default;
        [SerializeField] private PathStorageSO _pathStorage = default;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _pathStorage.lastPathTaken = _leadsToPath;

                //other.GetComponent<Damageable>().Kill();
            }
        }
    }
}