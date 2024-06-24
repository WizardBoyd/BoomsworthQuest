using System.Collections.Generic;
using System.IO;
using Events.ScriptableObjects;
using SaveSystem;
using SaveSystem.Interface;
using SaveSystem.SaveData;
using SaveSystem.ScriptableObjects;
using UnityEngine;

namespace SceneManagment.ScriptableObjects.Actions
{
    [CreateAssetMenu(fileName = "First Time Start up Action", menuName = "Scene/Scene Actions/First Time Start up Action", order = 0)]
    public class FirstTimeStartupAction : BaseSceneChangeAction
    {
        public override void PerformAction()
        {
            IDataService dataService = null;
            dataService = new FileDataService<string, JsonDataReadWriter>.
                    FileDataServiceBuilder<string, JsonDataReadWriter>()
                .WithSerializer(new JsonSerializer())
                .WithFileExtension("data")
                .WithDataPath(Application.persistentDataPath)
                .Build();
            
            if (!CheckApplicationStatus(dataService))
            {
                //This is most likely the first time setup then
#if UNITY_EDITOR
                Debug.Log($"Creating Core Start Files");
#endif
                CreateCoreFiles(dataService);
            }
        }

        private bool CheckApplicationStatus(IDataService service)
        {
            return service.FileExists("AppStatus");
        }

        private void CreateCoreFiles(IDataService service)
        {
            service.Save(new GameProgressData()
            {
                Name = "DefaultGameProgress"
            });
            service.Save(new PlayerSettingsData()
            {
                Name = "DefaultPlayerSettings"
            });
            service.Save(new PlayerResourceData()
            {
                Name = "DefaultResourceData"
            });
            //Create the app status
            ApplicationStatus applicationStatus = new ApplicationStatus()
            {
                Name = "AppStatus",
                CoreFilesCreated = true,
                FirstLaunch = true
            };
            service.Save(applicationStatus);
        }
    }
}