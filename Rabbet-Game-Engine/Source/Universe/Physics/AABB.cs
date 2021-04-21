using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    /*A struct for doing axis aligned bounding box collisions*/
    public struct AABB
    {
        public Vector3 pos;
        public Vector3 extents;
        public Vector3 minBounds;
        public Vector3 maxBounds;

        public static AABB fromExtents(Vector3 pos, Vector3 extents)
        {
            AABB result = new AABB();
            result.pos = pos;
            result.extents = extents;
            result.minBounds = pos - extents;
            result.maxBounds = pos + extents;
            return result;
        }

        public static AABB fromSize(Vector3 pos, Vector3 size)
        {
            AABB result = new AABB();
            result.pos = pos;
            result.extents = size * 0.5F;
            result.minBounds = pos - result.extents;
            result.maxBounds = pos + result.extents;
            return result;
        }

        public static AABB fromBounds(Vector3 minbounds, Vector3 maxBounds)
        {
            return new AABB().setBounds(minbounds, maxBounds);
        }

        public AABB setBounds(Vector3 minBounds, Vector3 maxBounds)
        {
            pos = (minBounds + maxBounds) * 0.5F;
            extents = maxBounds - pos;
            this.minBounds = minBounds;
            this.maxBounds = maxBounds;
            return this;
        }

        /*returns true if two bounding boxes are NOT touching in any way*/
        public static bool areBoxesNotTouching(AABB boxA, AABB boxB)
        {
            return boxA.minX > boxB.maxX || boxA.minY > boxB.maxY || boxA.minZ > boxB.maxZ
                || boxA.maxX < boxB.minX || boxA.maxY < boxB.minY || boxA.maxZ < boxB.minZ;
        }

        /*returns true if a vector is NOT within a box's bounds*/
        public static bool isPositionNotInsideBox(AABB box, Vector3 position)
        {
            return position.X > box.maxX || position.X < box.minX
                || position.Y > box.maxY || position.Y < box.minY
                || position.Z > box.maxZ || position.Z < box.minZ;
        }

        public static bool overlappingX(AABB a, AABB b)
        {
            return a.maxX > b.minX && b.maxX > a.minX;
        }

        public static bool overlappingY(AABB a, AABB b)
        {
            return a.maxY > b.minY && b.maxY > a.minY;
        }

        public static bool overlappingZ(AABB a, AABB b)
        {
            return a.maxZ > b.minZ && b.maxZ > a.minZ;
        }

        public static Vector3 clampPosToBox(Vector3 pos, AABB box)
        {
            return Vector3.Clamp(pos, box.minBounds, box.maxBounds);
        }

        public void offset(Vector3 direction)
        {
            pos += direction;
            minBounds += direction;
            maxBounds += direction;
        }

        public void offset(float x, float y, float z)
        {
            pos.X += x;
            pos.Y += y;
            pos.Z += z;
               
            minBounds.X += x;
            minBounds.Y += y;
            minBounds.Z += z;

            maxBounds.X += x;
            maxBounds.Y += y;
            maxBounds.Z += z;
        }

        public float minX { get => minBounds.X; set => minBounds.X = value; }
        public float minY { get => minBounds.Y; set => minBounds.Y = value; }
        public float minZ { get => minBounds.Z; set => minBounds.Z = value; }

        public float maxX { get => maxBounds.X; set => maxBounds.X = value; }
        public float maxY { get => maxBounds.Y; set => maxBounds.Y = value; }
        public float maxZ { get => maxBounds.Z; set => maxBounds.Z = value; }


        //extent is how much the aabb extends from its center 
        public float extentX { get => extents.X; set => extents.X = value; }
        public float extentY { get => extents.Y; set => extents.Y = value; }
        public float extentZ { get => extents.Z; set => extents.Z = value; }
    }
}