using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OEngine.Managers;

namespace OEngine.ComponentSystem.Components
{
    public abstract class ScriptComponent : BaseComponent
    {
      

        public ScriptComponent() : base(Managers.Managers.SCRIPTS)
        {
        }

      

        public override BaseComponent Clone()
        {
            throw new NotImplementedException();
        }

        public override void Init()
        {
            
        }

        public override bool MustUpdateEachFrame()
        {
            return true;
        }

        public override void OnDestroy()
        {
            
        }

        public override void Subscribe()
        {
            World.GameLogicSystem.Subscribe(this);
        }

        public override void Unsubscribe()
        {
            World.GameLogicSystem.Unsubscribe(this);
        }
    }
}
