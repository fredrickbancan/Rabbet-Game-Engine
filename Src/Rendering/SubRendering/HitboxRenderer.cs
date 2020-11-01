using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.Physics;
using System.Collections.Generic;
namespace RabbetGameEngine.SubRendering
{
    /*This class handles the rendering of hitboxes for debugging purposes.*/
    public static class HitboxRenderer
    {
        private static Model aabbModelPrefab;

        static HitboxRenderer()
        {
            aabbModelPrefab = new Model(CubePrefab.cubeVertices, LineCombiner.getIndicesForLineQuadCount(6)).setColor(CustomColor.magenta);
        }

        /*called on tick. Adds all of the provided colliders to a list of hitboxes to be dynamically batched and drawn.*/
        public static void addAllHitboxesToBeRendered(List<AABB> worldColliders, Dictionary<int, Entity> entities)
        {
            foreach (AABB hitBox in worldColliders)
            {
                addBoxToBeRendered(hitBox);
            }

            foreach(Entity ent in entities.Values)
            {
                addHitboxToBeRendered(ent.getCollider());
            }
        }

        private static void addHitboxToBeRendered(ICollider hitBox)
        {
            if(hitBox == null)
            {
                return;
            }

            switch(hitBox.getType())
            {
                case ColliderType.aabb:
                    addBoxToBeRendered((AABB)hitBox);
                    break;

                case ColliderType.point:
                    addPointToBeRendered((PointCollider)hitBox);
                    break;
            }
        }

        public static void addBoxToBeRendered(AABB box)
        {
            //add a copy of the aabb line model transformed to aabb collider specs
            Renderer.requestRender(BatchType.lines, null, aabbModelPrefab.copyModel().transformVertices(new Vector3((float)box.extentX * 2, (float)box.extentY * 2, (float)box.extentZ * 2), Vector3.Zero, box.centerVec));
          
        }

        public static void addPointToBeRendered(PointCollider point)
        {
            PointParticle pParticle = new PointParticle(point.pos, CustomColor.facility.toNormalVec4(), 0.05F, true);
            Renderer.requestRender(pParticle, false, false);

            Vector4 redColor = CustomColor.red.toNormalVec4();
            Vector4 greenColor = CustomColor.green.toNormalVec4();
            Vector4 blueColor = CustomColor.blue.toNormalVec4();

            Vertex[] lineVerts = new Vertex[]
            {
                new Vertex(point.pos, redColor, Vector2.Zero),//normal x line start
                new Vertex(point.pos + new Vector3(1F,0,0), redColor, Vector2.Zero),//normal x line end
                 new Vertex(point.pos, greenColor, Vector2.Zero),//normal y line start
                new Vertex(point.pos+ new Vector3(0,1F,0), greenColor, Vector2.Zero),//normal y line end
                 new Vertex(point.pos, blueColor, Vector2.Zero),//normal z line start
                new Vertex(point.pos+ new Vector3(0,0,1F), blueColor, Vector2.Zero)//normal z line end
            };

            uint[] lineIndices = new uint[]
            {
                0,1,
                2,3,
                4,5
            };
            Renderer.requestRender(BatchType.lines, null, new Model(lineVerts, lineIndices));
        }
    }
}
