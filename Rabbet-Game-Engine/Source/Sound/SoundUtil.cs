using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RabbetGameEngine
{
    public static class SoundUtil
    {
        private static Dictionary<string, List<Sound>> allSounds;
        public static readonly string fileExtension = ".ogg";
        public static void loadAllFoundSoundFiles()
        {
            allSounds = new Dictionary<string, List<Sound>>();
            allSounds.Add("debug", new List<Sound>());
            allSounds.ElementAt(0).Value.Add(new Sound("debug"));
            loadAllSoundsRecursive(ResourceUtil.getSoundFileDir());

        }
        private static void loadAllSoundsRecursive(string directory)
        {
            try
            {
                string[] allFiles = Directory.GetFiles(directory);
                string[] allDirectories = Directory.GetDirectories(directory);
                foreach (string file in allFiles)
                {
                    if (file.Contains(fileExtension))
                    {
                        tryAddNewSound(file);
                    }
                }

                foreach (string dir in allDirectories)
                {
                    loadAllSoundsRecursive(dir);
                }
            }
            catch(Exception e)
            {
                Application.error(e.Message);
            }
        }

        private static void tryAddNewSound(string soundDir)
        {
            string soundName = Path.GetFileName(soundDir).Replace(fileExtension, "");//removes directory
            if(soundName.Contains(".random"))
            {
                string splitName = soundName.Split(".random")[0];
                if(allSounds.TryGetValue(splitName, out List<Sound> value))
                {
                    value.Add(new Sound(soundDir));
                }
                else
                {
                    allSounds.Add(splitName, new List<Sound>(new Sound[] { new Sound(soundDir) }));
                }
            }
            else
            {
                allSounds.Add(soundName, new List<Sound>(new Sound[] { new Sound(soundDir) }));
            }
        }

        public static bool tryGetSound(string name, out Sound snd)
        {
            List<Sound> soundCollection = null;
            if(!allSounds.TryGetValue(name, out soundCollection))
            {
                Application.warn("Could not find sound for requested sound name: " + name + " , Assigning debug sound.");
                snd = allSounds.ElementAt(0).Value[0];//assign debug sound
                return false;
            }

            if(soundCollection.Count > 1)
            {
                snd =  soundCollection[GameInstance.rand.Next(0, soundCollection.Count)];
            }
            else
            {
                snd = soundCollection[0];
            }

            return true;
        }

        public static Sound getSound(string name)
        {
            Sound res = null;
            tryGetSound(name, out res);
            return res;
        }

        public static int getSoundFileCount()
        {
            return allSounds.Count;
        }

        public static void deleteAll()
        {
            Application.infoPrint("SoundUtil deleting " + allSounds.Count + " loaded sound files...");
            foreach (List<Sound> sc in allSounds.Values)
            {
                foreach(Sound s in sc)
                {
                    s.delete();
                }
            }
            Application.infoPrint("Done");
        }
    }
}
