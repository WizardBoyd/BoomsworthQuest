using System;
using System.Collections.Generic;
using System.Linq;
using AssetManagment;
using AssetManagment.ConcreteReferences;
using Factory;
using Levels.ScriptableObjects;
using Mkey;
using Pool;
using SaveSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Levels
{
    [RequireComponent(typeof(RectTransform), typeof(SceneCurve))]
    public class ZoneRuntime : MonoBehaviour
    {
    }
}