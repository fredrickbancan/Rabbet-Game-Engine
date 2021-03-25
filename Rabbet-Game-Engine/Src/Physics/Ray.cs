using OpenTK.Mathematics;

namespace RabbetGameEngine.Physics
{
    /// <summary>
    /// A stuct for creating and testing rays against boxes and other colliders
    /// This ray can not point exactly straight in any axis to avoid computational issues.
    /// For rays in straight axis', use struct AxisAlignedRay.
    /// </summary>
    public struct Ray
    {
        /// <summary>
        /// origin position of ray
        /// </summary>
        Vector3 origin;

        /// <summary>
        /// direction vector of ray (normalized)
        /// </summary>
        private Vector3 direction;

        /// <summary>
        /// length of this ray
        /// </summary>
        float length;

        public Ray(Vector3 origin, Vector3 directionAndLength)
        {
            this.origin = origin;
            if (directionAndLength.X == 0) direction.X = 0.000001F;
            if (directionAndLength.Y == 0) direction.Y = 0.000001F;
            if (directionAndLength.Z == 0) direction.Z = 0.000001F;
            length = directionAndLength.Length;
            direction = directionAndLength.Normalized();
        }

        public Ray(Vector3 origin, Vector3 direction, float length)
        {
            this.origin = origin;
            if (direction.X == 0) direction.X = 0.000001F;
            if (direction.Y == 0) direction.Y = 0.000001F;
            if (direction.Z == 0) direction.Z = 0.000001F;
            this.length = length;
            this.direction = direction.Normalized();
        }

        /// <summary>
        /// Tests if this ray intersects the provided box
        /// </summary>
        /// <param name="box">the box to test against</param>
        /// <param name="intersectionPos">the resulting location of intersection of the ray and box. If there is no intersection, intersection pos will be 0 in all dimensions.</param>
        /// <returns>true if intersection takes place. Else returns false.</returns>
        public bool getIntersection(AABB box, out Vector3 intersectionpos)
        {
            //distances from ray origin to each axis of box
            float xMin = 0, xMax = 0, yMin = 0, yMax = 0, zMin = 0, zMax = 0;

            //getting distances
            ////
            if (dirX < 0)
            {
                xMin = (box.minX -origin.X) /direction.X;
                xMax = (box.maxX -origin.X) /direction.X;
            }
            else
            {
                xMin = (box.maxX -origin.X) /direction.X;
                xMax = (box.minX -origin.X) /direction.X;
            }

            if (dirY < 0)
            {
                yMin = (box.maxY -origin.Y) /direction.Y;
                yMax = (box.minY -origin.Y) /direction.Y;
            }
            else
            {
                yMin = (box.minY -origin.Y) /direction.Y;
                yMax = (box.maxY -origin.Y) /direction.Y;
            }

            if (dirZ < 0)
            {
                zMin = (box.maxZ - origin.Z) / direction.Z;
                zMax = (box.minZ - origin.Z) / direction.Z;
            }
            else
            {
                zMin = (box.minZ - origin.Z) / direction.Z;
                zMax = (box.maxZ - origin.Z) / direction.Z;
            }
            ////

            //ensure ray is hitting box
            if (xMin > yMax || yMin > xMax || xMin > zMax || zMin > xMax || zMin > yMax || yMin > zMax)
            {
                intersectionpos = Vector3.Zero;
                return false;
            }
            float firstContactDist = MathUtil.max3(xMin, yMin, zMin);
            if (firstContactDist >= 0 && firstContactDist <= length)
            {
                intersectionpos = origin + (direction * firstContactDist);
                return true;
            }
            intersectionpos = Vector3.Zero;
            return false;
        }

        /// <summary>
        /// Tests if this ray intersects the provided box
        /// </summary>
        /// <param name="box">the box to test against</param>
        /// <returns>true if intersection takes place. Else returns false.</returns>
        public bool intersects(AABB box)
        {
            //distances from ray origin to each axis of box
            float xMin = 0, xMax = 0, yMin = 0, yMax = 0, zMin = 0, zMax = 0;

            //getting distances
            ////
            if (dirX < 0)
            {
                xMin = (box.minX - origin.X) / direction.X;
                xMax = (box.maxX - origin.X) / direction.X;
            }
            else
            {
                xMin = (box.maxX - origin.X) / direction.X;
                xMax = (box.minX - origin.X) / direction.X;
            }

            if (dirY < 0)
            {
                yMin = (box.maxY - origin.Y) / direction.Y;
                yMax = (box.minY - origin.Y) / direction.Y;
            }
            else
            {
                yMin = (box.minY - origin.Y) / direction.Y;
                yMax = (box.maxY - origin.Y) / direction.Y;
            }

            if (dirZ < 0)
            {
                zMin = (box.maxZ - origin.Z) / direction.Z;
                zMax = (box.minZ - origin.Z) / direction.Z;
            }
            else
            {
                zMin = (box.minZ - origin.Z) / direction.Z;
                zMax = (box.maxZ - origin.Z) / direction.Z;
            }
            ////

            //ensure ray is hitting box
            return !(xMin > yMax || yMin > xMax || zMin > xMax || xMin > zMax || zMin > yMax || yMin > zMax);
        }

        /// <summary>
        /// Gives a point along this ray closest to the provided point in space
        /// </summary>
        /// <param name="testPoint">The point to test</param>
        /// <returns>A Vector3 point along this ray closest to the provided point in space</returns>
        public Vector3 getClosestPointOnRay(Vector3 testPoint)
        {
            float dotProduct = Vector3.Dot(testPoint - origin, direction);
            float dotClamped = MathUtil.clamp(dotProduct, 0, length);
            return direction * dotClamped;
        }

        /// <summary>
        /// Gives a point along the provided ray closest to the provided point in space
        /// </summary>
        /// <param name="testPoint">The point to test</param>
        /// <param name="ray">The ray to test</param>
        /// <returns>A Vector3 point along the provided ray closest to the provided point in space</returns>
        public static Vector3 getClosestPointOnRay(Ray ray, Vector3 testPoint)
        {
            float dotProduct = Vector3.Dot(testPoint - ray.origin, ray.direction);
            float dotClamped = MathUtil.clamp(dotProduct, 0, ray.length);
            return ray.direction * dotClamped;
        }

        /// <summary>
        /// Tests if this ray intersects the provided box
        /// </summary>
        /// <param name="box">the box to test against</param>
        /// <param name="ray">the ray to test against</param>
        /// <param name="intersectionPos">resulting position of intersection. If there is no intersection, will be zero in all dimensions.</param>
        /// <returns>true if intersection takes place. Else returns false.</returns>
        public static bool getIntersection(Ray ray, AABB box, out Vector3 intersectionPos)
        {
            //distances from ray origin to each axis of box
            float xMin = 0, xMax = 0, yMin = 0, yMax = 0, zMin = 0, zMax = 0;

            //getting distances
            ////
            if (ray.dirX < 0)
            {
                xMin = (box.minX - ray.origin.X) / ray.direction.X;
                xMax = (box.maxX - ray.origin.X) / ray.direction.X;
            }
            else
            {
                xMin = (box.maxX - ray.origin.X) / ray.direction.X;
                xMax = (box.minX - ray.origin.X) / ray.direction.X;
            }

            if (ray.dirY < 0)
            {
                yMin = (box.maxY - ray.origin.Y) / ray.direction.Y;
                yMax = (box.minY - ray.origin.Y) / ray.direction.Y;
            }
            else
            {
                yMin = (box.minY - ray.origin.Y) / ray.direction.Y;
                yMax = (box.maxY - ray.origin.Y) / ray.direction.Y;
            }

            if (ray.dirZ < 0)
            {
                zMin = (box.maxZ - ray.origin.Z) / ray.direction.Z;
                zMax = (box.minZ - ray.origin.Z) / ray.direction.Z;
            }
            else
            {
                zMin = (box.minZ - ray.origin.Z) / ray.direction.Z;
                zMax = (box.maxZ - ray.origin.Z) / ray.direction.Z;
            }
            ////

            //ensure ray is hitting box
            if (xMin > yMax || yMin > xMax || zMin > xMax || xMin > zMax || zMin > yMax || yMin > zMax)
            {
                intersectionPos = Vector3.Zero;
                return false;
            }
            float firstContactDist = MathUtil.max3(xMin, yMin, zMin);
            if (firstContactDist >= 0 && firstContactDist <= ray.length)
            {
                intersectionPos = ray.origin + (ray.direction * firstContactDist);
                return true;
            }
            intersectionPos = Vector3.Zero;
            return false;
        }

        /// <summary>
        /// Tests if this ray intersects the provided box
        /// </summary>
        /// <param name="box">the box to test against</param>
        /// <param name="ray">the ray to test against</param>
        /// <returns>true if intersection takes place. Else returns false.</returns>
        public static bool intersects(Ray ray, AABB box)
        {
            //distances from ray origin to each axis of box
            float xMin = 0, xMax = 0, yMin = 0, yMax = 0, zMin = 0, zMax = 0;

            //getting distances
            ////
            if (ray.dirX < 0)
            {
                xMin = (box.minX - ray.origin.X) / ray.direction.X;
                xMax = (box.maxX - ray.origin.X) / ray.direction.X;
            }
            else
            {
                xMin = (box.maxX - ray.origin.X) / ray.direction.X;
                xMax = (box.minX - ray.origin.X) / ray.direction.X;
            }

            if (ray.dirY < 0)
            {
                yMin = (box.maxY - ray.origin.Y) / ray.direction.Y;
                yMax = (box.minY - ray.origin.Y) / ray.direction.Y;
            }
            else
            {
                yMin = (box.minY - ray.origin.Y) / ray.direction.Y;
                yMax = (box.maxY - ray.origin.Y) / ray.direction.Y;
            }

            if (ray.dirZ < 0)
            {
                zMin = (box.maxZ - ray.origin.Z) / ray.direction.Z;
                zMax = (box.minZ - ray.origin.Z) / ray.direction.Z;
            }
            else
            {
                zMin = (box.minZ - ray.origin.Z) / ray.direction.Z;
                zMax = (box.maxZ - ray.origin.Z) / ray.direction.Z;
            }
            ////

            //ensure ray is hitting box
            return !(xMin > yMax || yMin > xMax || zMin > xMax || xMin > zMax || zMin > yMax || yMin > zMax);
        }

        public float dirX { get => direction.X; }
        public float dirY { get => direction.Y; }
        public float dirZ { get => direction.Z; }
    }
}
