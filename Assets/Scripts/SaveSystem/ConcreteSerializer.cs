using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using SaveSystem.Interface;
using UnityEngine;

namespace SaveSystem
{
    public class BinarySerializer : ISerializer<byte[]>
    {
        public Encoding EncodingOption { get => m_EncodingOption; }
        private Encoding m_EncodingOption = null;
        private JsonSerializer m_jsonSerializer = null;

        public BinarySerializer(Encoding encodingOption = null)
        {
            this.m_EncodingOption = encodingOption ?? new UTF8Encoding();
            this.m_jsonSerializer = new JsonSerializer(this.m_EncodingOption);
        }
        

        public byte[] Serialize<TU>(TU obj)
        {
            byte[] serializedBytes = null;
            string json = m_jsonSerializer.Serialize<TU>(obj);
            if (json != null)
            {
                serializedBytes = m_EncodingOption.GetBytes(json);
                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryWriter writer = new BinaryWriter(stream, m_EncodingOption, false))
                    {
                        writer.Write(serializedBytes.Length);
                        writer.Write(serializedBytes);
                    }
                    serializedBytes = stream.ToArray();
                }
            }
            return serializedBytes;
        }
        

        public T Deserialize<T>(byte[] serializedData)
        {
            return m_jsonSerializer.
                Deserialize<T>(m_EncodingOption.GetString(serializedData));
        }
        
    }

    public class JsonSerializer : ISerializer<string>
    {
        private bool m_prettyPrint;
        public Encoding EncodingOption { get => m_EncodingOption; }
        private Encoding m_EncodingOption = null;

        public JsonSerializer(Encoding encodingOption = null)
        {
            this.m_EncodingOption = encodingOption ?? new UTF8Encoding();
        }

        public JsonSerializer(bool prettyPrint = false)
        {
            m_prettyPrint = prettyPrint;
        }
        
        public string Serialize<TU>(TU obj)
        {
            return JsonUtility.ToJson(obj, m_prettyPrint);
        }

        public T Deserialize<T>(string serializedData)
        {
            return JsonUtility.FromJson<T>(serializedData);
        }
    }
}