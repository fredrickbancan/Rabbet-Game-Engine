using OpenTK;
using RabbetGameEngine.Physics;
namespace RabbetGameEngine
{
    /*Class for the player. Contains the players name, inventory etc.*/
    public class EntityPlayer : EntityLiving
    {
        
        private string playerName;
        private Camera camera;
        public bool paused = false;
        public bool debugScreenOn = false;

        public static readonly Vector3 eyeOffset = new Vector3(0.0F, 0.62F, 0.0F);
        public EntityPlayer(string name) : base()
        {
            isPlayer = true;
            this.playerName = name;
            camera = new Camera(this);
            this.collider = new AABB(new Vector3(-0.5F, -1, -0.5F), new Vector3(0.5F, 1, 0.5F), this);
            this.hasCollider = true;
        }
        public EntityPlayer(string name, Vector3 spawnPosition) : base(spawnPosition)
        {
            isPlayer = true;
            this.playerName = name;
            camera = new Camera(this);
            this.collider = new AABB(new Vector3(-0.5F, -1, -0.5F), new Vector3(0.5F, 1, 0.5F), this);
            this.hasCollider = true;
        }

        public override void onTick()
        {
            if (!paused)
            {
                base.onTick();
            }
            camera.onTick();
        }

        public Vector3 getEyePosition()
        {
            return pos + EntityPlayer.eyeOffset;
        }

        public Vector3 getLerpEyePos()
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

        public string getName()
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

        public override bool doingAction(Action act)
        {
            return PlayerController.getDoingAction(act);
        }
    }
}
