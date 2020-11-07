using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

namespace RabbetGameEngine.Sound
{
    public class PlayingSound
    {
        public static readonly int extraPlayMills = 100;
        public static readonly float volumeDistanceFactor = 64.0F;
        public int srcID = 0;
        public Sound theSound = null;
        public bool finishedPlaying = false;
        public long timeStampMills = 0;
        private long soundLengthMills = 0;
        public Vector3 sndPos;
        public float volume = 0.0F;
        public float maxDist = 0;
        public bool loopingSound = false;
        public string loopingSoundName = "";

        /// <summary>
        /// true if this sound is positional
        /// </summary>
        public bool isPositional = true;

        /// <summary>
        /// constructor for non-positional sounds
        /// </summary>
        public PlayingSound(Sound snd, float volume, float speed, long timeStamp)
        {
            theSound = snd;
            timeStampMills = timeStamp;
            soundLengthMills = (long)snd.getTotalTime().TotalMilliseconds;
            srcID = SoundManager.getSourceID();

            AL.Source(srcID, ALSourcei.Buffer, snd.getBufferID());
            volume = MathHelper.Clamp(volume, 0, 1);
            AL.Source(srcID, ALSourcef.Gain, volume * GameSettings.masterVolume);
            AL.Source(srcID, ALSourcef.Pitch, speed);
            AL.Source(srcID, ALSourcei.SourceType, 4136 /*ALSourceType.Static*/);
            AL.SourcePlay(srcID);
            isPositional = false;
        }


        /// <summary>
        /// constructor for positional sounds
        /// </summary>
        public PlayingSound(Sound snd, float volume, float speed, Vector3 pos, long timeStamp)
        {
            theSound = snd;
            sndPos = pos;
            this.volume = volume;
            this.maxDist = volumeDistanceFactor * volume;
            timeStampMills = timeStamp;
            soundLengthMills = (long)snd.getTotalTime().TotalMilliseconds;
            srcID = SoundManager.getSourceID();
            AL.Source(srcID, ALSourcei.Buffer, snd.getBufferID());
            AL.Source(srcID, ALSourceb.SourceRelative, true);
            AL.Source(srcID, ALSourcef.Pitch, speed);
            AL.Source(srcID, ALSourcei.SourceType, 4136 /*ALSourceType.Static*/);

            EntityPlayer p = GameInstance.get.thePlayer;
            setGainBasedOnDistance(p.getEyePosition());
            setPanBasedOnAngle(p.getEyePosition(), p.getRightVector());
            AL.SourcePlay(srcID);
            isPositional = true;
        }

        /// <summary>
        /// constructor for non-positional looping sounds
        /// </summary>
        public PlayingSound(string sndName, Sound snd, float volume, float speed)
        {
            loopingSoundName = sndName;
            loopingSound = true;
            theSound = snd;
            srcID = SoundManager.getSourceID();
            AL.Source(srcID, ALSourcei.Buffer, snd.getBufferID());
            AL.Source(srcID, ALSourceb.Looping, true);
            volume = MathHelper.Clamp(volume, 0, 1);
            AL.Source(srcID, ALSourcef.Gain, volume * GameSettings.masterVolume);
            AL.Source(srcID, ALSourcef.Pitch, speed);
            AL.Source(srcID, ALSourcei.SourceType, 4136 /*ALSourceType.Static*/);
            AL.SourcePlay(srcID);
            isPositional = false;
        }

        /// <summary>
        /// constructor for positional looping sounds
        /// </summary>
        public PlayingSound(string sndName, Sound snd, float volume, float speed, Vector3 pos)
        {
            loopingSoundName = sndName;
            loopingSound = true;
            theSound = snd;
            sndPos = pos;
            this.volume = MathHelper.Clamp(volume, 0, 1);
            this.maxDist = volumeDistanceFactor * volume;
            srcID = SoundManager.getSourceID();
            AL.Source(srcID, ALSourcei.Buffer, snd.getBufferID());
            AL.Source(srcID, ALSourceb.SourceRelative, true);
            AL.Source(srcID, ALSourceb.Looping, true);
            AL.Source(srcID, ALSourcef.Pitch, speed);
            AL.Source(srcID, ALSourcei.SourceType, 4136 /*ALSourceType.Static*/);

            EntityPlayer p = GameInstance.get.thePlayer;
            setGainBasedOnDistance(p.getEyePosition());
            setPanBasedOnAngle(p.getEyePosition(), p.getRightVector());
            AL.SourcePlay(srcID);
            isPositional = true;
        }

        public void onTick(EntityPlayer listener, long currentMills)
        {
            if(!loopingSound)
            { 
                finishedPlaying = (currentMills - timeStampMills) > (soundLengthMills + extraPlayMills);
            }

            if(isPositional && !finishedPlaying)
            {
                setGainBasedOnDistance(listener.getEyePosition());
                setPanBasedOnAngle(listener.getEyePosition(), listener.getRightVector());
            }
        }

        private void setGainBasedOnDistance(Vector3 lPos)
        {
            float gain = 0;
            float dst = Vector3.Distance(sndPos, lPos);

            if(dst >= maxDist)
            {
                gain = 0.0F;
            }
            else
            {
                dst = 1.0F - dst / maxDist;
                gain = dst * dst;
                //Gain should only be between 1 and 0. 
                gain = MathHelper.Clamp(gain, 0, 1);
                gain *= volume;
            }

            AL.Source(srcID, ALSourcef.Gain, gain * GameSettings.masterVolume);
        }


        private void setPanBasedOnAngle(Vector3 lPos, Vector3 lRight)
        {
            Vector3 to = Vector3.Normalize(sndPos - lPos);
            float dot = Vector3.Dot(lRight, to);
            dot *= 0.5F;
            AL.Source(srcID, ALSource3f.Position, dot, 0, -1);
        }

        public void stopPlaying()
        {
            AL.SourceStop(srcID);
            AL.Source(srcID, ALSourcei.Buffer, 0);
        }
    }
}
