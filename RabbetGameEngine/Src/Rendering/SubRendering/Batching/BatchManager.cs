using OpenTK;
using RabbetGameEngine.Models;
using System.Collections.Generic;
using System.Linq;

namespace RabbetGameEngine.SubRendering
{
    public enum BatchType//do not change order
    {
        none,
        guiCutout,
        text2D,
        text3D,
        triangles,
        lines,
        lerpPoints,
        lerpTriangles,
        lerpLines,
        lerpPointsTransparent,
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
        public static void updateAll()
        {
            for (int i = 0; i < batches.Count; ++i)
            {
                batches.ElementAt(i).onTickStart();
            }
        }

        /// <summary>
        /// Should be called before rendering any batches. For instance, begining end of each tick loop.
        /// </summary>
        public static void prepareAll()
        {
            for (int i = 0; i < batches.Count; ++i)
            {
                Batch batchAt = batches.ElementAt(i);

                //if a batch has not been used, then it needs to be removed.
                if (!batchAt.hasBeenUsedInCurrentTick())
                {
                    batchAt.delete();
                    batches.RemoveAt(i);
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
            //if the type is a point rendering type
            if (type == BatchType.lerpPoints || type == BatchType.lerpPointsTransparent)
            {
                if (theModel.indices != null)
                {
                    Application.error("A render of point particles was requested to a batch, but no point cloud model was provided.");
                }
                return;
            }
            
            for(int i = 0; i < batches.Count; ++i)
            {
                Batch batchAt = batches.ElementAt(i);
                if(batchAt.getBatchType() == type && batchAt.getBatchtexture() == tex &&  batchAt.addToBatch(theModel))
                {
                    return;//successfull batch adding
                }

                if(i == batches.Count - 1)//if we have itterated through all batches and found no candidate, then add new batch.
                {
                    //ensure that all opaque batches come before transparent ones in the list.
                    if (type == BatchType.lerpTrianglesTransparent)
                    {
                        batches.Add(new Batch(type, tex));
                        batches.ElementAt(i + 1).addToBatch(theModel);
                    }
                    else
                    {
                        batches.Insert(0, new Batch(type, tex));
                        batches.ElementAt(0).addToBatch(theModel);
                    }
                    
                    return;
                }
            }
            if (batches.Count < 1)
            {
                batches.Add(new Batch(type, tex));
                batches.ElementAt(0).addToBatch(theModel);
            }
        }


        /// <summary>
        /// Can be called to request that the provided data be added to the appropriate existing batch
        /// or, if said batch does not exist or is full, creates and adds a new batch.
        /// </summary>
        /// <param name="type">The category of batch this data applies to</param>
        /// <param name="vertices">The vertices of this render data</param>
        /// <param name="indices">The indices of this render data, can be left blank if this does not need indices (i.e points)</param>
        public static void requestRender(PointCloudModel pParticleModel, bool transparency)
        {
            BatchType type = transparency ? BatchType.lerpPointsTransparent : BatchType.lerpPoints;
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
                        batches.Add(new Batch(transparency));
                        batches.ElementAt(i + 1).addToBatch(pParticleModel);
                    }
                    else
                    {
                        batches.Insert(0, new Batch(transparency));
                        batches.ElementAt(0).addToBatch(pParticleModel);
                    }

                    return;
                }
            }
        }

        public static void drawAll(Matrix4 projectionMatrix, Matrix4 viewMatrix, Vector3 fogColor)
        {
            foreach(Batch b in batches)
            {
                b.draw(projectionMatrix, viewMatrix, fogColor);
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
