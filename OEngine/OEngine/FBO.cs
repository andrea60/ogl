using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEngine
{
    public class FBO
    {
        public uint FramebufferID;
        public uint FramebufferOutputTexture;

        public FBO(uint framebufferID, uint framebufferTextureID)
        {
            FramebufferID = framebufferID;
            FramebufferOutputTexture = framebufferTextureID;
        }
    }
}
