using NoiseTest;

namespace RabbetGameEngine
{
    public class EarthNoise
    {
        private OpenSimplexNoise baseNoise = null;
        private static readonly int octaveCount = 4;
        private static readonly float[] xOffsets = null;
        private static readonly float[] zOffsets = null;
        private static readonly float[] frequencies = null;
        private static readonly float[] amplitudes = null;

        static EarthNoise()
        {
            xOffsets = new float[octaveCount];
            zOffsets = new float[octaveCount];
            frequencies = new float[octaveCount];
            amplitudes = new float[octaveCount];
            float curXOff = 0;
            float curZOff = 0;
            float curFreq = 0.005F * Chunk.VOXEL_PHYSICAL_SIZE;
            float curAmp = 1.5F;
            for(int i = 0; i < octaveCount; i++)
            {
                xOffsets[i] = curXOff;
                zOffsets[i] = curZOff;
                frequencies[i] = curFreq;
                amplitudes[i] = curAmp;
                curXOff += 4096;
                curZOff += 4096;
                curFreq *= 2.7F;
                curAmp *= 0.36F;
            }
        }

        public EarthNoise(long seed)
        {
            baseNoise = new OpenSimplexNoise(seed);
        }

        public double noise(int x, int z)
        {
            float currentFreq;
            float currentAmp;
            float totalAmp = 0;
            double result = 0;
            for(int i = 0; i < octaveCount; i++)
            {
                currentFreq = frequencies[i];
                currentAmp = amplitudes[i];
                result += (baseNoise.Evaluate(x * currentFreq + xOffsets[i], z * currentFreq + zOffsets[i]) + 1D) * 0.5D * currentAmp;
                totalAmp += currentAmp;
            }
            return result/totalAmp;
        }
    }
}
