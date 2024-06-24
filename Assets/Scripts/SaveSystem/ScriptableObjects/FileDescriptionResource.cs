using System.Collections.Generic;
using System.Linq;
using SaveSystem.Interface;
using SaveSystem.SaveData;
using UnityEngine;

namespace SaveSystem.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Files/Description/Player Resource Data", order = 0)]
    public class FileDescriptionResource : FileDescriptionSO
    {
        public override void CreateDefaultFile()
        {
            SaveManager manager = SaveManager.Instance;
            if (manager != null)
            {
                PlayerResourceData resourceData = new PlayerResourceData();
                //manager.Save(FileName,resourceData);
            }
        }

        public override void LoadFile()
        {
            SaveManager manager = SaveManager.Instance;
            if (manager != null)
            {
                ISaveInjectionReceiver<PlayerResourceData>[] saveables = FindObjectsOfType<MonoBehaviour>()
                    .OfType<ISaveInjectionReceiver<PlayerResourceData>>().ToArray();
                PlayerResourceData resourceData = new PlayerResourceData();
                //manager.Load(FileName, resourceData);
                foreach (var saveable in saveables)
                {
                    saveable.Receive(resourceData);
                }
            }
        }

        public override IDataSave[] GetSaveObject()
        {
            var saveList = new List<IDataSave>();
            ISaveInjectionReceiver<PlayerResourceData>[] saveables = FindObjectsOfType<MonoBehaviour>()
                .OfType<ISaveInjectionReceiver<PlayerResourceData>>().ToArray();
            foreach (ISaveInjectionReceiver<PlayerResourceData> saveInjectionReceiver in saveables)
            {
                //saveList.Add(saveInjectionReceiver.GetSave());
            }
            return saveList.ToArray();
        }

        public override bool VerifyIntegrity()
        {
            return true;
        }
    }
}