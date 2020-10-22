using OpenTK.Graphics.OpenGL;
using RabbetGameEngine.SubRendering;
using System;

namespace RabbetGameEngine
{
    /*Abstraction of a VAO for use with rendering.*/
    public class VertexArrayObject
    {
        //TODO: (HIGHEST PRIO) impliment functionality of VertexArrayObject For all batch types.
        
        private int id;
        private int vbo;
        private int ibo;
        private bool dynamic = false;
        private bool usesIndices = true;
        private int maxVertices = 0;
        private bool hasInitialized = false;
        private BatchType batchType = BatchType.triangles;
        private PrimitiveType primType = PrimitiveType.Triangles;
        private Vertex[] vertices;
        private uint[] indices;

        /// <summary>
        /// This constructor is for use when creating a vertex array object for a STATIC (non-dynamic) render object
        /// </summary>
        /// <param name="type">The type of batch to combine this object to</param>
        /// <param name="vertices">The static vertices for this object</param>
        /// <param name="indices">The static indices for this object</param>
        private VertexArrayObject(PrimitiveType primType, Vertex[] vertices, uint[] indices) 
        {
            this.primType = primType;
            usesIndices = primType != PrimitiveType.Points || indices != null;
            this.batchType = BatchType.none;
            this.vertices = vertices;
            this.indices = indices;
            dynamic = false;
            initializeStatic();
        }

        /// <summary>
        /// This constructor is for use when creating a vertex array object for DYNAMIC render objects
        /// </summary>
        /// <param name="type">The type to batch this object into</param>
        /// <param name="maxVertices">Maximum number of vertices this VAO can hold.</param>
        private VertexArrayObject(BatchType type, int maxVertices)//for dynamic
        {
            this.batchType = type;
            this.maxVertices = maxVertices;
            dynamic = true;

            //get correct primitive type
            switch (batchType)
            {
                case BatchType.none:
                    break;
                case BatchType.triangles:
                    primType = PrimitiveType.Triangles;
                    break;
                case BatchType.trianglesTransparent:
                    primType = PrimitiveType.Triangles;
                    break;
                case BatchType.lines:
                    primType = PrimitiveType.Lines;
                    break;
                case BatchType.lerpTriangles:
                    primType = PrimitiveType.Triangles;
                    break;
                case BatchType.lerpTrianglesTransparent:
                    primType = PrimitiveType.Triangles;
                    break;
                case BatchType.lerpPointCloud:
                    primType = PrimitiveType.Points;
                    break;
                case BatchType.lerpPointCloudTransparent:
                    primType = PrimitiveType.Points;
                    break;
                case BatchType.lerpSinglePoint:
                    primType = PrimitiveType.Points;
                    break;
                case BatchType.lerpSinglePointTransparent:
                    primType = PrimitiveType.Points;
                    break;
                case BatchType.lerpLines:
                    primType = PrimitiveType.Lines;
                    break;
                default:
                    break;
            }
            initializeDynamic();
        }

        /// <summary>
        /// creates a VAO for use with dynamically updating and drawing the provided batch type
        /// </summary>
        /// /// <param name="type">The type of batch this VAO applies to</param>
        /// <param name="maxVertices">The maximum number of vertices this VAO can hold</param>
        /// <returns>a VAO object for use with dynamically updating and drawing the provided batch type</returns>
        public static VertexArrayObject createDynamic(BatchType type, int maxVertices)
        {
            return new VertexArrayObject(type, maxVertices);
        }

        /// <summary>
        /// creates a VAO for use with statically drawing triangles
        /// </summary>
        /// <param name="vertices">The static vertices</param>
        /// <param name="indices">The static indices</param>
        /// <returns>a VAO object for use with static drawing of triangles provided</returns>
        public static VertexArrayObject createStaticTriangles(Vertex[] vertices, uint[] indices)
        {
            return new VertexArrayObject(PrimitiveType.Triangles, vertices, indices);
        }

        /// <summary>
        /// creates a VAO for use with statically drawing quads
        /// </summary>
        /// <param name="vertices">The static vertices</param>
        /// <param name="indices">The static indices</param>
        /// <returns>a VAO object for use with static drawing of quads provided</returns>
        public static VertexArrayObject createStaticQuads(Vertex[] vertices, uint[] indices)
        {
            return new VertexArrayObject(PrimitiveType.Triangles, vertices, indices);
        }

        /// <summary>
        /// creates a VAO for use with statically drawing lines
        /// </summary>
        /// <param name="vertices">The static vertices</param>
        /// <param name="indices">The static indices</param>
        /// <returns>a VAO object for use with static drawing of lines provided</returns>
        public static VertexArrayObject createStaticLines(Vertex[] vertices, uint[] indices)
        {
            return new VertexArrayObject(PrimitiveType.Lines, vertices, indices);
        }

        /// <summary>
        /// creates a VAO for use with statically drawing Points
        /// </summary>
        /// <param name="vertices">The static vertices</param>
        /// <param name="indices">The static indices</param>
        /// <returns>a VAO object for use with static drawing of Points provided</returns>
        public static VertexArrayObject createStaticPoints(Vertex[] vertices)
        {
            return new VertexArrayObject(PrimitiveType.Points, vertices, null);
        }

        /// <summary>
        /// Can be called to initialize this vao with the provided data.
        /// </summary>
        private void initializeStatic()
        {
            if (hasInitialized) return;
            id = GL.GenVertexArray();
            GL.BindVertexArray(id);
            //do initialization of buffers here
            if (usesIndices)
            {
                ibo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            }
            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Vertex.vertexByteSize, vertices, BufferUsageHint.StaticDraw);
            /*Stride: the size in bytes between the start of one vertex to the start of another.
              Offset: the size in byts between the start of the vertex to the first bit of information for this specific attribute.*/
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, Vertex.positionLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.positionOffset);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, Vertex.colorLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.colorOffset);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, Vertex.uvLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.uvOffset);
            hasInitialized = true;
        }

        /// <summary>
        /// Can be called to initialize this vao with the provided data.
        /// Once initialized for the first time, this does not do anything.
        /// </summary>
        private void initializeDynamic()
        {
            if (hasInitialized) return;
            id = GL.GenVertexArray();
            GL.BindVertexArray(id);

            //do initialization of buffers here

            //These types require the standard VBO layout.
            if (batchType <= BatchType.lerpPointCloud || batchType == BatchType.lerpTrianglesTransparent || batchType == BatchType.trianglesTransparent || batchType == BatchType.lerpPointCloudTransparent)
            {
                vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, maxVertices * Vertex.vertexByteSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, Vertex.positionLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.positionOffset);

                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, Vertex.colorLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.colorOffset);

                GL.EnableVertexAttribArray(2);
                GL.VertexAttribPointer(2, Vertex.uvLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.uvOffset);
               
                GL.EnableVertexAttribArray(3);
                GL.VertexAttribPointer(3, Vertex.objectIDLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.objectIDOffset);
            }

            //these types require two vertex arrays, vertices and previous tick vertices.
            //the two vertex arrays will be in the same VBO, non interlaced.
            else if(batchType == BatchType.lerpSinglePoint || batchType == BatchType.lerpSinglePointTransparent)
            {
                vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, maxVertices * Vertex.vertexByteSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, Vertex.positionLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.positionOffset);

                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, Vertex.colorLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.colorOffset);

                GL.EnableVertexAttribArray(2);
                GL.VertexAttribPointer(2, Vertex.uvLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.uvOffset);

                GL.EnableVertexAttribArray(3);
                GL.VertexAttribPointer(3, Vertex.objectIDLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.objectIDOffset);
            }

            hasInitialized = true;
        }

        /// <summary>
        /// Must be called before using this VAO in a draw call.
        /// </summary>
        public void bind()
        {
            GL.BindVertexArray(id);
        }

        public bool isDynamic()
        {
            return dynamic;
        }

        /*deallocates this vao. Should be called when deconstructing any parent object.*/
        public void delete()
        {
            if(!hasInitialized)
            {
                return;
            }
            if (usesIndices)
            {
                GL.DeleteBuffer(ibo);
            }
            GL.DeleteBuffer(vbo);
            GL.DeleteVertexArray(id);
        }
    }
}
