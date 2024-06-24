namespace SaveSystem.Interface
{
    public interface ISerializer<TF>
    {
        TF Serialize<TU>(TU obj);
        T Deserialize
        
    }
}