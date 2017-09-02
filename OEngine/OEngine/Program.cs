using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Platform;
using OpenTK.Input;
using OpenTK;
using System.Reflection;
using System.IO;
using OpenGL.Managers;
using OpenGL.ComponentSystems;
using OpenGL.ComponentSystem;
using OpenGL.ComponentSystem.Components;
using Jitter.Collision;
using Jitter;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;

namespace OpenGL
{
    class Program
    {

        static int Width = 800, Height = 600;
        static int VWidth = 1280, VHeight = 720;
        static float ScaleX, ScaleY;
        static INativeWindow Window;
        static IGraphicsContext Context;
        static int Frames = 0;
        static DateTime LastFrameRateUpdate = DateTime.Now;

        static void InitGraphics()
        {
            var graphicsMode = new GraphicsMode(32, 24, 0, 8);
            var displayDevice = DisplayDevice.GetDisplay(DisplayIndex.First);
            
            Window = new NativeWindow(Width, Height, "Prova", GameWindowFlags.FixedWindow, graphicsMode, displayDevice);
            Context = new GraphicsContext(graphicsMode, Window.WindowInfo, 4, 4, GraphicsContextFlags.Default);
            Context.MakeCurrent(Window.WindowInfo);
            (Context as IGraphicsContextInternal).LoadAll();

            Window.Visible = true;

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Cw);

            //Tests Hardware capabilities
            var testInfos = new GetPName[]
            {
                GetPName.MaxCombinedTextureImageUnits,
                GetPName.MaxCubeMapTextureSize,
                GetPName.MaxDrawBuffers,
                GetPName.MaxFragmentUniformComponents,
                GetPName.MaxTextureImageUnits,
                GetPName.MaxTextureSize,
                GetPName.MaxVaryingFloats,
                GetPName.MaxVertexAttribs,
                GetPName.MaxVertexTextureImageUnits,
                GetPName.MaxVertexUniformComponents,
                GetPName.MaxViewportDims,
                GetPName.Stereo
            };
            Debugger.Log("Hardware Report:");
            foreach (var info in testInfos)
            {
                GL.GetInteger(info, out int v);
                Debugger.Log($"{info.ToString()}: {v}");
            }
            Debugger.Log("Hardware Report End");

        }

        static int CreateGLProgram()
        {
            var vertexShader = CreateShader(ShaderType.VertexShader, "BasicVertex.vert");
            var fragmentShader = CreateShader(ShaderType.FragmentShader, "BasicFragment.frag");
            int program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);
            Debugger.Log("PROGRAM: " + GL.GetShaderInfoLog(program));
            return program;

        }

        static int CreateShader(ShaderType type, string shaderName)
        {
            var shaderFile = Assembly.GetExecutingAssembly().GetManifestResourceStream($"OpenGL.Shaders.{shaderName}");
            string shaderSource = "";
            try
            {
                using (var stream = new StreamReader(shaderFile))
                    shaderSource = stream.ReadToEnd();
            } catch(Exception e)
            {
                throw new Exception($"Cannot open shader file for {shaderName}", e);
            }

            if (string.IsNullOrEmpty(shaderSource))
                throw new Exception($"{shaderName} file is empty");

            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, shaderSource);
            GL.CompileShader(shader);
            Debugger.Log("VERTEX_SHADER: " + GL.GetShaderInfoLog(shader));
            
            return shader;

        }


        static void MainLoop()
        {
            var open = true;
            var camera = new Camera();
            Window.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>((object sender, System.ComponentModel.CancelEventArgs e) =>
            {
                open = false;
            });
            Window.MouseDown += new EventHandler<MouseButtonEventArgs>((object sender, MouseButtonEventArgs e) =>
            {
                float normX = (2f * e.Position.X) / Width - 1f;
                float normY = 1f - (2f * e.Position.Y) / Height;
                var mouseNormalisedCoords = new Vector3(normX, normY, 1f);
                var rayClip = new Vector4(mouseNormalisedCoords, -1f);
                var rayEye = rayClip; //Se avessi la projection matrix dovrei moltiplicare per l'inverso della stessa
                //Ripristino Z e W
                rayEye.Z = -1f;
                rayEye.W = 0f;

                //Trasformo da Eye Coordinate a World Coordinates
                var rayWorld = (rayEye * camera.ViewMatrix.Inverted()).Xyz;
                rayWorld.Normalize();

                //ora RayWorld è testabile per controllare se collide con qualche Bouding Box
                Debugger.Log($"Ray [WorldCoordinates]: {rayWorld.ToString()}");
            });
            //Create a VBO
            var vertexes = new float[]
            {
               -.5f,.5f,0,
               .5f,.5f,0,
               .5f,-.5f,0,

              
            };

            var colors = new float[]
            {
                1f,0,0,
                0,1f,0,
                0,0,2f
            };


            uint vertexVBO = 0;
            GL.GenBuffers(1, out vertexVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexes.Length * 4, vertexes, BufferUsageHint.StaticDraw);

            uint colorVBO = 0;
            GL.GenBuffers(1, out colorVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, colorVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, colors.Length * 4, colors, BufferUsageHint.StaticCopy);


            uint vao = 0;
            GL.GenVertexArrays(1, out vao);
            GL.BindVertexArray(vao);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexVBO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, colorVBO);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);


            var glProgram = CreateGLProgram();

            var modelMatrixLocation = GL.GetUniformLocation(glProgram, "model");
            var viewMatrixLocation = GL.GetUniformLocation(glProgram, "view");

            var translationMatrix = new Matrix4(
                    1f, 0f, 0f, 0f, 
                    0f, 1f, 0f, 0f, 
                    0f, 0f, 1f, 0f, 
                    0f, 0f, 0f, 1f);
            
            DateTime lastExec = DateTime.Now;
            float speed = 0.1f;
            float posX = 0f;
            float posY = 0f;
            float scale = 0.5f;
           
            float rotation = 0;

            var quat = new Quaternion(Vector3.UnitZ,0);

           
            camera.Position = new Vector3(0.3f, 0.3f, 0f);
            while (open) {
                var elapsed = DateTime.Now - lastExec;
                lastExec = DateTime.Now;
                UpdateFrameRate();

                
                rotation+= speed * (float)elapsed.TotalSeconds;
               

                //Assegna la matrice
                translationMatrix = Matrix4.Identity;
                //translationMatrix[0, 3] = posX;
                //translationMatrix[1, 3] = posY;
                var scaleMatrix = Matrix4.Identity;
                scaleMatrix[0, 0] = scale;
                scaleMatrix[1, 1] = scale;
                scaleMatrix[2, 2] = scale;

                
                quat = Quaternion.FromAxisAngle(Vector3.UnitZ, 0);
                
                var rotationMatrix = Matrix4.CreateFromQuaternion(quat);
                

                var objectWorldMatrix = translationMatrix * scaleMatrix  * rotationMatrix;

                camera.Position = new Vector3(posX, posY, 0f);
                
                
             

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.UseProgram(glProgram);

                var cameraViewMatrix = camera.ViewMatrix;
                GL.UniformMatrix4(modelMatrixLocation, true, ref objectWorldMatrix);
                GL.UniformMatrix4(viewMatrixLocation, true, ref cameraViewMatrix);
                GL.BindVertexArray(vao);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
                
                Context.SwapBuffers();
                Window.ProcessEvents();
            }
        }
        
        static void UpdateFrameRate()
        {
            Frames++;
            if ((DateTime.Now - LastFrameRateUpdate).TotalSeconds >= 1)
            {
                Window.Title = $"{Frames} FPS";
                Frames = 0;
                LastFrameRateUpdate = DateTime.Now;
            }
        }

        static void _Main(string[] args)
        {
            Debugger.Init();
            Debugger.Log("Starting...");

            InitGraphics();
            MainLoop();

            Debugger.Log("Shutting down...");


           
        }

        public static float DegreeToRadian(float angle)
        {
            return (float)Math.PI * angle / 180.0f;
        }
        public static float RadianToDegree(float angle)
        {
            return angle * (180.0f / (float)Math.PI);
        }

        static void Main(string[] args)
        {
            Debugger.Init();
            DisplayManager.Initialize(true);
            DisplayManager.MainCamera = new Camera();
            EntityManager.Initialize();
           

            var texture1 = ResourceManager.RegisterTexture("sample", "D:\\Users\\Pictures\\sprite.png");
            var BgTexture = ResourceManager.RegisterTexture("background", "D:\\Users\\Pictures\\BackgroundProva.png");
            var torchTexture = ResourceManager.RegisterTexture("torch", "D:\\Users\\Pictures\\torch.png");
            texture1.Subdivide(1, 1);
            BgTexture.Subdivide(1, 1);
            torchTexture.Subdivide(1, 1);

            var testObject = EntityManager.CreateGameObject("test1");
            testObject.Scale = new Vector3(0.2f, 0.2f, 0.2f);
            testObject.AddComponent(new ComponentSystem.Components.Renderer2DComponent(texture1.Frames[0]));

            var testObject2 = EntityManager.CreateGameObject("test2");
            testObject2.Scale = new Vector3(.2f, .2f, 0.2f);
            testObject2.AddComponent(new ComponentSystem.Components.Renderer2DComponent(texture1.Frames[0]));
            testObject2.Position = new Vector3(0.3f, 0f, 0f);


            var torch = EntityManager.CreateGameObject("torch");
            torch.Position = new Vector3(1f, 0f, 0f);
            torch.AddComponent(new Renderer2DComponent(torchTexture.Frames[0]));
            torch.Parent = testObject2;
            var light = EntityManager.CreateGameObject("light");
            light.AddComponent(new ComponentSystem.Components.LightComponent(1f));
            light.Parent = torch;
            light.Position = new Vector3(-0.01f, .05f, 0f);


            var bgObject = EntityManager.CreateGameObject("bg");
            bgObject.Position = new Vector3(0f, 0f, 1f);
            bgObject.AddComponent(new ComponentSystem.Components.Renderer2DComponent(BgTexture.Frames[0]));
            bgObject.Scale = new Vector3(3f, 3f, 3f);
           


            var lastFrame = DateTime.Now;
            const float speed = 0.5f;
            float radius = 1f;
            float exp = (float)Math.E;
            var collision = new CollisionSystemSAP();
            var world = new World(collision);
            var shape = new BoxShape(1f, 1f, 1f);
            var body = new RigidBody(shape);
            var lc = light.GetComponent<LightComponent>();
            var random = new Random();
            while(!DisplayManager.CloseRequested)
            {

                float delta = (float)(DateTime.Now - lastFrame).TotalSeconds;
                lastFrame = DateTime.Now;
                DisplayManager.PreUpdate();
         

                //DEBUG
                if (InputManager.IsKeyDown(OpenTK.Input.Key.A))
                    testObject2.Position += new Vector3(-speed * delta,0,0);
                else if (InputManager.IsKeyDown(Key.D))
                    testObject2.Position += new Vector3(speed * delta, 0, 0);
                if (InputManager.IsKeyDown(Key.W))
                    testObject2.Position += new Vector3(0,speed * delta, 0);
                else if (InputManager.IsKeyDown(Key.S))
                    testObject2.Position += new Vector3(0, -speed * delta, 0);

                if (InputManager.IsKeyDown(Key.Up))
                    radius += speed * delta;
                else if (InputManager.IsKeyDown(Key.Down))
                    radius -= speed * delta;

                if (InputManager.IsKeyDown(Key.Left))
                    exp -= 10 * delta;
                else if (InputManager.IsKeyDown(Key.Right))
                    exp += 10 * delta;

                lc.Radius = (1f + random.Next(0, 40)  * delta);
                
                //radius += speed * delta;
                //if (radius > 10f)
                //    radius = 1f;
                //testObject2.GetComponent<LightComponent>().ChangeRadius(radius);
                //FINE DEBUG

                EntityManager.Update(1);
                DisplayManager.PostUpdate();
                InputManager.ResetPress();
            }
        }
    }
   

    
}
