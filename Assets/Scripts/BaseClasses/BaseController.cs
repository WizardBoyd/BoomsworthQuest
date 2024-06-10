namespace BaseClasses
{
    public abstract class BaseController<T> where T : BaseModel
    {
        protected T Model;

        public virtual void Setup(T model)
        {
            this.Model = model;
        }
    }
}