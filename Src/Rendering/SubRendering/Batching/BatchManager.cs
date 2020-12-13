using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using System.Collections.Generic;
using System.Linq;

namespace RabbetGameEngine.SubRendering
{
    //TODO: Implement a limit for number of textures in a single batch, and implement ablility for batches to have multiple textures.
    //TODO: Seperate gui batches so they can be rendered last ontop of a framebuffer quad
    public static class BatchManager
    {
        private static List<Batch> batches = new List<Batch>();
        private static List<Batch> guiBatches = new List<Batch>();

        /// <summary>
        /// Should be called before any rendering requests.
        /// </summary>
        public static void preWorldRenderUpdate()
        {
            for (int i = 0; i < batches.Count; ++i)
            {
                batches.ElementAt(i).reset();
            }
        }

        /// <summary>
        /// Should be called before any rendering requests.
        /// </summary>
        public static void preGUIRenderUpdate()
        {
            for (int i = 0; i < guiBatches.Count; ++i)
            {
                guiBatches.ElementAt(i).reset();
            }
        }

        public static void postWorldRenderUpdate()
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


        public static void postGUIRenderUpdate()
        {
            for (int i = 0; i < guiBatches.Count; ++i)
            {
                Batch batchAt = guiBatches.ElementAt(i);

                //if a batch has not been used, then it needs to be removed.
                if (!batchAt.hasBeenUsedSinceLastUpdate())
                {
                    batchAt.deleteVAO();
                    guiBatches.Remove(batchAt);
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
        public static void requestRender(RenderType type, Texture tex, Model theModel, int renderLayer)
        {
            List<Batch> bl = isTypeGUI(type) ? guiBatches : batches;
            if (bl.Count < 1)
            {
                bl.Add(new Batch(type, tex, renderLayer));
                BatchUtil.tryToFitInBatchModel(theModel, bl.ElementAt(0));
                return;
            }

            for (int i = 0; i < bl.Count; ++i)
            {
                Batch batchAt = bl.ElementAt(i);
                if(batchAt.getRenderType() == type && renderLayer == batchAt.renderLayer && batchAt.getBatchtexture() == tex &&  BatchUtil.tryToFitInBatchModel(theModel, batchAt))
                {
                    return;//successfull batch adding
                }

                if(i == bl.Count - 1)//if we have itterated through all batches and found no candidate, then add new batch.
                {
                    //ensure that all opaque batches come before transparent ones in the list.
                    if (isTypeTransparent(type))
                    {
                        bl.Add(new Batch(type, tex, renderLayer));
                        BatchUtil.tryToFitInBatchModel(theModel, bl.ElementAt(i+1));
                    }
                    else
                    {
                        bl.Insert(0, new Batch(type, tex, renderLayer));
                        BatchUtil.tryToFitInBatchModel(theModel, bl.ElementAt(0));
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
                        batches.Add(new Batch(type, null));
                        BatchUtil.tryToFitInBatchPoints(pParticleModel, batches.ElementAt(i + 1));
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
                        batches.Add(new Batch(type, null));
                        BatchUtil.tryToFitInBatchSinglePoint(pParticle, batches.ElementAt(i + 1));
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
                        batches.Add(new Batch(type, null));
                        BatchUtil.tryToFitInBatchLerpPoint(pParticle, prevTickPParticle, batches.ElementAt(i + 1));
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

        public static void drawAllWorld(Matrix4 viewMatrix, Vector3 fogColor)
        {
            foreach(Batch b in batches)
            {
                b.draw(viewMatrix, fogColor);
                Renderer.totalDraws++;
            }
        }
        public static void drawAllGUI(Matrix4 viewMatrix, Vector3 fogColor)
        {
            foreach (Batch b in guiBatches)
            {
                b.draw(viewMatrix, fogColor);
                Renderer.totalDraws++;
            }
        }

        public static bool isTypeTransparent(RenderType type)
        {
            return type > RenderType.MARKER_TRANSPARENT_START && type < RenderType.MARKER_TRANSPARENT_END;
        }

        public static bool isTypeGUI(RenderType type)
        {
            return type > RenderType.MARKER_GUI_START && type < RenderType.MARKER_GUI_END;
        }

        public static bool isTypeLerp(RenderType type)
        {
            return type > RenderType.MARKER_LERP_START && type < RenderType.MARKER_LERP_END;
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
            foreach (Batch b in guiBatches)
            {
                b.deleteVAO();
            }
        }

        public static int batchCount { get => batches.Count; }
    }
}
