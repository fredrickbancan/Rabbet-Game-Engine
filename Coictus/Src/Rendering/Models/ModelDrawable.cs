using Coictus.SubRendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace Coictus.Models
{
    /*enum for determining how to draw a model. Important for dynamic models so they can automatically 
      chose a shader for the specific type. E.G: singlePoint only needs a position vector, not a whole
      model matrix.*/
    public enum ModelDrawType
    {
        trangles,
        points,
        singlePoint,
        lines
    };

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
        protected string shaderDir;
        protected string textureDir;
        private bool drawErrorPrinted = false;


        /*takes in directory for the shader and texture for this model, indices can be null if they wont be used*/
        public ModelDrawable(string shaderFile, string textureFile, Vertex[] vertices, uint[] indices = null, bool filtering = false) : base(vertices)
        {
            shaderDir = shaderFile;
            textureDir = textureFile;
            this.indices = indices;
            texture = new Texture(textureFile, filtering);
            shader = new Shader(shaderFile);
        }
        public ModelDrawable(string shaderFile, Texture tex, Vertex[] vertices, uint[] indices) : base(vertices)
        {
            this.indices = indices;
            texture = tex;
            shader = new Shader(shaderFile);
        }

        public virtual void setIndices(uint[] newind)
        {
            indices = newind;
        }

        public virtual void setNewTexture(Texture tex)
        {
            if(this.texture != null)
            {
                this.texture.Dispose();
            }
            this.texture = tex;
        }

        public virtual void setNewShader(Shader shad)
        {
            if (this.shader != null)
            {
                this.shader.Dispose();
            }
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
        }
        public virtual void draw(Matrix4 viewMatrix, Matrix4 projectionMatrix, Vector3 fogColour)
        {
            bind();
            shader.setUniformMat4F("projectionMatrix", projectionMatrix);
            shader.setUniformMat4F("viewMatrix", viewMatrix);
            shader.setUniformMat4F("modelMatrix", Matrix4.Identity);
            shader.setUniformVec3F("fogColour", fogColour);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
        }
        public virtual void draw(Matrix4 viewMatrix, Matrix4 projectionMatrix, Matrix4 modelMatrix, Vector3 fogColor)
        {
            bind();
            shader.setUniformMat4F("projectionMatrix", projectionMatrix);
            shader.setUniformMat4F("viewMatrix", viewMatrix);
            shader.setUniformMat4F("modelMatrix", modelMatrix);
            shader.setUniformVec3F("fogColour", fogColor);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
        }
        public virtual void draw(Matrix4 viewMatrix, Matrix4 projectionMatrix, Matrix4 modelMatrix, PrimitiveType primitive)
        {
            bind();
            shader.setUniformMat4F("projectionMatrix", projectionMatrix);
            shader.setUniformMat4F("viewMatrix", viewMatrix);
            shader.setUniformMat4F("modelMatrix", modelMatrix);

            GL.DrawElements(primitive, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
        }
        public virtual void draw(Matrix4 viewMatrix, Matrix4 projectionMatrix, Matrix4 modelMatrix)
        {
            bind();
            shader.setUniformMat4F("projectionMatrix", projectionMatrix);
            shader.setUniformMat4F("viewMatrix", viewMatrix);
            shader.setUniformMat4F("modelMatrix", modelMatrix);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
        }

        public virtual void draw(Matrix4 projectionMatrix, Matrix4 modelMatrix)
        {
            bind();
            shader.setUniformMat4F("projectionMatrix", projectionMatrix);
            shader.setUniformMat4F("modelMatrix", modelMatrix);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
        }
        public virtual void draw(Matrix4 modelMatrix)
        {
            bind();
            shader.setUniformMat4F("modelMatrix", modelMatrix);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
        }

        public virtual void draw(int textureID)
        {
            bind(false);
            shader.setUniform1I("renderedTexture", textureID);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
        }
        public virtual void draw(bool useTex = true)
        {
            bind(useTex);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
        }
        public virtual void draw(PrimitiveType primType, bool useTex = true)
        {
            bind(useTex);
            GL.DrawElements(primType, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
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
            shader.setUniformVec3F("fogColour", fogColor);
            shader.setUniformVec2F("viewPortSize", Renderer.useOffScreenBuffer ? new Vector2(OffScreen.getWidth, OffScreen.getHeight) : new Vector2(GameInstance.get.Width, GameInstance.get.Height));
            shader.setUniform1F("pointRadius", pointRadius);
            shader.setUniform1I("aoc", ambientOcclusion ? 1 : 0);
            //shader.setUniform1I("renderPass", pass);
            GL.DrawArrays(PrimitiveType.Points, 0, vertices.Length);
            unBind();
        }

        #endregion drawMethods

        /*Unbinds this model so the renderer can render different things.*/
        public virtual void unBind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.UseProgram(0);
            GL.BindVertexArray(0);
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

        /*Binds the provided data to the VAO using the provided information*/
        [Obsolete("This method is depricated as now the system works with Vertex objects instead of float arrays. Use storeVertexDataInAttributeList() instead.")]
        protected virtual void storeDataInAttributeList(int attributeNumber, int coordinateCount, float[] data)
        {
            int vbo = GL.GenBuffer();
            VBOS.Add(vbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
            /*Stride: the size in bytes between the start of one vertex to the start of another.
              Offset: the size in byts between the start of the vertex to the first bit of information for this specific attribute.*/
            GL.EnableVertexAttribArray(attributeNumber);
            GL.VertexAttribPointer(attributeNumber, coordinateCount, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
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
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        /*Used for binding the vertex buffer object in the array at the given index, any vbo's get unbound in unbind()*/
        public virtual void bindVertexBuffer(int index = 0)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOS[0]);
        }


        public ModelDrawable copyModelDrawable()
        {
            if (indices != null)
            {
                Vertex[] verticesCopy = new Vertex[vertices.Length];
                uint[] indicesCopy = new uint[indices.Length];
                Array.Copy(vertices, verticesCopy, vertices.Length);
                Array.Copy(indices, indicesCopy, indices.Length);
                return new ModelDrawable(shaderDir, textureDir, verticesCopy, indicesCopy);
            }

            Vertex[] verticesCopy2 = new Vertex[vertices.Length];
            Array.Copy(vertices, verticesCopy2, vertices.Length);
            return new ModelDrawable(shaderDir, textureDir, verticesCopy2, null);
        }
        /*Called when this model is no longer needed and will be replaced*/
        public virtual void delete()
        {
            GL.DeleteVertexArray(VAO);
            foreach (int vbo in VBOS)
            {
                GL.DeleteBuffer(vbo);
            }
            shader.Dispose();
            texture.Dispose();
        }
    }
}
