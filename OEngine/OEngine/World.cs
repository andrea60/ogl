using OEngine.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEngine
{
    public static class World
    {
        public static DisplayManager RenderSystem { get; internal set; }
        public static ScriptManager GameLogicSystem { get; internal set; } 
        public static Scene CurrentScene { get; internal set; }
        public static void Initialize()
        {
            RenderSystem = new DisplayManager();
            GameLogicSystem = new ScriptManager();

            RenderSystem.Initialize(true);

        }

        public static void LoadScene(Scene scene)
        {
            CurrentScene = scene;
            scene.Initialize();
            scene.Load();
        }
        public static void UnloadScene()
        {
            CurrentScene.Unload();
        }

        public static void Run()
        {
            var lastFrame = DateTime.Now;
            if (CurrentScene == null)
            {
                Debug.WriteLine("Warning: no scene loaded!");
            }
            while (!RenderSystem.CloseRequested)
            {
                float delta = (float)(DateTime.Now - lastFrame).TotalSeconds;
                lastFrame = DateTime.Now;
                RenderSystem.PreUpdate();
                RenderSystem.Update(delta);

                GameLogicSystem.Update(delta);
                
                RenderSystem.PostUpdate();
                InputManager.ResetPress();

            }
        }

    }
}
