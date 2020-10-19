using OpenTK;
using System.Collections.Generic;
using System.Linq;

namespace RabbetGameEngine.Physics
{
    /// <summary>
    /// used for detecting which type a collider is
    /// </summary>
    public enum ColliderType
    {
        aabb,
        point
    };

    /// <summary>
    /// Abstraction for collision code. This class will be responsable for calculating collisions by
    /// offsetting the hitbox of entities and altering their velocity. This also moves them by their velocity
    /// with respect to the colliders provided. All world colliders are not modified and are prioritised over
    /// entity colliders.
    /// </summary>
    public static class CollisionHandler
    {
        /// <summary>
        /// Enum for determining which offset direction to operate on
        /// </summary>
        private enum OffsetDirection
        {
            X,
            Y,
            Z
        };

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

            ICollider objCollider = obj.getColliderHandle();
            Vector3 objVel = obj.getVelocity();

            //do all world collisions first 
            for(int i = 0; i < worldColliders.Count; i++)
            {
                applyCollisionForColliderType(ref objVel, objCollider, worldColliders.ElementAt(i), OffsetDirection.Y);
            }
            objCollider.offset(0, objVel.Y, 0);

            for (int i = 0; i < worldColliders.Count; i++)
            {
                applyCollisionForColliderType(ref objVel, objCollider, worldColliders.ElementAt(i), OffsetDirection.Z);
            }
            objCollider.offset(0, 0, objVel.Z);

            for (int i = 0; i < worldColliders.Count; i++)
            {
                applyCollisionForColliderType(ref objVel, objCollider, worldColliders.ElementAt(i), OffsetDirection.X);
            }
            objCollider.offset(objVel.X, 0, 0);

            //do all entity collisions
          /*  for (int i = 0; i < entities.Count; i++)
            {
                applyCollisionForColliderType(ref objVel, objCollider, entities.Values.ElementAt(i).getCollider(), OffsetDirection.Y);
            }
            objCollider.offset(0, objVel.Y, 0);

            for (int i = 0; i < entities.Count; i++)
            {
                applyCollisionForColliderType(ref objVel, objCollider, entities.Values.ElementAt(i).getCollider(), OffsetDirection.Z);
            }
            objCollider.offset(0, 0, objVel.Z);

            for (int i = 0; i < entities.Count; i++)
            {
                applyCollisionForColliderType(ref objVel, objCollider, entities.Values.ElementAt(i).getCollider(), OffsetDirection.X);
            }
            objCollider.offset(objVel.X, 0, 0);*/

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
        /// <param name="dir">The direction to calculate offsets in</param>
        private static void applyCollisionForColliderType(ref Vector3 vel, ICollider objCollider, ICollider otherCollider, OffsetDirection dir)
        {
            if (otherCollider.getType() != ColliderType.aabb)
            { return; }

            switch (objCollider.getType())
            {
                case ColliderType.aabb:
                    switch(dir)
                    {
                        case OffsetDirection.X:
                            vel = applyCollisionAABBVsAABBX(vel, (AABB)objCollider, (AABB)otherCollider);
                            break;

                        case OffsetDirection.Y:
                            vel = applyCollisionAABBVsAABBY(vel, (AABB)objCollider, (AABB)otherCollider);
                            break;

                        case OffsetDirection.Z:
                            vel = applyCollisionAABBVsAABBZ(vel, (AABB)objCollider, (AABB)otherCollider);
                            break;

                        default:
                            break;
                    }
                    break;

                case ColliderType.point:
                    switch (dir)
                    {
                        case OffsetDirection.X:
                            vel = applyCollisionPointVsAABBX(vel, (PointCollider)objCollider, (AABB)otherCollider);
                            break;

                        case OffsetDirection.Y:
                            vel = applyCollisionPointVsAABBY(vel, (PointCollider)objCollider, (AABB)otherCollider);
                            break;

                        case OffsetDirection.Z:
                            vel = applyCollisionPointVsAABBZ(vel, (PointCollider)objCollider, (AABB)otherCollider);
                            break;

                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }
        }
       

        #region AABBvsAABB
        /// <summary>
        /// Applies collision resolution velocity to the provided object velocity vector. Resulting from an AABB vs AABB collision.
        /// </summary>
        /// <param name="objVel">velocity to be modified</param>
        /// <param name="objAABB">object AABB hitbox</param>
        /// <param name="otherAABB">other AABB hitbox</param>
        /// <returns>The provided velocity the object properly offset during collision resolution</returns>
        private static Vector3 applyCollisionAABBVsAABBX(Vector3 objVel, AABB objAABB, AABB otherAABB)
        {
            if (AABB.overlappingY(objAABB, otherAABB) && AABB.overlappingZ(objAABB, otherAABB))
            {
                float zDist;//distance between boxes in X direction, depending on position and velocitx

                //if ent is moving towards positive x and entitx box is "to left" of other box
                if (objVel.X > 0.0F && objAABB.maxX <= otherAABB.minX)
                {
                    zDist = otherAABB.minX - objAABB.maxX;
                    if (zDist < objVel.X)
                    {
                        objVel.X = zDist;
                    }
                }

                //if ent is moving towards negative x and entitx box is "to right" of other box
                if (objVel.X < 0.0F && objAABB.minX >= otherAABB.maxX)
                {
                    zDist = otherAABB.maxX - objAABB.minX;//creating negative dist for comparing with negative velocitx so there is no need to use abs() func
                    if (zDist > objVel.X)
                    {
                        objVel.X = zDist;
                    }
                }
            }
            return objVel;
        }

        /// <summary>
        /// Applies collision resolution velocity to the provided object velocity vector. Resulting from an AABB vs AABB collision.
        /// </summary>
        /// <param name="objVel">velocity to be modified</param>
        /// <param name="objAABB">object AABB hitbox</param>
        /// <param name="otherAABB">other AABB hitbox</param>
        /// <returns>The provided velocity the object properly offset during collision resolution</returns>
        private static Vector3 applyCollisionAABBVsAABBY(Vector3 objVel, AABB objAABB, AABB otherAABB)
        {
            if(AABB.overlappingX(objAABB, otherAABB) && AABB.overlappingZ(objAABB, otherAABB))
            {
                float yDist;//distance between boxes in Y direction, depending on position and velocity

                //if ent is moving towards positive y and entity box is "below" of other box
                if(objVel.Y > 0.0F && objAABB.maxY <= otherAABB.minY)
                {
                    yDist = otherAABB.minY - objAABB.maxY;
                    if(yDist < objVel.Y)
                    {
                        objVel.Y = yDist;
                    }
                }

                //if ent is moving towards negative y and entity box is "above" of other box
                if (objVel.Y < 0.0F && objAABB.minY >= otherAABB.maxY)
                {
                    yDist = otherAABB.maxY - objAABB.minY;//creating negative dist for comparing with negative velocity so there is no need to use abs() func
                    if (yDist > objVel.Y)
                    {
                        objVel.Y = yDist;
                    }
                }
            }
            return objVel;
        }

        /// <summary>
        /// Applies collision resolution velocity to the provided object velocity vector. Resulting from an AABB vs AABB collision.
        /// </summary>
        /// <param name="objVel">velocity to be modified</param>
        /// <param name="objAABB">object AABB hitbox</param>
        /// <param name="otherAABB">other AABB hitbox</param>
        /// <returns>The provided velocity the object properly offset during collision resolution</returns>
        private static Vector3 applyCollisionAABBVsAABBZ(Vector3 objVel, AABB objAABB, AABB otherAABB)
        {
            if (AABB.overlappingY(objAABB, otherAABB) && AABB.overlappingX(objAABB, otherAABB))
            {
                float zDist;//distance between boxes in Z direction, depending on position and velocitx

                //if ent is moving towards positive z and entitx box is "behnid" of other box
                if (objVel.Z > 0.0F && objAABB.maxZ <= otherAABB.minZ)
                {
                    zDist = otherAABB.minZ - objAABB.maxZ;
                    if (zDist < objVel.Z)
                    {
                        objVel.Z = zDist;
                    }
                }

                //if ent is moving towards negative z and entitx box is "in front" of other box
                if (objVel.Z < 0.0F && objAABB.minZ >= otherAABB.maxZ)
                {
                    zDist = otherAABB.maxZ - objAABB.minZ;//creating negative dist for comparing with negative velocitx so there is no need to use abs() func
                    if (zDist > objVel.Z)
                    {
                        objVel.Z = zDist;
                    }
                }
            }
            return objVel;
        }
        #endregion

        #region PointVsAABB
        /// <summary>
        /// Applies collision resolution velocity to the provided object velocity vector. Resulting from a Point vs AABB collision.
        /// </summary>
        /// <param name="objVel">velocity to be modified</param>
        /// <param name="objPoint">object Sphere hitbox</param>
        /// <param name="otherAABB">other plane hitbox</param>
        /// <returns>The provided Point collider for the object properly offset during collision resolution</returns>
        private static Vector3 applyCollisionPointVsAABBX(Vector3 objVel, PointCollider objPoint, AABB otherAABB)
        {
            //TODO: impliment
            return objVel;
        }

        /// <summary>
        /// Applies collision resolution velocity to the provided object velocity vector. Resulting from a Point vs AABB collision.
        /// </summary>
        /// <param name="objVel">velocity to be modified</param>
        /// <param name="objPoint">object Sphere hitbox</param>
        /// <param name="otherAABB">other plane hitbox</param>
        /// <returns>The provided Point collider for the object properly offset during collision resolution</returns>
        private static Vector3 applyCollisionPointVsAABBY(Vector3 objVel, PointCollider objPoint, AABB otherAABB)
        {
            //TODO: impliment
            return objVel;
        }

        /// <summary>
        /// Applies collision resolution velocity to the provided object velocity vector. Resulting from a Point vs AABB collision.
        /// </summary>
        /// <param name="objVel">velocity to be modified</param>
        /// <param name="objPoint">object Sphere hitbox</param>
        /// <param name="otherAABB">other plane hitbox</param>
        /// <returns>The provided Point collider for the object properly offset during collision resolution</returns>
        private static Vector3 applyCollisionPointVsAABBZ(Vector3 objVel, PointCollider objPoint, AABB otherAABB)
        {
            //TODO: impliment
            return objVel;
        }
        #endregion

    }
}
