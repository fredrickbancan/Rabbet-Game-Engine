using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Linq;

namespace RabbetGameEngine
{
    //TODO: add methods for resizing dynamic buffers
    //TODO: add support for requesting certain vertex attrib layouts for vbos and instance bo
    public class VertexArrayObject
    {
        private int vaoID;

        private List<VertexBufferObject> vbos;

        private IndexBufferObject ibo;
        private IndirectBufferObject indbo;
        private InstanceBufferObject instbo;

        private bool usesIndices = false;
        private bool hasSetUpIndices = false;

        private bool usesIndirect = false;
        private bool hasSetUpIndirect = false;

        private bool usesInstancing = false;
        private bool hasSetUpInstancing = false;

        public VertexArrayObject()
        {
            vaoID = GL.GenVertexArray();
        }

        /// <summary>
        /// should be called before adding any objects to this vao.
        /// objects should be added in specific orders and accessed in using indexes into the list of vbos. Order is important!.
        /// </summary>
        public void beginBuilding()
        {
            GL.BindVertexArray(vaoID);
        }

        public void addIndicesBuffer(uint[] indices)
        {
            if (hasSetUpIndices) return;
            ibo = new IndexBufferObject();
            ibo.initStatic(indices);
            hasSetUpIndices = true;
            usesIndices = true;
        }

        public void addInstanceBuffer<T2>(T2[] data, int sizeOfType) where T2 : struct
        {
            if (hasSetUpInstancing) return;
            instbo = new InstanceBufferObject();
            instbo.init<T2>(data, sizeOfType);
            hasSetUpInstancing = true;
            usesInstancing = true;
        }

        public void addIndicesBufferDynamic(int initialCount)
        {
            if (hasSetUpIndices) return;
            ibo = new IndexBufferObject();
            ibo.initDynamic(initialCount * sizeof(uint));
            hasSetUpIndices = true; 
            usesIndices = true;
        }

        public void addIndirectBuffer(int initialCount)
        {
            if (hasSetUpIndirect) return;
            indbo = new IndirectBufferObject();
            indbo.init(initialCount * DrawCommand.sizeInBytes);
            hasSetUpIndirect = true;
            usesIndirect = true;
        }

        
        public void addBuffer<T2>(T2[] data, int sizeOfType) where T2 : struct
        {
            VertexBufferObject vbo = new VertexBufferObject();
            vbo.initStatic<T2>(data, sizeOfType);
            vbos.Add(vbo);
        }

        public void addBufferDynamic(int initialByteSize)
        {
            VertexBufferObject vbo = new VertexBufferObject();
            vbo.initDynamic(initialByteSize);
            vbos.Add(vbo);
        }

        /// <summary>
        /// should be called after adding all objects to this vao and before using this vao to render.
        /// </summary>
        public void finishBuilding()
        {
            //build vertex attrib pointers based on requested layouts, etc.
        }


        public void updateBuffer<T2>(int vboIndex, T2[] data, int sizeToUpdate) where T2 : struct
        {
            if (vboIndex < vbos.Count)
            {
                VertexBufferObject vbo = vbos.ElementAt(vboIndex);

                if (vbo.isDynamic)
                    vbo.updateBuffer<T2>(data, sizeToUpdate);
                else
                    Application.warn("VAO Could not update non-dynamic buffer at index: " + vboIndex);
            }
        }
        public void updateIndices(uint[] data, int count)
        {
            if (usesIndices)
            {
                if (ibo.isDynamic)
                    ibo.updateData(data, count);
                else
                    Application.warn("VAO Could not update non-dynamic index buffer");
            }
            else
            {
                Application.warn("VAO does not use indices, but has been asked to update it.");
            }
        }

        public void updateIndirectBuffer(DrawCommand[] data, int countToUpdate)
        {
            if (usesIndirect)
            {
                indbo.updateData(data, countToUpdate);
            }
            else
            {
                Application.warn("VAO does not use Indirect, but has been asked to update it.");
            }
        }

        public void bind()
        {
            GL.BindVertexArray(vaoID);
            if (usesIndices) ibo.bind();
            if (usesIndirect) indbo.bind();
            if (usesInstancing) instbo.bind();
            foreach(VertexBufferObject vb in vbos)
            {
                vb.bind();
            }
        }

        public void delete()
        {
            foreach(VertexBufferObject b in vbos)
            {
                b.delete();
            }

            if (usesIndices && hasSetUpIndices) ibo.delete();
            if (usesIndirect && hasSetUpIndirect) indbo.delete();
            if (usesInstancing && hasSetUpInstancing) instbo.delete();

            GL.DeleteVertexArray(vaoID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

      /*  private void initializeDynamic()
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

            vboID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            GL.BufferData(BufferTarget.ArrayBuffer, bufferByteSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            if (!iSphereBased)
            {

                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, Vertex.positionLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.positionOffset);

                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, Vertex.colorLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.colorOffset);

                GL.EnableVertexAttribArray(2);
                GL.VertexAttribPointer(2, Vertex.uvLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.uvOffset);

                if (usesMatrices)
                {
                    matricesVboID = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, matricesVboID);
                    GL.BufferData(BufferTarget.ArrayBuffer, bufferByteSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

                    int sizeOfMatrix = 16 * sizeof(float);
                    int stride = sizeOfMatrix * 2;
                    //modelMatrixRow0
                    GL.EnableVertexAttribArray(3);
                    GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, stride, 0);
                    GL.VertexAttribDivisor(3, 1);
                    //modelMatrixRow1
                    GL.EnableVertexAttribArray(4);
                    GL.VertexAttribPointer(4, 4, VertexAttribPointerType.Float, false, stride, 16);
                    GL.VertexAttribDivisor(4, 1);
                    //modelMatrixRow2
                    GL.EnableVertexAttribArray(5);
                    GL.VertexAttribPointer(5, 4, VertexAttribPointerType.Float, false, stride, 32);
                    GL.VertexAttribDivisor(5, 1);
                    //modelMatrixRow3
                    GL.EnableVertexAttribArray(6);
                    GL.VertexAttribPointer(6, 4, VertexAttribPointerType.Float, false, stride, 48);
                    GL.VertexAttribDivisor(6, 1);

                    //prevTickModelMatrixRow0
                    GL.EnableVertexAttribArray(7);
                    GL.VertexAttribPointer(7, 4, VertexAttribPointerType.Float, false, stride, sizeOfMatrix + 0);
                    GL.VertexAttribDivisor(7, 1);
                    //prevTickModelMatrixRow1
                    GL.EnableVertexAttribArray(8);
                    GL.VertexAttribPointer(8, 4, VertexAttribPointerType.Float, false, stride, sizeOfMatrix + 16);
                    GL.VertexAttribDivisor(8, 1);
                    //prevTickModelMatrixRow2
                    GL.EnableVertexAttribArray(9);
                    GL.VertexAttribPointer(9, 4, VertexAttribPointerType.Float, false, stride, sizeOfMatrix + 32);
                    GL.VertexAttribDivisor(9, 1);
                    //prevTickModelMatrixRow3
                    GL.EnableVertexAttribArray(10);
                    GL.VertexAttribPointer(10, 4, VertexAttribPointerType.Float, false, stride, sizeOfMatrix + 48);
                    GL.VertexAttribDivisor(10, 1);
                }
                else if (usesPositions)
                {
                    pboID = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, pboID);
                    GL.BufferData(BufferTarget.ArrayBuffer, bufferByteSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    int stride;
                    if (usesPrevPositions)
                    {
                        stride = sizeof(float) * 3 * 2;
                        GL.EnableVertexAttribArray(3);
                        GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, stride, 0);
                        GL.VertexAttribDivisor(3, 1);

                        //prev pos ptr
                        GL.EnableVertexAttribArray(4);
                        GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, stride, sizeof(float) * 3);
                        GL.VertexAttribDivisor(4, 1);
                    
                        if(usesScales)
                        {
                            GL.EnableVertexAttribArray(5);
                            GL.VertexAttribPointer(5, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);
                            GL.VertexAttribDivisor(5, 1);
                        }
                    }
                    else
                    {
                        stride = sizeof(float) * 3;
                        GL.EnableVertexAttribArray(3);
                        GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, stride, 0);
                        GL.VertexAttribDivisor(3, 1);

                        if (usesScales)
                        {
                            GL.EnableVertexAttribArray(4);
                            GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);
                            GL.VertexAttribDivisor(4, 1);
                        }
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
                if (lerpISphereBased)
                {
                    int stride = PointParticle.pParticleByteSize * 2;

                    //current point data
                    GL.EnableVertexAttribArray(0);
                    GL.VertexAttribPointer(0, PointParticle.positionLength, VertexAttribPointerType.Float, false, stride, PointParticle.positionOffset);


                    GL.EnableVertexAttribArray(1);
                    GL.VertexAttribPointer(1, PointParticle.colorLength, VertexAttribPointerType.Float, false, stride, PointParticle.colorOffset);


                    GL.EnableVertexAttribArray(2);
                    GL.VertexAttribPointer(2, PointParticle.radiusLength, VertexAttribPointerType.Float, false, stride, PointParticle.radiusOffset);


                    GL.EnableVertexAttribArray(3);
                    GL.VertexAttribPointer(3, PointParticle.aocLength, VertexAttribPointerType.Float, false, stride, PointParticle.aocOffset);

                    //previous tick point data
                    GL.EnableVertexAttribArray(4);
                    GL.VertexAttribPointer(4, PointParticle.positionLength, VertexAttribPointerType.Float, false, stride, PointParticle.pParticleByteSize + PointParticle.positionOffset);


                    GL.EnableVertexAttribArray(5);
                    GL.VertexAttribPointer(5, PointParticle.colorLength, VertexAttribPointerType.Float, false, stride, PointParticle.pParticleByteSize + PointParticle.colorOffset);


                    GL.EnableVertexAttribArray(6);
                    GL.VertexAttribPointer(6, PointParticle.radiusLength, VertexAttribPointerType.Float, false, stride, PointParticle.pParticleByteSize + PointParticle.radiusOffset);


                    GL.EnableVertexAttribArray(7);
                    GL.VertexAttribPointer(7, PointParticle.aocLength, VertexAttribPointerType.Float, false, stride, PointParticle.pParticleByteSize + PointParticle.aocOffset);

                }
                else
                {
                    int stride = PointParticle.pParticleByteSize;

                    //current point data
                    GL.EnableVertexAttribArray(0);
                    GL.VertexAttribPointer(0, PointParticle.positionLength, VertexAttribPointerType.Float, false, stride, PointParticle.positionOffset);
                    GL.VertexAttribDivisor(0,1);

                    GL.EnableVertexAttribArray(1);
                    GL.VertexAttribPointer(1, PointParticle.colorLength, VertexAttribPointerType.Float, false, stride, PointParticle.colorOffset);
                    GL.VertexAttribDivisor(1, 1);

                    GL.EnableVertexAttribArray(2);
                    GL.VertexAttribPointer(2, PointParticle.radiusLength, VertexAttribPointerType.Float, false, stride, PointParticle.radiusOffset);
                    GL.VertexAttribDivisor(2, 1);

                    GL.EnableVertexAttribArray(3);
                    GL.VertexAttribPointer(3, PointParticle.aocLength, VertexAttribPointerType.Float, false, stride, PointParticle.aocOffset);
                    GL.VertexAttribDivisor(3, 1);

                    instVboID = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, instVboID);
                    GL.BufferData(BufferTarget.ArrayBuffer, spriteInstanceData.Length * sizeof(float) * 2, spriteInstanceData, BufferUsageHint.StaticDraw);
                    //quad vertex positions (vector 2 F)
                    GL.EnableVertexAttribArray(4);
                    GL.VertexAttribPointer(4, 2, VertexAttribPointerType.Float, false, sizeof(float) * 2, 0);
                }
                
            }
            hasInitialized = true;
        }*/
    }
}
