using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace TaskManagment.Loading
{

    public enum SyncState
    {
        Synchronized,
        Asynchronous,
        Parallel
    }
    
    public abstract class UnityLoadingDescriptor
    {
        protected UnityLoadingDescriptor(String descriptorName)
        {
            this.DescriptorName = descriptorName;
        }
        
        public string DescriptorName { get; protected set; }
        public abstract SyncState SyncState { get;}

        public abstract void Execute();
        
        
        public virtual void CancelTask(){}
    }
    
    public class UnityLoadingTaskDescriptor : UnityLoadingDescriptor
    {
        public UnityLoadingTaskDescriptor(string descriptorName, Func<Task> awaitableTask) : base(descriptorName)
        {
            this.ExecutionTask = awaitableTask;
        }

        public Func<Task> ExecutionTask { get; protected set; }
        public override SyncState SyncState
        {
            get => SyncState.Asynchronous;
        }

        public override async void Execute(){}

        public async Task ExecuteAsync()
        {
            await ExecutionTask();
        }
    }

    public class UnityLoadingThreadedTaskDescriptor : UnityLoadingDescriptor
    {
        public UnityLoadingThreadedTaskDescriptor(string descriptorName, Action<CancellationToken> executionAction, CancellationToken externalCancellationToken) : base(descriptorName)
        {
            this._executionAction = executionAction;
            this._internalCancellationSource = new CancellationTokenSource();
            this._jointCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(_internalCancellationSource.Token, externalCancellationToken);
            this.RunningTask = new Task(() =>
            {
                _executionAction(_jointCancellationSource.Token);
            }, _jointCancellationSource.Token);
        }

        public Task RunningTask;

        protected CancellationTokenSource _internalCancellationSource;
        protected CancellationTokenSource _jointCancellationSource;
        
        private Action<CancellationToken> _executionAction;

        public override SyncState SyncState
        {
            get => SyncState.Parallel;
        }

        public override void Execute()
        {
            RunningTask.Start();
        }

        public override void CancelTask()
        {
          _internalCancellationSource.Cancel();
        }
    }
    
    public class UnityLoadingActionDescriptor : UnityLoadingDescriptor
    {
        
        public UnityLoadingActionDescriptor(string descriptorName, Action executionAction) : base(descriptorName)
        {
            this.ExecutionAction = executionAction;
        }
        
        public Action ExecutionAction { get; protected set; }


        public override SyncState SyncState
        {
            get => SyncState.Synchronized;
        }

        public override void Execute()
        {
            ExecutionAction();
        }
        
    }
}