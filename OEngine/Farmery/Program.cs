using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OEngine;
using OEngine.Managers;
namespace Farmery
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game();
            game.Init();
            game.Run();
            game.Stop();
            
        }
    }
}
