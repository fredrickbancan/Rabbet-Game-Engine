using FredrickTechDemo.FredsMath;

namespace FredrickTechDemo
{
    /*An abstract interface for handling many different colliders.*/
    public interface ICollider
    {
        /*returns the results of a collision test between this and the provided icollider using the type of the icollider*/
        CollisionDirection getCollisionResultForColliderType(ICollider colliderToTest, out bool touching, out double overlap);

        //outputs the directions that the testing input collides with this collider.
        //for example, if the input lands on top of this collider, then in results, there will be CollisionDirection.bottom
        //the returned collisiondirection values will be used to calculate how to manipulate the child of the provided hitbox.
        //for example, if the provided hitbox collides with this hitbox from positive X, then in the results, there will be CollisionDirection.xPos.
        //And then, from that result, the CollisionHandler will then change the child entity of the provided hitbox's velocity on the positive X axis.

        //in this game engine (for now) all collisions are flat and axis aligned. If there was a need for more complex collisions such as off angles
        //then these methods can return a normal vector of the opposite direction of the collision.

        //the out bool touching variable is a flag representing if the two hitboxes atleast are touching in any way.
        //useful in the case of having two hitboxes that dont collide but you want to detect if they are touching.
        //in an axis aligned only context for example, a point and a sphere, or two spheres, can not collide in only one direction (except for impossibly rare situations)
        //so that will return a collisiondirection of "none" but if they are touching then the out bool touching will be true. This will let us
        //do things such as hit detection with a sphere hitbox bullet and a sphere hitbox target for example.
        CollisionDirection getCollisionResultAABB(AABBCollider boxToTest, out bool touching, out double overlap);
        CollisionDirection getCollisionResultPoint(Vector3D pointToTest, out bool touching, out double overlap);
        CollisionDirection getCollisionResultSphere(SphereCollider sphereToTest, out bool touching, out double overlap);

        /*Returns a copy of this hitbox which has been moved by the parent entities velocity by one tick which can be used to predict and account for collisions*/
        ICollider getNextTickPredictedHitbox();

        //update the collider, eg, resizing, repositioning.
        void onTick();

        //returns a bool determining if this collider has a parent entity and/or if the parent entity is not equal to null
        bool getHasParent();
    }
}
