using System;
using UnityEngine;

namespace BaseClasses
{
    public class BaseView <M,C> : MonoBehaviour
    where M : BaseModel
    where C: BaseController<M>, new()
    {
        [SerializeField] protected M Model = default;
        protected C Controller;

        protected virtual void Awake()
        {
            Controller = new C();
            Controller.Setup(Model);
        }
    }
}