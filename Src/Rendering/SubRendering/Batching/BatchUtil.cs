namespace RabbetGameEngine.SubRendering
{
    //TODO: do VAO building here based on batch type.
    //TODO: add method(s) to help with attempting to add new data to a batch that works with different batch types.
    public static class BatchUtil
    {
        public static void buildBatch(Batch theBatch)
        {
            VertexArrayObject vao = new VertexArrayObject();
            switch(theBatch.getBatchType())
            {

            }
            theBatch.setVAO(vao);
        }
    }
}
