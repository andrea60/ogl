using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenGL
{
    public class Camera
    {
        private Matrix4 TranslationMatrix = Matrix4.Identity;
        private Vector3 _Position = new Vector3(0f,0f,2f);

        public Vector3 Position { get
            {
                return _Position;
            }
            set
            {
                _Position = value;
                TranslationMatrix[0, 3] = -Position.X;
                TranslationMatrix[1, 3] = -Position.Y;
                TranslationMatrix[2, 3] = -Position.Z;
            }
        }

        public Matrix4 ViewMatrix
        {
            get
            {
                return TranslationMatrix; //aggiungere projection matrix e rotazioni se necessario
            }
        }

        public Matrix4 LookAt(Vector3 position)
        {
            return TranslationMatrix;
            var d = new Vector3(position.X - Position.X, position.Y - Position.Y, position.Z - Position.Z);
            var m = (float)Math.Sqrt(d.X * d.X + d.Y * d.Y + d.Z * d.Z);
            var f = new Vector3(d.X / m, d.Y / m, d.Z / m);

            var r = f * Vector3.UnitX;
            var u = r * f;

            var rotationMatrix = Matrix4.Identity;
            rotationMatrix[0, 0] = r.X;
            rotationMatrix[0, 1] = r.Y;
            rotationMatrix[0, 2] = r.Z;
            rotationMatrix[1, 0] = u.X;
            rotationMatrix[1, 1] = u.Y;
            rotationMatrix[1, 2] = u.Z;
            rotationMatrix[2, 0] = -f.X;
            rotationMatrix[2, 1] = -f.Y;
            rotationMatrix[2, 2] = -f.Z;
            
            return rotationMatrix * TranslationMatrix;

        }
    }
}
