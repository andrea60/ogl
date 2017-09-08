using OEngine.ComponentSystem.Components;
using OEngine.Tile;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OEngine.Managers
{
    public enum MatrixType
    {
        ModelMatrix,
        ViewMatrix,
        ProjectionMatrix
    }

    public class DisplayManager
    {
        private bool Debug;

        int Width = 1280, Height =720;
        readonly int VWidth = 1280, VHeight = 720;
        Vector2 Scale;

        INativeWindow Window;
        IGraphicsContext Context;
        int Frames = 0;
        DateTime LastFrameRateUpdate = DateTime.Now;

        public Camera MainCamera { get; set; } = new Camera();

        public bool CloseRequested = false;

        private TextureUnit CurrentMaxTexture = TextureUnit.Texture0;
        /// <summary>
        /// Gets or sets the ambient light. To disable lightning simply set to [1f,1f,1f]
        /// </summary>
        public Vector3 AmbientLight = new Vector3(1f,1f,1f);

        private FBO MainFBO;
        private FBO LightningFBO;


        private float[] SceneVertex;
        private float[] SceneUV;
        private uint SceneVertexVBO;
        private uint SceneTextureVBO;
        private uint SceneVAO;

        private GLProgram BlenderProgram, LightProgram, MainProgram;
        public GLProgram CurrentProgram;

        private List<Renderer2DComponent> Sprites = new List<Renderer2DComponent>();
        private List<LightComponent> Lights = new List<LightComponent>();

        private List<List<TileChunk>> TileChunks = new List<List<TileChunk>>();

        //PROVE
        float[] VertexArray = new float[24*10000];
        GLProgram BatchingProgram;
        GCHandle VertexArrayHandle;
        uint VAVBO, UVVBO;
        uint VAVAO;

        public void Initialize(bool debug)
        {
            VertexArrayHandle = GCHandle.Alloc(VertexArray, GCHandleType.Pinned);
            Debug = debug;
            var graphicsMode = new GraphicsMode(32, 24, 0, 8);
            var displayDevice = DisplayDevice.GetDisplay(DisplayIndex.First);

            Window = new NativeWindow(Width, Height, "Prova", GameWindowFlags.FixedWindow,graphicsMode, displayDevice);
            
            Context = new GraphicsContext(graphicsMode, Window.WindowInfo, 4, 4, Debug ? GraphicsContextFlags.Debug : GraphicsContextFlags.Default);
            Context.MakeCurrent(Window.WindowInfo);
            (Context as IGraphicsContextInternal).LoadAll();

            Window.Visible = true;

            float tas = VWidth / (float)VHeight;
            int width = Width;
            int height = Convert.ToInt32(Width / tas);

            float ScaleY = 1;
            float ScaleX = Height / (float)Width;
            ScaleX *= VHeight / (float)Height;
            ScaleY *= VHeight / (float)Height;
            Scale = new Vector2(ScaleX, ScaleY);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Cw);

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            
            MainProgram = new OEngine.GLProgram(CreateGLProgram("Basic"));
            MainProgram.AddUniform("view", UniformType.Matrix4);
            MainProgram.AddUniform("model", UniformType.Matrix4);
            MainProgram.AddUniform("stretch_scale", UniformType.Vector2);
            //GLProgram = CreateGLProgram("Basic");
            BlenderProgram = new OEngine.GLProgram(CreateGLProgram("Blender"));

            BlenderProgram.AddUniform("first_tex", UniformType.Integer);
            BlenderProgram.AddUniform("second_tex", UniformType.Integer);


            //var GLBlenderProgram = CreateGLProgram("Blender");
            LightProgram = new OEngine.GLProgram(CreateGLProgram("Light"));
            LightProgram.AddUniform("light_center", UniformType.Vector3);
            LightProgram.AddUniform("t_mat", UniformType.Matrix4);
            LightProgram.AddUniform("s_mat", UniformType.Matrix4);
            LightProgram.AddUniform("view", UniformType.Matrix4);
            LightProgram.AddUniform("light_color", UniformType.Vector3);
            LightProgram.AddUniform("light_intensity", UniformType.Float);
            LightProgram.AddUniform("stretch_scale", UniformType.Vector2);
            //GLLightingProgram = CreateGLProgram("Light");

            BatchingProgram = new GLProgram(CreateGLProgram("Batching"));
            BatchingProgram.AddUniform("stretch_scale", UniformType.Vector2);
            BatchingProgram.AddUniform("view", UniformType.Matrix4);
            BatchingProgram.AddUniform("scale", UniformType.Matrix4);


            Debugger.Log($"Program Info log: {GL.GetProgramInfoLog(MainProgram.GLID)}");
            Debugger.Log($"Program Info log: {GL.GetProgramInfoLog(BlenderProgram.GLID)}");
            Debugger.Log($"Program Info log: {GL.GetProgramInfoLog(LightProgram.GLID)}");

            SwitchProgram(BlenderProgram);
            BlenderProgram.UniformValue("first_tex", 0f);
            BlenderProgram.UniformValue("second_tex", 1f);

            //var f_tex = GL.GetUniformLocation(GLBlenderProgram, "first_tex");
            //var s_tex = GL.GetUniformLocation(GLBlenderProgram, "second_tex");
            //GL.Uniform1(f_tex, 0);
            //GL.Uniform1(s_tex, 1);


            SwitchProgram(MainProgram);

            var prova = 0;
            GL.GetProgram(MainProgram.GLID, GetProgramParameterName.ActiveUniforms, out prova);


            if (Debug)
            {
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

                var param = new IntPtr(-1);                
                //GL.Khr.DebugMessageCallback(new DebugProcKhr(DebugCallback), param);

                //GL.DebugMessageCallback(new DebugProc(DebugCallback), param);
                GL.Enable(EnableCap.DebugOutputSynchronous);
                Debugger.Log($"** DEBUG MODE ENABLED **");
            }

            var (frameBuffer,frameBufferTex) = GenerateFrameBuffer();
            MainFBO = new FBO(frameBuffer, frameBufferTex);

            (frameBuffer, frameBufferTex) = GenerateFrameBuffer();
            LightningFBO = new FBO(frameBuffer, frameBufferTex);
            

            SceneVertex = new float[]
            {
                -1,1,
                1,-1,
                -1,-1,
                -1,1,
                1,1,
                1,-1
            };

            SceneUV = new float[]
            {
                0,1,
                1,0,
                0,0,
                0,1,
                1,1,
                1,0
            };

            GL.GenBuffers(1, out SceneVertexVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, SceneVertexVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, 4 * SceneVertex.Length, SceneVertex, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1,out SceneTextureVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, SceneTextureVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, 4 * SceneUV.Length, SceneUV, BufferUsageHint.StaticDraw);

            GL.GenVertexArrays(1, out SceneVAO);
            GL.BindVertexArray(SceneVAO);

            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, SceneVertexVBO);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);

            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, SceneTextureVBO);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);



            BindInputs();

          
            int vp_x = (Width / 2) - (width / 2);
            int vp_y = (Height / 2) - (height / 2);
            GL.Viewport(0, 0, Width, Height);

            //PROVE
            var test = new float[]
            {
               0,0,0,
               0,0,

               0,0,0,
               0,0,

               0,0,0,
               0,0,

               0,0,0,
               0,0,

               0,0,0,
               0,0,

               0,0,0,
               0,0
            };

            var uvs = new float[]
            {
                0,1,
                1,0,
                0,0,
                0,1,
                1,1,
                1,0
            };
            GL.GenBuffers(1, out VAVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VAVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * test.Length, test, BufferUsageHint.StaticDraw);

            
            GL.GenBuffers(1, out UVVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, UVVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * uvs.Length, uvs, BufferUsageHint.StaticDraw);

            GL.GenVertexArrays(1, out VAVAO);
            GL.BindVertexArray(VAVAO);

            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VAVBO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, UVVBO);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);

         
        }

        //private static int LightCenterPos = 0;
        //private static int LightMM = 0, LightVM = 0;
        private (uint,uint) GenerateFrameBuffer()
        {
            uint fb = 0;
            GL.GenFramebuffers(1, out fb);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fb);

            uint fb_tex = 0;
            GL.GenTextures(1, out fb_tex);
            GL.BindTexture(TextureTarget.Texture2D, fb_tex);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, new IntPtr(0));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);

            int rb = 0;
            GL.GenRenderbuffers(1, out rb);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rb);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, Width, Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rb);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fb);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, fb_tex, 0);


            var drawBufs = new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0 };
            GL.DrawBuffers(1, drawBufs);

            Debugger.Log($"Frame Buffer status: {GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer).ToString()}");
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            return (fb,fb_tex);
        }

        private void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            Debugger.Log($"Source: {source.ToString()}, Type: {type.ToString()}, ID: {id}, Severity: {severity.ToString()}, Message: {Marshal.PtrToStringAuto(message, length)}");
        }

        void BindInputs()
        {
            InputManager.Initialize(Window);
        }

        int CreateGLProgram(string name)
        {
            var vertexShader = CreateShader(ShaderType.VertexShader, $"{name}Vertex.vert");
            var fragmentShader = CreateShader(ShaderType.FragmentShader, $"{name}Fragment.frag");
            int program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            Debugger.Log($"(Program: {name}) Vertex Shader: {GL.GetShaderInfoLog(vertexShader)}");
            GL.AttachShader(program, fragmentShader);
            Debugger.Log($"(Program: {name}) Fragment Shader: {GL.GetShaderInfoLog(fragmentShader)}");
            GL.LinkProgram(program);
            
            return program;

        }

        int CreateShader(ShaderType type, string shaderName)
        {
            var shaderFile = Assembly.GetExecutingAssembly().GetManifestResourceStream($"OEngine.Shaders.{shaderName}");
            string shaderSource = "";
            try
            {
                using (var stream = new StreamReader(shaderFile))
                    shaderSource = stream.ReadToEnd();
               
            }
            catch (Exception e)
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
        void UpdateFrameRate()
        {
            Frames++;
            if ((DateTime.Now - LastFrameRateUpdate).TotalSeconds >= 1)
            {
                Window.Title = $"{Frames} FPS";
                Frames = 0;
                LastFrameRateUpdate = DateTime.Now;
            }
        }

        void SwitchProgram(GLProgram program)
        {
            CurrentProgram = program;
            CurrentProgram.Use();
        }

        public void PreUpdate()
        {
            UpdateFrameRate();
            SwitchProgram(MainProgram);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, MainFBO.FramebufferID);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            GL.Enable(EnableCap.Blend);
            GL.DepthMask(false);

            var cameraViewMatrix = MainCamera.ViewMatrix;
            CurrentProgram.UniformValue("view", MainCamera.ViewMatrix);
            CurrentProgram.UniformValue("stretch_scale", Scale);
        }
        bool first = true;
        float[] UVArray;

        public void Update(float deltaTime)
        {
            SwitchProgram(BatchingProgram);
            CurrentProgram.UniformValue("stretch_scale", Scale);
            CurrentProgram.UniformValue("view", MainCamera.ViewMatrix);
            CurrentProgram.UniformValue("scale", MainCamera.ZoomMatrix);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, ResourceManager.SceneTextures["main_tileset"].Frames[0,0].TextureImage.TextureID);
            foreach (var chunk in TileChunks)
                chunk.Draw();


          
           

        }

        private void RenderSprite(Renderer2DComponent sprite)
        {

          
            sprite.TranslationMatrix[0, 3] = sprite.GameObject.Position.X;
            sprite.TranslationMatrix[1, 3] = sprite.GameObject.Position.Y;
            sprite.TranslationMatrix[2, 3] = sprite.GameObject.Position.Z;
            var RotationMatrix = Matrix4.CreateFromQuaternion(sprite.GameObject.Rotation);
            var ScaleMatrix = sprite.ScaleMatrix;
            ScaleMatrix[0, 0] = sprite.GameObject.Scale.X;
            ScaleMatrix[1, 1] = sprite.GameObject.Scale.Y;
            ScaleMatrix[2, 2] = sprite.GameObject.Scale.Z;

            var modelMatrix = sprite.TranslationMatrix * ScaleMatrix;
            World.RenderSystem.CurrentProgram.UniformValue("model", modelMatrix);
            GL.BindVertexArray(sprite.VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            


        }


        public void PostUpdate()
        {
            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);

            //Draw the lightning mask on the other FBO
            
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, LightningFBO.FramebufferID);
            SwitchProgram(LightProgram);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(AmbientLight.X,AmbientLight.Y,AmbientLight.Z, 1f);
            GL.Enable(EnableCap.Blend);
            GL.DepthMask(false);

            GL.BindVertexArray(SceneVAO);
            
            var cameraViewMatrix = MainCamera.ViewMatrix;
            CurrentProgram.UniformValue("view", MainCamera.ViewMatrix);
            CurrentProgram.UniformValue("stretch_scale", Scale);
           
            foreach (var light in Lights)
                light.Update(0);


            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);
            //Draw the Texture on the main frambuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            SwitchProgram(BlenderProgram);
          

            GL.BindVertexArray(SceneVAO);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, LightningFBO.FramebufferOutputTexture);
            GL.ActiveTexture(TextureUnit.Texture1);
            
            GL.BindTexture(TextureTarget.Texture2D, MainFBO.FramebufferOutputTexture);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);


          
            Context.SwapBuffers();
            
            //while(true)
            Window.ProcessEvents();
            var error = GL.GetError();
            if (error != ErrorCode.NoError)
                Debugger.Log($"Error: {error.ToString()}");
        }

        

        public Vector3 ScreenToWorldCoordinates(Vector2 screenCoordinates)
        {
            float normX = (2f * screenCoordinates.X) / Width - 1f;
            float normY = 1f - (2f * screenCoordinates.Y) / Height;
            var mouseNormalisedCoords = new Vector3(normX, normY, 1f);
            var rayClip = new Vector4(mouseNormalisedCoords, -1f);
            var rayEye = rayClip; //Se avessi la projection matrix dovrei moltiplicare per l'inverso della stessa
                                  //Ripristino Z e W
            rayEye.Z = -1f;
            rayEye.W = 0f;

            //Trasformo da Eye Coordinate a World Coordinates
            var rayWorld = (rayEye * MainCamera.ViewMatrix.Inverted()).Xyz;
            rayWorld.Normalize();

            
            return rayWorld;
        }

        public void Shutdown()
        {

        }

        public TextureImage LoadGLTexture(byte[] image, int width, int height)
        {
            
            
            uint tex;
            GL.GenTextures(1, out tex);
            GL.ActiveTexture(CurrentMaxTexture);
            GL.BindTexture(TextureTarget.Texture2D,tex);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,(int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
            CurrentMaxTexture = (TextureUnit)((int)CurrentMaxTexture) + 1;
            return new TextureImage
            {
                GLTextureUnit = CurrentMaxTexture,
                Width = width,
                Height = height,
                TextureID = tex
            };
        }



        public void Subscribe(Renderer2DComponent component)
        {
            Sprites.Add(component);
        }
        public void Subscribe(LightComponent component)
        {
            Lights.Add(component);
        }

        public void Unsubscribe(Renderer2DComponent component)
        {
            Sprites.Remove(component);
        }
        public void Unsubscribe(LightComponent component)
        {
            Lights.Remove(component);
        }

        public void Subscribe(RenderTileComponent component)
        {
            int x = component.TileMapX;
            int y = component.TileMapY;

            int chunkX = x / TileChunk.ChunkSize;
            int chunkY = y / TileChunk.ChunkSize;

            if (chunkX > TileChunks.Count)
                TileChunks.Insert(chunkX,new List<TileChunk>());
            if (chunkY > TileChunks[chunkX].Count)
                TileChunks[chunkX].Insert(chunkY, new TileChunk());
            TileChunks[chunkX][chunkY].AddTile(component, x, y);

        }

        public void Unsubscribe(RenderTileComponent component)
        {
            int x = component.TileMapX;
            int y = component.TileMapY;

            int chunkX = x / TileChunk.ChunkSize;
            int chunkY = y / TileChunk.ChunkSize;

            if (chunkX > TileChunks.Count)
                TileChunks.Insert(chunkX, new List<TileChunk>());
            if (chunkY > TileChunks[chunkX].Count)
                TileChunks[chunkX].Insert(chunkY, new TileChunk());
            TileChunks[chunkX][chunkY].RemoveTile(x, y);

        }


    }
}
