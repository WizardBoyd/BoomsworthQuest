using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SceneManagment.ScriptableObjects.Actions
{
    [CreateAssetMenu(fileName = "Create Files  Action", menuName = "Scene/Scene Actions/Create Files Action", order = 0)]
    public class CreateFiles : BaseSceneChangeAction
    {
        [SerializeField] 
        private List<string> m_filesToCheck = new List<string>();
        
        public override void PerformAction()
        {
            foreach (string s in m_filesToCheck)
            {
                string fullPath = Path.Combine(Application.persistentDataPath, s);
                string directoryPath = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    Debug.Log($"Create Directory {directoryPath}");
                }
                if (!File.Exists(fullPath))
                {
                    File.Create(fullPath);
                    Debug.Log($"Create File {Path.GetFileName(fullPath)}");
                }
            }
        }
    }
}