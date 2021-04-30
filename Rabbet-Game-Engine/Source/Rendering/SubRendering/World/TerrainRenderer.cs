using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
namespace RabbetGameEngine
{
    public static class TerrainRenderer
    {
        private static ChunkComparer sorter = new ChunkComparer();
        private static Shader voxelShader = null;
        private static Texture terrainTex = null;
        private static Terrain theTerrain = null;
        public static int chunkDrawCalls = 0;
        public static void init()
        {
            ShaderUtil.tryGetShader(ShaderUtil.voxelName, out voxelShader);
            TextureUtil.tryGetTexture("mcterrain", out terrainTex);
        }

        public static void setTerrainToRender(Terrain t)
        {
            theTerrain = t;
        }

        public static void renderTerrain(Camera viewer)
        {
            if (theTerrain == null) return;
            Profiler.startSection("chunkPrep");
            theTerrain.doFrameRenderUpdate(viewer);
            Profiler.endStartSection("chunkDraw");
            voxelShader.use();
            terrainTex.bind();
            GL.PatchParameter(PatchParameterInt.PatchVertices, 4);
            Matrix4 projview = Renderer.viewMatrix * Renderer.projMatrix;
            foreach (Chunk cr in theTerrain.chunksRenderable)
            {
                if (cr.isEmpty) continue;
                voxelShader.setUniformMat4F("projViewModel", cr.translationMatrix * projview);
                cr.localRenderer.bindVAO();
                GL.DrawElements(PrimitiveType.Patches, cr.localRenderer.addedVoxelFaceIndiciesCount, DrawElementsType.UnsignedInt, 0);
            }
            chunkDrawCalls = theTerrain.chunksRenderable.Count;
            Profiler.endCurrentSection();
        }

        public static void onClosing()
        {
            theTerrain = null;
        }
    }
}
