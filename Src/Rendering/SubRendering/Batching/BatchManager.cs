using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using System.Collections.Generic;
using System.Linq;

namespace RabbetGameEngine.SubRendering
{
    public enum BatchType
    {
        none,
        guiCutout,
        guiText,
        /// <summary>
        /// text3D objects should be built relative to 0,0,0. And then a position vector should be sent to the GPU for the position of the text in real world.
        /// </summary>
        text3D,
        triangles,
        lines,
        iSpheres,
        iSpheresTransparent,
        lerpISpheres,
        lerpTriangles,
        lerpLines,
        lerpISpheresTransparent,
        lerpTrianglesTransparent,
        trianglesTransparent
    }

    /*Class for constructing*/
    public static class BatchManager
    {
        private static List<Batch> batches = new List<Batch>();

        /// <summary>
        /// Should be called before any rendering requests. For instance, begining of each tick loop.
        /// </summary>
        public static void onTickStart()
        {
            for (int i = 0; i < batches.Count; ++i)
            {
                batches.ElementAt(i).onTickStart();
            }
        }

        /// <summary>
        /// Should be called before rendering any batches. For instance, begining end of each tick loop.
        /// </summary>
        public static void onTickEnd()
        {
            for (int i = 0; i < batches.Count; ++i)
            {
                Batch batchAt = batches.ElementAt(i);

                //if a batch has not been used, then it needs to be removed.
                if (!batchAt.hasBeenUsedInCurrentTick())
                {
                    batchAt.delete();
                    batches.Remove(batchAt);
                    --i;
                    continue;
                }
                batchAt.onTickEnd();
            }
        }

        /// <summary>
        /// Can be called to request that the provided data be added to the appropriate existing batch
        /// or, if said batch does not exist or is full, creates and adds a new batch.
        /// </summary>
        /// <param name="type">The category of batch this data applies to</param>
        /// <param name="vertices">The vertices of this render data</param>
        /// <param name="indices">The indices of this render data, can be left blank if this does not need indices (i.e points)</param>
        public static void requestRender(BatchType type, Texture tex, Model theModel)
        {
            if (batches.Count < 1)
            {
                batches.Add(new Batch(type, tex));
                batches.ElementAt(0).addToBatch(theModel);
                return;
            }

            for (int i = 0; i < batches.Count; ++i)
            {
                Batch batchAt = batches.ElementAt(i);
                if(batchAt.getBatchType() == type && batchAt.getBatchtexture() == tex &&  batchAt.addToBatch(theModel))
                {
                    return;//successfull batch adding
                }

                if(i == batches.Count - 1)//if we have itterated through all batches and found no candidate, then add new batch.
                {
                    //ensure that all opaque batches come before transparent ones in the list.
                    if (type == BatchType.guiText)
                    {
                        batches.Add(new Batch(type, tex));
                        batches.ElementAt(i + 1).addToBatch(theModel);
                    }
                    else if (type == BatchType.lerpTrianglesTransparent || type == BatchType.trianglesTransparent)
                    {
                        insertTransparentNonGUIBatch(new Batch(type, tex), theModel);
                    }
                    else
                    {
                        batches.Insert(0, new Batch(type, tex));
                        batches.ElementAt(0).addToBatch(theModel);
                    }
                    
                    return;
                }
            }
            
        }
        public static void requestRender(PointCloudModel pParticleModel, bool transparency, bool lerp)
        {
            BatchType type;
            if(transparency)
            {
                if(!lerp)
                {
                    type = BatchType.iSpheresTransparent;
                }
                else
                {
                    type = BatchType.lerpISpheresTransparent;
                }
            }
            else
            {
                if (!lerp)
                {
                    type = BatchType.iSpheres;
                }
                else
                {
                    type = BatchType.lerpISpheres;
                }
            }

            for (int i = 0; i < batches.Count; ++i)
            {
                Batch batchAt = batches.ElementAt(i);
                if (batchAt.getBatchType() == type && batchAt.addToBatch(pParticleModel))
                {
                    return;//successfull batch adding
                }

                if (i == batches.Count - 1)//if we have itterated through all batches and found no candidate, then add new batch.
                {
                    //ensure that all opaque batches come before transparent ones in the list.
                    if (transparency)
                    {
                        insertTransparentNonGUIBatch(new Batch(transparency, lerp), pParticleModel);
                    }
                    else
                    {
                        batches.Insert(0, new Batch(transparency, lerp));
                        batches.ElementAt(0).addToBatch(pParticleModel);
                    }

                    return;
                }
            }
        }


        public static void requestRender(PointParticle pParticle, bool transparency, bool lerp)
        {
            BatchType type;
            if (transparency)
            {
                if (!lerp)
                {
                    type = BatchType.iSpheresTransparent;
                }
                else
                {
                    type = BatchType.lerpISpheresTransparent;
                }
            }
            else
            {
                if (!lerp)
                {
                    type = BatchType.iSpheres;
                }
                else
                {
                    type = BatchType.lerpISpheres;
                }
            }
            for (int i = 0; i < batches.Count; ++i)
            {
                Batch batchAt = batches.ElementAt(i);
                if (batchAt.getBatchType() == type && (lerp ? batchAt.addToBatch(pParticle, pParticle) : batchAt.addToBatch(pParticle)))
                {
                    return;//successfull batch adding
                }

                if (i == batches.Count - 1)//if we have itterated through all batches and found no candidate, then add new batch.
                {
                    //ensure that all opaque batches come before transparent ones in the list.
                    if (transparency)
                    {
                        insertTransparentNonGUIBatch(new Batch(transparency, lerp), pParticle, lerp);
                    }
                    else
                    {
                        batches.Insert(0, new Batch(transparency, lerp));
                        batches.ElementAt(0).addToBatch(pParticle);
                    }

                    return;
                }
            }
        }

        public static void requestRender(PointParticle pParticle, PointParticle prevTickPParticle, bool transparency)
        {
            BatchType type;
            if (transparency)
            {
                type = BatchType.lerpISpheresTransparent;

            }
            else
            {
                type = BatchType.lerpISpheres;

            }
            for (int i = 0; i < batches.Count; ++i)
            {
                Batch batchAt = batches.ElementAt(i);
                if (batchAt.getBatchType() == type && batchAt.addToBatch(pParticle, prevTickPParticle))
                {
                    return;//successfull batch adding
                }

                if (i == batches.Count - 1)//if we have itterated through all batches and found no candidate, then add new batch.
                {
                    //ensure that all opaque batches come before transparent ones in the list.
                    if (transparency)
                    {
                        batches.Add(new Batch(transparency, true));
                        batches.ElementAt(i + 1).addToBatch(pParticle, prevTickPParticle);
                    }
                    else
                    {
                        batches.Insert(0, new Batch(transparency, true));
                        batches.ElementAt(0).addToBatch(pParticle, prevTickPParticle);
                    }

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
                    batches.ElementAt(i + 1).addToBatch(mod);
                    return;
                }
            }
            batches.Insert(0, theBatch);
            batches.ElementAt(0).addToBatch(mod);
        }

        private static void insertTransparentNonGUIBatch(Batch theBatch,PointCloudModel pModel)
        {
            for (int i = batches.Count - 1; i >= 0; --i)
            {
                if (!batches.ElementAt(i).transparentGUI)
                {
                    batches.Insert(i + 1, theBatch);
                    batches.ElementAt(i + 1).addToBatch(pModel);
                    return;
                }
            }
            batches.Insert(0, theBatch);
            batches.ElementAt(0).addToBatch(pModel);
        }

        
        private static void insertTransparentNonGUIBatch(Batch theBatch, PointParticle p, bool lerp)
        {
            for (int i = batches.Count - 1; i >= 0; --i)
            {
                if (!batches.ElementAt(i).transparentGUI)
                {
                    batches.Insert(i + 1, theBatch);
                    if (lerp)
                    {
                        batches.ElementAt(i + 1).addToBatch(p, p);
                    }
                    else
                    {
                        batches.ElementAt(i + 1).addToBatch(p);
                    }
                    return;
                }
            }
            batches.Insert(0, theBatch);

            if (lerp)
            {
                batches.ElementAt(0).addToBatch(p, p);
            }
            else
            {
                batches.ElementAt(0).addToBatch(p);
            }
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
                b.delete();
            }
        }

        public static int batchCount { get => batches.Count; }
    }
}
