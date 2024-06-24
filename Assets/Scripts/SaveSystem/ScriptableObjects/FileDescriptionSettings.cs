using System.Collections.Generic;
using System.Linq;
using SaveSystem.Interface;
using SaveSystem.SaveData;
using UnityEngine;

namespace SaveSystem.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Files/Description/Player Settings Data", order = 0)]
    public class FileDescriptionSettings : FileDescriptionSO
    {
        public override void CreateDefaultFile()
        {
            SaveManager manager = SaveManager.Instance;
            if (manager != null)
            {
                PlayerSettingsData playerSettings = new PlayerSettingsData();
                //manager.Save(FileName,playerSettings);
            }
        }

        public override void LoadFile()
        {
            SaveManager manager = SaveManager.Instance;
            if (manager != null)
            {
                ISaveInjectionReceiver<PlayerSettingsData>[] saveables = FindObjectsOfType<MonoBehaviour>()
                    .OfType<ISaveInjectionReceiver<PlayerSettingsData>>().ToArray();
                PlayerSettingsData playerSettings = new PlayerSettingsData();
                //manager.Load(FileName, playerSettings);
                foreach (var saveable in saveables)
                {
                    saveable.Receive(playerSettings);
                }
            }
        }

        public override IDataSave[] GetSaveObject()
        {
            var saveList = new List<IDataSave>();
            ISaveInjectionReceiver<PlayerSettingsData>[] saveables = FindObjectsOfType<MonoBehaviour>()
                .OfType<ISaveInjectionReceiver<PlayerSettingsData>>().ToArray();
            foreach (ISaveInjectionReceiver<PlayerSettingsData> saveInjectionReceiver in saveables)
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