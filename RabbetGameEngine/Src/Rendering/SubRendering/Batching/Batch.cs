namespace RabbetGameEngine.SubRendering
{
    /*Class for containing information of a render batch. New batches will need to be created for 
      different types of rendering and/or if a previous batch is too full.*/
    public class Batch//TODO: impliment
    {
        public static readonly int maxVertexCount = 2500000 / Vertex.vertexByteSize;
        private int VAO;//TODO abstract construction, binding and destruction of vertex array objects
        private int VBO;
        private Vertex[] batchedVertices = new Vertex[maxVertexCount];
        private int submittedVerticesCount = 0;
        private uint[] indices;
        private BatchType batchType;

        public Batch(BatchType type)
        {
            batchType = type;
            //TODO: impliment
        }
        
        /*attempts to add the provided data to the batch. Returns true if successful, and returns false if not  enough room.*/
        public bool addToBatch(Vertex[] vertices, uint[] indices)
        {
            if (submittedVerticesCount + vertices.Length > maxVertexCount)
            {
                return false;
            }
            submittedVerticesCount += vertices.Length;
            return true;//TODO: impliment
        }
    }
}
