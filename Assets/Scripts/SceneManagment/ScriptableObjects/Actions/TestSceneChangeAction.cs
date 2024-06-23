using System.Collections.Generic;
using UnityEngine;

namespace SceneManagment.ScriptableObjects.Actions
{
    [CreateAssetMenu(fileName = "Test Scene Change Action", menuName = "Scene/Scene Actions/Test Action", order = 0)]
    public class TestSceneChangeAction : BaseSceneChangeAction
    {
        public override void PerformAction()
        {
            Debug.Log("This is a test");
        }
    }
}