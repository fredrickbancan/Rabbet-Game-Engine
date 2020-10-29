using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

namespace RabbetGameEngine.Sound
{
    //TODO: properly impliment multiple sounds and directional audio
    public static class SoundManager
    {
        private static ALContext context = ALContext.Null;
        private static ALDevice device = ALDevice.Null;
        private static int buf = 0;
        private static int src = 0;
        private static bool initialized = false;
        public static void init()
        {
            device = ALC.OpenDevice(null);
            context = ALC.CreateContext(device, new int[] { });
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
            }

        }

        public static void setListener(EntityPlayer player)
        {
            if (!initialized) return;
            AL.Listener(ALListener3f.Position,ref player.getPositionHandle());
            AL.Listener(ALListener3f.Velocity,ref player.getPositionHandle());
        }

        public static void playSoundAux(string soundName, float volume = 1.0F, float speed = 1.0F)
        {
            if (!initialized) return;
            Application.debugPrint("playing sound: " + soundName);
            Sound snd = SoundUtil.getSound(soundName);
           
            buf = AL.GenBuffer();
            src = AL.GenSource();
            AL.BufferData(buf, snd.isStereo() ? ALFormat.Stereo16 : ALFormat.Mono16, snd.getData(), snd.getSampleRate());
            AL.Source(src, ALSourcei.Buffer, buf);
            AL.Source(src, ALSourceb.Looping, false);
            AL.Source(src, ALSourcef.Gain, volume);
            AL.Source(src, ALSourcef.Pitch, speed);
            AL.SourcePlay(src);
        }

        public static void playSoundAt(string soundName, Vector3 pos, float volume = 1.0F, float speed = 1.0F)
        {
            if (!initialized) return;

        }

        public static void onFrame()
        {
            if (!initialized) return;

        }
        public static void onTick()
        {
            if (!initialized) return;

        }

        public static void onClosing()
        {
            if (!initialized) return;
            AL.DeleteSource(src);
            if(AL.IsBuffer(buf))
            AL.DeleteBuffer(buf);
            if(context != ALContext.Null)
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
