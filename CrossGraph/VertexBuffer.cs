#if OPENTK
using OpenTK.Graphics.OpenGL;
#elif PSM
using VBuffer = Sce.PlayStation.Core.Graphics.VertexBuffer;
using VFormat = Sce.PlayStation.Core.Graphics.VertexFormat;
using GContext = Sce.PlayStation.Core.Graphics.GraphicsContext;
using DMode = Sce.PlayStation.Core.Graphics.DrawMode;
#endif
using CrossGraph.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CrossGraph
{
    public enum PrimitiveType{
        Triangles, TriangleStrip, Lines, LineStrip, Points
    }

    public struct Vertex
    {
        public Vector3 Position, Normal;
        public Vector2 TexCoord;
        public Vector4 Color;

        public static readonly int Stride = Marshal.SizeOf(default(Vertex));
    }

    public class VertexBuffer
    {
#if OPENTK
        uint[] BufIds;
        ushort[] Indices;
#elif PSM
		VBuffer buff;
		Vertex[] Vertices;
		ushort[] Indices;
		public static GContext graphics;
#endif
        public VertexBuffer()
        {
#if OPENTK
            BufIds = new uint[1];
            GL.GenBuffers(BufIds.Length, BufIds);
#endif
        }

        public void SetVertices(Vertex[] Data)
        {
#if OPENTK
            GL.BindBuffer(BufferTarget.ArrayBuffer, BufIds[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(Data.Length * Vertex.Stride), Data, BufferUsageHint.StaticDraw);
#elif PSM
			Vertices = Data;
			if(Indices == null)
			{
				Indices = new ushort[Data.Length];
				ushort i = 0;
				while(i < Data.Length)
				{
					Indices[i] = i++;	
				}
			}
#endif
        }

        public void SetIndices(ushort[] data)
        {
#if OPENTK
            Indices = data;
#elif PSM
			Indices = data;
#endif
        }

        public void Draw(PrimitiveType type, int first, int count)
        {
#if OPENTK
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.IndexArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, BufIds[0]);
            GL.VertexPointer(3, VertexPointerType.Float, Vertex.Stride, new IntPtr(0));
            GL.NormalPointer(NormalPointerType.Float, Vertex.Stride, new IntPtr(Marshal.SizeOf(default(Vector3))  ));
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.Stride, new IntPtr(2 * Marshal.SizeOf(default(Vector3)) ));

            if (Indices != null) GL.DrawElements((BeginMode)Enum.Parse(typeof(BeginMode), type.ToString(), true), count, DrawElementsType.UnsignedShort, Indices);
            else GL.DrawArrays((BeginMode)Enum.Parse(typeof(BeginMode), type.ToString(), true), first, count);
#elif PSM
			if(buff == null || (buff.IndexCount > Indices.Length | buff.VertexCount > Vertices.Length))
			{
				if(buff != null)buff.Dispose();
				buff = new VBuffer(Vertices.Length, Indices.Length,  
				                   VFormat.Float3, 
				                   VFormat.Float3, 
				                   VFormat.Float2,
				                   VFormat.Float4);
				buff.SetVertices(0, Vertices, 0, Vertex.Stride);
				buff.SetVertices(1, Vertices, Marshal.SizeOf(default(Vector3)), Vertex.Stride);
				buff.SetVertices(2, Vertices, 2 * Marshal.SizeOf(default(Vector3)), Vertex.Stride);
				buff.SetIndices(Indices);
				
				//Free up normal memory and force Garbage Collection
				Indices = null;
				Vertices = null;
				GC.Collect();
			}
			
			if(graphics != null)
			{
				graphics.SetVertexBuffer(0, buff);
				graphics.DrawArrays((DMode)Enum.Parse(typeof(DMode), type.ToString(), true), first, count);
			}else{
				throw new NullReferenceException("You need to set VertexBuffer.graphics to a valid GraphicsContext");	
			}
#endif
        }
    }
}
