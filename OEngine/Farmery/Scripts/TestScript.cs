using OEngine.ComponentSystem.Components;
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
            GameObject.Position += new OpenTK.Vector3(0.1f * deltaTime, 0f, 0f);
        }
    }
}
