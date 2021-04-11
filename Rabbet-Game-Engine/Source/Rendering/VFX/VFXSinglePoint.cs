using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public class VFXSinglePoint : VFX
    {
        public PointParticle theParticle;
        public PointParticle prevParticle;
        bool transparency = false;
        float randomHorizontalVelocity = 0;
        float randomVerticalVelocity = 0;
        public VFXSinglePoint(World w, Vector3 initialPos, Color color, float radius, float existingSeconds, bool aoc, bool transparency) : base(w, initialPos, existingSeconds, transparency ? RenderType.lerpISpheresTransparent : RenderType.lerpISpheres)
        {
            this.transparency = transparency;
            theParticle = new PointParticle(initialPos, color.toNormalVec4(), radius, aoc);
            prevParticle = new PointParticle(initialPos, color.toNormalVec4(), radius, aoc);
        }

        public VFXSinglePoint setColorNonLerp(Color col)
        {
            theParticle.color = col.toNormalVec4();
            prevParticle.color = col.toNormalVec4();
            return this;
        }

        public VFXSinglePoint setColorLerp(Color col)
        {
            prevParticle.color = theParticle.color;
            theParticle.color = col.toNormalVec4();
            return this;
        }

        public VFXSinglePoint setRandomHorizontalVelocity(float n)
        {
            this.randomHorizontalVelocity = n;
            return this;
        }

        public VFXSinglePoint setRandomVerticalVelocity(float n)
        {
            this.randomVerticalVelocity = n;
            return this;
        }

        public override void preTick()
        {
            prevParticle.pos = prevTickPos;
            prevTickPos = pos;
            theParticle.pos = pos;
        }

        public override void onTick(float timeStep)
        {
            velocity.X += (1.0F - (float)GameInstance.rand.NextDouble() * 2.0F) * randomHorizontalVelocity * timeStep;
            velocity.Z += (1.0F - (float)GameInstance.rand.NextDouble() * 2.0F) * randomHorizontalVelocity * timeStep;
            velocity.Y += (1.0F - (float)GameInstance.rand.NextDouble() * 2.0F) * randomVerticalVelocity * timeStep;

            base.onTick(timeStep);
        }

        public override void sendRenderRequest()
        {
            Renderer.requestRender(theParticle, prevParticle, transparency);
        }

    }
}
