namespace SaveSystem.Interface
{
    // ReSharper disable once TypeParameterCanBeVariant
    public interface ISaveInjectionReceiver<T>
    {
        public T GetSave();
        public void Receive(T data);
    }
}