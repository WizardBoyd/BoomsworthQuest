using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using SaveSystem.Interface;
using SaveSystem.SaveData;
using UnityEngine;

namespace SaveSystem
{
    public class FileDataService<T,TRW> : IDataService where TRW : IFileDataReadWriter<T>,new()
    {
        private ISerializer<T> m_serializer;
        private string m_dataPath;
        private string m_fileExtension;
        public void Save(GameData data, bool overwrite = true)
        {
            string fileLocation = GetPathToFile(data.Name);
            if (!overwrite && File.Exists(fileLocation))
            {
                throw new IOException($"The file {data.Name}.{m_fileExtension}" +
                                      $" already exists and cannot be overwritten");
            }else if (overwrite && File.Exists(fileLocation))
            {
                WriteFile(fileLocation, data, FileMode.Truncate);
            }
            else
            {
                WriteFile(fileLocation, data, FileMode.Append);
            }
        }

        public TU Load<TU>(string fileName) where TU : GameData,new()
        {
            string fileLocation = GetPathToFile(fileName);
            if (!File.Exists(fileLocation))
            {
                throw new FileNotFoundException($"No Persisted Game Data With name {fileName}");
            }

            using (FileStream fileStream = new FileStream(fileLocation, FileMode.Open))
            {
                TRW readWriter = new TRW();
                TU newGameData = new TU();
                newGameData = readWriter.Read<TU>(m_serializer, newGameData, fileStream);
                return newGameData;
            }
        }

        public void Delete(string fileName)
        {
            string fileLocation = GetPathToFile(fileName);
            File.Delete(fileLocation);
        }

        public void DeleteAll()
        {
            foreach (string file in Directory.GetFiles(m_dataPath))
            {
                File.Delete(file);
            }
        }

        public IEnumerable<string> ListSaves()
        {
            foreach (string file in Directory.GetFiles(m_dataPath))
            {
                if (Path.GetExtension(file) == m_fileExtension)
                {
                    yield return Path.GetFileNameWithoutExtension(file);
                }
            }
        }

        public bool FileExists(string fileName)
        {
            string fileLocation = GetPathToFile(fileName);
            if (!File.Exists(fileLocation))
            {
                return false;
            }

            return true;
        }

        private string GetPathToFile(string fileName)
        {
            return Path.Combine(m_dataPath, string.Concat(fileName, ".", m_fileExtension));
        }

        private void WriteFile(string fileLocation, GameData data, FileMode mode)
        {
            using (FileStream stream = new FileStream(fileLocation, mode))
            {
                TRW writer = new TRW();
                writer.Write(m_serializer, data, stream);
            }
        }
        

        public class FileDataServiceBuilder<U,URW> where URW : IFileDataReadWriter<U>, new()
        {
            private string m_datapath = null;
            private string m_fileExtension = null;
            private ISerializer<U> m_serializer = null;

            public FileDataServiceBuilder<U,URW> WithFileExtension(string fileExtension)
            {
                this.m_fileExtension = fileExtension;
                return this;
            }

            public FileDataServiceBuilder<U,URW> WithDataPath(string dataPath)
            {
                this.m_datapath = dataPath;
                return this;
            }

            public FileDataServiceBuilder<U,URW> WithSerializer(ISerializer<U> serializer)
            {
                this.m_serializer = serializer;
                return this;
            }

            public FileDataService<U,URW> Build()
            {
                FileDataService<U,URW> newFileDataService =
                    new FileDataService<U,URW>
                    {
                        m_fileExtension = this.m_fileExtension,
                        m_dataPath = this.m_datapath,
                        m_serializer = this.m_serializer
                    };
                return newFileDataService;
            }
        }
    }

    public interface IFileDataReadWriter<T>
    {
        void Write<TU>(ISerializer<T> serializer, TU data, FileStream stream) where TU : GameData;
        TU Read<TU>(ISerializer<T> serializer, TU data, FileStream stream) where TU : GameData;
    }
    public class BinaryDataReadWriter : IFileDataReadWriter<byte[]>
    {
        private byte[] ReadAllBytesFromStream(Stream stream)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(stream);
                return memoryStream.ToArray();
            }
        }

        void IFileDataReadWriter<byte[]>.Write<TU>(ISerializer<byte[]> serializer, TU data, FileStream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream, serializer.EncodingOption))
            {
                writer.Write(serializer.Serialize<TU>(data));
            }
        }

        TU IFileDataReadWriter<byte[]>.Read<TU>(ISerializer<byte[]> serializer, TU data, FileStream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream, serializer.EncodingOption))
            {
                data = serializer.Deserialize<TU>(ReadAllBytesFromStream(stream));
            }

            return data;
        }
        
    }

    public class JsonDataReadWriter : IFileDataReadWriter<string>
    {
        public void Write<TU>(ISerializer<string> serializer, TU data, FileStream stream) where TU : GameData
        {
            using (StreamWriter streamWriter = new StreamWriter(stream))
            {
                streamWriter.Write(serializer.Serialize(data));
            }
        }

        public TU Read<TU>(ISerializer<string> serializer, TU data, FileStream stream) where TU : GameData
        {
            using (StreamReader streamReader = new StreamReader(stream))
            {
                string serializedData = streamReader.ReadToEnd();
                data = serializer.Deserialize<TU>(serializedData);
            }
            return data;
        }
    }
    
}