using System.IO;

namespace SaveSystem.Interface
{
    public interface IBinarySerializable
    {
        public void Write(BinaryWriter writer);
        public void Read(BinaryReader reader);
    }
}