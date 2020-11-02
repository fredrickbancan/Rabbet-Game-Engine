using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.SubRendering;
using System;

namespace RabbetGameEngine
{
    /*Abstraction of a VAO for use with rendering.*/
    public class VertexArrayObject
    {
        private int vaoID;
        private int vboID;
        private int iboID;
        private int pboID;
        private int instVboID;
        private int matricesVboID;
        private int indirectBufferID;
        private bool dynamic = false;
        private bool usesIndices = true;
        private bool usesMatrices = false;
        private bool usesIndirect = false;

        /// <summary>
        /// True if this vao has a buffer for vec3 positions, i.e: 3d text. Not nececarially lerp
        /// </summary>
        private bool usesPositions = false;

        /// <summary>
        /// true if thi vao has a buffer for prev vec3 positions, i.e: lerp 3d text.
        /// </summary>
        private bool usesPrevPositions = false;

        private bool particleBased = false;
        private bool lerpParticles = false;
        private int bufferByteSize = 0;
        private bool hasInitialized = false;
        private Vector2[] spriteInstanceData = null;
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
            hasInitialized = true;
        }
        /// <summary>
        /// This constructor is for use when creating a vertex array object for a STATIC (non-dynamic) render object using points
        /// </summary>
        /// <param name="points">The points for this object</param>
        private VertexArrayObject(PointParticle[] points)
        {
            this.primType = PrimitiveType.TriangleStrip;
            usesIndices = false;
            this.batchType = BatchType.none;
            dynamic = false;
            particleBased = true;
            vaoID = GL.GenVertexArray();
            GL.BindVertexArray(vaoID);

            vboID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            GL.BufferData(BufferTarget.ArrayBuffer, points.Length * PointParticle.pParticleByteSize, points, BufferUsageHint.StaticDraw);

            int stride = PointParticle.pParticleByteSize;
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, PointParticle.positionLength, VertexAttribPointerType.Float, false, stride, PointParticle.positionOffset);
            GL.VertexAttribDivisor(0, 1);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, PointParticle.colorLength, VertexAttribPointerType.Float, false, stride, PointParticle.colorOffset);
            GL.VertexAttribDivisor(1, 1);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, PointParticle.radiusLength, VertexAttribPointerType.Float, false, stride, PointParticle.radiusOffset);
            GL.VertexAttribDivisor(2, 1);

            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, PointParticle.aocLength, VertexAttribPointerType.Float, false, stride, PointParticle.aocOffset);
            GL.VertexAttribDivisor(3, 1);

            spriteInstanceData = new Vector2[QuadPrefab.quadVertices.Length];
            for (int i = 0; i < QuadPrefab.quadVertices.Length; i++)
            {
                spriteInstanceData[i] = new Vector2(QuadPrefab.quadVertices[i].pos.X, QuadPrefab.quadVertices[i].pos.Y);
            }

            instVboID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, instVboID);
            GL.BufferData(BufferTarget.ArrayBuffer, spriteInstanceData.Length * sizeof(float) * 2, spriteInstanceData, BufferUsageHint.StaticDraw);
            //quad vertex positions (vector 2 F)
            GL.EnableVertexAttribArray(4);
            GL.VertexAttribPointer(4, 2, VertexAttribPointerType.Float, false, sizeof(float) * 2, 0);
            hasInitialized = true;
        }

      
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
                case BatchType.text3D:
                    primType = PrimitiveType.Triangles;
                    usesIndirect = true;
                    usesPositions = true;
                    break;

                case BatchType.triangles:
                case BatchType.trianglesTransparent:
                    primType = PrimitiveType.Triangles;
                    break;

                case BatchType.lerpTriangles:
                case BatchType.lerpTrianglesTransparent:
                    primType = PrimitiveType.Triangles;
                    usesMatrices = true;
                    usesIndirect = true;
                    break;

                case BatchType.lines:
                    primType = PrimitiveType.Lines;
                    break;

                case BatchType.lerpLines:
                    primType = PrimitiveType.Lines;
                    usesMatrices = true;
                    usesIndirect = true;
                    break;

                case BatchType.iSpheres:
                case BatchType.iSpheresTransparent:
                    primType = PrimitiveType.TriangleStrip;
                    this.spriteInstanceData = new Vector2[QuadPrefab.quadVertices.Length];
                    for (int i = 0; i < QuadPrefab.quadVertices.Length; ++i)
                    {
                        spriteInstanceData[i] = new Vector2(QuadPrefab.quadVertices[i].pos.X, QuadPrefab.quadVertices[i].pos.Y);
                    }

                    usesIndices = false;
                    particleBased = true;
                    lerpParticles = false;
                    break;

                case BatchType.lerpISpheres:
                case BatchType.lerpISpheresTransparent:
                    primType = PrimitiveType.TriangleStrip;
                    this.spriteInstanceData = new Vector2[QuadPrefab.quadVertices.Length];
                    for (int i = 0; i < QuadPrefab.quadVertices.Length; ++i)
                    {
                        spriteInstanceData[i] = new Vector2(QuadPrefab.quadVertices[i].pos.X, QuadPrefab.quadVertices[i].pos.Y);
                    }

                    usesIndices = false;
                    particleBased = true;
                    lerpParticles = true;
                    break;
                     
                default:
                    break;
            }
            initializeDynamic();
            hasInitialized = true;
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
        public static VertexArrayObject createStaticPoints(PointParticle[] points)
        {
            return new VertexArrayObject(points);
        }

        /// <summary>
        /// Can be called to initialize this vao with the provided data.
        /// </summary>
        private void initializeStatic(Vertex[] vertices, uint[] indices)
        {
            if (hasInitialized) return;
            vaoID = GL.GenVertexArray();
            GL.BindVertexArray(vaoID);

            //do initialization of buffers here
            if (usesIndices)
            {
                iboID = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, iboID);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            }
            vboID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Vertex.vertexByteSize, vertices, BufferUsageHint.StaticDraw);
            /*Stride: the size in bytes between the start of one vertex to the start of another.
              Offset: the size in byts between the start of the vertex to the first bit of information for this specific attribute.*/
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, Vertex.positionLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.positionOffset);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, Vertex.colorLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.colorOffset);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, Vertex.uvLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.uvOffset);
           
            GL.EnableVertexAttribArray(4);
            GL.VertexAttribPointer(4, Vertex.objectIDLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.objectIDOffset);
            hasInitialized = true;
        }

        /// <summary>
        /// Can be called to initialize this vao with the provided data.
        /// Once initialized for the first time, this does not do anything.
        /// </summary>
        private void initializeDynamic()
        {
            if (hasInitialized) return;
            vaoID = GL.GenVertexArray();
            GL.BindVertexArray(vaoID);

            //do initialization of buffers here
            if (usesIndices)
            {
                iboID = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, iboID);
                GL.BufferData(BufferTarget.ElementArrayBuffer, bufferByteSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            }

            //These types require the standard VBO layout.
            if (!particleBased)
            {
                vboID = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
                GL.BufferData(BufferTarget.ArrayBuffer, bufferByteSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, Vertex.positionLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.positionOffset);

                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, Vertex.colorLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.colorOffset);

                GL.EnableVertexAttribArray(2);
                GL.VertexAttribPointer(2, Vertex.uvLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.uvOffset);
               
                GL.EnableVertexAttribArray(3);
                GL.VertexAttribPointer(3, Vertex.objectIDLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.objectIDOffset);
               
                if(usesMatrices)
                {
                    matricesVboID = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, matricesVboID);
                    GL.BufferData(BufferTarget.ArrayBuffer, bufferByteSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

                    int sizeOfMatrix = 16 * sizeof(float);
                    int stride = sizeOfMatrix * 2;
                    //modelMatrixRow0
                    GL.EnableVertexAttribArray(4);
                    GL.VertexAttribPointer(4, 4, VertexAttribPointerType.Float, false, stride, 0);
                    GL.VertexAttribDivisor(4, 1);
                    //modelMatrixRow1
                    GL.EnableVertexAttribArray(5);
                    GL.VertexAttribPointer(5, 4, VertexAttribPointerType.Float, false, stride, 16);
                    GL.VertexAttribDivisor(5, 1);
                    //modelMatrixRow2
                    GL.EnableVertexAttribArray(6);
                    GL.VertexAttribPointer(6, 4, VertexAttribPointerType.Float, false, stride, 32);
                    GL.VertexAttribDivisor(6, 1);
                    //modelMatrixRow3
                    GL.EnableVertexAttribArray(7);
                    GL.VertexAttribPointer(7, 4, VertexAttribPointerType.Float, false, stride, 48);
                    GL.VertexAttribDivisor(7, 1);

                    //prevTickModelMatrixRow0
                    GL.EnableVertexAttribArray(8);
                    GL.VertexAttribPointer(8, 4, VertexAttribPointerType.Float, false, stride, sizeOfMatrix +  0);
                    GL.VertexAttribDivisor(8, 1);
                    //prevTickModelMatrixRow1
                    GL.EnableVertexAttribArray(9);
                    GL.VertexAttribPointer(9, 4, VertexAttribPointerType.Float, false, stride, sizeOfMatrix + 16);
                    GL.VertexAttribDivisor(9, 1);
                    //prevTickModelMatrixRow2
                    GL.EnableVertexAttribArray(10);
                    GL.VertexAttribPointer(10, 4, VertexAttribPointerType.Float, false, stride, sizeOfMatrix + 32);
                    GL.VertexAttribDivisor(10, 1);
                    //prevTickModelMatrixRow3
                    GL.EnableVertexAttribArray(11);
                    GL.VertexAttribPointer(11, 4, VertexAttribPointerType.Float, false, stride, sizeOfMatrix + 48);
                    GL.VertexAttribDivisor(11, 1);
                }
                else if (usesPositions)
                {
                    pboID = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, pboID);
                    GL.BufferData(BufferTarget.ArrayBuffer, bufferByteSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    int stride;
                    if(usesPrevPositions)
                    {
                        stride = sizeof(float) * 3 * 2;
                        GL.EnableVertexAttribArray(4);
                        GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, stride, 0);
                        GL.VertexAttribDivisor(4, 1);

                        //prev pos ptr
                        GL.EnableVertexAttribArray(5);
                        GL.VertexAttribPointer(5, 3, VertexAttribPointerType.Float, false, stride, sizeof(float) * 3);
                        GL.VertexAttribDivisor(5, 1);
                    }
                    else
                    {
                        stride = sizeof(float) * 3;
                        GL.EnableVertexAttribArray(4);
                        GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, stride, 0);
                        GL.VertexAttribDivisor(4, 1);
                    }
                    
                }

                if (usesIndirect)
                {
                    indirectBufferID = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.DrawIndirectBuffer, indirectBufferID);
                    GL.BufferData(BufferTarget.DrawIndirectBuffer, bufferByteSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                }
            }
            else
            {
                
                vboID = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
                GL.BufferData(BufferTarget.ArrayBuffer, bufferByteSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                
                int stride = lerpParticles ? PointParticle.pParticleByteSize * 2 : PointParticle.pParticleByteSize;
                //current point data
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, PointParticle.positionLength, VertexAttribPointerType.Float, false, stride, PointParticle.positionOffset);
                GL.VertexAttribDivisor(0, 1);

                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, PointParticle.colorLength, VertexAttribPointerType.Float, false, stride, PointParticle.colorOffset);
                GL.VertexAttribDivisor(1, 1);

                GL.EnableVertexAttribArray(2);
                GL.VertexAttribPointer(2, PointParticle.radiusLength, VertexAttribPointerType.Float, false, stride, PointParticle.radiusOffset);
                GL.VertexAttribDivisor(2, 1);

                GL.EnableVertexAttribArray(3);
                GL.VertexAttribPointer(3, PointParticle.aocLength, VertexAttribPointerType.Float, false, stride, PointParticle.aocOffset);
                GL.VertexAttribDivisor(3, 1);

                if (lerpParticles)
                {
                    //previous tick point data
                    GL.EnableVertexAttribArray(4);
                    GL.VertexAttribPointer(4, PointParticle.positionLength, VertexAttribPointerType.Float, false, stride, PointParticle.pParticleByteSize + PointParticle.positionOffset);
                    GL.VertexAttribDivisor(4, 1);

                    GL.EnableVertexAttribArray(5);
                    GL.VertexAttribPointer(5, PointParticle.colorLength, VertexAttribPointerType.Float, false, stride, PointParticle.pParticleByteSize + PointParticle.colorOffset);
                    GL.VertexAttribDivisor(5, 1);

                    GL.EnableVertexAttribArray(6);
                    GL.VertexAttribPointer(6, PointParticle.radiusLength, VertexAttribPointerType.Float, false, stride, PointParticle.pParticleByteSize + PointParticle.radiusOffset);
                    GL.VertexAttribDivisor(6, 1);

                    GL.EnableVertexAttribArray(7);
                    GL.VertexAttribPointer(7, PointParticle.aocLength, VertexAttribPointerType.Float, false, stride, PointParticle.pParticleByteSize + PointParticle.aocOffset);
                    GL.VertexAttribDivisor(7, 1);
                    instVboID = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, instVboID);
                    GL.BufferData(BufferTarget.ArrayBuffer, spriteInstanceData.Length * sizeof(float) * 2, spriteInstanceData, BufferUsageHint.StaticDraw);
                    //quad vertex positions (vector 2 F)
                    GL.EnableVertexAttribArray(8);
                    GL.VertexAttribPointer(8, 2, VertexAttribPointerType.Float, false, sizeof(float) * 2, 0);
                }
                else
                {
                    instVboID = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, instVboID);
                    GL.BufferData(BufferTarget.ArrayBuffer, spriteInstanceData.Length * sizeof(float) * 2, spriteInstanceData, BufferUsageHint.StaticDraw);
                    //quad vertex positions (vector 2 F)
                    GL.EnableVertexAttribArray(4);
                    GL.VertexAttribPointer(4, 2, VertexAttribPointerType.Float, false, sizeof(float) * 2, 0);
                }

            }

            hasInitialized = true;
        }

        /// <summary>
        /// Must be called before using this VAO in a draw call.
        /// </summary>
        public void bindVAO()
        {
            GL.BindVertexArray(vaoID);
        }

        public void bindMatricesVBO()
        {
            if(usesMatrices)
            GL.BindBuffer(BufferTarget.ArrayBuffer, matricesVboID);
        }

        public void bindVaoVboIbo()
        {
            GL.BindVertexArray(vaoID);
            bindVBO();
            bindIBO();

        }

        public void bindVBO()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
        }

        public void bindIBO()
        {
            if(usesIndices)
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, iboID);
        }

        public void bindPBO()
        {
            if (usesPositions)
                GL.BindBuffer(BufferTarget.ArrayBuffer, pboID);
        }

        public void bindInstVBO()
        {
            if (particleBased)
                GL.BindBuffer(BufferTarget.ArrayBuffer, instVboID);
        }

        public void bindIndirectBufferObject()
        {
            if (usesIndirect)
                GL.BindBuffer(BufferTarget.DrawIndirectBuffer, indirectBufferID);
        }
        public void unbindIndirectBufferObject()
        {
            if (usesIndirect)
                GL.BindBuffer(BufferTarget.DrawIndirectBuffer, 0);
        }

        public void unbindVBO()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void unbindIBO()
        {
            if (usesIndices)
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void unbindAll()
        {
            unbindVBO();
            unbindIBO();
            unbindIndirectBufferObject();
            GL.BindVertexArray(0);
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
            unbindAll();
            if (usesIndices)
            {
                GL.DeleteBuffer(iboID);
            }

            if(usesMatrices)
            {
                GL.DeleteBuffer(matricesVboID);
            }

            if(usesIndirect)
            {
                GL.DeleteBuffer(indirectBufferID);
            }

            if(usesPositions)
            {
                GL.DeleteBuffer(pboID);
            }

            if(particleBased)
            {
                GL.DeleteBuffer(instVboID);
            }

            GL.DeleteBuffer(vboID);
            GL.DeleteVertexArray(vaoID);
        }
    }
}
