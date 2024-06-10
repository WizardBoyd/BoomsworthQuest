namespace AssetManagment.Pooling
{
    public delegate void DelegateAwakeFromPool();
    public delegate void DelegateReturnToPool();
    
    public interface IAssetPoolObject
    {
        event DelegateAwakeFromPool AwakeFromPoolEvent;
        event DelegateReturnToPool ReturnToPoolEvent;

        bool inPool { get; }
        IAssetPool myPool { get; }

        void AwakeFromPool();
        void ReturnToPool();
    }
}