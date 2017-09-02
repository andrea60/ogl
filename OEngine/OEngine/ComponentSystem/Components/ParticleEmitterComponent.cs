using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL.ComponentSystem.Components
{
    public class ParticleEmitterComponent : BaseComponent
    {
        public override BaseComponent Clone()
        {
            throw new NotImplementedException();
        }

        public override void Initialize()
        {
            
        }

        public override bool MustUpdateEachFrame()
        {
            return true;
        }

        public override void OnDestroy()
        {
            
        }
    }
}
