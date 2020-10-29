using OpenTK.Mathematics;
using RabbetGameEngine.Debugging;
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
        private static readonly float pushVel = 0.05F * 0.5F;
        //TODO: impliment some sort of space partitioning (i.e, chunks) to avoid O(n^2 - n) complexity. Make sure to perform tests to measure performance changes.
        /// <summary>
        /// Does collisions with each entity against all other entities it is touching.
        /// No hard collisions, only pushing eachother away.
        /// </summary>
        /// <param name="entities"> All world entities to collide eachother.</param>
        public static void collideEntities(Dictionary<int, Entity> entities)
        {
            Profiler.beginEndProfile("EntCollisions");
            for(int i = 0; i < entities.Count; ++i)
            { 
                Entity entAt = entities.Values.ElementAt(i);
                if (!entAt.getHasCollider())
                {
                    continue;
                }
                //Exclude entity collisions if they dont use aabb
                if(entAt.getCollider().getType() != ColliderType.aabb)
                {
                    continue;
                }

                Vector2 entXZ;
                Vector2 otherEntXZ;
                Vector2 pushVec;
                for (int j = i + 1; j < entities.Count; ++j)
                {
                    Entity otherEntAt = entities.Values.ElementAt(j);
                    if(!otherEntAt.getHasCollider() || otherEntAt.getCollider().getType() != ColliderType.aabb)
                    {
                        continue;
                    }

                    //if boxes touch
                    if(!AABB.areBoxesNotTouching((AABB)entAt.getCollider(), (AABB)otherEntAt.getCollider()))
                    {
                        if ((otherEntXZ = otherEntAt.getPosition().Xz) != (entXZ = entAt.getPosition().Xz))
                        {
                           pushVec = Vector2.Normalize(otherEntXZ - entXZ) * pushVel;
                        }
                        else
                        { 
                            //avoiding divide by zero in normalize func
                            pushVec = Vector2.Normalize((otherEntXZ - new Vector2(0.0001F, 0.0001F)) - entXZ) * pushVel;
                        }
                        entAt.addVelocity(new Vector3(-pushVec.X, 0, -pushVec.Y));
                        otherEntAt.addVelocity(new Vector3(pushVec.X, 0, pushVec.Y));
                        entAt.onCollideWithEntity(otherEntAt);
                        otherEntAt.onCollideWithEntity(entAt);
                    }
                }

            }
            Profiler.beginEndProfile("EntCollisions");
        }

        /// <summary>
        /// Should be done after on tick and before post tick.
        /// tries to move the provided positional object by its velocity with respect to all of the provided colliders.
        /// </summary>
        /// <param name="obj"> the object to move </param>
        /// <param name="worldAABB"> all world colliders </param>
        public static void tryToMoveObject(PositionalObject obj, List<AABB> worldAABB)
        {
            if (!obj.getHasCollider())
            {
                obj.setPosition(obj.getPosition() + obj.getVelocity());
                return;
            }
            Profiler.beginEndProfile("Collisions");

            ICollider objCollider = obj.getColliderHandle();
            Vector3 objVel = obj.getVelocity();
            Vector3 prevObjVel = objVel;

            switch(objCollider.getType())
            {
                case ColliderType.aabb:
                    AABB objBox = (AABB)objCollider;//using this to avoid casting inside the loops
                    for (int i = 0; i < worldAABB.Count; i++)
                    {
                        objVel = applyCollisionAABBVsAABBY(objVel, objBox, worldAABB.ElementAt(i));
                    }
                    objCollider.offset(0, objVel.Y, 0);
                    objBox = (AABB)objCollider;
                    for (int i = 0; i < worldAABB.Count; i++)
                    {
                        objVel = applyCollisionAABBVsAABBX(objVel, objBox, worldAABB.ElementAt(i));
                    }
                    objCollider.offset(objVel.X, 0, 0);
                    objBox = (AABB)objCollider;
                    for (int i = 0; i < worldAABB.Count; i++)
                    {
                        objVel = applyCollisionAABBVsAABBZ(objVel, objBox, worldAABB.ElementAt(i));
                    }
                    objCollider.offset(0, 0, objVel.Z);
                    break;

                case ColliderType.point:
                    PointCollider objPoint = (PointCollider)objCollider;
                    for (int i = 0; i < worldAABB.Count; i++)
                    {
                        objVel = applyCollisionPointVsAABBY(objVel, objPoint, worldAABB.ElementAt(i));
                    }
                    objCollider.offset(0, objVel.Y, 0);

                    for (int i = 0; i < worldAABB.Count; i++)
                    {
                        objVel = applyCollisionPointVsAABBZ(objVel, objPoint, worldAABB.ElementAt(i));
                    }
                    objCollider.offset(0, 0, objVel.Z);

                    for (int i = 0; i < worldAABB.Count; i++)
                    {
                        objVel = applyCollisionPointVsAABBX(objVel, objPoint, worldAABB.ElementAt(i));
                    }
                    objCollider.offset(objVel.X, 0, 0);
                    break;
            }
            
            if(prevObjVel.X != objVel.X || prevObjVel.Y != objVel.Y || prevObjVel.Z != objVel.Z)
            {
                obj.setHasCollided(true);
            }

            //if the object was moving downwards and a collision has changed its vertical velocity then it is now grounded
            if(prevObjVel.Y != objVel.Y && prevObjVel.Y < 0.0F)
            {
                obj.setIsGrounded(true);
            }
            
            //apply new velocity
            obj.setVelocity(objVel);

            //lastly, position obj to center of hitbox offset
            obj.setPosition(objCollider.getCenterVec());
            Profiler.beginEndProfile("Collisions");
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
                float xDist;//distance between boxes in X direction, depending on position and velocitx

                //if ent is moving towards positive x and entitx box is "to left" of other box
                if (objVel.X > 0.0F && objAABB.maxX <= otherAABB.minX)
                {
                    xDist = otherAABB.minX - objAABB.maxX;
                    if (xDist < objVel.X)
                    {
                        objVel.X = xDist;
                    }
                }

                //if ent is moving towards negative x and entitx box is "to right" of other box
                if (objVel.X < 0.0F && objAABB.minX >= otherAABB.maxX)
                {
                    xDist = otherAABB.maxX - objAABB.minX;//creating negative dist for comparing with negative velocitx so there is no need to use abs() func
                    if (xDist > objVel.X)
                    {
                        objVel.X = xDist;
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
        //TODO: impliment point vs AABB collisions
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
            return objVel;
        }
        #endregion
    }
}
