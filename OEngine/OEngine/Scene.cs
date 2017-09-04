using OEngine.ComponentSystem;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEngine
{
    public abstract class Scene
    {
        
        public Dictionary<string, GameObject> SceneObjects { get; internal set; } = new Dictionary<string, GameObject>();

     
        public GameObject CreateGameObject(string name)
        {

            name = GetFreeName(name);

            var go = new GameObject(new Vector3(0, 0, 0), Quaternion.Identity);
            SceneObjects.Add(name, go);
            return go;
        }
        public GameObject CreateGameObject(string name, Vector3 position, Quaternion? rotation = null)
        {
            if (rotation == null)
                rotation = Quaternion.Identity;
            name = GetFreeName(name);

            var go = new GameObject(position, rotation.Value);
            SceneObjects.Add(name, go);
            return go;

        }
        public void AddGameObject(string name, GameObject gameObject)
        {
            name = GetFreeName(name);
            if (!SceneObjects.Keys.Contains(name))
                SceneObjects.Add(name, gameObject);
        }
        private string GetFreeName(string name)
        {
            if (!IsGameObject(name))
                return name;
            var i = 1;
            while (IsGameObject($"{name} ({i})"))
                i++;
            return $"{name} ({i})";

        }
        public bool IsGameObject(string name)
        {
            return SceneObjects.Keys.Contains(name);
        }
        public abstract void Load();

        public void Initialize()
        {
            
        }

        public void Unload()
        {
            foreach (var go in SceneObjects)
                go.Value.Destroy();
        }
    }
}
