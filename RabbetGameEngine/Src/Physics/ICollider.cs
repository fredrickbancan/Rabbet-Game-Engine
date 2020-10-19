using OpenTK;

namespace RabbetGameEngine.Physics
{
    /*An abstract interface for handling many different colliders.*/
    public interface ICollider
    {
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

        ColliderType getType();
    }
}
