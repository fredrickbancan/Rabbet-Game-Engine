using FredrickTechDemo.FredsMath;
using System;

namespace FredrickTechDemo
{
    /*Class for the player. Contains the players name, inventory etc.*/
    public class EntityPlayer : EntityLiving
    {
        private String playerName;
        private Camera camera;
        public bool menuOpen = false;
        public bool debugScreenOn = false;
        public EntityPlayer(String name) : base()
        {
            this.playerName = name;
            camera = new Camera(this);
        }
        public EntityPlayer(String name, Vector3F spawnPosition) : base(spawnPosition)
        {
            this.playerName = name;
            camera = new Camera(this);
        }

        public override void onTick()
        {
            base.onTick();//do first
        }

        /*Called before game renders, each frame.*/
        public void onCameraUpdate()
        {
            if (!menuOpen)
            {
                this.camera.onUpdate();
            }
        }

        public String getName()
        {
            return this.playerName;
        }
        public void toggleOpenMenu()
        {
            if(!menuOpen)
            {
                menuOpen = true;
            }
            else
            {
                menuOpen = false;
            }
        }
        public Camera getCamera()
        {
            return this.camera;
        }
    }
}
