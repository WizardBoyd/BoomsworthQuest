using System.Collections.Generic;
using SaveSystem;
using SaveSystem.Interface;
using SaveSystem.ScriptableObjects;
using UnityEngine;

namespace SceneManagment.ScriptableObjects.Actions
{
    [CreateAssetMenu(fileName = "Save Files Action", menuName = "Scene/Scene Actions/Save Files Action", order = 0)]
    public class SaveFilesChangeAction : BaseSceneChangeAction
    {
        [SerializeField] 
        private List<FileDescriptionSO> m_filenames = new List<FileDescriptionSO>();
        
        public override void PerformAction()
        {
            //Get instance of the save manager
            SaveManager manager = SaveManager.Instance;
            if (manager != null)
            {
                foreach (FileDescriptionSO descriptionSo in m_filenames)
                {
                    foreach (IDataSave dataSave in descriptionSo.GetSaveObject())
                    {
                        manager.Save(descriptionSo.FileName, dataSave);
                    }
                }
            }
        }
    }
}