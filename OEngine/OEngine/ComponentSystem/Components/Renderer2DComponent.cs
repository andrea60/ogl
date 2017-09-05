using OEngine.Managers;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEngine.ComponentSystem.Components
{
    [Component("Render2DComponent")]
    public class Renderer2DComponent : BaseComponent
    {
        private float[] Vertexes;
        private float[] Normals;
        public float[] TextureUV;
        private uint VertexVBO;
        //private uint NormalsVBO;
        public uint VAO;
        private uint TextureUVVBO;

        public Matrix4 TranslationMatrix;
        public Matrix4 ScaleMatrix;
        public Matrix4 RotationMatrix;
        private TextureFrame _Sprite;
        public TextureFrame Sprite {
            get
            {
                return _Sprite;
            }
            set
            {
                SetSprite(value);
            }
        }

        public Renderer2DComponent() : base(Managers.Managers.RENDERER)
        {
            TranslationMatrix = Matrix4.Identity;
            ScaleMatrix = Matrix4.Identity;
            RotationMatrix = Matrix4.Identity;
        }
        public Renderer2DComponent(TextureFrame texture) : base(Managers.Managers.RENDERER)
        {
            Sprite = texture;
        }
        public void SetSprite(TextureFrame sprite)
        {
            _Sprite = sprite;
            TextureUV = new float[]
            {
                sprite.UpLeft.X,sprite.UpLeft.Y,
                sprite.DownRight.X,sprite.DownRight.Y,
                sprite.DownLeft.X,sprite.DownLeft.Y,
                sprite.UpLeft.X,sprite.UpLeft.Y,
                sprite.UpRight.X,sprite.UpRight.Y,
                sprite.DownRight.X,sprite.DownRight.Y
            };
        }
        public override void Initialize()
        {
            Vertexes = new float[]
            {
               -.5f,.5f,0f,
               .5f,-.5f,0f,
               -.5f,-.5f,0f,
               -.5f,.5f,0f,
               .5f,.5f,0f,
               .5f,-.5f,0f
            };

            Normals = new float[]
            {
                0,0,1f,
                0,0,1f,
                0,0,1f,
                0,0,1f,
                0,0,1f,
                0,0,1f
            };


            GL.GenBuffers(1, out VertexVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertexes.Length * 4, Vertexes, BufferUsageHint.StaticDraw);

            //GL.GenBuffers(1, out NormalsVBO);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, NormalsVBO);
            //GL.BufferData(BufferTarget.ArrayBuffer, Normals.Length * 4, Normals, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out TextureUVVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, TextureUVVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, TextureUV.Length * 4, TextureUV, BufferUsageHint.StaticDraw);

            GL.GenVertexArrays(1, out VAO);
            GL.BindVertexArray(VAO);

            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexVBO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            //GL.EnableVertexAttribArray(1);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, NormalsVBO);
            //GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, TextureUVVBO);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);

            ScaleMatrix = Matrix4.Identity;
            RotationMatrix = Matrix4.Identity; 
            TranslationMatrix = Matrix4.Identity;
        }

        public override bool MustUpdateEachFrame()
        {
            return true;
        }

        public override void Update(float deltaTime)
        {

            //var cameraViewMatrix = DisplayManager.MainCamera.ViewMatrix;
            //GL.UniformMatrix4(DisplayManager.ViewMatrixLocation, true, ref cameraViewMatrix);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Sprite.TextureImage.TextureID);
            TranslationMatrix[0, 3] = GameObject.Position.X;
            TranslationMatrix[1, 3] = GameObject.Position.Y;
            TranslationMatrix[2, 3] = GameObject.Position.Z;
            RotationMatrix= Matrix4.CreateFromQuaternion(GameObject.Rotation);
            ScaleMatrix[0, 0] = GameObject.Scale.X;
            ScaleMatrix[1, 1] = GameObject.Scale.Y;
            ScaleMatrix[2, 2] = GameObject.Scale.Z;

            var modelMatrix = TranslationMatrix * ScaleMatrix * RotationMatrix;

            World.RenderSystem.CurrentProgram.UniformValue("model", modelMatrix);
            
            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0,6);
        }

        public override void OnDestroy()
        {
            
        }

        public override BaseComponent Clone()
        {
            var clone = new Renderer2DComponent
            {
                Sprite = Sprite
            };
            clone.Initialize();
            return clone;
        }

        public override void Subscribe()
        {
            World.RenderSystem.Subscribe(this);
        }
        public override void Unsubscribe()
        {
            World.RenderSystem.Unsubscribe(this);
        }
    }
}
