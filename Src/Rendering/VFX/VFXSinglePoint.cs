using OpenTK.Mathematics;

namespace RabbetGameEngine.VisualEffects
{
    public class VFXSinglePoint : VFX
    {
        public PointParticle theParticle;
        public PointParticle prevParticle;
        bool transparency = false;
        float randomHorizontalVelocity = 0;
        float randomVerticalVelocity = 0;
        public VFXSinglePoint(Vector3 initialPos, Color color, float radius, float existingSeconds, bool aoc, bool transparency) : base(initialPos, 0, "none", null, existingSeconds,transparency ? RenderType.lerpISpheresTransparent : RenderType.lerpISpheres)
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
            prevParticle.pos = previousTickPos;
            previousTickPos = pos;
            theParticle.pos = pos;
        }

        public override void onTick()
        {
            if(!isGrounded)
            {
                velocity.X += (1.0F - (float)GameInstance.rand.NextDouble() * 2.0F) * randomHorizontalVelocity;
                velocity.Z += (1.0F - (float)GameInstance.rand.NextDouble() * 2.0F) * randomHorizontalVelocity;
                velocity.Y += (1.0F - (float)GameInstance.rand.NextDouble() * 2.0F) * randomVerticalVelocity;
            }
            base.onTick();
        }

        public override void sendRenderRequest()
        {
            Renderer.requestRender(theParticle, prevParticle, transparency);
        }

    }
}
