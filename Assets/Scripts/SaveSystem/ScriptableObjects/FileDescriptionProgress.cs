using System.Collections.Generic;
using System.Linq;
using SaveSystem.Interface;
using SaveSystem.SaveData;
using UnityEngine;

namespace SaveSystem.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Files/Description/Game Progress Data", order = 0)]
    public class FileDescriptionProgress : FileDescriptionSO
    {
        public override void CreateDefaultFile()
        {
            SaveManager manager = SaveManager.Instance;
            if (manager != null)
            {
                GameProgressData progressData = new GameProgressData();
                //manager.Save(FileName,progressData);
            }
        }

        public override void LoadFile()
        {
            SaveManager manager = SaveManager.Instance;
            if (manager != null)
            {
                ISaveInjectionReceiver<GameProgressData>[] saveables = FindObjectsOfType<MonoBehaviour>()
                    .OfType<ISaveInjectionReceiver<GameProgressData>>().ToArray();
                GameProgressData progressData = new GameProgressData();
                //manager.Load(FileName, progressData);
                foreach (var saveable in saveables)
                {
                    saveable.Receive(progressData);
                }
            }
        }

        public override IDataSave[] GetSaveObject()
        {
            var saveList = new List<IDataSave>();
            ISaveInjectionReceiver<GameProgressData>[] saveables = FindObjectsOfType<MonoBehaviour>()
                .OfType<ISaveInjectionReceiver<GameProgressData>>().ToArray();
            foreach (ISaveInjectionReceiver<GameProgressData> saveInjectionReceiver in saveables)
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