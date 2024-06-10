using System;
using BaseClasses;
using Misc.FileManagment;
using UnityEngine;

namespace SaveSystem.ScriptableObjects
{
    public abstract class SaveSO : SerializableScriptableObject
    {
        [SerializeField] 
        public string SavedFileName;

        private void Awake()
        {
            //SavedFileName = 
        }

        public bool DoesSaveExist()
        {
            if (!FileManager.FileExists(SavedFileName))
            {
                //var BackupFile = 
            }
            return true;
        }
    }
}