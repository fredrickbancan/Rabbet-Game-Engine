namespace FredrickTechDemo
{
    /*An abstract interface for handling many different colliders.*/
    public interface ICollider
    {
        //update the collider, eg, resizing, repositioning. this MUST be done AFTER all of the parents movement is applied.
        void onTick();

        //returns a bool determining if this collider has a parent entity and/or if the parent entity is not equal to null
        bool getHasParent();

        Entity getParent();

        /*gets this hitbox moved by one tick*/
        ICollider getNextTickPredictedHitbox();

        ColliderType getType();
    }
}
