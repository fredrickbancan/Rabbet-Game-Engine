using Coictus.FredsMath;
using System;

namespace Coictus
{
    /*Class for the player. Contains the players name, inventory etc.*/
    public class EntityPlayer : EntityLiving
    {
        private String playerName;
        private Camera camera;
        public bool paused = false;
        public bool debugScreenOn = false;
        public static readonly Vector3D eyeOffset = new Vector3D(0.0D, 0.62D, 0.0D);
        public EntityPlayer(String name) : base()
        {
            this.playerName = name;
            camera = new Camera(this);
            this.setCollider(new AABBCollider(new Vector3D(-0.5, -1, -0.5), new Vector3D(0.5, 1, 0.5), this));
        }
        public EntityPlayer(String name, Vector3D spawnPosition) : base(spawnPosition)
        {
            this.playerName = name;
            camera = new Camera(this);
            this.setCollider(new AABBCollider(new Vector3D(-0.5, -1, -0.5), new Vector3D(0.5, 1, 0.5), this));
        }

        public override void onTick()
        {
            if (!paused)
            {
                base.onTick();//do first
            }
            camera.onTick();
        }

        /*Called by input when user left clicks*/
        public void onLeftClick()
        {
            if(currentVehicle != null)
            {
                currentVehicle.onLeftClick();
            }
        }

        /*When the player is rotated by something other than the camera, it needs to apply the rotation to the camera smoothly in between ticks. 
          The camera will then apply this rotation to the player entity.*/
        public override void rotateYaw(double amount)
        {
            camera.tickRotateYaw(amount);
        }

        public Vector3D getEyePosition()
        {
            return pos + EntityPlayer.eyeOffset;
        }

        public Vector3D getLerpEyePos()
        {
            return this.getLerpPos() + EntityPlayer.eyeOffset;
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
                TicksAndFps.pause();
            }
            else
            {
                paused = false;
                TicksAndFps.unPause();
            }
        }
        public Matrix4 getViewMatrix()
        {
            return camera.getViewMatrix();
        }
        public Camera getCamera()
        {
            return this.camera;
        }
    }
}
