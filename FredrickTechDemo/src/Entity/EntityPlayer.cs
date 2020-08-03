using FredrickTechDemo.FredsMath;
using System;

namespace FredrickTechDemo
{
    /*Class for the player. Contains the players name, inventory etc.*/
    public class EntityPlayer : EntityLiving
    {
        private String playerName;
        private Camera camera;
        public bool paused = false;
        public bool debugScreenOn = false;
        public EntityPlayer(String name) : base()
        {
            this.playerName = name;
            camera = new Camera(this);
        }
        public EntityPlayer(String name, Vector3D spawnPosition) : base(spawnPosition)
        {
            this.playerName = name;
            camera = new Camera(this);
        }

        public override void onTick()
        {
            if (!paused)
            {
                base.onTick();//do first
            }
        }

        /*Called before game renders, each frame.*/
        public void onCameraUpdate()
        {
            if (!paused)
            {
                this.camera.onUpdate();
            }
        }

        public String getName()
        {
            return this.playerName;
        }
        public void togglePause()
        {
            if(!paused)
            {
                paused = true;
            }
            else
            {
                paused = false;
            }
        }
        public Camera getCamera()
        {
            return this.camera;
        }
    }
}
