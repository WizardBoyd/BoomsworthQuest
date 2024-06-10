using System.Collections.Generic;

namespace TaskManagment.Loading
{
    public interface ILoadingScript
    {
        public UnityLoadingDescriptor CurrentMainTask { get; set; }
        
        public Queue<UnityLoadingDescriptor> TaskQueue { get; }
        
        public List<UnityLoadingThreadedTaskDescriptor> OffloadedTasks { get; }
    }
}