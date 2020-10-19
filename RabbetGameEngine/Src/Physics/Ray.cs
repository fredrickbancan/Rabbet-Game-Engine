using OpenTK;

namespace RabbetGameEngine.Physics
{
    /// <summary>
    /// A stuct for creating and testing rays against boxes and other things
    /// </summary>
    public struct Ray
    {
        Vector3 pos;
        Vector3 direction;
        float length;

        public Ray(Vector3 pos, Vector3 directionAndLength)
        {
            length = directionAndLength.Length;
            direction = directionAndLength.Normalized();
            this.pos = pos;
        }

        public Ray(Vector3 pos, Vector3 direction, float length)
        {
            this.pos = pos;
            this.direction = direction.Normalized();
            this.length = length;
        }

        /// <summary>
        /// Tests if this ray intersects the provided box
        /// </summary>
        /// <param name="box">the box to test against</param>
        /// <param name="intersectionPos">the resulting location of intersection of the ray and box</param>
        /// <returns>true if intersection takes place. Else returns false.</returns>
        public bool intersects(AABB box, out Vector3 intersectionpos)
        {
            //TODO: impliment
            intersectionpos = new Vector3();
            return false;
        }

        /// <summary>
        /// Tests if this ray intersects the provided box
        /// </summary>
        /// <param name="box">the box to test against</param>
        /// <returns>true if intersection takes place. Else returns false.</returns>
        public bool intersects(AABB box)
        {
            //TODO: impliment
            return false;
        }

        /// <summary>
        /// Tests if this ray intersects the provided box
        /// </summary>
        /// <param name="box">the box to test against</param>
        /// <param name="ray">the ray to test against</param>
        /// <param name="intersectionPos">resulting position of intersection</param>
        /// <returns>true if intersection takes place. Else returns false.</returns>
        public static bool intersects(Ray ray, AABB box, out Vector3 intersectionPos)
        {
            //TODO: impliment
            intersectionPos = new Vector3();
            return false;
        }

        /// <summary>
        /// Tests if this ray intersects the provided box
        /// </summary>
        /// <param name="box">the box to test against</param>
        /// <param name="ray">the ray to test against</param>
        /// <returns>true if intersection takes place. Else returns false.</returns>
        public static bool intersects(Ray ray, AABB box)
        {
            //TODO: impliment
            return false;
        }
    }
}
