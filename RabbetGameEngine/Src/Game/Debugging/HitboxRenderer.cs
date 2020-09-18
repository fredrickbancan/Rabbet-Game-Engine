using OpenTK;
using OpenTK.Graphics.OpenGL;
using RabbetGameEngine.Models;
using RabbetGameEngine.Physics;
using RabbetGameEngine.SubRendering;
using System;
using System.Collections.Generic;
namespace RabbetGameEngine.Debugging
{
    /*This class handles the rendering of hitboxes for debugging purposes.*/
    public static class HitboxRenderer
    {
        public static readonly string linesShaderName = "Color3D.shader";


        public static readonly int maxAABBRenderCount = 512;
        private static ModelDrawableDynamic aabbDynamicModel;
        private static Model aabbModelPrefab;
        private static List<Model> aabbToBeRendered = new List<Model>();

        /*Constructs each hitbox model for instanced rendering. Requires Renderer to be initialized first.*/
        public static void init()
        {
            buildAABBModel();
        }

        /*Builds the prefab model for an AABB, basically a cube made of lines*/
        private static void buildAABBModel()
        {
            aabbDynamicModel = new ModelDrawableDynamic(linesShaderName, "none", LineBatcher.getIndicesForLineQuadCount(maxAABBRenderCount * 6), maxAABBRenderCount * 48 /*aabb models have 48 vertices*/);//initializing dynamic model for drawing aabb

            aabbModelPrefab = CubePrefab.copyModel().setColor(CustomColor.magenta);
        }

        /*called on tick. Adds all of the provided colliders to a list of hitboxes to be dynamically batched and drawn.*/
        public static void addAllHitboxesToBeRendered(List<ICollider> worldColliders, Dictionary<int, ICollider> entityColliders)
        {
            foreach(ICollider hitBox in worldColliders)
            {
                addHitboxToBeRendered(hitBox);
            }

            foreach(ICollider hitBox in entityColliders.Values)
            {
                addHitboxToBeRendered(hitBox);
            }

            combineAndSubmitAABBModels();
        }

        private static void addHitboxToBeRendered(ICollider hitBox)
        {
            switch(hitBox.getType())
            {
                case ColliderType.aabb:
                    addBoxToBeRendered((AABBCollider)hitBox);
                    break;

                case ColliderType.sphere:
                    addSphereToBeRendered((SphereCollider)hitBox);
                    break;

                case ColliderType.plane:
                    addPlaneToBeRendered((PlaneCollider)hitBox);
                    break;

                case ColliderType.point:
                    addPointToBeRendered((PointCollider)hitBox);
                    break;
            }
        }

        public static void addBoxToBeRendered(AABBCollider box)
        {
            if(aabbToBeRendered.Count < maxAABBRenderCount)
            {
                //add a copy of the aabb line model transformed to aabb collider specs
                aabbToBeRendered.Add(aabbModelPrefab.copyModel().transformVertices(new Vector3((float)box.extentX * 2, (float)box.extentY * 2, (float)box.extentZ * 2), Vector3.Zero, box.centerVec));
            }
        }

        public static void addSphereToBeRendered(SphereCollider sphere)
        {

        }

        public static void addPointToBeRendered(PointCollider point)
        {

        }

        public static void addPlaneToBeRendered(PlaneCollider plane)
        {

        }

        /*Renders all of the requested hitboxes via dynamic draw*/
        public static void renderAll(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            aabbDynamicModel.draw(viewMatrix, projectionMatrix, Matrix4.Identity, PrimitiveType.Lines);
        }

        private static void combineAndSubmitAABBModels()
        {
            Vertex[] fillerVertices = new Vertex[maxAABBRenderCount*48];
            Vertex[] batchedModelVertices;
            QuadBatcher.combineData(aabbToBeRendered.ToArray(), out batchedModelVertices);
            Array.Copy(batchedModelVertices, fillerVertices, batchedModelVertices.Length);
            aabbDynamicModel.submitData(fillerVertices);
            aabbToBeRendered.Clear();
        }
    }
}
