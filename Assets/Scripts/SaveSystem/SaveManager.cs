using System.Collections.Generic;
using Misc.Singelton;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;
using SaveSystem.Interface;
using UnityEngine;

namespace SaveSystem
{
    public class SaveManager : MonoBehaviorSingleton<SaveManager>
    {
        private HashSet<IDataSave> m_dataSaves;

        protected override void Awake()
        {
            base.Awake();
            m_dataSaves = new HashSet<IDataSave>();
        }

        public bool RegisterDataSave(IDataSave saveData)
        {
            if (!m_dataSaves.Contains(saveData))
            {
                m_dataSaves.Add(saveData);
                return true;
            }
            return false;
        }

        public bool UnregisterDataSave(IDataSave saveData)
        {
            if (m_dataSaves.Contains(saveData))
            {
                m_dataSaves.Remove(saveData);
                return true;
            }
            return false;
        }

        public void SaveAll()
        {
            foreach (IDataSave dataSave in m_dataSaves)
            {
                Save(Path.Combine(dataSave.FilePath,dataSave.FileName));
            }
        }

        public void LoadAll()
        {
            foreach (IDataSave dataSave in m_dataSaves)
            {
                Load(Path.Combine(dataSave.FilePath,dataSave.FileName));
            }
        }

        public void Save(params string[] fileNames)
        {
            foreach (string fileName in fileNames)
            {
                Save(fileName);
            }
        }

        public void Save(string fileName)
        {
            if (m_dataSaves.Any(x => x.FileName == fileName))
            {
                IDataSave save = m_dataSaves.First(x => x.FileName == fileName);
                Save(save);
            }
        }

        public void Load(params string[] fileNames)
        {
            foreach (string fileName in fileNames)
            {
                Load(fileName);
            }
        }
        public void Load(string fileName)
        {
            if (m_dataSaves.Any(x => x.FileName == fileName))
            {
                IDataSave save = m_dataSaves.First(x => x.FileName == fileName);
                Load(save);
            }
        }
        private void Save(IDataSave save)
        {
            using (FileStream file = new FileStream(Path.Combine(Application.persistentDataPath, Path.Combine(save.FilePath, save.FileName)),
                       FileMode.OpenOrCreate))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(file, Encoding.UTF8))
                {
                    save.WriteDataToFile(binaryWriter);
                }
            }
        }
        
        private void Load(IDataSave save)
        {
            using (FileStream file = new FileStream(Path.Combine(Application.persistentDataPath, Path.Combine(save.FilePath, save.FileName)),
                       FileMode.Open))
            {
                using (BinaryReader binaryRader = new BinaryReader(file, Encoding.UTF8))
                {
                   save.ReadDataFromFile(binaryRader);
                }
            }
        }
    }
}