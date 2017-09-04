using OEngine.ComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEngine.Managers
{
    [Flags]
    public enum Managers
    {
        RENDERER = 1,
        SCRIPTS = 2
    }

    public interface IManager
    {
        void SubscribeComponent(BaseComponent component);
    }
}
