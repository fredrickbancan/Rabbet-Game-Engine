using OpenTK;

namespace RabbetGameEngine
{
    public enum ColliderType//used for detecting which type a collider is
    {
        none,
        aabb,
        plane,
        sphere,
        point
    }
    //TODO: make collision resolution more accurate, i.e, make collisions compensate for high velocities pushing colliders to wrong side of objects.
    //TODO: make colliders on moving objects work with interpolation.
    //TODO: make collisions calculate faster at high numbers.
    public static class CollisionUtil
    {
        public static bool getOverlapAndDirectionForColliderTypes(ICollider colliderA, ICollider colliderB, out Vector3 direction, out float overlap)
        {
            if (colliderA.getType() == ColliderType.plane)
            {
                if (colliderB.getType() == ColliderType.aabb)
                {
                    return getOverlapAndDirection((PlaneCollider)colliderA, (AABBCollider)colliderB, out direction, out overlap);
                }
                else if (colliderB.getType() == ColliderType.sphere)
                {
                    return getOverlapAndDirection((PlaneCollider)colliderA, (SphereCollider)colliderB, out direction, out overlap);
                }
                else if (colliderB.getType() == ColliderType.point)
                {
                    return getOverlapAndDirection((PlaneCollider)colliderA, (PointCollider)colliderB, out direction, out overlap);
                }
            }
            else if (colliderA.getType() == ColliderType.aabb)
            {
                if (colliderB.getType() == ColliderType.aabb)
                {
                    return getOverlapAndDirection((AABBCollider)colliderA, (AABBCollider)colliderB, out direction, out overlap);
                }
                else if (colliderB.getType() == ColliderType.sphere)
                {
                    return getOverlapAndDirection((AABBCollider)colliderA, (SphereCollider)colliderB, out direction, out overlap);
                }
                else if (colliderB.getType() == ColliderType.point)
                {
                    overlap = 0;
                    direction = Vector3.Zero;
                    return !AABBCollider.isPositionNotInsideBox((AABBCollider)colliderA, ((PointCollider)colliderB).pos);
                }
            }
            else if (colliderA.getType() == ColliderType.sphere)
            {
                if (colliderB.getType() == ColliderType.aabb)
                {
                    return getOverlapAndDirection((SphereCollider)colliderA, (AABBCollider)colliderB, out direction, out overlap);
                }
                else if (colliderB.getType() == ColliderType.sphere)
                {
                    return getOverlapAndDirection((SphereCollider)colliderA, (SphereCollider)colliderB, out direction, out overlap);
                }
                else if (colliderB.getType() == ColliderType.point)
                {
                    return getOverlapAndDirection((SphereCollider)colliderA, (PointCollider)colliderB, out direction, out overlap);
                }
            }
            else if (colliderA.getType() == ColliderType.point)
            {
                if (colliderB.getType() == ColliderType.aabb)
                {
                    overlap = 0;
                    direction = Vector3.Zero;
                    return !AABBCollider.isPositionNotInsideBox((AABBCollider)colliderB, ((PointCollider)colliderA).pos);
                }
                else if (colliderB.getType() == ColliderType.sphere)
                {
                    return getOverlapAndDirection((SphereCollider)colliderB, (PointCollider)colliderA, out direction, out overlap);
                }
            }
            direction = Vector3.Zero;
            overlap = 0;
            return false;
        }

        #region sphereCollisions
        public static bool getOverlapAndDirection(SphereCollider sphereA, SphereCollider sphereB, out Vector3 direction, out float overlap)
        {
            float radiusCombined = sphereA.radius + sphereB.radius;
            float distanceBetweenCenters = Vector3.DistanceSquared(sphereA.pos, sphereB.pos);

            if (distanceBetweenCenters < radiusCombined * radiusCombined)//if they are touching
            {
                overlap = radiusCombined * radiusCombined - distanceBetweenCenters;
                direction = Vector3.Zero;//For now, we do not need to do collision resolutions with spheres and spheres.
                return true;
            }
            direction = Vector3.Zero;
            overlap = 0;
            return false;
        }
        public static bool getOverlapAndDirection(SphereCollider sphere, PointCollider point, out Vector3 direction, out float overlap)
        {
            float distanceBetweenCenters = Vector3.DistanceSquared(sphere.pos, point.pos);

            if (distanceBetweenCenters < sphere.radius * sphere.radius)//if they are touching
            {
                overlap = sphere.radius * sphere.radius - distanceBetweenCenters;
                direction = Vector3.Zero;//For now, we do not need to do collision resolusions with spheres and points.
                return true;
            }
            direction = Vector3.Zero;
            overlap = 0;
            return false;
        }

        /*In a case where a sphere is colliding a aabb*/
        public static bool getOverlapAndDirection(SphereCollider sphere, AABBCollider aabb, out Vector3 direction, out float overlap)
        {
            Vector3 closestPointOnBoxToSphere = Vector3.Clamp(sphere.pos, aabb.minBounds, aabb.maxBounds);
            float distanceFromSphereCenterToBox = Vector3.DistanceSquared(sphere.pos, closestPointOnBoxToSphere);

            if (distanceFromSphereCenterToBox < sphere.radius * sphere.radius)//if they are touching
            {
                overlap = sphere.radius * sphere.radius - distanceFromSphereCenterToBox;
                direction = Vector3.Zero;//For now, we dont need spheres to move AABBs
                return true;
            }
            direction = Vector3.Zero;
            overlap = 0;
            return false;
        }

        /*in a case where an aabb is colliding a sphere*/
        public static bool getOverlapAndDirection(AABBCollider aabb, SphereCollider sphere, out Vector3 direction, out float overlap)
        {
            Vector3 closestPointOnBoxToSphere = Vector3.Clamp(sphere.pos, aabb.minBounds, aabb.maxBounds);
            float distanceFromSphereCenterToBox = Vector3.DistanceSquared(sphere.pos, closestPointOnBoxToSphere);

            if (distanceFromSphereCenterToBox < sphere.radius * sphere.radius)//if they are touching
            {
                //Here we will be using a variation of the "shallowest axis" method to get overlap and normal, sort of treats sphere like a box, but thats ok for our purposes.
                float overlapX;
                float overlapY;
                float overlapZ;

                Vector3 boxCenter = aabb.centerVec;

                bool isSphereOnRight = boxCenter.X < sphere.pos.X;
                bool isSphereOnTop = boxCenter.Y < sphere.pos.Y;
                bool isSphereBehind = boxCenter.Z < sphere.pos.Z;

                overlapX = aabb.extentX + sphere.radius - (isSphereOnRight ? sphere.pos.X - boxCenter.X : boxCenter.X - sphere.pos.X);
                overlapY = aabb.extentY + sphere.radius - (isSphereOnTop ? sphere.pos.Y - boxCenter.Y : boxCenter.Y - sphere.pos.Y);
                overlapZ = aabb.extentZ + sphere.radius - (isSphereBehind ? sphere.pos.Z - boxCenter.Z : boxCenter.Z - sphere.pos.Z);

                float smallestOverlap = MathUtil.min3(overlapX, overlapY, overlapZ);


                if (overlapX == smallestOverlap)
                {
                    overlap = overlapX;
                    direction = new Vector3(isSphereOnRight ? 1 : -1, 0, 0);
                }
                else if (overlapY == smallestOverlap)
                {
                    overlap = overlapY;
                    direction = new Vector3(0, isSphereOnTop ? 1 : -1, 0);
                }
                else if (overlapZ == smallestOverlap)
                {
                    overlap = overlapZ;
                    direction = new Vector3(0, 0, isSphereBehind ? 1 : -1);
                }
                else
                {
                    overlap = 0;
                    direction = Vector3.Zero;
                }
                return true;
            }
            direction = Vector3.Zero;
            overlap = 0;
            return false;
        }
        #endregion sphereCollisions

        #region aabbCollisions

        public static bool getOverlapAndDirection(AABBCollider boxA, AABBCollider boxB, out Vector3 direction, out float overlap)
        {
            if (!AABBCollider.areBoxesNotTouching(boxA, boxB))
            {
                //Here we will be using the "shallowest axis" method to determine the resulting hit normal.
                float overlapX;
                float overlapY;
                float overlapZ;
                Vector3 boxACenter = boxA.centerVec;
                Vector3 boxBCenter = boxB.centerVec;

                bool isBoxBOnRight = boxACenter.X < boxBCenter.X;
                bool isBoxBOnTop = boxACenter.Y < boxBCenter.Y;
                bool isBoxBBehind = boxACenter.Z < boxBCenter.Z;

                overlapX = boxA.extentX + boxB.extentX - (isBoxBOnRight ? boxBCenter.X - boxACenter.X : boxACenter.X - boxBCenter.X);
                overlapY = boxA.extentY + boxB.extentY - (isBoxBOnTop ? boxBCenter.Y - boxACenter.Y : boxACenter.Y - boxBCenter.Y);
                overlapZ = boxA.extentZ + boxB.extentZ - (isBoxBBehind ? boxBCenter.Z - boxACenter.Z : boxACenter.Z - boxBCenter.Z);

                float smallestOverlap = MathUtil.min3(overlapX, overlapY, overlapZ);

                if (overlapX == smallestOverlap)
                {
                    overlap = overlapX;
                    direction = new Vector3(isBoxBOnRight ? 1 : -1, 0, 0);
                }
                else if (overlapY == smallestOverlap)
                {
                    overlap = overlapY;
                    direction = new Vector3(0, isBoxBOnTop ? 1 : -1, 0);
                }
                else if (overlapZ == smallestOverlap)
                {
                    overlap = overlapZ;
                    direction = new Vector3(0, 0, isBoxBBehind ? 1 : -1);
                }
                else
                {
                    overlap = 0;
                    direction = Vector3.Zero;
                }
                return true;
            }
            overlap = 0;
            direction = Vector3.Zero;
            return false;
        }

        #endregion aabbCollisions

        #region planeCollisions
        public static bool getOverlapAndDirection(PlaneCollider plane, SphereCollider sphere, out Vector3 direction, out float overlap)
        {
            overlap = -(PlaneCollider.vectorDistanceFromPlane(plane, sphere.pos) - sphere.radius);
            direction = plane.normal;
            return overlap > 0;
        }

        public static bool getOverlapAndDirection(PlaneCollider plane, AABBCollider aabb, out Vector3 direction, out float overlap)
        {
            float radiusOfTesterSphere = 0;
            //Check if the plane aligns with two of any axis
            if (plane.normal.Y >= 1D || plane.normal.Y <= -1D)
            {
                radiusOfTesterSphere = aabb.extentY;//if the plane aligns with a given axis, then the sphere will have the radius of the aabb extent of that axis,so the spheres edge will be aligning with the "Face" of the aabb
            }
            else if (plane.normal.X >= 1D || plane.normal.X <= -1D)
            {
                radiusOfTesterSphere = aabb.extentX;
            }
            else if (plane.normal.Z >= 1D || plane.normal.Z <= -1D)
            {
                radiusOfTesterSphere = aabb.extentZ;
            }
            else
            {
                radiusOfTesterSphere = MathUtil.max6(
                    Vector3.Dot(aabb.vecToBackRight, plane.normal), Vector3.Dot(aabb.vecToBackLeft, plane.normal), Vector3.Dot(aabb.vecToFrontRight, plane.normal),
                    Vector3.Dot(-aabb.vecToBackRight, plane.normal), Vector3.Dot(-aabb.vecToBackLeft, plane.normal), Vector3.Dot(-aabb.vecToFrontRight, plane.normal));
            }
            return getOverlapAndDirection(plane, new SphereCollider(aabb.centerVec, radiusOfTesterSphere), out direction, out overlap);
        }

        public static bool getOverlapAndDirection(PlaneCollider plane, PointCollider point, out Vector3 direction, out float overlap)//this func can return a negative overlap, no issue. Just means there is no collision.
        {
            overlap = -PlaneCollider.vectorDistanceFromPlane(plane, point.pos);
            direction = plane.normal;
            return overlap > 0;
        }

        #endregion planeCollisions
    }
}
