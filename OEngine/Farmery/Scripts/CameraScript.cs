using OEngine;
using OEngine.ComponentSystem.Components;
using OEngine.Managers;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmery.Scripts
{
    public class CameraScript : ScriptComponent
    {
        public override void Update(float deltaTime)
        {
            var mouseDeltaX = InputManager.MousePosition.X / 1280f * 100;
            var mouseDeltaY = InputManager.MousePosition.Y / 720f * 100;
            
            if (mouseDeltaX < 5)
                World.RenderSystem.MainCamera.Position -= new Vector3(.1f * deltaTime, 0f, 0f);
            else if (mouseDeltaX > 95)
                World.RenderSystem.MainCamera.Position += new Vector3(.1f * deltaTime, 0f, 0f);
            if (mouseDeltaY < 5)
                World.RenderSystem.MainCamera.Position += new Vector3(0, .1f * deltaTime, 0f);
            else if (mouseDeltaY > 95)
                World.RenderSystem.MainCamera.Position -= new Vector3(0, .1f * deltaTime, 0f);
            else if (InputManager.IsKeyPress(OpenTK.Input.Key.Space))
                World.RenderSystem.MainCamera.Position = new Vector3(0f, 0f, 0f);

            if (InputManager.IsKeyPress(OpenTK.Input.Key.Z))
                World.RenderSystem.MainCamera.Zoom += 0.1f;
            else if (InputManager.IsKeyPress(OpenTK.Input.Key.X))
                World.RenderSystem.MainCamera.Zoom -= 0.1f;
        }
    }
}
