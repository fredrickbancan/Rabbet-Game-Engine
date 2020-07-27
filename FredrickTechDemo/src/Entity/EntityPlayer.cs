using FredrickTechDemo.FredsMath;
using System;

namespace FredrickTechDemo
{
    /*Class for the player. Contains the players name, inventory etc.*/
    class EntityPlayer : Entity// a middle "alive" entity class needs to be created with the vectors, health , movement speed etc.
    {
        private String playerName;
        private Vector3F frontVector;
        private Vector3F upVector;
        private Vector3F movementVector;
        private Camera camera;
        public EntityPlayer(String name)
        {
            this.playerName = name;
            camera = new Camera(this);
            frontVector = new Vector3F(0.0F, 0.0F, -1.0F);
            upVector = new Vector3F(0.0F, 1.0F, 0.0F);
        }
        public EntityPlayer(String name, Vector3F spawnPosition) : base(spawnPosition)
        {
            this.playerName = name;
            camera = new Camera(this);
        }

        public Vector3F getFrontVector()
        {
            return this.frontVector;
        }

        public Vector3F GetVector()
        {
            return this.upVector;
        }
        public Camera getCamera()
        {
            return this.camera;
        }
    }
}
