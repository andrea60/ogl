using OEngine.ComponentSystem;
using OEngine.ComponentSystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEngine.Managers
{
    public class ScriptManager
    {
        private List<ScriptComponent> Scripts = new List<ScriptComponent>();
        public void Update(float deltaTime)
        {
            foreach (var script in Scripts)
                script.Update(deltaTime);
        }

        public void Subscribe(ScriptComponent component)
        {
            Scripts.Add(component);
        }
        public void Unsubscribe(ScriptComponent component)
        {
            Scripts.Remove(component);
        }

        
    }
}
