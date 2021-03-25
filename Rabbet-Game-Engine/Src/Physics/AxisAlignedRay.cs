using OpenTK.Mathematics;

namespace RabbetGameEngine.Physics
{
    /// <summary>
    /// The direction that a AxisAligned ray should point.
    /// </summary>
    public enum RayDirection
    {
        POSX,
        NEGX,
        POSY,
        NEGY,
        POSZ,
        NEGZ
    };

    /// <summary>
    /// A ray which can only point in straight directions of each axis.
    /// Length can be variable.
    /// This can be faster than a multi directional ray.
    /// </summary>
    public struct AxisAlignedRay
    {
        private RayDirection direction;
        private Vector3 dirVec;
        Vector3 origin;
        float length;

        public AxisAlignedRay(Vector3 pos, RayDirection dir, float len)
        {
            origin = pos;
            length = len;
            direction = dir;
            switch (direction)
            {
                case RayDirection.POSX:
                    dirVec = Vector3.UnitX;
                    break;
                case RayDirection.NEGX:
                    dirVec = -Vector3.UnitX;
                    break;
                case RayDirection.POSY:
                    dirVec = Vector3.UnitY;
                    break;
                case RayDirection.NEGY:
                    dirVec = -Vector3.UnitY;
                    break;
                case RayDirection.POSZ:
                    dirVec = Vector3.UnitZ;
                    break;
                case RayDirection.NEGZ:
                    dirVec = -Vector3.UnitZ;
                    break;
                default:
                    dirVec = Vector3.Zero;
                    break;
            }
        }

        /// <summary>
        /// Tests if this ray intersects the provided box
        /// </summary>
        /// <param name="box">the box to test against</param>
        /// <param name="intersectionPos">the resulting location of intersection of the ray and box. If there is no intersection, intersection pos will be 0 in all dimensions.</param>
        /// <returns>true if intersection takes place. Else returns false.</returns>
        public bool getIntersection(AABB box, out Vector3 intersectionPos)
        {
            switch(direction)
            {
                case RayDirection.POSX:
                    if (origin.Y > box.minY && origin.Y < box.maxY && origin.Z > box.minZ && origin.Z < box.maxZ && origin.X < box.maxX)//ensure ray is in bounds of box in x dimension and is left of box
                    {
                        float xDist = box.minX - origin.X;
                        intersectionPos.X = origin.X + xDist;
                        intersectionPos.Y = origin.Y;
                        intersectionPos.Z = origin.Z;
                        return xDist <= length;
                    }
                    break;
                case RayDirection.NEGX:
                    if (origin.Y > box.minY && origin.Y < box.maxY && origin.Z > box.minZ && origin.Z < box.maxZ && origin.X > box.minX)//ensure ray is in bounds of box in x dimension and is right of box
                    {
                        float xDist = origin.X - box.maxX;
                        intersectionPos.X = origin.X - xDist;
                        intersectionPos.Y = origin.Y;
                        intersectionPos.Z = origin.Z;
                        return xDist <= length;
                    }
                    break;
                case RayDirection.POSY:
                    if (origin.X > box.minX && origin.X < box.maxX && origin.Z > box.minZ && origin.Z < box.maxZ && origin.Y < box.maxY)//ensure ray is in bounds of box in y dimension and is below box
                    {
                        float yDist = box.minY - origin.Y;
                        intersectionPos.X = origin.X;
                        intersectionPos.Y = origin.Y + yDist;
                        intersectionPos.Z = origin.Z;
                        return yDist <= length;
                    }
                    break;
                case RayDirection.NEGY:
                    if (origin.X > box.minX && origin.X < box.maxX && origin.Z > box.minZ && origin.Z < box.maxZ && origin.Y > box.minY)//ensure ray is in bounds of box in y dimension and is above box
                    {
                        float yDist = origin.Y - box.minY;
                        intersectionPos.X = origin.X;
                        intersectionPos.Y = origin.Y - yDist;
                        intersectionPos.Z = origin.Z;
                        return yDist <= length;
                    }
                    break;
                case RayDirection.POSZ:
                    if (origin.X > box.minX && origin.X < box.maxX && origin.Y > box.minY && origin.Y < box.maxY && origin.Z < box.maxZ)//ensure ray is in bounds of box in z dimension and is behind box
                    {
                        float zDist = box.minZ - origin.Z;
                        intersectionPos.X = origin.X;
                        intersectionPos.Y = origin.Y;
                        intersectionPos.Z = origin.Z + zDist;
                        return zDist <= length;
                    }
                    break;
                case RayDirection.NEGZ:
                    if (origin.X > box.minX && origin.X < box.maxX && origin.Y > box.minY && origin.Y < box.maxY && origin.Z > box.minZ)//ensure ray is in bounds of box in z dimension and is in front of box
                    {
                        float zDist = origin.Z - box.maxZ;
                        intersectionPos.X = origin.X;
                        intersectionPos.Y = origin.Y;
                        intersectionPos.Z = origin.Z - zDist;
                        return zDist <= length;
                    }
                    break;
            }
            intersectionPos = Vector3.Zero;
            return false;
        }

        /// <summary>
        /// Tests if this ray intersects the provided box
        /// </summary>
        /// <param name="box">the box to test against</param>
        /// <returns>true if intersection takes place. Else returns false.</returns>
        public bool intersects(AABB box)
        {
            switch (direction)
            {
                case RayDirection.POSX:
                    if (origin.Y > box.minY && origin.Y < box.maxY && origin.Z > box.minZ && origin.Z < box.maxZ && origin.X < box.maxX)//ensure ray is in bounds of box in x dimension and is left of box
                    {
                        return box.minX - origin.X <= length;
                    }
                    break;
                case RayDirection.NEGX:
                    if (origin.Y > box.minY && origin.Y < box.maxY && origin.Z > box.minZ && origin.Z < box.maxZ && origin.X > box.minX)//ensure ray is in bounds of box in x dimension and is right of box
                    {
                        return origin.X - box.maxX <= length;
                    }
                    break;
                case RayDirection.POSY:
                    if (origin.X > box.minX && origin.X < box.maxX && origin.Z > box.minZ && origin.Z < box.maxZ && origin.Y < box.maxY)//ensure ray is in bounds of box in y dimension and is below box
                    {
                        return box.minY - origin.Y <= length;
                    }
                    break;
                case RayDirection.NEGY:
                    if (origin.X > box.minX && origin.X < box.maxX && origin.Z > box.minZ && origin.Z < box.maxZ && origin.Y > box.minY)//ensure ray is in bounds of box in y dimension and is above box
                    {
                        return origin.Y - box.minY <= length;
                    }
                    break;
                case RayDirection.POSZ:
                    if (origin.X > box.minX && origin.X < box.maxX && origin.Y > box.minY && origin.Y < box.maxY && origin.Z < box.maxZ)//ensure ray is in bounds of box in z dimension and is behind box
                    {
                        return box.minZ - origin.Z <= length;
                    }
                    break;
                case RayDirection.NEGZ:
                    if (origin.X > box.minX && origin.X < box.maxX && origin.Y > box.minY && origin.Y < box.maxY && origin.Z > box.minZ)//ensure ray is in bounds of box in z dimension and is in front of box
                    {
                        return origin.Z - box.maxZ <= length;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// Gives a point along this ray closest to the provided point in space
        /// </summary>
        /// <param name="testPoint">The point to test</param>
        /// <returns>A Vector3 point along this ray closest to the provided point in space</returns>
        public Vector3 getClosestPointOnRay(Vector3 testPoint)
        {
            float dotProduct = Vector3.Dot(testPoint - origin, dirVec);
            float dotClamped = MathUtil.clamp(dotProduct, 0, length);
            return dirVec * dotClamped;
        }

        /// <summary>
        /// Gives a point along this ray closest to the provided point in space
        /// </summary>
        /// <param name="testPoint">The point to test</param>
        /// <returns>A Vector3 point along this ray closest to the provided point in space</returns>
        public static Vector3 getClosestPointOnRay(AxisAlignedRay ray, Vector3 testPoint)
        {
            float dotProduct = Vector3.Dot(testPoint - ray.origin, ray.dirVec);
            float dotClamped = MathUtil.clamp(dotProduct, 0, ray.length);
            return ray.dirVec * dotClamped;
        }

        /// <summary>
        /// Tests if this ray intersects the provided box
        /// </summary>
        /// <param name="box">the box to test against</param>
        /// <param name="intersectionPos">the resulting location of intersection of the ray and box. If there is no intersection, intersection pos will be 0 in all dimensions.</param>
        /// <returns>true if intersection takes place. Else returns false.</returns>
        public static bool getIntersection(AxisAlignedRay ray, AABB box, out Vector3 intersectionPos)
        {
            switch (ray.direction)
            {
                case RayDirection.POSX:
                    if (ray.origin.Y > box.minY && ray.origin.Y < box.maxY && ray.origin.Z > box.minZ && ray.origin.Z < box.maxZ && ray.origin.X < box.maxX)//ensure ray is in bounds of box in x dimension and is left of box
                    {
                        float xDist = box.minX - ray.origin.X;
                        intersectionPos.X = ray.origin.X + xDist;
                        intersectionPos.Y = ray.origin.Y;
                        intersectionPos.Z = ray.origin.Z;
                        return xDist <= ray.length;
                    }
                    break;
                case RayDirection.NEGX:
                    if (ray.origin.Y > box.minY && ray.origin.Y < box.maxY && ray.origin.Z > box.minZ && ray.origin.Z < box.maxZ && ray.origin.X > box.minX)//ensure ray is in bounds of box in x dimension and is right of box
                    {
                        float xDist = ray.origin.X - box.maxX;
                        intersectionPos.X = ray.origin.X - xDist;
                        intersectionPos.Y = ray.origin.Y;
                        intersectionPos.Z = ray.origin.Z;
                        return xDist <= ray.length;
                    }
                    break;
                case RayDirection.POSY:
                    if (ray.origin.X > box.minX && ray.origin.X < box.maxX && ray.origin.Z > box.minZ && ray.origin.Z < box.maxZ && ray.origin.Y < box.maxY)//ensure ray is in bounds of box in y dimension and is below box
                    {
                        float yDist = box.minY - ray.origin.Y;
                        intersectionPos.X = ray.origin.X;
                        intersectionPos.Y = ray.origin.Y + yDist;
                        intersectionPos.Z = ray.origin.Z;
                        return yDist <= ray.length;
                    }
                    break;
                case RayDirection.NEGY:
                    if (ray.origin.X > box.minX && ray.origin.X < box.maxX && ray.origin.Z > box.minZ && ray.origin.Z < box.maxZ && ray.origin.Y > box.minY)//ensure ray is in bounds of box in y dimension and is above box
                    {
                        float yDist = ray.origin.Y - box.minY;
                        intersectionPos.X = ray.origin.X;
                        intersectionPos.Y = ray.origin.Y - yDist;
                        intersectionPos.Z = ray.origin.Z;
                        return yDist <= ray.length;
                    }
                    break;
                case RayDirection.POSZ:
                    if (ray.origin.X > box.minX && ray.origin.X < box.maxX && ray.origin.Y > box.minY && ray.origin.Y < box.maxY && ray.origin.Z < box.maxZ)//ensure ray is in bounds of box in z dimension and is behind box
                    {
                        float zDist = box.minZ - ray.origin.Z;
                        intersectionPos.X = ray.origin.X;
                        intersectionPos.Y = ray.origin.Y;
                        intersectionPos.Z = ray.origin.Z + zDist;
                        return zDist <= ray.length;
                    }
                    break;
                case RayDirection.NEGZ:
                    if (ray.origin.X > box.minX && ray.origin.X < box.maxX && ray.origin.Y > box.minY && ray.origin.Y < box.maxY && ray.origin.Z > box.minZ)//ensure ray is in bounds of box in z dimension and is in front of box
                    {
                        float zDist = ray.origin.Z - box.maxZ;
                        intersectionPos.X = ray.origin.X;
                        intersectionPos.Y = ray.origin.Y;
                        intersectionPos.Z = ray.origin.Z - zDist;
                        return zDist <= ray.length;
                    }
                    break;
            }
            intersectionPos = Vector3.Zero;
            return false;
        }

        /// <summary>
        /// Tests if this ray intersects the provided box
        /// </summary>
        /// <param name="box">the box to test against</param>
        /// <returns>true if intersection takes place. Else returns false.</returns>
        public static bool intersects(AxisAlignedRay ray, AABB box)
        {
            switch (ray.direction)
            {
                case RayDirection.POSX:
                    if (ray.origin.Y > box.minY && ray.origin.Y < box.maxY && ray.origin.Z > box.minZ && ray.origin.Z < box.maxZ && ray.origin.X < box.maxX)//ensure ray is in bounds of box in x dimension and is left of box
                    {
                        return box.minX - ray.origin.X <= ray.length;
                    }
                    break;
                case RayDirection.NEGX:
                    if (ray.origin.Y > box.minY && ray.origin.Y < box.maxY && ray.origin.Z > box.minZ && ray.origin.Z < box.maxZ && ray.origin.X > box.minX)//ensure ray is in bounds of box in x dimension and is right of box
                    {
                        return ray.origin.X - box.maxX <= ray.length;
                    }
                    break;
                case RayDirection.POSY:
                    if (ray.origin.X > box.minX && ray.origin.X < box.maxX && ray.origin.Z > box.minZ && ray.origin.Z < box.maxZ && ray.origin.Y < box.maxY)//ensure ray is in bounds of box in y dimension and is below box
                    {
                        return box.minY - ray.origin.Y <= ray.length;
                    }
                    break;
                case RayDirection.NEGY:
                    if (ray.origin.X > box.minX && ray.origin.X < box.maxX && ray.origin.Z > box.minZ && ray.origin.Z < box.maxZ && ray.origin.Y > box.minY)//ensure ray is in bounds of box in y dimension and is above box
                    {
                        return ray.origin.Y - box.minY <= ray.length;
                    }
                    break;
                case RayDirection.POSZ:
                    if (ray.origin.X > box.minX && ray.origin.X < box.maxX && ray.origin.Y > box.minY && ray.origin.Y < box.maxY && ray.origin.Z < box.maxZ)//ensure ray is in bounds of box in z dimension and is behind box
                    {
                        return box.minZ - ray.origin.Z <= ray.length;
                    }
                    break;
                case RayDirection.NEGZ:
                    if (ray.origin.X > box.minX && ray.origin.X < box.maxX && ray.origin.Y > box.minY && ray.origin.Y < box.maxY && ray.origin.Z > box.minZ)//ensure ray is in bounds of box in z dimension and is in front of box
                    {
                        return ray.origin.Z - box.maxZ <= ray.length;
                    }
                    break;
            }
            return false;
        }
    }
}
