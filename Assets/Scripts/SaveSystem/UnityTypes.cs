using System.IO;
using SaveSystem.Interface;

namespace SaveSystem
{
    [System.Serializable]
    public struct SaveVector3 : IBinarySerializable
    {
        public SaveVector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public void Write(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
        }
        public void Read(BinaryReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
        }
    }
    [System.Serializable]
    public struct SaveVector2 : IBinarySerializable
    {
        public SaveVector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
        public float X { get; set; }
        public float Y { get; set; }
        public void Write(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
        }

        public void Read(BinaryReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
        }
    }
}