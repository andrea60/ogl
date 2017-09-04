using Farmery.Scenes;
using OEngine;
using OEngine.ComponentSystems;
using OEngine.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmery
{
    public class Game
    {
        public void Init()
        {
            Debugger.Init();
            World.Initialize();
            ResourceManager.RegisterTexture("main_tileset", @"C:\Users\Andrea\Pictures\Tiles x32 test.png").Subdivide(16,8);

        }


        public void Run()
        {
            World.LoadScene(new TestScene());
            World.Run();
        }

        public void Stop()
        {

        }
    }
}
