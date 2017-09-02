using OpenGL.ComponentSystem;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL.ComponentSystems
{
    public static class EntityManager
    {
        private static Dictionary<string, GameObject> GameObjects { get; set; } = new Dictionary<string, GameObject>();

        public static void Initialize()
        {

        }
        public static GameObject CreateGameObject(string name)
        {
           
            name = GetFreeName(name);

            var go = new GameObject(new Vector3(0,0,0), Quaternion.Identity);
            GameObjects.Add(name, go);
            return go;
        }
        public static GameObject CreateGameObject(string name, Vector3 position, Quaternion? rotation=null)
        {
            if (rotation == null)
                rotation = Quaternion.Identity;
            name = GetFreeName(name);

            var go = new GameObject(position,rotation.Value);
            GameObjects.Add(name, go);
            return go;

        }
        public static void AddGameObject(string name, GameObject gameObject)
        {
            name = GetFreeName(name);
            if (!GameObjects.Keys.Contains(name))
                GameObjects.Add(name,gameObject);
        }
        private static string GetFreeName(string name)
        {
            if (!IsGameObject(name))
                return name;
            var i = 1;
            while (IsGameObject($"{name} ({i})"))
                i++;
            return $"{name} ({i})";

        }
        public static bool IsGameObject(string name)
        {
            return GameObjects.Keys.Contains(name);
        }

        public static void Update(float deltaTime)
        {
            foreach (var gameObject in GameObjects)
                gameObject.Value.Update(deltaTime);
        }
    }
}
