using OpenTK;

namespace RabbetGameEngine.Physics
{
    /*An abstract interface for handling many different colliders.*/
    public interface ICollider
    {
        //update the collider, eg, resizing, repositioning. this MUST be done AFTER all of the parents movement is applied.
        void onTick();

        //returns a bool determining if this collider has a parent entity and/or if the parent entity is not equal to null
        bool getHasParent();

        PositionalObject getParent();

        void setParent(PositionalObject parent);

        /// <summary>
        /// returns a world relative positional vector at the center of this hitbox
        /// </summary>
        /// <returns>returns a world relative positional vector at the center of this hitbox</returns>
        Vector3 getCenterVec();

        /// <summary>
        /// offsets this hitbox by the provided direction vector
        /// </summary>
        /// <param name="direction">direction vector of offset</param>
        void offset(Vector3 direction);

        int getCollisionWeight();

        /*gets this hitbox moved by one tick*/
        ICollider getNextTickPredictedHitbox();

        ColliderType getType();
    }
}
