using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OEngine.ComponentSystem
{
    public class Component : Attribute
    {
        public string Name { get; private set; }
        public Component(string name)
        {
            Name = name;
        }
    }
}
