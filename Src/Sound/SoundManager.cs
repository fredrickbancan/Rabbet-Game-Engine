using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;
using RabbetGameEngine.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbetGameEngine.Sound
{
    //TODO: Add support for sounds from moving objects
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
                ALContextAttributes c = new ALContextAttributes();
                c.MonoSources = 255;
                c.StereoSources = 127;
                context = ALC.CreateContext(device, c);
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
                sounds = new List<PlayingSound>();
                AL.DistanceModel(ALDistanceModel.None);
                initialized = true;
            }

        }

        public static void playSound(string soundName, float volume = 1.0F, float speed = 1.0F)
        {
            if (!initialized) return;
            Profiler.beginEndProfile("sounds");
            Sound snd = SoundUtil.getSound(soundName);
            sounds.Add(new PlayingSound(snd, volume, speed, TicksAndFrames.getRealTimeMills()));
            Profiler.beginEndProfile("sounds");
        }

        public static void playSoundAt(string soundName, Vector3 pos, float volume = 1.0F, float speed = 1.0F)
        {
            if (!initialized) return;
            Profiler.beginEndProfile("sounds");
            Sound snd = SoundUtil.getSound(soundName);
            if (snd.isStereo()) { Profiler.beginEndProfile("sounds"); return; }
            sounds.Add(new PlayingSound(snd, volume, speed, pos, TicksAndFrames.getRealTimeMills()));
            Profiler.beginEndProfile("sounds");
        }

        public static void playSoundLooping(string soundName, float volume = 1.0F, float speed = 1.0F)
        {
            if (!initialized) return;
            Profiler.beginEndProfile("sounds");
            Sound snd = SoundUtil.getSound(soundName);
            sounds.Add(new PlayingSound(soundName, snd, volume, speed));
            Profiler.beginEndProfile("sounds");
        }

        public static void playSoundLoopingAt(string soundName, Vector3 pos, float volume = 1.0F, float speed = 1.0F)
        {
            if (!initialized) return;
            Profiler.beginEndProfile("sounds");
            Sound snd = SoundUtil.getSound(soundName);
            if (snd.isStereo()) { Profiler.beginEndProfile("sounds"); return; }
            sounds.Add(new PlayingSound(soundName, snd, volume, speed, pos));
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
            EntityPlayer p = GameInstance.get.thePlayer;

            for (int i = 0; i < sounds.Count; i++)
            {
                PlayingSound s = sounds.ElementAt(i);

                s.onTick(p, ms);

                if (s.finishedPlaying)
                {
                    s.delete();
                    sounds.RemoveAt(i);
                    i--;
                }
            }
            Profiler.beginEndProfile("sounds");
        }

        public static void stopPlayingSoundLooping(string soundName)
        {
            Profiler.beginEndProfile("sounds");
            foreach (PlayingSound s in sounds)
            {
                if(s.loopingSound && !s.isPositional)
                {
                    if(s.loopingSoundName == soundName)
                    {
                        s.finishedPlaying = true;
                    }
                }
            }
            Profiler.beginEndProfile("sounds");
        }

        public static void stopPlayingSoundLoopingAt(string soundName, Vector3 pos)
        {
            Profiler.beginEndProfile("sounds");
            foreach (PlayingSound s in sounds)
            {
                if (s.loopingSound && s.isPositional)
                {
                    if (s.loopingSoundName == soundName && s.sndPos.Equals(pos))
                    {
                        s.finishedPlaying = true;
                    }
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
