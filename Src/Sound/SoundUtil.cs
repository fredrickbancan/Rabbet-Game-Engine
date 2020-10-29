using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RabbetGameEngine.Sound
{
    public static class SoundUtil
    {
        private static Dictionary<string, Sound> allSounds;
        public static readonly string fileExtension = ".ogg";
        public static void loadAllFoundSoundFiles()
        {
            allSounds = new Dictionary<string, Sound>();
            allSounds.Add("debug", new Sound("debug"));
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
            allSounds.Add(soundName, new Sound(soundDir));
        }

        public static bool tryGetSound(string name, out Sound snd)
        {
            if(!allSounds.TryGetValue(name, out snd))
            {
                Application.warn("Could not find sound for requested sound name: " + name + "!");
                snd = allSounds.ElementAt(0).Value;
                return false;
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
            foreach(Sound s in allSounds.Values)
            {
                s.delete();
            }
        }
    }
}
