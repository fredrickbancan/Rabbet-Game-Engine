using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.Physics;
using RabbetGameEngine.SubRendering;
using RabbetGameEngine.VisualEffects;
using System.Collections.Generic;
namespace RabbetGameEngine
{
    /*This class handles the rendering of hitboxes for debugging purposes.*/
    public static class HitboxRenderer
    {
        private static Model aabbModelPrefab;
        private static bool acceptingRequests = true;
        static HitboxRenderer()
        {
            aabbModelPrefab = new Model(CubePrefab.cubeVertices, LineCombiner.getIndicesForLineQuadCount(6)).setColor(CustomColor.magenta);
        }

        public static void beforeTick()
        {
            acceptingRequests = true;
        }

        public static void onTickEnd()
        {
            acceptingRequests = false;
        }

        /*called on tick. Adds all of the provided colliders to a list of hitboxes to be dynamically batched and drawn.*/
        public static void addAllHitboxesToBeRendered(List<AABB> worldColliders, Dictionary<int, Entity> entities, List<VFX> effects)
        {
            if(!acceptingRequests) return;
            foreach (AABB hitBox in worldColliders)
            {
                addBoxToBeRendered(hitBox);
            }

            foreach(VFX v in effects)
            {
                renderPointWithNormalLines(v.getBox().centerVec);
            }

            foreach(Entity ent in entities.Values)
            {
                addBoxToBeRendered(ent.getBox());
            }
        }

        public static void addBoxToBeRendered(AABB box)
        {
            if (!acceptingRequests) return;
            //add a copy of the aabb line model transformed to aabb collider specs
            Renderer.requestRender(RenderType.lines, null, aabbModelPrefab.copyModel().transformVertices(new Vector3((float)box.extentX * 2, (float)box.extentY * 2, (float)box.extentZ * 2), Vector3.Zero, box.centerVec));
            renderPointWithNormalLines(box.centerVec);
        }

        public static void renderPointWithNormalLines(Vector3 pos)
        {
            if (!acceptingRequests) return;
            PointParticle pParticle = new PointParticle(pos, new Vector4(0.15F, 0.15F, 0.15F, 1), 0.025F, true);
          //  Renderer.requestRender(pParticle, false);

            Vector4 redColor = CustomColor.red.toNormalVec4();
            Vector4 greenColor = CustomColor.green.toNormalVec4();
            Vector4 blueColor = CustomColor.blue.toNormalVec4();

            Vertex[] lineVerts = new Vertex[]
            {
                new Vertex(pos, redColor, Vector2.Zero),//normal x line start
                new Vertex(pos + new Vector3(1F,0,0), new Vector4(0.0F), Vector2.Zero),//normal x line end
                 new Vertex(pos, greenColor, Vector2.Zero),//normal y line start
                new Vertex(pos+ new Vector3(0,1F,0), new Vector4(0.0F), Vector2.Zero),//normal y line end
                 new Vertex(pos, blueColor, Vector2.Zero),//normal z line start
                new Vertex(pos+ new Vector3(0,0,1F), new Vector4(0.0F), Vector2.Zero)//normal z line end
            };

            uint[] lineIndices = new uint[]
            {
                0,1,
                2,3,
                4,5
            };
            Renderer.requestRender(RenderType.lines, null, new Model(lineVerts, lineIndices));
        }
    }
}
