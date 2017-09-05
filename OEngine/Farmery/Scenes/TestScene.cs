using OpenTK;
using OEngine.ComponentSystem.Components;
using OEngine.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OEngine;

namespace Farmery.Scenes
{
    public class TestScene : Scene
    {
        public override void Load()
        {
            //World.RenderSystem.MainCamera.Position = new Vector3(-.5f, -.5f, 0f);
            const int xWidth = 10, yWidth = 10;
            const float startX = -2.5f, startY = 1f;
            for (var i = 0; i < xWidth; i++)
                for (var j = 0; j < yWidth; j++)
                {
                    var name = $"tile_{i}_{j}";
                    CreateGameObject(name);
                    SceneObjects[name].AddComponent(new Renderer2DComponent(ResourceManager.SceneTextures["main_tileset"].Frames[0, 0]));
                    SceneObjects[name].Scale = new Vector3(.1f, .1f, .1f);
                    SceneObjects[name].Position += new Vector3(startX + (i*.1f), startY - (j*.1f), 0f);
                    SceneObjects[name].AddComponent(new Scripts.TestScript());
                }
           

            CreateGameObject("tile2").AddComponent(new Renderer2DComponent(ResourceManager.SceneTextures["main_tileset"].Frames[0, 0]));
            SceneObjects["tile2"].Scale = new Vector3(.1f, .1f, .1f);
            SceneObjects["tile2"].Position += new Vector3(.3f, .2f, .2f);

            

        }
    }
}
