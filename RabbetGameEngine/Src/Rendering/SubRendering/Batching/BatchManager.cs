using RabbetGameEngine.Models;
using System.Collections.Generic;
using System.Linq;

namespace RabbetGameEngine.SubRendering
{
    public enum BatchType//do not change order
    {
        none,
        text2D,
        text3D,
        triangles,
        lines,
        lerpTriangles,
        lerpLines,
        lerpPointCloud,
        lerpSinglePoint,
        lerpTrianglesTransparent,
        lerpPointCloudTransparent,
        lerpSinglePointTransparent,
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
                Batch batchAt = batches.ElementAt(i);
                //if a batch has not been used, then it needs to be removed.
                if(!batchAt.hasBeenUsedSinceLastUpdate())
                {
                    batchAt.delete();
                    batches.RemoveAt(i);
                    --i;
                    continue;
                }
                batchAt.onUpdate();
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
            if (type >= BatchType.lerpPointCloud && type <= BatchType.lerpSinglePointTransparent)
            {
                if (theModel.indices != null)
                {
                    Application.warn("A render of GL_POINTS was requested, but indicies were provided, was this a mistake?");
                }
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
                    if (isTypeTransparent(type))
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
        }

        public static void drawAll()
        {
            foreach(Batch b in batches)
            {
                b.draw();
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

        /// <summary>
        /// returns true if the provided batch type needs to be drawn in a transparency pass
        /// </summary>
        /// <param name="type">the type to test</param>
        /// <returns>returns true if the provided batch type needs to be drawn in a transparency pass</returns>
        public static bool isTypeTransparent(BatchType type)
        {
            //this can be changed if batch types are added or removed in the future
            return type >= BatchType.lerpPointCloudTransparent;
        }

        /// <summary>
        /// returns true if the provided batch type uses a shader that requires matrix uniforms
        /// </summary>
        /// <param name="type">the type to test</param>
        /// <returns>returns true if the provided batch type uses a shader that requires matrix uniforms</returns>
        public static bool doesTypeUseMatrixUniforms(BatchType type)
        {
            return type >= BatchType.lerpTriangles && type < BatchType.trianglesTransparent;
        }

    }
}
