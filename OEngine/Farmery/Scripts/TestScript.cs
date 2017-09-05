using OEngine;
using OEngine.ComponentSystem.Components;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmery.Scripts
{
    public class TestScript : ScriptComponent
    {

        public override void Update(float deltaTime)
        {
            if (InputManager.IsKeyDown(OpenTK.Input.Key.D))
                GameObject.Position += new OpenTK.Vector3(0.1f * deltaTime, 0f, 0f);
            else if (InputManager.IsKeyDown(OpenTK.Input.Key.A))
                GameObject.Position -= new OpenTK.Vector3(0.1f * deltaTime, 0f, 0f);
            if (InputManager.IsKeyDown(OpenTK.Input.Key.W))
                GameObject.Position += new OpenTK.Vector3(0f, .1f * deltaTime, 0f);
            else if (InputManager.IsKeyDown(OpenTK.Input.Key.S))
                GameObject.Position -= new OpenTK.Vector3(0f, .1f * deltaTime, 0f);

        }
    }
}
