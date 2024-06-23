using System.IO;

namespace SaveSystem.Interface
{
    public interface IDataSave
    {
        public string FileName { get; }
        public string FilePath { get; }

        void WriteDataToFile(BinaryWriter writer);
        void ReadDataFromFile(BinaryReader reader);
        
    }
}