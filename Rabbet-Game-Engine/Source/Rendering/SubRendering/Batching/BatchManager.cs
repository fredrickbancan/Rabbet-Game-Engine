using RabbetGameEngine.Models;
using System.Collections.Generic;
using System.Linq;

namespace RabbetGameEngine.SubRendering
{
    //TODO: Implement a limit for number of textures in a single batch, and implement ablility for batches to have multiple textures.
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
                batches.ElementAt(i).preRenderUpdate();
            }
        }

        /// <summary>
        /// Should be called before any rendering requests.
        /// </summary>
        public static void preGUIRenderUpdate()
        {
            for (int i = 0; i < guiBatches.Count; ++i)
            {
                guiBatches.ElementAt(i).preRenderUpdate();
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

            if (bl.Count == 0)//adding first batch
            {
                Batch b = BatchUtil.getBatchForType(type, renderLayer).addTexture(tex);
                b.tryToFitInBatchModel(theModel);
                bl.Add(b);
                return;
            }

            for (int i = 0; i < bl.Count; ++i)
            {
                Batch batchAt = bl.ElementAt(i);
              
                if(batchAt.getRenderType() == type && renderLayer == batchAt.getRenderLayer())//if there is a compatible batch, try to fit the model in.
                {
                    
                    if((batchAt.containsTexture(tex) || batchAt.tryAddTexture(tex)) && batchAt.tryToFitInBatchModel(theModel))
                    {
                        return;//successfull batch adding
                    }
                }

                if(i == bl.Count - 1)//if we have itterated through all batches and found no candidate, then add new batch.
                {
                    //ensure that all opaque batches come before transparent ones in the list.
                    if (isTypeTransparent(type))
                    {
                        insertTransparentBatchToEnd(bl, BatchUtil.getBatchForType(type, renderLayer).addTexture(tex), theModel);
                    }
                    else
                    {
                        insertOpaqueBatchToStart(bl, BatchUtil.getBatchForType(type, renderLayer).addTexture(tex), theModel);
                    }
                    
                    return;
                }
            }
            
        }

        private static void insertTransparentBatchToEnd(List<Batch> bl, Batch b, Model m)
        {
            if(bl.Count == 0)
            {
                b.tryToFitInBatchModel(m);
                bl.Add(b);
                return;
            }

            for(int i = bl.Count - 1; i >= 0; i--)
            {
                if(bl.ElementAt(i).getRenderLayer() <= b.getRenderLayer())
                {
                    b.tryToFitInBatchModel(m);
                    bl.Insert(i+1, b);
                    return;
                }
            }
        }

        private static void insertOpaqueBatchToStart(List<Batch> bl, Batch b, Model m)
        {
            if (bl.Count == 0)
            {
                b.tryToFitInBatchModel(m);
                bl.Add(b);
                return;
            }

            for (int i = 0; i < bl.Count; i++)
            {
                if (b.getRenderLayer() <= bl.ElementAt(i).getRenderLayer() || isTypeTransparent(bl.ElementAt(i).getRenderType()))
                {
                    b.tryToFitInBatchModel(m);
                    bl.Insert(i, b);
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
                if (batchAt.getRenderType() == type && batchAt.tryToFitInBatchPoints(pParticleModel))
                {
                    return;//successfull batch adding
                }

                if (i == batches.Count - 1)//if we have itterated through all batches and found no candidate, then add new batch.
                {
                    Batch b = BatchUtil.getBatchForType(type);
                    b.tryToFitInBatchPoints(pParticleModel);

                    //ensure that all opaque batches come before transparent ones in the list.
                    if (transparency)
                        batches.Add(b);
                    else
                        batches.Insert(0, b);
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
                if (batchAt.getRenderType() == type && batchAt.tryToFitInBatchSinglePoint(pParticle))
                {
                    return;//successfull batch adding
                }

                if (i == batches.Count - 1)//if we have itterated through all batches and found no candidate, then add new batch.
                {
                    //ensure that all opaque batches come before transparent ones in the list.
                    Batch b = BatchUtil.getBatchForType(type);
                    b.tryToFitInBatchSinglePoint(pParticle);

                    //ensure that all opaque batches come before transparent ones in the list.
                    if (transparency)
                        batches.Add(b);
                    else
                        batches.Insert(0, b);
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
                if (batchAt.getRenderType() == type && batchAt.tryToFitInBatchLerpPoint(pParticle, prevTickPParticle))
                {
                    return;//successfull batch adding
                }

                if (i == batches.Count - 1)//if we have itterated through all batches and found no candidate, then add new batch.
                {
                    //ensure that all opaque batches come before transparent ones in the list.
                    Batch b = BatchUtil.getBatchForType(type);
                    b.tryToFitInBatchLerpPoint(pParticle, prevTickPParticle);

                    //ensure that all opaque batches come before transparent ones in the list.
                    if (transparency)
                        batches.Add(b);
                    else
                        batches.Insert(0, b);

                    return;
                }
            }
        }

        //TODO: add support for transparent sprites with their own type
        public static void requestRender(Sprite3D s, Texture tex, bool transparency = false)
        {
            if (batches.Count == 0)//adding first batch
            {
                Batch b = BatchUtil.getBatchForType(RenderType.spriteCylinder).addTexture(tex);
                b.tryToFitInBatchSprite3D(s);
                batches.Add(b);
                return;
            }

            for (int i = 0; i < batches.Count; ++i)
            {
                Batch batchAt = batches.ElementAt(i);
                if (batchAt.getRenderType() == RenderType.spriteCylinder)
                {
                    if ((batchAt.containsTexture(tex) || batchAt.tryAddTexture(tex)) && batchAt.tryToFitInBatchSprite3D(s))
                        return;//successfull batch adding
                }

                if (i == batches.Count - 1)//if we have itterated through all batches and found no candidate, then add new batch.
                {
                    Batch b = BatchUtil.getBatchForType(RenderType.spriteCylinder).addTexture(tex);
                    b.tryToFitInBatchSprite3D(s);
                    batches.Insert(0, b);
                    return;
                }
            }
        }

        public static void onWindowResize()
        {
            World w = GameInstance.get.currentWorld;
            foreach (Batch b in batches)
            {
                b.updateUniforms(w);
            }
            foreach (Batch b in guiBatches)
            {
                b.updateUniforms(w);
            }
        }

        public static void onVideoSettingsChanged()
        {
            World w = GameInstance.get.currentWorld;
            foreach (Batch b in batches)
            {
                b.updateUniforms(w);
            }
            foreach (Batch b in guiBatches)
            {
                b.updateUniforms(w);
            }
        }

        public static void drawAllWorld()
        {
            World w = GameInstance.get.currentWorld;
            foreach (Batch b in batches)
            {
                b.drawBatch(w);
                Renderer.totalDraws++;
            }
        }
        public static void drawAllGUI()
        {
            World w = GameInstance.get.currentWorld;
            foreach (Batch b in guiBatches)
            {
                b.drawBatch(w);
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
        public static int guiBatchCount { get => guiBatches.Count; }
    }
}
