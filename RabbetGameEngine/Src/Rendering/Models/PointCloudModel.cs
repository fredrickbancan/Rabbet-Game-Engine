using OpenTK;
using System;

namespace RabbetGameEngine.Models
{
    public class PointCloudModel
    {
        public PointParticle[] points = null;
        public PointParticle[] prevPoints = null;

        public PointCloudModel(PointParticle[] points)
        {
            this.points = points;
            this.prevPoints = new PointParticle[points.Length];
            preTick();
        }

        /*sets all point colors to this color*/
        public PointCloudModel setColor(CustomColor color)
        {
            return this.setColor(color.toNormalVec4());
        }
        public PointCloudModel setColor(Vector4 color)
        {
            for(int i = 0; i < points.Length; i++)
            {
                points[i].color = color;
            }
            return this;
        }
        
        /// <summary>
        /// stores the current state of all points in this cloud in the previous tick array.
        /// This allows for interpolation.
        /// THIS MUST BE CALLED BEFORE ANY CHANGES TO THE POINTS OF THIS CLOUD MODEL ARE APPLIED.
        /// </summary>
        public void preTick()
        {
            for(int i = 0; i < points.Length; i++)
            {
                prevPoints[i] = points[i];
            }
        }

        /// <summary>
        /// transforms the points of this model by the provided model matrix.
        /// This must be used to transform pointcloudmodels before rendering.
        /// Linear interpolation of pos and color is done by the GPU on a point by point basis.
        /// </summary>
        /// <param name="modelMatrix">The transformation matrix</param>
        /// <returns>this (builder method)</returns>
        public PointCloudModel transformPoints(Matrix4 modelMatrix)
        {
            for(int i = 0; i < points.Length; ++i)
            {
                points[i].pos = Vector3.TransformPerspective(points[i].pos, modelMatrix);
            }
            return this;
        }

        /*Changes the vertices in the float arrays before rendering. Usefull for copying an already layed out model
          and batch rendering it in multiple different locations with different transformations.*/
        public PointCloudModel transformPoints(Vector3 scale, Vector3 rotate, Vector3 translate)
        {
            for(int i = 0; i < points.Length; i ++)
            {
                MathUtil.scaleXYZFloats(scale, points[i].pos.X, points[i].pos.Y, points[i].pos.Z, out points[i].pos.X, out points[i].pos.Y, out points[i].pos.Z);
                MathUtil.rotateXYZFloats(rotate, points[i].pos.X, points[i].pos.Y, points[i].pos.Z, out points[i].pos.X, out points[i].pos.Y, out points[i].pos.Z);
                MathUtil.translateXYZFloats(translate, points[i].pos.X, points[i].pos.Y, points[i].pos.Z, out points[i].pos.X, out points[i].pos.Y, out points[i].pos.Z);
            }
            return this;
        }
        public PointCloudModel scalePoints(Vector3 scale)
        {
            for (int i = 0; i < points.Length; i ++)
            {
                MathUtil.scaleXYZFloats(scale, points[i].pos.X, points[i].pos.Y, points[i].pos.Z, out points[i].pos.X, out points[i].pos.Y, out points[i].pos.Z);
            }
            return this;
        }
        public PointCloudModel rotatePoints(Vector3 rotate)
        {
            for (int i = 0; i < points.Length; i++)
            {
                MathUtil.rotateXYZFloats(rotate, points[i].pos.X, points[i].pos.Y, points[i].pos.Z, out points[i].pos.X, out points[i].pos.Y, out points[i].pos.Z);
            }
            return this;
        }

        public PointCloudModel translatePoints(Vector3 translate)
        {
            for (int i = 0; i < points.Length; i++)
            {
                MathUtil.translateXYZFloats(translate, points[i].pos.X, points[i].pos.Y, points[i].pos.Z, out points[i].pos.X, out points[i].pos.Y, out points[i].pos.Z);
            }
            return this;
        }

        public PointCloudModel scaleRadii(float scale)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i].radius *= scale;
            }
            return this;
        }
        public PointCloudModel setRadii(float rad)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i].radius = rad;
            }
            return this;
        }

        /*generates a new model using copies of this models arrays.*/
        public PointCloudModel copyCloudModel()
        {
            PointParticle[] pointsCopy = new PointParticle[points.Length];
            Array.Copy(points, pointsCopy, points.Length);
            return new PointCloudModel(pointsCopy);
        }

    }
}
