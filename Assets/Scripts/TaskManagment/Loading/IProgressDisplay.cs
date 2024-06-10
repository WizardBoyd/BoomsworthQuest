namespace TaskManagment.Loading
{
    public interface IProgressDisplay
    {
        public string CurrentTaskName { get; }

        public void BeginLoadingProcess();
        public void FinishLoadingProcess();
    }
}