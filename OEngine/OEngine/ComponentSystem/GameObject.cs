using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL.ComponentSystem
{
    public class GameObject
    {
        protected string Name { get; set; }

        public Vector3 _Position = new Vector3();
        public Vector3 _Scale = new Vector3(1, 1, 1);
        public Quaternion _Rotation = Quaternion.Identity;

        public Vector3 Position {
            get
            {
                if (Parent != null)
                    return _Position + Parent.Position;
                else
                    return _Position;
            }
            set
            {
                _Position = value;
            }
        }
        public Quaternion Rotation {
            get
            {
                if (Parent != null)
                    return _Rotation + Parent.Rotation;
                else
                    return _Rotation;
            }
            set
            {
                _Rotation = value;
            }
        }
        public Vector3 Scale {
            get
            {
                if (Parent != null)
                    return _Scale * Parent.Scale;
                else
                    return _Scale;
            }
            set
            {
                _Scale = value;
            
            }
        } 

        private GameObject _Parent;
        public GameObject Parent
        {
            get
            {
                return _Parent;
            }
            set
            {
                _Parent?.Childs.Remove(this);
                value?.Childs.Add(this);
                _Parent = value;
            }
        }

        public List<GameObject> Childs = new List<GameObject>();

        protected List<BaseComponent> Components = new List<BaseComponent>();


        public GameObject() { }
        public GameObject(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }
        public GameObject(Vector3 position, Quaternion rotation) : this(position,rotation,new Vector3(1,1,1)) { }
        public GameObject(Vector3 position) : this (position, Quaternion.Identity) { }


        /// <summary>
        /// Removes a component
        /// </summary>
        /// <param name="component">The component to remove (it must belong to this GameObject)</param>
        public void RemoveComponent(BaseComponent component)
        {
            Components.Remove(component);
        }

        public T GetComponent<T>(string name=null) where T : BaseComponent
        {
            var candidates = Components.Where(c => c is T);
            if (candidates.Count() > 1 && name != null)
                return (T)candidates.Where(c => c.Name == name).SingleOrDefault();
            else
                return (T)candidates.SingleOrDefault();

        }

        /// <summary>
        /// Adds an existing component to the current GameObject
        /// </summary>
        /// <param name="component">The component to add (if part of another GameObject it will be deattached from it and attached to the new one)</param>
        /// <returns></returns>
        public bool AddComponent(BaseComponent component)
        {
            if (Components.Any(c => c == component))
                return false;
            component.Initialize();
            Components.Add(component);
            if (component.GameObject != null && component.GameObject != this)
                component.GameObject.RemoveComponent(component);
            component.GameObject = this;
            return true;
        }

        /// <summary>
        /// Creates a component from a class and adds it to the GameObject
        /// </summary>
        /// <typeparam name="T">The Component type (must inherit BaseComponent)</typeparam>
        /// <param name="name">The GameObject-unique component name</param>
        /// <returns></returns>
        public T CreateComponent<T>(string name) where T : BaseComponent, new()
        {
            var component = new T();
            component.Name = name;
            AddComponent(component);
            return component;

        }

        public void Update(float deltaTime)
        {
            foreach (var component in Components)
                if (component.MustUpdateEachFrame())
                    component.Update(deltaTime);
        }
        
        public GameObject Clone()
        {
            var copiedComponents = new List<BaseComponent>();
            foreach (var c in Components)
                copiedComponents.Add(c.Clone());
            var copiedChilds = new List<GameObject>();
            foreach (var c in Childs)
                copiedChilds.Add(c);
            return new GameObject
            {
                Name = Name,
                Position = _Position,
                Rotation = _Rotation,
                Scale = _Scale,
                Parent = Parent,
                Components = copiedComponents,
                Childs = copiedChilds
            };
        }

    }
}
