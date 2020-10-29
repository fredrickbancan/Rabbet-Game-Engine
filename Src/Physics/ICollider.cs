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

        /// <summary>
        /// offsets this hitbox by the provided directions
        /// </summary>
        /// <param name="x">x direction of offset</param>
        /// <param name="y">y direction of offset</param>
        /// <param name="z">z direction of offset</param>
        void offset(float x, float y, float z);

        ColliderType getType();
    }
}
