using System.Collections.Generic;
using System.IO;
using SaveSystem.ScriptableObjects;
using UnityEngine;

namespace SceneManagment.ScriptableObjects.Actions
{
    [CreateAssetMenu(fileName = "Create Files  Action", menuName = "Scene/Scene Actions/Create Files Action", order = 0)]
    public class CreateFiles : BaseSceneChangeAction
    {
        [SerializeField] 
        private List<FileDescriptionSO> m_filesToCheck = new List<FileDescriptionSO>();
        
        public override void PerformAction()
        {
            foreach (FileDescriptionSO s in m_filesToCheck)
            {
                string fullPath = Path.Combine(Application.persistentDataPath, s.FileName);
                string directoryPath = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    Debug.Log($"Create Directory {directoryPath}");
                }

                if (!File.Exists(fullPath))
                {
                    s.CreateDefaultFile();
                }
            }
        }
    }
}