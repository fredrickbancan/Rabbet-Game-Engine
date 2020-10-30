using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;
using RabbetGameEngine.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbetGameEngine.Sound
{
    //TODO: Add support for looping sounds
    public static class SoundManager
    {
        private static ALContext context = ALContext.Null;
        private static ALDevice device = ALDevice.Null;
        private static bool initialized = false;
        private static List<PlayingSound> sounds = null;
        public static void init()
        {
            try
            {
                device = ALC.OpenDevice(null);
                context = ALC.CreateContext(device, new int[] { });
            }
            catch(Exception e)
            {
                Application.warn("Could not instantiate OpenAL context! Exception: " + e.Message);
                return;
            }

            if (!ALC.MakeContextCurrent(context) || context == ALContext.Null || device == ALDevice.Null)
            {
                Application.warn("Could not instantiate OpenAL context!\nDevice null: " + (device == ALDevice.Null) + "\nContext null: " + (context == ALContext.Null) + (device != ALDevice.Null ? ("\nAL Device error: " + ALC.GetError(device)) : ""));
            }
            else
            {
                Application.infoPrint("Open AL version: " + AL.Get(ALGetString.Version));
                Application.infoPrint("Open AL Vendor: " + AL.Get(ALGetString.Vendor));
                Application.infoPrint("Open AL Renderer: " + AL.Get(ALGetString.Renderer));
                Application.infoPrint("Open AL Extensions: " + AL.Get(ALGetString.Extensions));
                SoundUtil.loadAllFoundSoundFiles();
                Application.infoPrint("Loaded " + SoundUtil.getSoundFileCount() + " sound files.");
                initialized = true;
                sounds = new List<PlayingSound>();
            }

        }

        public static void setListenerInfo(EntityPlayer player)
        {
            if (!initialized) return;
            AL.Listener(ALListener3f.Position, ref player.getPositionHandle());
            AL.Listener(ALListener3f.Velocity, ref player.getVelocityHandle());
            AL.Listener(ALListenerfv.Orientation, ref player.getFrontVectorHandle(), ref player.getUpVectorHandle()) ;
        }

        public static void playSoundAux(string soundName, float volume = 1.0F, float speed = 1.0F)
        {
            if (!initialized) return;
            Profiler.beginEndProfile("sounds");
            Sound snd = SoundUtil.getSound(soundName);
            int buf = AL.GenBuffer();
            int src = AL.GenSource();
            sounds.Add(new PlayingSound(buf, src, snd, TicksAndFrames.getRealTimeMills()));
            AL.BufferData(buf, snd.isStereo() ? ALFormat.Stereo16 : ALFormat.Mono16, snd.getData(), snd.getSampleRate());
            AL.Source(src, ALSourcei.Buffer, buf);
            AL.Source(src, ALSourceb.SourceRelative, true);
            AL.Source(src, ALSourcef.Gain, volume);
            AL.Source(src, ALSourcef.Pitch, speed);
            AL.SourcePlay(src);
            Profiler.beginEndProfile("sounds");
        }

        public static void playSoundAt(string soundName, ref Vector3 pos, float volume = 1.0F, float speed = 1.0F)
        {
            if (!initialized) return;
            Profiler.beginEndProfile("sounds");
            Sound snd = SoundUtil.getSound(soundName);
            if (snd.isStereo()) { Profiler.beginEndProfile("sounds"); return; }
            int buf = AL.GenBuffer();
            int src = AL.GenSource();
            sounds.Add(new PlayingSound(buf, src, snd, TicksAndFrames.getRealTimeMills()));
            AL.BufferData(buf, ALFormat.Mono16, snd.getData(), snd.getSampleRate());
            AL.Source(src, ALSourcei.Buffer, buf);
            AL.Source(src, ALSourcef.Pitch, speed);
            AL.Source(src, ALSourcef.MaxGain, volume);
            AL.Source(src, ALSourcef.MaxDistance, volume * 32.0F);
            AL.Source(src, ALSourcef.RolloffFactor, 0.5F);
            AL.Source(src, ALSource3f.Position, ref pos);
            AL.SourcePlay(src);
            Profiler.beginEndProfile("sounds");
        }

        public static void onFrame()
        {
            if (!initialized) return;

        }
        public static void onTick()
        {
            if (!initialized) return;
            Profiler.beginEndProfile("sounds");
            long ms = TicksAndFrames.getRealTimeMills();
            for (int i = 0; i < sounds.Count; i++)
            {
                PlayingSound s = sounds.ElementAt(i);
                s.onTick(ms);
                if (s.finishedPlaying)
                {
                    s.delete();
                    sounds.RemoveAt(i);
                    i--;
                }
            }
            Profiler.beginEndProfile("sounds");
        }

        public static int getPlayingSoundsCount()
        {
            if(!initialized)return 0;
            return sounds.Count;
        }

        public static void onClosing()
        {
            if (!initialized) return;
            foreach (PlayingSound s in sounds)
            {
                s.delete();
            }
            if (context != ALContext.Null)
            {
                ALC.MakeContextCurrent(ALContext.Null);
                ALC.DestroyContext(context);
                context = ALContext.Null;
            }

            if(device != ALDevice.Null)
            {
                ALC.CloseDevice(device);
                device = ALDevice.Null;
            }

            SoundUtil.deleteAll();
        }
    }
}
