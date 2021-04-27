using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;
namespace RabbetGameEngine
{
    //TODO: Optimize batching and draw calls so each batch has the ideal amount of data
    //TODO: Render chunks front to back
    //TODO: Fustrum cull chunks each frame
    public static class TerrainRenderer
    {
        private static ChunkComparer sorter = new ChunkComparer();
        private static List<Chunk> sortedChunksReference = null;
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
        public static void doRenderUpdate(Camera viewer)
        {
            if (theTerrain == null) return;
            sortedChunksReference = theTerrain.sortedChunks;
            if (sortedChunksReference == null) return;
            Profiler.startTickSection("chunkUpdates");
            foreach (Chunk cr in sortedChunksReference)
            {
                if (!cr.isInFrustum) continue;
                if(!cr.isOnWorldEdge && cr.isScheduledForPopulation)
                {
                    theTerrain.populator.populateChunk(cr);
                    break;
                }
                if (!cr.isEmpty && cr.isMarkedForRenderUpdate && !cr.isOnWorldEdge && !cr.isMarkedForRemoval)
                {
                    cr.localRenderer.updateVoxelMesh(theTerrain);
                    break;
                }
            }
            Profiler.endCurrentTickSection();
        }

        public static void renderTerrain(Camera viewer)
        {
            if (sortedChunksReference == null) return;
            Profiler.startSection("frustumCheck");
            theTerrain.doFrustumCheck(viewer);
            Profiler.endStartSection("renderChunks");
            chunkDrawCalls = 0;
            GL.PatchParameter(PatchParameterInt.PatchVertices, 4);
            voxelShader.use();
            terrainTex.bind();
            foreach (Chunk cr in sortedChunksReference)
            {
                if (!cr.isEmpty && cr.isInFrustum) renderChunk(cr);
            }
            Profiler.endCurrentSection();
        }

        private static void renderChunk(Chunk c)
        {
            Profiler.startSection("chunkDraw");
            voxelShader.setUniformMat4F("projViewModel", Matrix4.CreateTranslation((Vector3)c.coord * Chunk.CHUNK_PHYSICAL_SIZE) * Renderer.viewMatrix * Renderer.projMatrix);
            c.localRenderer.bindVAO();
            GL.DrawElements(PrimitiveType.Patches, c.localRenderer.addedVoxelFaceCount * 4, DrawElementsType.UnsignedInt, 0);
            chunkDrawCalls++;
            Profiler.endCurrentSection();
        }

        public static void onClosing()
        {
            sortedChunksReference = null;
            theTerrain = null;
        }
    }
}
