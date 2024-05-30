using System;
using UnityEditor;
using UnityEngine;

namespace Mkey
{
    [InitializeOnLoad]
    public class HierarchyController
    {
        static HierarchyController()
        {
            EditorApplication.hierarchyChanged -= OnHierarchy_Change;
            EditorApplication.hierarchyChanged += OnHierarchy_Change;
        }

        private static void OnHierarchy_Change()
        {
            GameObject g = null;// Debug.Log("manually hierarchy changed ");
            MapMaker[] mGs = GameObject.FindObjectsOfType<MapMaker>();
            if (mGs != null && mGs.Length > 0)
            {
                foreach (var mG in mGs)
                {
                    mG.OnManuallyHierChanged();
                }
            }
        }
    }
}
/*
        if (Selection.activeObject) g = Selection.activeGameObject;

        if (g)
        {
            Debug.Log("manually hierarchy changed " + g.name);
            MapMaker mp = g.GetComponentInParent<MapMaker>();
            if (mp) mp.OnManuallyHierChanged();
        }
        */