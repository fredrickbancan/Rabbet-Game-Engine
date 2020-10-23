using OpenTK.Graphics.OpenGL;
using RabbetGameEngine.SubRendering;
using System;

namespace RabbetGameEngine
{
    /*Abstraction of a VAO for use with rendering.*/
    public class VertexArrayObject
    {
        private int id;
        private int vbo;
        private int ibo;
        private bool dynamic = false;
        private bool usesIndices = true;
        private int bufferByteSize = 0;
        private bool hasInitialized = false;
        private BatchType batchType = BatchType.triangles;
        private PrimitiveType primType = PrimitiveType.Triangles;

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
            dynamic = false;
            initializeStatic(vertices, indices);
        }

        /// <summary>
        /// This constructor is for use when creating a vertex array object for DYNAMIC render objects
        /// </summary>
        /// <param name="type">The type to batch this object into</param>
        /// <param name="
        /// 
        /// 
        /// 
        /// ">Maximum number of vertices this VAO can hold.</param>
        private VertexArrayObject(BatchType type, int bufferSize)//for dynamic
        {
            this.batchType = type;
            this.bufferByteSize = bufferSize;
            dynamic = true;

            //get correct primitive type
            switch (batchType)
            {
                case BatchType.none:
                    break;
                case BatchType.triangles:
                case BatchType.lerpTriangles:
                case BatchType.trianglesTransparent:
                case BatchType.lerpTrianglesTransparent:
                    primType = PrimitiveType.Triangles;
                    break;
                case BatchType.lines:
                case BatchType.lerpLines:
                    primType = PrimitiveType.Lines;
                    break;
                case BatchType.lerpPoints:
                case BatchType.lerpPointsTransparent:
                    primType = PrimitiveType.Points;
                    usesIndices = false;
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
        /// <param name="bufferSize">The maximum bytes this dynamic VAO can hold</param>
        /// <returns>a VAO object for use with dynamically updating and drawing the provided batch type</returns>
        public static VertexArrayObject createDynamic(BatchType type, int bufferSize)
        {
            return new VertexArrayObject(type, bufferSize);
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
        private void initializeStatic(Vertex[] vertices, uint[] indices)
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
            if (usesIndices)
            {
                ibo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, bufferByteSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            }

            //These types require the standard VBO layout.
            if (batchType != BatchType.lerpPoints && batchType != BatchType.lerpPointsTransparent)
            {
                vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, bufferByteSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, Vertex.positionLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.positionOffset);

                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, Vertex.colorLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.colorOffset);

                GL.EnableVertexAttribArray(2);
                GL.VertexAttribPointer(2, Vertex.uvLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.uvOffset);
               
                GL.EnableVertexAttribArray(3);
                GL.VertexAttribPointer(3, Vertex.objectIDLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.objectIDOffset);
            }

            //point types require two point arrays, points and previous tick points.
            //the two point arrays will be in the same VBO, non interlaced. The previous tick points will come after the current.
            else
            {
                vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, bufferByteSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, PointParticle.positionLength, VertexAttribPointerType.Float, false, PointParticle.pParticleByteSize, PointParticle.positionOffset);

                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, PointParticle.colorLength, VertexAttribPointerType.Float, false, PointParticle.pParticleByteSize, PointParticle.colorOffset);

                GL.EnableVertexAttribArray(2);
                GL.VertexAttribPointer(2, PointParticle.radiusLength, VertexAttribPointerType.Float, false, PointParticle.pParticleByteSize, PointParticle.radiusOffset);
                
                GL.EnableVertexAttribArray(3);
                GL.VertexAttribPointer(3, PointParticle.aocLength, VertexAttribPointerType.Float, false, PointParticle.pParticleByteSize, PointParticle.aocOffset);
               
                //previous tick point data
                //TODO: TEST AND CONFIRM PROPER RENDERING OF POINTS WITH LERP
                GL.EnableVertexAttribArray(4);
                GL.VertexAttribPointer(4, PointParticle.positionLength, VertexAttribPointerType.Float, false, PointParticle.pParticleByteSize, (bufferByteSize / 2) + PointParticle.positionOffset);

                GL.EnableVertexAttribArray(5);
                GL.VertexAttribPointer(5, PointParticle.colorLength, VertexAttribPointerType.Float, false, PointParticle.pParticleByteSize, (bufferByteSize / 2) + PointParticle.colorOffset);

                GL.EnableVertexAttribArray(6);
                GL.VertexAttribPointer(6, PointParticle.radiusLength, VertexAttribPointerType.Float, false, PointParticle.pParticleByteSize, (bufferByteSize / 2) + PointParticle.radiusOffset);

                GL.EnableVertexAttribArray(7);
                GL.VertexAttribPointer(7, PointParticle.aocLength, VertexAttribPointerType.Float, false, PointParticle.pParticleByteSize, (bufferByteSize / 2) + PointParticle.aocOffset);
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

        public void bindVBO()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        }

        public void bindIBO()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
        }


        public bool isDynamic()
        {
            return dynamic;
        }

        public bool doesUseIndices()
        {
            return usesIndices;
        }

        public PrimitiveType getPrimType()
        {
            return this.primType;
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
