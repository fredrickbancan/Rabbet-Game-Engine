using FredrickTechDemo.FredsMath;
using System;

namespace FredrickTechDemo.VFX
{
    /*Base class for any VFX using point particles*/
    public class VFXPointParticles : VFXBase
    {
        private static String shaderDir = ResourceHelper.getShaderFileDir(@"VFX\PointParticleFogShader.shader");  
        public VFXPointParticles(Vector3D pos, Vector3D initalVelocity, ColourF color, float spread, float radius, bool ambientOcclusion, float maxExistingSeconds = 0.5F) : base(pos, 1.0F, shaderDir, "none", "none", maxExistingSeconds, VFXRenderType.points)
        {
        }
    }
}
