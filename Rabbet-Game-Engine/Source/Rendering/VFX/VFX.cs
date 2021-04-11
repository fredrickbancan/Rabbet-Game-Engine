using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public class VFX : PositionalObject
    {
        protected World currentWorld;
        protected bool disposed = false;
        protected double maxExistingTicks;
        protected uint ticksExisted;
        protected bool removalFlag = false;// true if this entity should be removed in the next tick
        protected RenderType renderType;
        public string vfxName = "";

        public VFX(World w, Vector3 pos, float maxExistingSeconds = 0, RenderType type = RenderType.triangles)
        {
            currentWorld = w;
            this.pos = pos;
            this.renderType = type;
            maxExistingTicks = TicksAndFrames.getNumOfTicksForSeconds(maxExistingSeconds);
        }

        /*called every tick*/
        public virtual void preTick()
        {
            prevTickPos = pos;
            prevTickPitch = pitch;
            prevTickYaw = yaw;
            prevTickRoll = roll;

            ticksExisted++;
            if (maxExistingTicks > 0.0F)
            {
                if (ticksExisted >= maxExistingTicks)
                {
                    ceaseToExist();
                }
            }
        }

        public virtual void onTick(float timeStep)
        {

        }

        public virtual void setVFXName(string n)
        {
            this.vfxName = n;
        }

        public virtual void sendRenderRequest()
        {
           
        }

        public virtual void ceaseToExist()
        {
            removalFlag = true;
        }

        public RenderType getRenderType()
        {
            return renderType;
        }

        public virtual bool exists()
        {
            return !removalFlag;
        }
    }
}
