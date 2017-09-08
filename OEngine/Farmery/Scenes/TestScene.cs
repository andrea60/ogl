using OpenTK;
using OEngine.ComponentSystem.Components;
using OEngine.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OEngine;
using Farmery.Scripts;

namespace Farmery.Scenes
{
    public class TestScene : Scene
    {
        public override void Load()
        {
            //World.RenderSystem.MainCamera.Position = new Vector3(-.5f, -.5f, 0f);
            var random = new Random();
            const int xWidth = 75, yWidth = 75;
            const float startX =-2f,  startY = 1;
            for (var i = 0; i < xWidth; i++)
                for (var j = 0; j < yWidth; j++)
                {
                    var name = $"tile_{i}_{j}";
                    CreateGameObject(name);
                    var tileIndex = random.Next(5, 7);
                    SceneObjects[name].AddComponent(new RenderTileComponent(ResourceManager.SceneTextures["main_tileset"].Frames[0, tileIndex]));
                   
                    SceneObjects[name].Scale = new Vector3(.05f, .05f, 0.05f);
                    SceneObjects[name].Position += new Vector3(startX + (i*.05f), startY - (j*.05f), 0f);
                    //SceneObjects[name].AddComponent(new Scripts.TestScript());
                }


            CreateGameObject("world").AddComponent(new CameraScript());

            //SceneObjects["tile2"].Scale = new Vector3(.1f, .1f, .1f);
            //SceneObjects["tile2"].Position += new Vector3(.3f, .2f, .2f);

            

        }
    }
}
