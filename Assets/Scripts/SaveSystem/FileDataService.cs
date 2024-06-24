using System.Collections.Generic;
using SaveSystem.Interface;
using SaveSystem.SaveData;

namespace SaveSystem
{
    public class FileDataService : IDataService
    {
        
        
        public void Save(IBinarySerializable data, bool overwrite = true)
        {
            throw new System.NotImplementedException();
        }

        public T Load<T>(string fileName) where T : GameData
        {
            throw new System.NotImplementedException();
        }

        public void Delete(string fileName)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteAll()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> ListSaves()
        {
            throw new System.NotImplementedException();
        }
    }
}