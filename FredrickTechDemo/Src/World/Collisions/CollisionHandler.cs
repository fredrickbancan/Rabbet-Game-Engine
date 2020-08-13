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
            Dictionary<int, ICollider> allPredictedColliders = getTempNextTickHitboxes(allColliders);



        }



        /*creates a copy of all the hitboxes that have moved by the parent entities velocity to simulate their state in the next tick.*
         *from this we can calculate how to modify the parent entities velocities to apply the collisions.
          also weeds out any null colliders*/
        private static Dictionary<int, ICollider> getTempNextTickHitboxes(Dictionary<int, ICollider> allColliders)
        {
            Dictionary<int, ICollider> result = new Dictionary<int, ICollider>();

            foreach(KeyValuePair<int, ICollider> pair in allColliders)
            {
                if (pair.Value != null)
                {
                    if (pair.Key == -1)//-1 means non moving inanimate collider with no parent.
                    {
                        result.Add(pair.Key, pair.Value);
                    }
                    else
                    {
                        result.Add(pair.Key, pair.Value.getNextTickPredictedHitbox());
                    }
                }
            }

            return result;
        }
    }
}
