using FredrickTechDemo.FredsMath;
using System.Collections.Generic;

namespace FredrickTechDemo
{
    public enum CollisionDirection //used for giving collision results. i.e, the direction an object has collided with another object.
    {
        none,
        xPos,
        xNeg,
        yPos,
        yNeg,
        zPos,
        zNeg
    }

    /*An abstraction class which manipulates multiple entity velocities based on their colliders and detects collisions.
      All collisions will be FLAT using this system. Meaning all collisions will be axis aligned. Any collision on any axis should
      cancel the respective axis on the velocity of the child entities.*/
    public static class CollisionHandler
    {

        /*Takes in a list of all the colliders in a given space/world/planet and calculates their collisions and applies the result to the parent entities*/
        public static void doCollisions(Dictionary<int, ICollider> allColliders)
        {

        }


        #region sphereCollisions
        public static bool getOverlapSphereSphere(SphereCollider sphereA, SphereCollider sphereB, out double overlap)
        {
            double radiusCombined = sphereA.radius + sphereB.radius;
            double distanceBetweenCenters = Vector3D.distance(sphereA.pos, sphereB.pos);

            if (distanceBetweenCenters < radiusCombined)//if they are touching
            {
                overlap = radiusCombined - distanceBetweenCenters;
                return true;
            }
            overlap = 0;
            return false;
        }
        public static bool getOverlapSpherePoint(SphereCollider sphere, PointCollider point, out double overlap)
        {
            double distanceBetweenCenters = Vector3D.distance(sphere.pos, point.pos);

            if (distanceBetweenCenters < sphere.radius)//if they are touching
            {
                overlap = sphere.radius - distanceBetweenCenters;
                return true;
            }
            overlap = 0;
            return false;
        }

        public static bool getOverlapSphereAABB(SphereCollider sphere, AABBCollider aabb, out double overlap)
        {
            Vector3D closestPointOnBoxToSphere = Vector3D.clamp(sphere.pos, aabb.minBounds, aabb.maxBounds);
            double distanceFromSphereCenterToBox = Vector3D.distance(sphere.pos, closestPointOnBoxToSphere);

            if (distanceFromSphereCenterToBox < sphere.radius)//if they are touching
            {
                overlap = sphere.radius - distanceFromSphereCenterToBox;//TODO: WRONG, this will only return a maximum of the radius, meaning if the sphere's center is inside the box then the overlap will not take its penetration depth into account!
                return true;
            }

            overlap = 0;
            return false;
        }
        #endregion sphereCollisions

        #region aabbCollisions

        /*//there is no need to have this as a function, since we do not need points to do physics with aabb's for now. Simply use !isPositionNotInsideBox()
        public static bool getOverlapAABBPoint(AABBCollider box, PointCollider point, out double overlap)
        {
        }*/

        /*Calculating the amount of overlap of two aabb requires knowledge of their direction in velocity, So these will be done elsewhere.
        public static bool getOverlapAABBAABB(AABBCollider boxA, AABBCollider boxB, out double overlap)
        {
        }*/

        /*returns true if two bounding boxes are NOT touching in any way*/
        public static bool areBoxesNotTouching(AABBCollider boxA, AABBCollider boxB)
        {
            return boxA.minX > boxB.maxX || boxA.minY > boxB.maxY || boxA.minZ > boxB.maxZ
                || boxA.maxX < boxB.minX || boxA.maxY < boxB.minY || boxA.maxZ < boxB.minZ;
        }

        /*returns true if a vector is NOT within a box's bounds*/
        public static bool isPositionNotInsideBox(AABBCollider box, Vector3D position)
        {
            return position.x > box.maxX || position.x < box.minX
                || position.y > box.maxY || position.y < box.minY
                || position.z > box.maxZ || position.z < box.minZ;
        }
        #endregion aabbCollisions

        #region planeCollisions
        public static bool getOverlapPlaneSphere(PlaneCollider plane, SphereCollider sphere, out double overlap)
        {
            overlap = -(vectorDistanceFromPlane(plane, sphere.pos) - sphere.radius);//TODO: WRONG, this will only return a maximum of the radius, meaning if the sphere's center is behind the plane then the overlap will not take its penetration depth into account!
            return overlap > 0;
        }
        public static bool getOverlapPlaneAABB(PlaneCollider plane, AABBCollider aabb, out double overlap)
        {
            double radiusOfTesterSphere = 0;
            //Check if the plane aligns with two of any axis
            if (plane.normal.y >= 1D || plane.normal.y <= -1D)
            {
                radiusOfTesterSphere = aabb.extentY;//if the plane aligns with a given axis, then the sphere will have the radius of the aabb extent of that axis,so the spheres edge will be aligning with the "Face" of the aabb
            }
            else if (plane.normal.x >= 1D || plane.normal.x <= -1D)
            {
                radiusOfTesterSphere = aabb.extentX;
            }
            else if (plane.normal.z >= 1D || plane.normal.z <= -1D)
            {
                radiusOfTesterSphere = aabb.extentZ;
            }

            //TODO calculate radius for all other angles, for now the radius will be set to the corners of the aabb otherwise.
            else
            {
                radiusOfTesterSphere = Vector3D.distance(aabb.minBounds, aabb.maxBounds) / 2D;//edge of sphere aligns with aabb corners.
            }
            return getOverlapPlaneSphere(plane, new SphereCollider(aabb.centerVec, radiusOfTesterSphere), out overlap);
        }

        public static bool getOverlapPlanePoint(PlaneCollider plane, PointCollider point, out double overlap)//this func can return a negative overlap, no issue. Just means there is no collision.
        {
            overlap = -vectorDistanceFromPlane(plane, point.pos);
            return overlap > 0;
        }

        public static double vectorDistanceFromPlane(PlaneCollider plane, Vector3D vec)
        {
            return Vector3D.dot(vec, plane.normal) + plane.scalar;
        }

        #endregion planeCollisions
    }
}
