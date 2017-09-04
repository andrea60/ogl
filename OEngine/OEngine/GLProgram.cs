using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK;

namespace OEngine
{
    public class GLProgram
    {
        public int GLID;
        private Dictionary<string, UniformValue> Uniforms = new Dictionary<string, UniformValue>();

        public GLProgram(int glID)
        {
            GLID = glID;
        }

        public void AddUniform(string name, UniformType type)
        {
            var id = GL.GetUniformLocation(GLID, name);
            if (Uniforms.Keys.Contains(name))
                throw new ArgumentException("The specified uniform name has already been added");
            if (id < 0)
            {
                var message = $"The specified uniform name does not exists in this program ({name})";
                Debugger.Log(message, Severity.Warning);
                //throw new ArgumentException(message);
            }
            Uniforms.Add(name, new UniformValue(id,type));
        }

        public void UniformValue(string uniformName, object value)
        {
            if (Uniforms.Keys.Contains(uniformName))
            {
                Uniforms[uniformName].SendValue(value);
            }
            else
                throw new ArgumentException("The specified uniform name is not valid or has never been bound");
        }

        public void Use()
        {
            GL.UseProgram(GLID);
        }

    }

    public enum UniformType
    {
        Matrix4,
        Matrix3,
        Float,
        Integer,
        Vector3,
        Vector2
    }

    public class UniformValue
    {
        public int GLID { get; set; }
        private UniformType Type;

        public UniformValue(int glID, UniformType type)
        {
            GLID = glID;
            Type = type;
        }

        public void SendValue(object value)
        {
            switch(Type)
            {
                case UniformType.Matrix4:
                    var matrix4 = (Matrix4)value;
                    GL.UniformMatrix4(GLID, true, ref matrix4);
                    break;
                case UniformType.Integer:
                    var i = Convert.ToInt32(value);
                    GL.Uniform1(GLID, i);
                    break;
                case UniformType.Float:
                    float f = Convert.ToSingle(value);
                    GL.Uniform1(GLID, f);
                    break;
                case UniformType.Matrix3:
                    var matrix3 = (Matrix3)value;
                    GL.UniformMatrix3(GLID, true, ref matrix3);
                    break;
                case UniformType.Vector3:
                    var vector3 = (Vector3)value;
                    GL.Uniform3(GLID, ref vector3);
                    break;
                case UniformType.Vector2:
                    var vector2 = (Vector2)value;
                    GL.Uniform2(GLID, ref vector2);
                    break;
            }
        }
    }

}
