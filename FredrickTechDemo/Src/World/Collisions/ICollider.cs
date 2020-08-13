using FredrickTechDemo.FredsMath;

namespace FredrickTechDemo
{
    /*An abstract interface for handling many different colliders.*/
    public interface ICollider
    {
        //outputs the directions that the testing input collides with this collider.
        //for example, if the input lands on top of this collider, then in results, there will be CollisionDirection.bottom
        //the returned collisiondirection values will be used to calculate how to manipulate the child of the provided hitbox.
        //for example, if the provided hitbox collides with this hitbox from positive X, then in the results, there will be CollisionDirection.xPos.
        //And then, from that result, the CollisionHandler will then change the child entity of the provided hitbox's velocity on the positive X axis.

        //in this game engine (for now) all collisions are flat and axis aligned. If there was a need for more complex collisions such as off angles
        //then these methods can return a normal vector of the opposite direction of the collision.
        CollisionDirection getCollisionResultAABB(AABBCollider boxToTest);
        CollisionDirection getCollisionResultPoint(Vector3D pointToTest);
        CollisionDirection getCollisionResultSphere(SphereCollider sphereToTest);

        //update the collider, eg, resizing, repositioning.
        void onTick();

        //returns a bool determining if this collider has a parent entity and/or if the parent entity is not equal to null
        bool getHasParent();
    }
}
