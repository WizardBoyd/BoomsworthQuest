using System.Collections.Generic;
using SaveSystem;
using UnityEngine;

namespace SceneManagment.ScriptableObjects.Actions
{
    [CreateAssetMenu(fileName = "Save Files Action", menuName = "Scene/Scene Actions/Save Files Action", order = 0)]
    public class SaveFilesChangeAction : BaseSceneChangeAction
    {
        [SerializeField] 
        private List<string> m_filenames = new List<string>();
        
        public override void PerformAction()
        {
            //Get instance of the save manager
            SaveManager manager = SaveManager.Instance;
            if (manager != null)
            {
                manager.Save(m_filenames.ToArray());
            }
        }
    }
}