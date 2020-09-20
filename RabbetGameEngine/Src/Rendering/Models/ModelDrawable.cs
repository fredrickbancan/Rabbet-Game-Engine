using OpenTK;
using OpenTK.Graphics.OpenGL;
using RabbetGameEngine.SubRendering;
using System;
using System.Collections.Generic;

namespace RabbetGameEngine.Models
{

    /*Base class for models that can be drawn individually with additional draw calls and have
     *individual VAO's.*/
    public class ModelDrawable : Model
    {
        /*ModelDrawables can have null indices. In which case the vertices will be drawn without the use of indices.
          In which case, a draw function which does not use indices must be utilized (e.g: drawPoints())*/
        public uint[] indices;
        protected bool hasInitialized = false;
        protected int indicesBufferObject;
        protected List<int> VBOS = new List<int>();
        protected int VAO;
        protected Texture texture;
        protected Shader shader;
        private bool drawErrorPrinted = false;

        /*takes in the shader and texture for this model, indices can be null if they wont be used*/
        public ModelDrawable(string shader, string texture, Vertex[] vertices, uint[] indices = null) : base(vertices)
        {
            ShaderUtil.tryGetShader(shader, out this.shader);
            TextureUtil.tryGetTexture(texture, out this.texture);
            this.indices = indices;
        }
        public ModelDrawable(Shader shader, Texture texture, Vertex[] vertices, uint[] indices = null) : base(vertices)
        {
            this.shader = shader;
            this.texture = texture;
            this.indices = indices;
        }

        public virtual void setIndices(uint[] newind)
        {
            indices = newind;
        }

        public virtual ModelDrawable setTexture(Texture tex)
        {
            this.texture = tex;
            return this;
        }

        public virtual void setShader(Shader shad)
        {
            this.shader = shad;
        }

        public virtual void setUniformVec3(string name, Vector3 vec)
        {
            shader.setUniformVec3F(name, vec);
        }

        /*called when model is first bound. can be used for more*/
        protected virtual void init()
        {
            genBuffers();
            hasInitialized = true;
        }

        /*Generates the attributes for each array of data and stores them in this model's VAO.*/
        protected virtual void genBuffers()
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            if (indices != null)
            {
                genAndBindIndices(); //for indices
            }
            storeVertexDataInAttributeList();
        }
        /*Unbinds this model so the renderer can render different things.*/
        public virtual void unBind()
        {
            /* GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
             GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
             GL.BindTexture(TextureTarget.Texture2D, 0);
             GL.UseProgram(0);
             GL.BindVertexArray(0);*/
        }

        /*Binds the indicie buffer to the VAO*/
        protected virtual void genAndBindIndices()
        {
            if (indices != null)
            {
                indicesBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            }
        }

        protected virtual void storeVertexDataInAttributeList()
        {
            int vbo = GL.GenBuffer();
            VBOS.Add(vbo);
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
        }

        /*Used for binding the vertex buffer object in the array at the given index, any vbo's get unbound in unbind()*/
        public virtual void bindVertexBuffer(int index = 0)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOS[index]);
        }

        /*Binds the models texture and shader, if its the first time binding, will gen buffers. Can be overwritten and used for more*/
        public virtual void bind(bool useTexture = true, bool useShader = true)
        {
            if (!hasInitialized)
            {
                init();
            }
            else
            {
                GL.BindVertexArray(VAO);//must be bound first before indices
            }
            if (indices != null)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesBufferObject);
            }
            if(useTexture)texture.use();
            if(useShader)shader.use();
        }

        #region drawMethods
        /*Draws this model. If its the first draw call, and firtst bind call, the model will be initialized.*/
        public virtual void draw(Matrix4 viewMatrix, Matrix4 projectionMatrix, Vector3 skyTopColor, Vector3 skyHorizonColor)//for skybox
        {
            bind();
            shader.setUniformMat4F("projectionMatrix", projectionMatrix);
            shader.setUniformMat4F("viewMatrix", viewMatrix);
            shader.setUniformMat4F("modelMatrix", Matrix4.Identity);
            shader.setUniformVec3F("skyTop", skyTopColor);
            shader.setUniformVec3F("skyHorizon", skyHorizonColor);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
            Renderer.totalDraws++;
        }
        public virtual void draw(Matrix4 viewMatrix, Matrix4 projectionMatrix, Vector3 fogColor)
        {
            bind();
            shader.setUniformMat4F("projectionMatrix", projectionMatrix);
            shader.setUniformMat4F("viewMatrix", viewMatrix);
            shader.setUniformMat4F("modelMatrix", Matrix4.Identity);
            shader.setUniformVec3F("fogColor", fogColor);
            shader.setUniform1I("frame", Renderer.frame);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
            Renderer.totalDraws++;
        }
        public virtual void draw(Matrix4 viewMatrix, Matrix4 projectionMatrix, Matrix4 modelMatrix, Vector3 fogColor)
        {
            bind();
            shader.setUniformMat4F("projectionMatrix", projectionMatrix);
            shader.setUniformMat4F("viewMatrix", viewMatrix);
            shader.setUniformMat4F("modelMatrix", modelMatrix);
            shader.setUniformVec3F("fogColor", fogColor);
            shader.setUniform1I("frame", Renderer.frame);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
            Renderer.totalDraws++;
        }
        public virtual void draw(Matrix4 viewMatrix, Matrix4 projectionMatrix, Matrix4 modelMatrix, PrimitiveType primitive)
        {
            bind();
            shader.setUniformMat4F("projectionMatrix", projectionMatrix);
            shader.setUniformMat4F("viewMatrix", viewMatrix);
            shader.setUniformMat4F("modelMatrix", modelMatrix);

            GL.DrawElements(primitive, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
            Renderer.totalDraws++;
        }
        public virtual void draw(Matrix4 viewMatrix, Matrix4 projectionMatrix, Matrix4 modelMatrix)
        {
            bind();
            shader.setUniformMat4F("projectionMatrix", projectionMatrix);
            shader.setUniformMat4F("viewMatrix", viewMatrix);
            shader.setUniformMat4F("modelMatrix", modelMatrix);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
            Renderer.totalDraws++;
        }

        public virtual void draw(Matrix4 projectionMatrix, Matrix4 modelMatrix)
        {
            bind();
            shader.setUniformMat4F("projectionMatrix", projectionMatrix);
            shader.setUniformMat4F("modelMatrix", modelMatrix);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
            Renderer.totalDraws++;
        }
        public virtual void draw(Matrix4 modelMatrix)
        {
            bind();
            shader.setUniformMat4F("modelMatrix", modelMatrix);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
            Renderer.totalDraws++;
        }

        public virtual void draw(int textureID)
        {
            bind(false);
            shader.setUniform1I("renderedTexture", textureID);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
            Renderer.totalDraws++;
        }
        public virtual void draw(bool useTex = true)
        {
            bind(useTex);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
            Renderer.totalDraws++;
        }
        public virtual void draw(PrimitiveType primType, bool useTex = true)
        {
            bind(useTex);
            GL.DrawElements(primType, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
            Renderer.totalDraws++;
        }

        public virtual void drawPoints(Matrix4 viewMatrix, Matrix4 projectionMatrix, Matrix4 modelMatrix, Vector3 fogColor, float pointRadius, bool ambientOcclusion)
        {
            if (!drawErrorPrinted && indices != null && GL.IsBuffer(indicesBufferObject))
            {
                Application.warn("drawPoints called for object with indices!");
                drawErrorPrinted = true;
            }
            bind();
            shader.setUniformMat4F("projectionMatrix", projectionMatrix);
            shader.setUniformMat4F("viewMatrix", viewMatrix);
            shader.setUniformMat4F("modelMatrix", modelMatrix);
            shader.setUniformVec3F("fogColor", fogColor);
            shader.setUniformVec2F("viewPortSize", Renderer.useOffScreenBuffer ? new Vector2(OffScreen.getWidth, OffScreen.getHeight) : new Vector2(GameInstance.get.Width, GameInstance.get.Height));
            shader.setUniform1F("pointRadius", pointRadius);
            shader.setUniform1I("aoc", ambientOcclusion ? 1 : 0);
            shader.setUniform1I("frame", Renderer.frame);
            //shader.setUniform1I("renderPass", pass);
            GL.DrawArrays(PrimitiveType.Points, 0, vertices.Length);
            unBind();
            Renderer.totalDraws++;
        }

        #endregion drawMethods

        
        public virtual Shader getShader()
        {
            return shader;
        }

        public ModelDrawable copyModelDrawable()
        {
            if (indices != null)
            {
                Vertex[] verticesCopy = new Vertex[vertices.Length];
                uint[] indicesCopy = new uint[indices.Length];
                Array.Copy(vertices, verticesCopy, vertices.Length);
                Array.Copy(indices, indicesCopy, indices.Length);
                return new ModelDrawable(shader, texture, verticesCopy, indicesCopy);
            }

            Vertex[] verticesCopy2 = new Vertex[vertices.Length];
            Array.Copy(vertices, verticesCopy2, vertices.Length);
            return new ModelDrawable(shader, texture, verticesCopy2, null);
        }

        /*Called when this model is no longer needed and will be replaced*/
        public virtual void delete()
        {
            GL.DeleteVertexArray(VAO);
            foreach (int vbo in VBOS)
            {
                GL.DeleteBuffer(vbo);
            }
        }
    }
}
