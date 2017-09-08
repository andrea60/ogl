using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OEngine.Managers;

namespace OEngine.ComponentSystem.Components
{
    public class ParticleEmitterComponent : BaseComponent
    {
        public ParticleEmitterComponent(Managers.Managers handler) : base(handler)
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
            throw new NotImplementedException();
        }

        public override void Unsubscribe()
        {
            throw new NotImplementedException();
        }
    }
}
