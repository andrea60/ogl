using OEngine.Managers;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace OEngine.ComponentSystem.Components
{
    [Component("LightComponent")]
    public class LightComponent : BaseComponent
    {
        private float[] Vertexes = new float[0];

        private uint VBO;
        private uint VAO;

        private Matrix4 TranslationMatrix;
        private Matrix4 ScaleMatrix;
        private Vector3 LightCenter = new Vector3(0,0,0);


        public Vector3 LightColor { get; set; } = new Vector3(0.7f, 0.5f, 0.5f);
        private float _Radius = 1f;
        public float Radius
        {
            get
            {
                return _Radius;
            }
            set
            {
                _Radius = value;
                ScaleMatrix = Matrix4.Mult(Matrix4.Identity, Radius);
                ScaleMatrix[3, 3] = 1;
            }
        }
        
        public float LightIntensity = 20;

        public LightComponent(float radius) : base(Managers.Managers.RENDERER)
        {
            Radius = radius;
        }
        public LightComponent() : base(Managers.Managers.RENDERER)
        {
            Radius = 1f;   
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

            ScaleMatrix = Matrix4.Identity;
            ScaleMatrix = Matrix4.Mult(Matrix4.Identity, Radius);
            ScaleMatrix[3, 3] = 1;
            TranslationMatrix = Matrix4.Identity;

            GL.GenBuffers(1, out VBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, 4 * Vertexes.Length, Vertexes, BufferUsageHint.StaticDraw);

            GL.GenVertexArrays(1, out VAO);
            GL.BindVertexArray(VAO);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

        }



        public override void Update(float deltaTime)
        {
            TranslationMatrix[0, 3] = GameObject.Position.X;
            TranslationMatrix[1, 3] = GameObject.Position.Y;
            TranslationMatrix[2, 3] = GameObject.Position.Z;


            World.RenderSystem.CurrentProgram.UniformValue("t_mat", TranslationMatrix);
            World.RenderSystem.CurrentProgram.UniformValue("s_mat", ScaleMatrix);
            World.RenderSystem.CurrentProgram.UniformValue("light_center", LightCenter);
            World.RenderSystem.CurrentProgram.UniformValue("light_color", LightColor);
            World.RenderSystem.CurrentProgram.UniformValue("light_intensity", LightIntensity);


            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

        }
       

        public override bool MustUpdateEachFrame()
        {
            return false;
        }

        public override BaseComponent Clone()
        {
            return new LightComponent
            {
                Radius = Radius,
                LightColor = LightColor,
                Vertexes = Vertexes,
                LightCenter = LightCenter,
                ScaleMatrix = ScaleMatrix,
                TranslationMatrix = TranslationMatrix,
                VBO = VBO,
                VAO = VAO
            };
        }

        public override void Subscribe()
        {
            World.RenderSystem.Subscribe(this);
        }
        public override void Unsubscribe()
        {
            World.RenderSystem.Unsubscribe(this);
        }

        public override void OnDestroy()
        {
     
        }
    }
  
}
