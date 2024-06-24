using System.IO;

namespace SaveSystem.Interface
{
    public interface IDataSave
    {
        void WriteDataToFile(BinaryWriter writer);
        void ReadDataFromFile(BinaryReader reader);
        
    }
}