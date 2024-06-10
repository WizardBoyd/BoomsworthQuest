namespace AssetManagment.Pooling
{
    public interface IAssetPool<in TPoolObjectType> where TPoolObjectType : IAssetPoolObject
    {
        /// <summary>
        /// Allows a pool object to inform it's pool that is has been returned to the pool.
        /// </summary>
        /// <param name="poolObject">The pool object that has returned to the pool.</param>
        void PoolObjectReturned(TPoolObjectType poolObject);
    }
    
    public interface IAssetPool
    {
        void PoolObjectReturned(PoolObject poolObject);
    }
}