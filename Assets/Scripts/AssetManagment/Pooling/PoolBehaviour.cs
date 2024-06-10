using UnityEngine;

namespace AssetManagment.Pooling
{
    public class PoolBehaviour : MonoBehaviour
    {
        IAssetPoolObject _poolObject;

        public void SetPoolObject(IAssetPoolObject iPoolObject)
        {
            if (_poolObject != null)
            {
                _poolObject.AwakeFromPoolEvent -= OnAwakeFromPool;
                _poolObject.ReturnToPoolEvent -= OnReturnToPool;
                _poolObject = null;
            }

            _poolObject = iPoolObject;
            if (_poolObject != null)
            {
                _poolObject.AwakeFromPoolEvent += OnAwakeFromPool;
                _poolObject.ReturnToPoolEvent += OnReturnToPool;
            }
        }


        protected virtual void OnAwakeFromPool()
        {

        }

        protected virtual void OnReturnToPool()
        {

        }
    }
}