using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL.ComponentSystem
{
    public interface IComponent
    {
        void Initialize();
        void Update(float deltaTime);
        bool MustUpdateEachFrame();
        void Destroy();
    }

    public abstract class BaseComponent : IComponent
    {
        public GameObject GameObject;
        public string Name;

        public abstract bool MustUpdateEachFrame();
        public abstract void Initialize();
        public abstract void OnDestroy();
        public void Destroy()
        {
            GameObject.RemoveComponent(this);
            OnDestroy();
        }

        public virtual void Update(float deltaTime) { }

        public abstract BaseComponent Clone();
    }

}
