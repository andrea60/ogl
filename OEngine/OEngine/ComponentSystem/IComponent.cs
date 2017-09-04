using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEngine.ComponentSystem
{
    public interface IComponent
    {
        void Initialize();
        void Update(float deltaTime);
        bool MustUpdateEachFrame();
        void Destroy();
    }

    public abstract class BaseComponent : IComponent, IDisposable 
    {
        public GameObject GameObject;
        public string Name;
        public Managers.Managers SystemManager;

        public abstract bool MustUpdateEachFrame();
        public abstract void Initialize();
        public abstract void OnDestroy();
        public abstract void Subscribe();
        public abstract void Unsubscribe();
        public void Destroy()
        {
            GameObject.RemoveComponent(this);
            OnDestroy();
        }

        public BaseComponent(Managers.Managers handler)
        {
            SystemManager = handler;
            Subscribe();
        }
        ~BaseComponent()
        {
            Unsubscribe();
        }

        public virtual void Update(float deltaTime) { }

        public abstract BaseComponent Clone();

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

}
        
