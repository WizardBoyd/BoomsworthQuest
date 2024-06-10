using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Misc.Singelton;
using TaskManagment.Loading;
using Unity.Services.Lobbies.Scheduler;

namespace TaskManagment
{
    
    public class  TaskScheduler : MonoBehaviorSingleton<TaskScheduler>
    {
        
        private List<UnityLoadingThreadedTaskDescriptor> _workerThreadTasks;
        private Queue<UnityLoadingDescriptor> _mainThreadTaskQueue;
        private UnityLoadingDescriptor MainThreadCurrentTask
        {
            get => _mainThreadTaskQueue.Peek();
        }

        private List<UnityLoadingThreadedTaskDescriptor> CurrentWorkerTasks;
        
        private object m_lock = new object();
        private Thread m_mainThread = null;
        
        
        public bool IsMainThread => (m_mainThread == System.Threading.Thread.CurrentThread);

        protected override void Awake()
        {
            base.Awake();
            m_mainThread = System.Threading.Thread.CurrentThread;
            _workerThreadTasks = new List<UnityLoadingThreadedTaskDescriptor>();
            _mainThreadTaskQueue = new Queue<UnityLoadingDescriptor>();
            CurrentWorkerTasks =
                new List<UnityLoadingThreadedTaskDescriptor>(_workerThreadTasks);
        }

        #region Loading Tasks

        public void AddLoadingTask(string taskName, Func<Task> awaitableTask){}
        public void AddLoadingTask(string taskName, Action executableAction){}
        public void AddLoadingTask(string taskName, Action<CancellationToken> executableAction){}
        public void AddLoadingTask(UnityLoadingDescriptor loadingDescriptor){}
        public void AddLoadingTask(UnityLoadingThreadedTaskDescriptor loadingDescriptor){}

        #endregion
        #region Generic Execution Tasks

        public void RegisterLoadingExecution(string taskName, Func<Task> awaitableTask)
        {
            if (IsMainThread)
            {
                _mainThreadTaskQueue.Enqueue(new UnityLoadingTaskDescriptor(taskName, awaitableTask));
            }
            else
            {
                lock (m_lock)
                {
                    _mainThreadTaskQueue.Enqueue(new UnityLoadingTaskDescriptor(taskName, awaitableTask));   
                }
            }
        }
        public void RegisterLoadingExecution(string taskName, Action executableAction)
        {
            if (IsMainThread)
            {
                _mainThreadTaskQueue.Enqueue(new UnityLoadingActionDescriptor(taskName, executableAction));   
            }
            else
            {
                lock (m_lock)
                {
                    _mainThreadTaskQueue.Enqueue(new UnityLoadingActionDescriptor(taskName, executableAction));   
                }
            }
        }
        public void RegisterLoadingExecution(string taskName, Action<CancellationToken> executableAction)
        {
            _workerThreadTasks.Add(new UnityLoadingThreadedTaskDescriptor(taskName, executableAction, destroyCancellationToken));
        }
        public void RegisterLoadingExecution(UnityLoadingDescriptor loadingDescriptor){}
        public void RegisterLoadingExecution(UnityLoadingThreadedTaskDescriptor loadingDescriptor){}

        #endregion

        public async Task ExecuteLoadingProcess()
        {
            CurrentWorkerTasks =
                new List<UnityLoadingThreadedTaskDescriptor>(_workerThreadTasks);
            _workerThreadTasks.Clear();
            Queue<UnityLoadingDescriptor> taskQueue = null;
            lock (m_lock)
            {
                taskQueue = new Queue<UnityLoadingDescriptor>(_mainThreadTaskQueue);
                _mainThreadTaskQueue.Clear();
            }
            
            //Startup each worker thread
            CurrentWorkerTasks.ForEach((x) => x.Execute());

            //start executing the main thread tasks one by one
            
            foreach (UnityLoadingDescriptor loadingDescriptor in taskQueue)
            {
                switch (loadingDescriptor.SyncState)
                {
                    case SyncState.Synchronized:
                        loadingDescriptor.Execute();
                        break;
                    case SyncState.Asynchronous:
                        if (loadingDescriptor is UnityLoadingTaskDescriptor descriptor)
                        {
                            await descriptor.ExecuteAsync();
                        }
                        break;
                }
            }

            //Wait for all other thread tasks to be done
            await Task.WhenAll(CurrentWorkerTasks.Select((x) => x.RunningTask));
        }

        public async Task ReceiveLoadingScript(ILoadingScript script, Action onLoadingFinished)
        {
            List<UnityLoadingThreadedTaskDescriptor> ScriptOffloadedTasks =
                new List<UnityLoadingThreadedTaskDescriptor>(script.OffloadedTasks);
            Queue<UnityLoadingDescriptor> taskQueue = new Queue<UnityLoadingDescriptor>(script.TaskQueue);
            
            //Startup each worker thread
            ScriptOffloadedTasks.ForEach((x) => x.Execute());

            //start executing the main thread tasks one by one
            
            foreach (UnityLoadingDescriptor loadingDescriptor in taskQueue)
            {
                switch (loadingDescriptor.SyncState)
                {
                    case SyncState.Synchronized:
                        loadingDescriptor.Execute();
                        break;
                    case SyncState.Asynchronous:
                        if (loadingDescriptor is UnityLoadingTaskDescriptor descriptor)
                        {
                            await descriptor.ExecuteAsync();
                        }
                        break;
                }
            }
            await Task.WhenAll(ScriptOffloadedTasks.Select((x) => x.RunningTask));
            onLoadingFinished?.Invoke();
        }

        private void OnDestroy()
        {
            //get rid of all worker threads
            foreach (UnityLoadingThreadedTaskDescriptor workerTask in CurrentWorkerTasks)
            {
                workerTask.CancelTask();
            }
        }
        
    }
}