using OpenTK;
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
        public static readonly Vector3d eyeOffset = new Vector3d(0.0D, 0.62D, 0.0D);
        public EntityPlayer(String name) : base()
        {
            isPlayer = true;
            this.playerName = name;
            camera = new Camera(this);
            this.setCollider(new AABBCollider(new Vector3d(-0.5, -1, -0.5), new Vector3d(0.5, 1, 0.5), this));
        }
        public EntityPlayer(String name, Vector3d spawnPosition) : base(spawnPosition)
        {
            isPlayer = true;
            this.playerName = name;
            camera = new Camera(this);
            this.setCollider(new AABBCollider(new Vector3d(-0.5, -1, -0.5), new Vector3d(0.5, 1, 0.5), this));
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

        public Vector3d getEyePosition()
        {
            return pos + EntityPlayer.eyeOffset;
        }

        public Vector3d getLerpEyePos()
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

        public override void applyCollision(Vector3d direction, double overlap)
        {
            if(!GameSettings.noclip)
            base.applyCollision(direction, overlap);
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
