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
        
        

        public void Save(string filename, IDataSave save)
        {
            using (FileStream file = new FileStream(Path.Combine(Application.persistentDataPath, Path.Combine(filename)),
                       FileMode.OpenOrCreate))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(file, Encoding.UTF8))
                {
                    save.WriteDataToFile(binaryWriter);
                }
            }
        }

        public void Load(string filename, IDataSave save)
        {
            using (FileStream file = new FileStream(Path.Combine(Application.persistentDataPath, Path.Combine(filename)),
                       FileMode.Open))
            {
                using (BinaryReader binaryReader = new BinaryReader(file, Encoding.UTF8))
                {
                    save.ReadDataFromFile(binaryReader);
                }
            }
        }
    }
}