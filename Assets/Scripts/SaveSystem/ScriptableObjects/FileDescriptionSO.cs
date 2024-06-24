using System.Linq;
using SaveSystem.Interface;
using SaveSystem.SaveData;
using UnityEngine;

namespace SaveSystem.ScriptableObjects
{
    
    public abstract class FileDescriptionSO : ScriptableObject
    {
        public string FileName;

        public abstract void CreateDefaultFile();
        
        public abstract void LoadFile();

        public abstract IDataSave[] GetSaveObject();

        public abstract bool VerifyIntegrity();
    }
    
}