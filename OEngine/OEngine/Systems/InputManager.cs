using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEngine
{
    public static class InputManager
    {
        public static bool MouseDown;
        public static Point MousePosition { get; set; }
        private static bool[] KeyPress = new bool[128];
        private static bool[] KeyDown = new bool[(int)OpenTK.Input.Key.LastKey];

        public static void Initialize(INativeWindow window)
        {
            window.MouseDown += OnMouseDown;
            window.MouseUp += OnMouseUp;
            window.KeyDown += OnKeyDown;
            window.KeyUp += OnKeyUp;
            window.KeyPress += OnKeyPress;
            window.MouseMove += OnMouseMove;
        }

        public static void ResetPress()
        {
            for (var i = 0; i < KeyPress.Length; i++)
                KeyPress[i] = false;
        }

        private static void OnMouseMove(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {
            MousePosition = e.Position;
        }

        private static void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            
            KeyPress[e.KeyChar] = true; 
        }

        private static void OnKeyUp(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            KeyDown[(int)e.Key] = false;
        }

        private static void OnKeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            KeyDown[(int)e.Key] = true;
        }

        private static void OnMouseUp(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            MouseDown = false;
        }

        private static void OnMouseDown(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            MouseDown = true;
        }

        public static bool IsKeyDown(OpenTK.Input.Key key)
        {
            return KeyDown[(int)key];
        }
    }
}
