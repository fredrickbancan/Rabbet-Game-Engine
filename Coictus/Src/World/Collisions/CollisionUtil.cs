using OpenTK;

namespace Coictus
{
    public enum ColliderType//used for detecting which type a collider is
    {
        none,
        aabb,
        plane,
        sphere,
        point
    }
    public static class CollisionUtil
    {
        public static bool getOverlapAndDirectionForColliderTypes(ICollider colliderA, ICollider colliderB, out Vector3d direction, out double overlap)
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
                    direction = Vector3d.Zero;
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
                    direction = Vector3d.Zero;
                    return !AABBCollider.isPositionNotInsideBox((AABBCollider)colliderB, ((PointCollider)colliderA).pos);
                }
                else if (colliderB.getType() == ColliderType.sphere)
                {
                    return getOverlapAndDirection((SphereCollider)colliderB, (PointCollider)colliderA, out direction, out overlap);
                }
            }
            direction = Vector3d.Zero;
            overlap = 0;
            return false;
        }

        #region sphereCollisions
        public static bool getOverlapAndDirection(SphereCollider sphereA, SphereCollider sphereB, out Vector3d direction, out double overlap)
        {
            double radiusCombined = sphereA.radius + sphereB.radius;
            double distanceBetweenCenters = Vector3d.Distance(sphereA.pos, sphereB.pos);

            if (distanceBetweenCenters < radiusCombined)//if they are touching
            {
                overlap = radiusCombined - distanceBetweenCenters;
                direction = Vector3d.Zero;//For now, we do not need to do collision resolutions with spheres and spheres.
                return true;
            }
            direction = Vector3d.Zero;
            overlap = 0;
            return false;
        }
        public static bool getOverlapAndDirection(SphereCollider sphere, PointCollider point, out Vector3d direction, out double overlap)
        {
            double distanceBetweenCenters = Vector3d.Distance(sphere.pos, point.pos);

            if (distanceBetweenCenters < sphere.radius)//if they are touching
            {
                overlap = sphere.radius - distanceBetweenCenters;
                direction = Vector3d.Zero;//For now, we do not need to do collision resolusions with spheres and points.
                return true;
            }
            direction = Vector3d.Zero;
            overlap = 0;
            return false;
        }

        /*In a case where a sphere is colliding a aabb*/
        public static bool getOverlapAndDirection(SphereCollider sphere, AABBCollider aabb, out Vector3d direction, out double overlap)
        {
            Vector3d closestPointOnBoxToSphere = Vector3d.Clamp(sphere.pos, aabb.minBounds, aabb.maxBounds);
            double distanceFromSphereCenterToBox = Vector3d.Distance(sphere.pos, closestPointOnBoxToSphere);

            if (distanceFromSphereCenterToBox < sphere.radius)//if they are touching
            {
                overlap = sphere.radius - distanceFromSphereCenterToBox;
                direction = Vector3d.Zero;//For now, we dont need spheres to move AABBs
                return true;
            }
            direction = Vector3d.Zero;
            overlap = 0;
            return false;
        }

        /*in a case where an aabb is colliding a sphere*/
        public static bool getOverlapAndDirection(AABBCollider aabb, SphereCollider sphere, out Vector3d direction, out double overlap)
        {
            Vector3d closestPointOnBoxToSphere = Vector3d.Clamp(sphere.pos, aabb.minBounds, aabb.maxBounds);
            double distanceFromSphereCenterToBox = Vector3d.Distance(sphere.pos, closestPointOnBoxToSphere);

            if (distanceFromSphereCenterToBox < sphere.radius)//if they are touching
            {
                //Here we will be using a variation of the "shallowest axis" method to get overlap and normal, sort of treats sphere like a box, but thats ok for our purposes.
                double overlapX;
                double overlapY;
                double overlapZ;

                Vector3d boxCenter = aabb.centerVec;

                bool isSphereOnRight = boxCenter.X < sphere.pos.X;
                bool isSphereOnTop = boxCenter.Y < sphere.pos.Y;
                bool isSphereBehind = boxCenter.Z < sphere.pos.Z;

                overlapX = aabb.extentX + sphere.radius - (isSphereOnRight ? sphere.pos.X - boxCenter.X : boxCenter.X - sphere.pos.X);
                overlapY = aabb.extentY + sphere.radius - (isSphereOnTop ? sphere.pos.Y - boxCenter.Y : boxCenter.Y - sphere.pos.Y);
                overlapZ = aabb.extentZ + sphere.radius - (isSphereBehind ? sphere.pos.Z - boxCenter.Z : boxCenter.Z - sphere.pos.Z);

                double smallestOverlap = MathUtil.min3(overlapX, overlapY, overlapZ);


                if (overlapX == smallestOverlap)
                {
                    overlap = overlapX;
                    direction = new Vector3d(isSphereOnRight ? 1D : -1D, 0, 0);
                }
                else if (overlapY == smallestOverlap)
                {
                    overlap = overlapY;
                    direction = new Vector3d(0, isSphereOnTop ? 1D : -1D, 0);
                }
                else if (overlapZ == smallestOverlap)
                {
                    overlap = overlapZ;
                    direction = new Vector3d(0, 0, isSphereBehind ? 1D : -1D);
                }
                else
                {
                    overlap = 0;
                    direction = Vector3d.Zero;
                }
                return true;
            }
            direction = Vector3d.Zero;
            overlap = 0;
            return false;
        }
        #endregion sphereCollisions

        #region aabbCollisions

        public static bool getOverlapAndDirection(AABBCollider boxA, AABBCollider boxB, out Vector3d direction, out double overlap)
        {
            if (!AABBCollider.areBoxesNotTouching(boxA, boxB))
            {
                //Here we will be using the "shallowest axis" method to determine the resulting hit normal.
                double overlapX;
                double overlapY;
                double overlapZ;
                Vector3d boxACenter = boxA.centerVec;
                Vector3d boxBCenter = boxB.centerVec;

                bool isBoxBOnRight = boxACenter.X < boxBCenter.X;
                bool isBoxBOnTop = boxACenter.Y < boxBCenter.Y;
                bool isBoxBBehind = boxACenter.Z < boxBCenter.Z;

                overlapX = boxA.extentX + boxB.extentX - (isBoxBOnRight ? boxBCenter.X - boxACenter.X : boxACenter.X - boxBCenter.X);
                overlapY = boxA.extentY + boxB.extentY - (isBoxBOnTop ? boxBCenter.Y - boxACenter.Y : boxACenter.Y - boxBCenter.Y);
                overlapZ = boxA.extentZ + boxB.extentZ - (isBoxBBehind ? boxBCenter.Z - boxACenter.Z : boxACenter.Z - boxBCenter.Z);

                double smallestOverlap = MathUtil.min3(overlapX, overlapY, overlapZ);

                if (overlapX == smallestOverlap)
                {
                    overlap = overlapX;
                    direction = new Vector3d(isBoxBOnRight ? 1D : -1D, 0, 0);
                }
                else if (overlapY == smallestOverlap)
                {
                    overlap = overlapY;
                    direction = new Vector3d(0, isBoxBOnTop ? 1D : -1D, 0);
                }
                else if (overlapZ == smallestOverlap)
                {
                    overlap = overlapZ;
                    direction = new Vector3d(0, 0, isBoxBBehind ? 1D : -1D);
                }
                else
                {
                    overlap = 0;
                    direction = Vector3d.Zero;
                }
                return true;
            }
            overlap = 0;
            direction = Vector3d.Zero;
            return false;
        }

        #endregion aabbCollisions

        #region planeCollisions
        public static bool getOverlapAndDirection(PlaneCollider plane, SphereCollider sphere, out Vector3d direction, out double overlap)
        {
            overlap = -(PlaneCollider.vectorDistanceFromPlane(plane, sphere.pos) - sphere.radius);
            direction = plane.normal;
            return overlap > 0;
        }

        public static bool getOverlapAndDirection(PlaneCollider plane, AABBCollider aabb, out Vector3d direction, out double overlap)
        {
            double radiusOfTesterSphere = 0;
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
                    Vector3d.Dot(aabb.vecToBackRight, plane.normal), Vector3d.Dot(aabb.vecToBackLeft, plane.normal), Vector3d.Dot(aabb.vecToFrontRight, plane.normal),
                    Vector3d.Dot(-aabb.vecToBackRight, plane.normal), Vector3d.Dot(-aabb.vecToBackLeft, plane.normal), Vector3d.Dot(-aabb.vecToFrontRight, plane.normal));
            }
            return getOverlapAndDirection(plane, new SphereCollider(aabb.centerVec, radiusOfTesterSphere), out direction, out overlap);
        }

        public static bool getOverlapAndDirection(PlaneCollider plane, PointCollider point, out Vector3d direction, out double overlap)//this func can return a negative overlap, no issue. Just means there is no collision.
        {
            overlap = -PlaneCollider.vectorDistanceFromPlane(plane, point.pos);
            direction = plane.normal;
            return overlap > 0;
        }

        #endregion planeCollisions
    }
}
