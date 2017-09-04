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
            CreateGameObject("tile1").AddComponent(new Renderer2DComponent(ResourceManager.SceneTextures["main_tileset"].Frames[0, 5]));
            SceneObjects["tile1"].Scale = new Vector3(.1f, .1f, .1f);
            SceneObjects["tile1"].Position += new Vector3(.2f, .2f, .2f);
            SceneObjects["tile1"].AddComponent(new Scripts.TestScript());

            CreateGameObject("tile2").AddComponent(new Renderer2DComponent(ResourceManager.SceneTextures["main_tileset"].Frames[0, 5]));
            SceneObjects["tile2"].Scale = new Vector3(.1f, .1f, .1f);
            SceneObjects["tile2"].Position += new Vector3(.3f, .2f, .2f);

            

        }
    }
}
