using Coictus.FredsMath;
using Coictus.Models;
using Coictus.SubRendering;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace Coictus.Debugging
{
    /*This class handles the rendering of hitboxes for debugging purposes.*/
    public static class HitboxRenderer
    {
        private static String aabbPlaneShaderDir = ResourceHelper.getShaderFileDir("Color3D.shader");
        private static ModelDrawable aabbModel;
        private static ModelDrawable sphereModel;
        private static ModelDrawable planeModel;
        private static ModelDrawable pointModel;

        /*Constructs each hitbox model for instanced rendering. Requires Renderer to be initialized first.*/
        public static void init()
        {
            buildAABBModel();
        }

        /*Builds the model for an AABB, basically a cube*/
        private static void buildAABBModel()
        {
            Model[] boxSides = new Model[6];

            for(int i = 0; i < boxSides.Length; i++)
            {
                boxSides[i] = QuadPrefab.getNewModel().setColor(ColourF.aquaPain);
            }
            
            boxSides[0].rotateVertices(new Vector3F(0, 180, 0)).translateVertices(new Vector3F(0, 0, -0.5F));//negZ face
            boxSides[1].translateVertices(new Vector3F(0, 0, 0.5F));//posZ face
            boxSides[2].rotateVertices(new Vector3F(0, 90, 0)).translateVertices(new Vector3F(-0.5F, 0, 0));//negX face
            boxSides[3].rotateVertices(new Vector3F(0, -90, 0)).translateVertices(new Vector3F(0.5F, 0, 0));//posX face
            boxSides[4].rotateVertices(new Vector3F(-90, 0, 0)).translateVertices(new Vector3F(0, -0.5F, 0));//negY face
            boxSides[5].rotateVertices(new Vector3F(90, 0, 0)).translateVertices(new Vector3F(0, 0.5F, 0));//posY face

            aabbModel = QuadBatcher.batchQuadModels(boxSides, aabbPlaneShaderDir, "none");
            aabbModel.setIndices(LineBatcher.getIndicesForLineQuadCount(6));
        }
        public static void renderAllHitboxes(List<ICollider> worldColliders, Dictionary<int, ICollider> entityColliders)
        {
            foreach(ICollider hitBox in worldColliders)
            {
                renderHitboxByType(hitBox);
            }

            foreach(ICollider hitBox in entityColliders.Values)
            {
                renderHitboxByType(hitBox);
            }
        }

        public static void renderHitboxByType(ICollider hitBox)
        {
            switch(hitBox.getType())
            {
                case ColliderType.aabb:
                    renderBox((AABBCollider)hitBox);
                    break;

                case ColliderType.sphere:
                    renderSphere((SphereCollider)hitBox);
                    break;

                case ColliderType.plane:
                    renderPlane((PlaneCollider)hitBox);
                    break;

                case ColliderType.point:
                    renderPoint((PointCollider)hitBox);
                    break;
            }
        }

        public static void renderBox(AABBCollider box)//TODO: VERY inefficient. Need to created either a dynamic model or instancing to cut down on draws. Avoiding calcualting the model matrix each frame would also help.
        {
            Matrix4F modelMatrix = Matrix4F.scale(new Vector3F((float)box.extentX*2, (float)box.extentY * 2, (float)box.extentZ * 2)) * Matrix4F.translate(Vector3F.convert(box.centerVec));

            aabbModel.draw(GameInstance.get.thePlayer.getViewMatrix(), Renderer.projMatrix, modelMatrix, PrimitiveType.Lines);
        }

        public static void renderSphere(SphereCollider sphere)
        {

        }

        public static void renderPoint(PointCollider point)
        {

        }

        public static void renderPlane(PlaneCollider plane)
        {

        }

       
    }
}
