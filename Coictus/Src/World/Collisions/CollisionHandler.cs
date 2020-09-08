using OpenTK;
using System.Collections.Generic;
using System.Linq;

namespace Coictus
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
            Vector3d currentCollisionDirection;
            double currentCollisionOverlapResult = 0;
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
            double currentCollisionOverlapResult = 0;
            Vector3d currentCollisionDirection;
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

        
    }
}
