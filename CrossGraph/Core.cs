using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossGraph
{
    public class Core
    {
#if OPENTK
        public static void Initialize(object gcontext, Type type)
        {
            VFS.BaseType = type;
        }
#elif PSM
        public static void Initialize(Sce.PlayStation.Core.Graphics.GraphicsContext graphics, Type type)
        {
            VFS.BaseType = type;
            Texture.graphics = graphics;
            VertexBuffer.graphics = graphics;
        }
#endif
    }
}
