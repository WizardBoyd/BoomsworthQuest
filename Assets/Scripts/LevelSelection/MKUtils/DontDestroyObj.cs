using UnityEngine;
using System.Collections;

/*
   11112019 - first
 */

namespace Mkey
{
    public class DontDestroyObj : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}