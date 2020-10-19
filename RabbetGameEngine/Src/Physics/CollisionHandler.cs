using OpenTK;
using System.Collections.Generic;
using System.Linq;

namespace RabbetGameEngine.Physics
{
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
            Vector3 currentCollisionDirection;
            float currentCollisionOverlapResult = 0;
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

                    if(CollisionUtil.getOverlapAndDirectionForColliderTypes(currentWorldCollider, currentEntityCollider, out currentCollisionDirection, out currentCollisionOverlapResult))
                    {
                        currentEntityCollider.getParent().applyCollision(currentCollisionDirection, currentCollisionOverlapResult);
                    }
                }
            }
        }


        /*Takes in a dictionary of all the entity colliders in the world and tests them against eachother.*/
        public static void doEntityCollisions(Dictionary<int, ICollider> entityColliders)
        {
            float currentCollisionOverlapResult = 0;
            Vector3 currentCollisionDirection;
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
                    if(currentColliderB == null)//check and weed out any null colliders
                    {
                        Application.warn("CollisionHander.doEntityCollisions() has detcted a null collider object at index " + j + ", removing.");
                        entityColliders.Remove(entityColliders.Keys.ElementAt(j));
                        continue;
                    }
                    if (CollisionUtil.getOverlapAndDirectionForColliderTypes(currentColliderA, currentColliderB, out currentCollisionDirection, out currentCollisionOverlapResult))
                    {
                        if (currentColliderA.getCollisionWeight() >= currentColliderB.getCollisionWeight())
                        {
                            currentColliderB.getParent().applyCollision(currentCollisionDirection, currentCollisionOverlapResult);
                        }

                        /*Send info about eachother and parents can decide what to do with it*/
                        if (currentColliderA.getHasParent() && currentColliderB.getHasParent())
                        {
                            currentColliderB.getParent().onCollidedBy(currentColliderA.getParent());
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Should be done after on tick and before post tick.
        /// tries to move the provided positional object by its velocity with respect to all of the provided colliders.
        /// NOTE: This loops through each collider for now! meaning this is O(n^2) Complexity since entities collide with each other!
        /// </summary>
        /// <param name="obj"> the object to move </param>
        /// <param name="worldColliders"> all world colliders </param>
        /// <param name="entities"> all entity colliders </param>
        public static void tryToMoveObject(PositionalObject obj, List<ICollider> worldColliders, Dictionary<int, Entity> entities)
        {
            if(!obj.getHasCollider())
            {
                obj.setPosition(obj.getPosition() + obj.getVelocity());
                return;
            }

            ICollider objCollider = obj.getCollider();
            Vector3 objVel = obj.getVelocity();

            //do all world collisions first
            for(int i = 0; i < worldColliders.Count; i++)
            {
                applyCollisionForColliderType(ref objVel, ref objCollider, worldColliders.ElementAt(i));
            }

            //do all entity collisions
            for (int i = 0; i < entities.Count; i++)
            {
                Entity entAt;
                if((entAt = entities.Values.ElementAt(i)).getHasCollider())
                applyCollisionForColliderType(ref objVel, ref objCollider, entAt.getCollider());
            }

            objCollider.offset(objVel);

            //set modified collider
            obj.setCollider(objCollider);

            //apply new velocity
            obj.setVelocity(objVel);

            //lastly, position obj to center of hitbox offset
            obj.setPosition(objCollider.getCenterVec());
            
        }

        /// <summary>
        /// Changes the provided velocity vector depending if it is colliding with the provided collider.
        /// </summary>
        /// <param name="vel">The velocity vector to be changed</param>
        /// <param name="objCollider">The collider of the object</param>
        /// <param name="otherColider">The collider to test collisions with</param>
        private static void applyCollisionForColliderType(ref Vector3 vel, ref ICollider objCollider, ICollider otherCollider)
        { 
            switch(objCollider.getType())
            {
                case ColliderType.aabb:
                    switch (otherCollider.getType())
                    {
                        case ColliderType.plane:
                            objCollider = applyCollisionAABBVsPlane(ref vel, (AABBCollider)objCollider, (PlaneCollider)otherCollider);
                            break;
                        case ColliderType.aabb:
                            objCollider = applyCollisionAABBVsAABB(ref vel, (AABBCollider)objCollider, (AABBCollider)otherCollider);
                            break;
                        case ColliderType.sphere:
                            objCollider = applyCollisionAABBVsSphere(ref vel, (AABBCollider)objCollider, (SphereCollider)otherCollider);
                            break;
                        default:
                            break;
                    }
                    break;
                case ColliderType.sphere:
                    switch (otherCollider.getType())
                    {
                        case ColliderType.plane:
                            objCollider = applyCollisionSphereVsPlane(ref vel, (SphereCollider)objCollider, (PlaneCollider)otherCollider);
                            break;
                        case ColliderType.aabb:
                            objCollider = applyCollisionSphereVsAABB(ref vel, (SphereCollider)objCollider, (AABBCollider)otherCollider);
                            break;
                        case ColliderType.sphere:
                            objCollider = applyCollisionSphereVsSphere(ref vel, (SphereCollider)objCollider, (SphereCollider)otherCollider);
                            break;
                        default:
                            break;
                    }
                    break;
                case ColliderType.point:
                    switch (otherCollider.getType())
                    {
                        case ColliderType.plane:
                            objCollider = applyCollisionPointVsPlane(ref vel, (PointCollider)objCollider, (PlaneCollider)otherCollider);
                            break;
                        case ColliderType.aabb:
                            objCollider = applyCollisionPointVsAABB(ref vel, (PointCollider)objCollider, (AABBCollider)otherCollider);
                            break;
                        case ColliderType.sphere:
                            objCollider = applyCollisionPointVsSphere(ref vel, (PointCollider)objCollider, (SphereCollider)otherCollider);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Applies collision resolution velocity to the provided object velocity vector. Resulting from an AABB vs Plane collision.
        /// </summary>
        /// <param name="objVel">object velocity to be modified</param>
        /// <param name="objAABB">object AABB hitbox</param>
        /// <param name="worldPlane">plane collider usually from the world</param>
        /// <returns>The provided AABB collider for the object properly offset during collision resolution</returns>
        private static AABBCollider applyCollisionAABBVsPlane(ref Vector3 objVel, AABBCollider objAABB, PlaneCollider worldPlane)
        {
            float dotProduct;
            //if object velocity is moving in general direction towards plane
            if((dotProduct = Vector3.Dot(objVel, worldPlane.normal)) < 0.0F)
            {
                float radiusOfTesterSphere;
                //Check if the plane normal aligns with any axis
                if (worldPlane.normal.X == 0 || worldPlane.normal.Y == 0 || worldPlane.normal.Z == 0)
                {
                    //if the plane normal aligns with a given axis, then the sphere will have the radius of the aabb extent of that axis,so the spheres edge will be aligning with the "Face" of the aabb
                    radiusOfTesterSphere = worldPlane.normal.X != 0 ? objAABB.extentX : worldPlane.normal.Y != 0 ? objAABB.extentY : objAABB.extentZ;
                }
                else
                {
                    radiusOfTesterSphere = MathUtil.max6(
                        Vector3.Dot(objAABB.vecToBackRight, worldPlane.normal), Vector3.Dot(objAABB.vecToBackLeft, worldPlane.normal), Vector3.Dot(objAABB.vecToFrontRight, worldPlane.normal),
                        Vector3.Dot(-objAABB.vecToBackRight, worldPlane.normal), Vector3.Dot(-objAABB.vecToBackLeft, worldPlane.normal), Vector3.Dot(-objAABB.vecToFrontRight, worldPlane.normal));
                }
                float dist = PlaneCollider.vectorDistanceFromPlane(worldPlane, objAABB.centerVec) - radiusOfTesterSphere;
                if(dist <= objVel.Length)
                {
                    objVel -= (dotProduct + dist) * worldPlane.normal;
                }
            }
            return objAABB;
        }

        /// <summary>
        /// Applies collision resolution velocity to the provided object velocity vector. Resulting from an AABB vs AABB collision.
        /// </summary>
        /// <param name="objVel">velocity to be modified</param>
        /// <param name="objAABB">object AABB hitbox</param>
        /// <param name="otherAABB">other AABB hitbox</param>
        /// <returns>The provided AABB collider for the object properly offset during collision resolution</returns>
        private static AABBCollider applyCollisionAABBVsAABB(ref Vector3 objVel, AABBCollider objAABB, AABBCollider otherAABB)
        {
            //TODO: impliment
            return objAABB;
        }

        /// <summary>
        /// Applies collision resolution velocity to the provided object velocity vector. Resulting from an AABB vs Sphere collision.
        /// </summary>
        /// <param name="objVel">velocity to be modified</param>
        /// <param name="objAABB">object AABB hitbox</param>
        /// <param name="otherSphere">other sphere hitbox</param>
        /// <returns>The provided AABB collider for the object properly offset during collision resolution</returns>
        private static AABBCollider applyCollisionAABBVsSphere(ref Vector3 objVel, AABBCollider objAABB, SphereCollider otherSphere)
        {
            //TODO: impliment
            return objAABB;
        }

        /// <summary>
        /// Applies collision resolution velocity to the provided object velocity vector. Resulting from a Sphere vs Plane collision.
        /// </summary>
        /// <param name="objVel">velocity to be modified</param>
        /// <param name="objSphere">object Sphere hitbox</param>
        /// <param name="otherPlane">other plane hitbox</param>
        /// <returns>The provided Sphere collider for the object properly offset during collision resolution</returns>
        private static SphereCollider applyCollisionSphereVsPlane(ref Vector3 objVel, SphereCollider objSphere, PlaneCollider otherPlane)
        {
            //TODO: impliment
            return objSphere;
        }


        /// <summary>
        /// Applies collision resolution velocity to the provided object velocity vector. Resulting from a Sphere vs AABB collision.
        /// </summary>
        /// <param name="objVel">velocity to be modified</param>
        /// <param name="objSphere">object Sphere hitbox</param>
        /// <param name="otherAABB">other AABB hitbox</param>
        /// <returns>The provided Sphere collider for the object properly offset during collision resolution</returns>
        private static SphereCollider applyCollisionSphereVsAABB(ref Vector3 objVel, SphereCollider objSphere, AABBCollider otherAABB)
        {
            //TODO: impliment
            return objSphere;
        }

        /// <summary>
        /// Applies collision resolution velocity to the provided object velocity vector. Resulting from a Sphere vs Sphere collision.
        /// </summary>
        /// <param name="objVel">velocity to be modified</param>
        /// <param name="objSphere">object Sphere hitbox</param>
        /// <param name="otherSphere">other plane hitbox</param>
        /// <returns>The provided Sphere collider for the object properly offset during collision resolution</returns>
        private static SphereCollider applyCollisionSphereVsSphere(ref Vector3 objVel, SphereCollider objSphere, SphereCollider otherSphere)
        {
            //TODO: impliment
            return objSphere;
        }


        /// <summary>
        /// Applies collision resolution velocity to the provided object velocity vector. Resulting from a Point vs Plane collision.
        /// </summary>
        /// <param name="objVel">velocity to be modified</param>
        /// <param name="objPoint">object Sphere hitbox</param>
        /// <param name="otherPlane">other plane hitbox</param>
        /// <returns>The provided Point collider for the object properly offset during collision resolution</returns>
        private static PointCollider applyCollisionPointVsPlane(ref Vector3 objVel, PointCollider objPoint, PlaneCollider otherPlane)
        {
            //TODO: impliment
            return objPoint;
        }

        /// <summary>
        /// Applies collision resolution velocity to the provided object velocity vector. Resulting from a Point vs AABB collision.
        /// </summary>
        /// <param name="objVel">velocity to be modified</param>
        /// <param name="objPoint">object Sphere hitbox</param>
        /// <param name="otherAABB">other plane hitbox</param>
        /// <returns>The provided Point collider for the object properly offset during collision resolution</returns>
        private static PointCollider applyCollisionPointVsAABB(ref Vector3 objVel, PointCollider objPoint, AABBCollider otherAABB)
        {
            //TODO: impliment
            return objPoint;
        }

        /// <summary>
        /// Applies collision resolution velocity to the provided object velocity vector. Resulting from a Point vs Sphere collision.
        /// </summary>
        /// <param name="objVel">velocity to be modified</param>
        /// <param name="objPoint">object Sphere hitbox</param>
        /// <param name="otherSphere">other plane hitbox</param>
        /// <returns>The provided Point collider for the object properly offset during collision resolution</returns>
        private static PointCollider applyCollisionPointVsSphere(ref Vector3 objVel, PointCollider objPoint, SphereCollider otherSphere)
        {
            //TODO: impliment
            return objPoint;
        }
    }
}
