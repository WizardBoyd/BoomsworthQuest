using System.IO;
using System.Text;

namespace SaveSystem.Interface
{
    public interface ISerializer<TF>
    {
        public Encoding EncodingOption { get; }
        TF Serialize<TU>(TU obj);
        T Deserialize<T>(TF serializedData);
        
    }
    
}