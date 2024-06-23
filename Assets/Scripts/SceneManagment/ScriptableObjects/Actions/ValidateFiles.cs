using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SceneManagment.ScriptableObjects.Actions
{
    [CreateAssetMenu(fileName = "Validate Files  Action", menuName = "Scene/Scene Actions/Validate Files Action", order = 0)]
    public class ValidateFiles : BaseSceneChangeAction
    {
        [SerializeField] 
        private List<string> m_filesToCheck = new List<string>();
        
        public override void PerformAction()
        {
            foreach (string s in m_filesToCheck)
            {
                string fullPath = Path.Combine(Application.persistentDataPath, s);
                if (!File.Exists(fullPath))
                {
                    //Create the file and then continue
                    File.Create(fullPath);
                    Debug.Log("Created");
                    continue;
                }
                ValidateFileIntegrity(fullPath);
            }
        }

        private bool ValidateFileIntegrity(string path)
        {
            Debug.Log("Validating File integrity");
            return true;
        }
    }
}