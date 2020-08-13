using System.Collections.Generic;

namespace FredrickTechDemo
{
    public enum CollisionDirection //used for giving collision results. i.e, the direction an object has collided with another object.
    {
        none,
        xNeg,
        xPos,
        top,
        bottom,
        zNeg,
        zPos
    }

    /*An abstraction class which manipulates multiple entity velocities based on their colliders and detects collisions.
      All collisions will be FLAT using this system. Meaning all collisions will be axis aligned. Any collision on any axis should
      cancel the respective axis on the velocity of the child entities.*/
    public static class CollisionHandler
    {

        /*Takes in a list of all the colliders in a given space/world/planet and calculates their collisions and applies the result to the parent entities*/
        public static void doCollisions(Dictionary<int, ICollider> allColliders)
        {
            foreach(KeyValuePair<int, ICollider> collider in allColliders)
            {
                if(collider.Value != null)
                {
                    ICollider theCollider = collider.Value;














                }
            }
        }

    }
}
