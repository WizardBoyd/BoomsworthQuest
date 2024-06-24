using System.Collections.Generic;
using SaveSystem.SaveData;

namespace SaveSystem.Interface
{
    public interface IDataService
    {
        void Save(GameData data, bool overwrite = true);
        T Load<T>(string fileName) where T : GameData,new();
        void Delete(string fileName);
        void DeleteAll();
        IEnumerable<string> ListSaves();
        bool FileExists(string fileName);
    }
}