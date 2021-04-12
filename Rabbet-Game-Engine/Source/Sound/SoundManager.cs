using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbetGameEngine
{
    //TODO: Implement openal in a way where theres no complications with .dll files
    //TODO: Add support for sounds from moving objects
    //TODO: Add support for Looping sounds from moving objects
    public static class SoundManager
    {
        private static ALContext context = ALContext.Null;
        private static ALDevice device = ALDevice.Null;
        private static bool initialized = false;
        private static List<PlayingSound> sounds = null;
        private static List<int> freeSourceIDs;
        private static List<int> busySourceIDs;
        private static List<int> usedSourceIDs;

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
                Application.warn("Could not instantiate OpenAL context! Exception: " + e.Message + "\nSound will not be working.");
                return;
            }

            if (!ALC.MakeContextCurrent(context) || context == ALContext.Null || device == ALDevice.Null)
            {
                Application.warn("Could not instantiate OpenAL context!\nDevice null: " + (device == ALDevice.Null) + "\nContext null: " + (context == ALContext.Null) + (device != ALDevice.Null ? ("\nAL Device error: " + ALC.GetError(device)) : "") + "\nSound will not be working.");
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
                freeSourceIDs = new List<int>();
                busySourceIDs = new List<int>();
                usedSourceIDs = new List<int>();
                AL.DistanceModel(ALDistanceModel.LinearDistance);
                initialized = true;
            }

        }

        public static void playSound(string soundName, float volume = 1.0F, float speed = 1.0F)
        {
            if (!initialized) return;
            Sound snd = SoundUtil.getSound(soundName);
            sounds.Add(new PlayingSound(snd, volume, speed, TicksAndFrames.getRealTimeMills()));
        }

        public static void playSoundAt(string soundName, Vector3 pos, float volume = 1.0F, float speed = 1.0F)
        {
            if (!initialized) return;
            Sound snd = SoundUtil.getSound(soundName);
            if (snd.isStereo())  return; 
            sounds.Add(new PlayingSound(snd, volume, speed, pos, TicksAndFrames.getRealTimeMills()));
        }

        public static void playSoundLooping(string soundName, float volume = 1.0F, float speed = 1.0F)
        {
            if (!initialized) return;
            Sound snd = SoundUtil.getSound(soundName);
            sounds.Add(new PlayingSound(soundName, snd, volume, speed));
        }

        public static void playSoundLoopingAt(string soundName, Vector3 pos, float volume = 1.0F, float speed = 1.0F)
        {
            if (!initialized) return;
            Sound snd = SoundUtil.getSound(soundName);
            if (snd.isStereo()) return; 
            sounds.Add(new PlayingSound(soundName, snd, volume, speed, pos));
        }

        public static void onFrame()
        {
            if (!initialized) return;
          /*  EntityPlayer p = GameInstance.get.thePlayer;
            for (int i = 0; i < sounds.Count; i++)
            {
                PlayingSound s = sounds.ElementAt(i);

                s.onFrame(p);
            }*/
        }

        public static void onUpdate()
        {
            if (!initialized) return;
            long ms = TicksAndFrames.getRealTimeMills();

            for (int i = 0; i < sounds.Count; i++)
            {
                PlayingSound s = sounds.ElementAt(i);

                s.onTick(ms);

                if (s.finishedPlaying)
                {
                    s.stopPlaying();
                    freeSourceID(s.srcID);
                    sounds.RemoveAt(i);
                    i--;
                }
            }
        }

        public static void stopPlayingSoundLooping(string soundName)
        {
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
        }

        public static void stopPlayingSoundLoopingAt(string soundName, Vector3 pos)
        {
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
        }

        public static int getPlayingSoundsCount()
        {
            if(!initialized)return 0;
            return sounds.Count;
        }
        public static int getSourceID()
        {
            int id;
            if (freeSourceIDs.Count > 0)
            {
                id = freeSourceIDs.ElementAt(freeSourceIDs.Count - 1);
                freeSourceIDs.RemoveAt(freeSourceIDs.Count - 1);
                busySourceIDs.Add(id);
                return id;
            }
            id = AL.GenSource();
            busySourceIDs.Add(id);
            usedSourceIDs.Add(id);
            return id;
        }

        public static void freeSourceID(int id)
        {
            if (busySourceIDs.Remove(id))
            {
                freeSourceIDs.Add(id);
            }
        }

        public static void onClosing()
        {
            if (!initialized) return;

            Application.infoPrint("SoundManager stopping all sounds...");
            foreach (PlayingSound s in sounds)
            {
                s.stopPlaying();
            }
            Application.infoPrint("Done");

            Application.infoPrint("SoundManager deleting " + usedSourceIDs.Count + " used source IDs...");
            foreach (int id in usedSourceIDs)
            {
                AL.DeleteSource(id);
            }
            Application.infoPrint("Done");

            Application.infoPrint("SoundManager destroying context...");
            if (context != ALContext.Null)
            {
                ALC.MakeContextCurrent(ALContext.Null);
                ALC.DestroyContext(context);
                context = ALContext.Null;
            }
            Application.infoPrint("Done");

            Application.infoPrint("SoundManager closing device...");
            if (device != ALDevice.Null)
            {
                ALC.CloseDevice(device);
                device = ALDevice.Null;
            }
            Application.infoPrint("Done");

            SoundUtil.deleteAll();
        }
    }
}
