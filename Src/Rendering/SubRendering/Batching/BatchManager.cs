using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using System.Collections.Generic;
using System.Linq;

namespace RabbetGameEngine.SubRendering
{
    //TODO: Implement a limit for number of textures in a single batch, and implement ablility for batches to have multiple textures.
    public static class BatchManager
    {
        private static List<Batch> batches = new List<Batch>();

        /// <summary>
        /// Should be called before any rendering requests.
        /// </summary>
        public static void preRenderUpdate()
        {
            for (int i = 0; i < batches.Count; ++i)
            {
                batches.ElementAt(i).reset();
            }
        }

        public static void postRenderUpdate()
        {
            for (int i = 0; i < batches.Count; ++i)
            {
                Batch batchAt = batches.ElementAt(i);

                //if a batch has not been used, then it needs to be removed.
                if (!batchAt.hasBeenUsedSinceLastUpdate())
                {
                    batchAt.deleteVAO();
                    batches.Remove(batchAt);
                    --i;
                    continue;
                }
                
                batchAt.postRenderUpdate();
            }

        }

        /// <summary>
        /// Can be called to request that the provided data be added to the appropriate existing batch
        /// or, if said batch does not exist or is full, creates and adds a new batch.
        /// </summary>
        public static void requestRender(RenderType type, Texture tex, Model theModel)
        {
            if (batches.Count < 1)
            {
                batches.Add(new Batch(type, tex));
                BatchUtil.tryToFitInBatchModel(theModel, batches.ElementAt(0));
                return;
            }

            for (int i = 0; i < batches.Count; ++i)
            {
                Batch batchAt = batches.ElementAt(i);
                if(batchAt.getRenderType() == type && batchAt.getBatchtexture() == tex &&  BatchUtil.tryToFitInBatchModel(theModel, batchAt))
                {
                    return;//successfull batch adding
                }

                if(i == batches.Count - 1)//if we have itterated through all batches and found no candidate, then add new batch.
                {
                    //ensure that all opaque batches come before transparent ones in the list.
                    if (type == RenderType.guiText || type == RenderType.guiTransparent)
                    {
                        batches.Add(new Batch(type, tex));
                        BatchUtil.tryToFitInBatchModel(theModel, batches.ElementAt(i + 1));
                    }
                    else if (type == RenderType.lerpTrianglesTransparent || type == RenderType.trianglesTransparent)
                    {
                        insertTransparentNonGUIBatch(new Batch(type, tex), theModel);
                    }
                    else
                    {
                        batches.Insert(0, new Batch(type, tex));
                        BatchUtil.tryToFitInBatchModel(theModel, batches.ElementAt(0));
                    }
                    
                    return;
                }
            }
            
        }
        public static void requestRender(PointCloudModel pParticleModel, bool transparency, bool lerp)
        {
            RenderType type;
            if(transparency)
            {
                if(!lerp)
                {
                    type = RenderType.iSpheresTransparent;
                }
                else
                {
                    type = RenderType.lerpISpheresTransparent;
                }
            }
            else
            {
                if (!lerp)
                {
                    type = RenderType.iSpheres;
                }
                else
                {
                    type = RenderType.lerpISpheres;
                }
            }

            for (int i = 0; i < batches.Count; ++i)
            {
                Batch batchAt = batches.ElementAt(i);
                if (batchAt.getRenderType() == type && BatchUtil.tryToFitInBatchPoints(pParticleModel, batchAt))
                {
                    return;//successfull batch adding
                }

                if (i == batches.Count - 1)//if we have itterated through all batches and found no candidate, then add new batch.
                {
                    //ensure that all opaque batches come before transparent ones in the list.
                    if (transparency)
                    {
                        insertTransparentNonGUIBatch(new Batch(type, null), pParticleModel);
                    }
                    else
                    {
                        batches.Insert(0, new Batch(type, null));
                        BatchUtil.tryToFitInBatchPoints(pParticleModel, batches.ElementAt(0));
                    }

                    return;
                }
            }
        }


        public static void requestRender(PointParticle pParticle, bool transparency)
        {
            RenderType type;
            if (transparency)
            {
                    type = RenderType.iSpheresTransparent;
                
            }
            else
            {
                    type = RenderType.iSpheres;
                
            }
            for (int i = 0; i < batches.Count; ++i)
            {
                Batch batchAt = batches.ElementAt(i);
                if (batchAt.getRenderType() == type && BatchUtil.tryToFitInBatchSinglePoint(pParticle, batchAt))
                {
                    return;//successfull batch adding
                }

                if (i == batches.Count - 1)//if we have itterated through all batches and found no candidate, then add new batch.
                {
                    //ensure that all opaque batches come before transparent ones in the list.
                    if (transparency)
                    {
                        insertTransparentNonGUIBatch(new Batch(type, null), pParticle);
                    }
                    else
                    {
                        batches.Insert(0, new Batch(type, null));
                        BatchUtil.tryToFitInBatchSinglePoint(pParticle, batches.ElementAt(0));
                    }

                    return;
                }
            }
        }

        public static void requestRender(PointParticle pParticle, PointParticle prevTickPParticle, bool transparency)
        {
            RenderType type;
            if (transparency)
            {
                type = RenderType.lerpISpheresTransparent;

            }
            else
            {
                type = RenderType.lerpISpheres;

            }
            for (int i = 0; i < batches.Count; ++i)
            {
                Batch batchAt = batches.ElementAt(i);
                if (batchAt.getRenderType() == type && BatchUtil.tryToFitInBatchLerpPoint(pParticle, prevTickPParticle, batchAt))
                {
                    return;//successfull batch adding
                }

                if (i == batches.Count - 1)//if we have itterated through all batches and found no candidate, then add new batch.
                {
                    //ensure that all opaque batches come before transparent ones in the list.
                    if (transparency)
                    {
                        insertTransparentNonGUIBatch(new Batch(type, null), pParticle, prevTickPParticle);
                    }
                    else
                    {
                        batches.Insert(0, new Batch(type, null));
                        BatchUtil.tryToFitInBatchLerpPoint(pParticle, prevTickPParticle, batches.ElementAt(0));
                    }

                    return;
                }
            }
        }

        public static void requestRender(Sprite3D s, Texture tex)
        {
            for (int i = 0; i < batches.Count; ++i)
            {
                Batch batchAt = batches.ElementAt(i);
                if (batchAt.getRenderType() == RenderType.spriteCylinder && batchAt.batchTex == tex && BatchUtil.tryToFitInBatchSprite3D(s, batchAt))
                {
                    return;//successfull batch adding
                }

                if (i == batches.Count - 1)//if we have itterated through all batches and found no candidate, then add new batch.
                {
                    batches.Insert(0, new Batch(RenderType.spriteCylinder, tex));
                    BatchUtil.tryToFitInBatchSprite3D(s, batches.ElementAt(0));
                    return;
                }
            }
        }

        private static void insertTransparentNonGUIBatch(Batch theBatch, Model mod)
        {
            for (int i = batches.Count - 1; i >= 0; --i)
            {
                if (!batches.ElementAt(i).transparentGUI)
                {
                    batches.Insert(i + 1, theBatch);
                    BatchUtil.tryToFitInBatchModel(mod, theBatch);
                    return;
                }
            }
            batches.Insert(0, theBatch);
            BatchUtil.tryToFitInBatchModel(mod, theBatch);
        }

        private static void insertTransparentNonGUIBatch(Batch theBatch, PointParticle p, PointParticle prevP)
        {
            for (int i = batches.Count - 1; i >= 0; --i)
            {
                if (!batches.ElementAt(i).transparentGUI)
                {
                    batches.Insert(i + 1, theBatch);
                    BatchUtil.tryToFitInBatchLerpPoint(p, prevP, theBatch);
                    return;
                }
            }
            batches.Insert(0, theBatch);
            BatchUtil.tryToFitInBatchLerpPoint(p, prevP, theBatch);
        }

        private static void insertTransparentNonGUIBatch(Batch theBatch, PointCloudModel p)
        {
            for (int i = batches.Count - 1; i >= 0; --i)
            {
                if (!batches.ElementAt(i).transparentGUI)
                {
                    batches.Insert(i + 1, theBatch);
                    BatchUtil.tryToFitInBatchPoints(p, theBatch);
                    return;
                }
            }
            batches.Insert(0, theBatch);
            BatchUtil.tryToFitInBatchPoints(p, theBatch);
        }

        private static void insertTransparentNonGUIBatch(Batch theBatch, PointParticle p)
        {
            for (int i = batches.Count - 1; i >= 0; --i)
            {
                if (!batches.ElementAt(i).transparentGUI)
                {
                    batches.Insert(i + 1, theBatch);
                    BatchUtil.tryToFitInBatchSinglePoint(p, theBatch);
                    return;
                }
            }
            batches.Insert(0, theBatch);
            BatchUtil.tryToFitInBatchSinglePoint(p, theBatch);
        }

        public static void drawAll(Matrix4 viewMatrix, Vector3 fogColor)
        {
            foreach(Batch b in batches)
            {
                b.draw(viewMatrix, fogColor);
                Renderer.totalDraws++;
            }
        }

        /// <summary>
        /// Required to call this before closing application.
        /// </summary>
        public static void deleteAll()
        {
            foreach(Batch b in batches)
            {
                b.deleteVAO();
            }
        }

        public static int batchCount { get => batches.Count; }
    }
}
