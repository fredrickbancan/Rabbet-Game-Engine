using FredrickTechDemo.FredsMath;
using System.Collections.Generic;
using System.Linq;

namespace FredrickTechDemo
{
    public enum ColliderType//used for detecting which type a collider is
    {
        none,
        aabb,
        plane,
        sphere,
        point
    }

    /*An abstraction class which manipulates multiple entity velocities based on their colliders and detects collisions.
      All collisions will be FLAT using this system. Meaning all collisions will be axis aligned. Any collision on any axis should
      cancel the respective axis on the velocity of the child entities.*/
    public static class CollisionHandler
    {
        /*Takes in a list of worldColliders and entity colliders and tests them against eachother, correcting the entities.
          This should be done before Entity vs entity collisions.*/
        public static void doWorldCollisions(List<ICollider> worldColliders, Dictionary<int, ICollider> entityColliders)
        {
            ICollider currentWorldCollider;
            ICollider currentEntityCollider;
            Vector3D currentCollisionDirection;
            double currentCollisionOverlapResult = 0;
            bool currentCollisionTouching = false;
            for (int i = 0; i < worldColliders.Count; i++)
            {
                currentWorldCollider = worldColliders.ElementAt(i);
                if (currentWorldCollider == null)
                {
                    Application.warn("CollisionHander.doWorldCollisions() has detcted a null world collider object at index " + i + ", removing.");
                    worldColliders.RemoveAt(i);
                    continue;
                }

                for(int j = 0; j < entityColliders.Count; j++)
                {
                    currentEntityCollider = entityColliders.Values.ElementAt(j);
                    if (currentEntityCollider == null)
                    {
                        Application.warn("CollisionHander.doWorldCollisions() has detcted a null entity collider object at index " + i + ", removing.");
                        entityColliders.Remove(entityColliders.Keys.ElementAt(j));
                        continue;
                    }

                    if(currentCollisionTouching = getOverlapAndDirectionForColliderTypes(currentWorldCollider, currentEntityCollider, out currentCollisionDirection, out currentCollisionOverlapResult))
                    {
                        currentEntityCollider.getParent().applyCollision(currentCollisionDirection, currentCollisionOverlapResult);//TODO: PLACEHOLDER, need to properly calculate collisions for each different type of collider.
                    }
                }
            }
        }


        /*Takes in a dictionary of all the entity colliders in the world and tests them against eachother.*/
        public static void doEntityCollisions(Dictionary<int, ICollider> entityColliders)
        {
            double currentCollisionOverlapResult = 0;
            bool currentCollisionTouching = false;
            ICollider currentColliderA;//collider A is tested with all other colliders after it, which are assigned to collider B. Collision results are applied to collider B's parent respectively.
            ICollider currentColliderB;

            for(int i = 0; i < entityColliders.Values.Count; i++)//Loop through each collider and chose the next one to be colliderA
            {
                currentColliderA = entityColliders.Values.ElementAt(i);
                if (currentColliderA == null)
                {
                    Application.warn("CollisionHander.doEntityCollisions() has detcted a null collider object at index " + i + ", removing.");
                    entityColliders.Remove(entityColliders.Keys.ElementAt(i));
                    continue;
                }

                for (int j = 0; j < entityColliders.Values.Count; j++)//Loop through each collider and chose the next one to be colliderB
                {
                    if (i == j)/*if this is the same entity collider, skip. We dont want an entity collding with itself.*/continue;

                    currentColliderB = entityColliders.Values.ElementAt(j);
                    if(currentColliderB == null)
                    {
                        Application.warn("CollisionHander.doEntityCollisions() has detcted a null collider object at index " + j + ", removing.");
                        entityColliders.Remove(entityColliders.Keys.ElementAt(j));
                        continue;
                    }
                    //TODO: impliment entity v entity collisions.
                }
            }
        }

        private static bool getOverlapAndDirectionForColliderTypes(ICollider colliderA, ICollider colliderB, out Vector3D direction, out double overlap)
        {
            if(colliderA.getType() == ColliderType.plane)
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
            else if(colliderA.getType() == ColliderType.aabb)
            {
                if (colliderB.getType() == ColliderType.aabb)
                {
                    return getOverlapAndDirection((AABBCollider)colliderA, (AABBCollider)colliderB, out direction, out overlap);
                }
                else if (colliderB.getType() == ColliderType.sphere)
                {
                    return getOverlapAndDirection((SphereCollider)colliderB, (AABBCollider)colliderA, out direction, out overlap);
                }
                else if (colliderB.getType() == ColliderType.point)
                {
                    overlap = 0;
                    direction = Vector3D.zero;
                    return !isPositionNotInsideBox((AABBCollider)colliderA, ((PointCollider)colliderB).pos);
                }
            }
            else if(colliderA.getType() == ColliderType.sphere)
            {
                if (colliderB.getType() == ColliderType.aabb)
                {
                    return getOverlapAndDirection((SphereCollider)colliderA, (AABBCollider)colliderB, out direction, out overlap);
                }
                else if (colliderB.getType() == ColliderType.sphere)
                {
                    return getOverlapAndDirection((SphereCollider)colliderA, (SphereCollider)colliderB,out direction, out overlap);
                }
                else if (colliderB.getType() == ColliderType.point)
                {
                    return getOverlapAndDirection((SphereCollider)colliderA, (PointCollider)colliderB, out direction, out overlap);
                }
            }
            else if(colliderA.getType() == ColliderType.point)
            {
                if (colliderB.getType() == ColliderType.aabb)
                {
                    overlap = 0;
                    direction = Vector3D.zero;
                    return !isPositionNotInsideBox((AABBCollider)colliderB, ((PointCollider)colliderA).pos) ;
                }
                else if (colliderB.getType() == ColliderType.sphere)
                {
                    return getOverlapAndDirection((SphereCollider)colliderB, (PointCollider)colliderA, out direction, out overlap);
                }
            }
            direction = Vector3D.zero;
            overlap = 0;
            return false;
        }

        #region sphereCollisions
        public static bool getOverlapAndDirection(SphereCollider sphereA, SphereCollider sphereB, out Vector3D direction, out double overlap)
        {
            double radiusCombined = sphereA.radius + sphereB.radius;
            double distanceBetweenCenters = Vector3D.distance(sphereA.pos, sphereB.pos);

            if (distanceBetweenCenters < radiusCombined)//if they are touching
            {
                overlap = radiusCombined - distanceBetweenCenters;
                direction = Vector3D.zero;//TODO: placeholder, impliment and remove.
                return true;
            }
            direction = Vector3D.zero;
            overlap = 0;
            return false;
        }
        public static bool getOverlapAndDirection(SphereCollider sphere, PointCollider point, out Vector3D direction, out double overlap)
        {
            double distanceBetweenCenters = Vector3D.distance(sphere.pos, point.pos);

            if (distanceBetweenCenters < sphere.radius)//if they are touching
            {
                overlap = sphere.radius - distanceBetweenCenters;
                direction = Vector3D.zero;//TODO: placeholder, impliment and remove.
                return true;
            }
            direction = Vector3D.zero;
            overlap = 0;
            return false;
        }

        public static bool getOverlapAndDirection(SphereCollider sphere, AABBCollider aabb, out Vector3D direction, out double overlap)
        {
            Vector3D closestPointOnBoxToSphere = Vector3D.clamp(sphere.pos, aabb.minBounds, aabb.maxBounds);
            double distanceFromSphereCenterToBox = Vector3D.distance(sphere.pos, closestPointOnBoxToSphere);

            if (distanceFromSphereCenterToBox < sphere.radius)//if they are touching
            {
                overlap = sphere.radius - distanceFromSphereCenterToBox;//TODO: WRONG, this will only return a maximum of the radius, meaning if the sphere's center is inside the box then the overlap will not take its penetration depth into account!
                direction = Vector3D.zero;//TODO: placeholder, impliment and remove.
                return true;
            }
            direction = Vector3D.zero;
            overlap = 0;
            return false;
        }
        #endregion sphereCollisions

        #region aabbCollisions

        /*//there is no need to have this as a function, since we do not need points to do physics with aabb's for now. Simply use !isPositionNotInsideBox()
        public static bool getOverlapAndDirectionAABBPoint(AABBCollider box, PointCollider point, out double overlap)
        {
        }*/

       
        public static bool getOverlapAndDirection(AABBCollider boxA, AABBCollider boxB, out Vector3D direction, out double overlap)
        {
            direction = Vector3D.zero;
            overlap = 0;
            return false;
            //TODO:Impliemnt aabb aabb overlap function
        }

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
        public static bool getOverlapAndDirection(PlaneCollider plane, SphereCollider sphere, out Vector3D direction, out double overlap)
        {
            overlap = -(vectorDistanceFromPlane(plane, sphere.pos) - sphere.radius);//TODO: WRONG, this will only return a maximum of the radius, meaning if the sphere's center is behind the plane then the overlap will not take its penetration depth into account!
            direction = Vector3D.zero;//TODO: placeholder, impliment and remove.
            return overlap > 0;
        }

        public static bool getOverlapAndDirection(PlaneCollider plane, AABBCollider aabb, out Vector3D direction, out double overlap)
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
            else
            {
                radiusOfTesterSphere = MathUtil.max6(
                    Vector3D.dot(aabb.maxXmaxYmaxZ, plane.normal), Vector3D.dot(aabb.minXmaxYmaxZ, plane.normal), Vector3D.dot(aabb.maxXmaxYminZ, plane.normal),
                    Vector3D.dot(-aabb.maxXmaxYmaxZ, plane.normal), Vector3D.dot(-aabb.minXmaxYmaxZ, plane.normal), Vector3D.dot(-aabb.maxXmaxYminZ, plane.normal));
            }
            return getOverlapAndDirection(plane, new SphereCollider(aabb.centerVec, radiusOfTesterSphere), out direction, out overlap);
        }

        public static bool getOverlapAndDirection(PlaneCollider plane, PointCollider point, out Vector3D direction, out double overlap)//this func can return a negative overlap, no issue. Just means there is no collision.
        {
            overlap = -vectorDistanceFromPlane(plane, point.pos);
            direction = plane.normal;
            return overlap > 0;
        }

        public static double vectorDistanceFromPlane(PlaneCollider plane, Vector3D vec)
        {
            return Vector3D.dot(vec, plane.normal) - plane.scalar;
        }

        #endregion planeCollisions
    }
}
