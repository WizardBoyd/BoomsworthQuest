using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace TaskManagment.Loading.LoadingScripts
{
    public class StartupLoading : MonoBehaviour, ILoadingScript
    {
        #region ILoadingScript
        public UnityLoadingDescriptor CurrentMainTask { get; set; }
        public Queue<UnityLoadingDescriptor> TaskQueue
        {
            get => _taskQueue;
        }
        public List<UnityLoadingThreadedTaskDescriptor> OffloadedTasks
        {
            get => _offLoadedTasks;
        }

        private Queue<UnityLoadingDescriptor> _taskQueue = new Queue<UnityLoadingDescriptor>();

        private List<UnityLoadingThreadedTaskDescriptor> _offLoadedTasks =
            new List<UnityLoadingThreadedTaskDescriptor>();

        #endregion

        [SerializeField]
        private IProgressDisplay LoadingProgressDisplay;

        private void Awake()
        {
            // if (LoadingProgressDisplay == null)
            //     LoadingProgressDisplay = FindObjectsOfType<MonoBehaviour>().OfType<IProgressDisplay>().First();
        }

        private void Start()
        {
            TaskQueue.Enqueue(new UnityLoadingTaskDescriptor("Load Addressables", async () =>
            {
                Debug.Log("Loading The Addressables");
                await Task.Delay(TimeSpan.FromSeconds(5));
                Debug.Log("Finished Loading Addressables");
            }));
            OffloadedTasks.Add(new UnityLoadingThreadedTaskDescriptor("TestThreadedTask", token =>
            {
                int i = 0;
                while (i < 100 && !token.IsCancellationRequested)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                    Debug.Log("Test Thead Task");
                    i++;
                }
            }, destroyCancellationToken));
            TaskScheduler.Instance.ReceiveLoadingScript(this, Test);
        }

        public void Test()
        {
            Debug.Log("Finished Executing Loading Process");
        }

    }
}