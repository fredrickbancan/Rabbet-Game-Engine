using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System;
using System.Threading;

namespace RabbetGameEngine.Sound
{
    //TODO: set up automated random playing of sound files with number suffixes
    public static class SoundManager
    {
        private static AudioContext context;
        static int buf = 0;
        static int src = 0;
        public static void init()
        {
            try
            {
                context = new AudioContext(System.Environment.MachineName);
                context.MakeCurrent();
                Application.infoPrint("Created audio context for device: " + context.CurrentDevice);
                Application.infoPrint("Open AL version: " + AL.Get(ALGetString.Version));
                Application.infoPrint("Open AL Vendor: " + AL.Get(ALGetString.Vendor));
                Application.infoPrint("Open AL Renderer: " + AL.Get(ALGetString.Renderer));
                Application.infoPrint("Open AL Extensions: " + AL.Get(ALGetString.Extensions));
                SoundUtil.loadAllFoundSoundFiles();
                Application.infoPrint("Loaded " + SoundUtil.getSoundFileCount() + " sound files.");
                buf = AL.GenBuffer();
                src = AL.GenSource();
            }
            catch(AudioContextException e)
            {
                Application.error(e.Message);
            }
        }

        public static void setListener(EntityPlayer player)
        {
            AL.Listener(ALListener3f.Position, ref player.getPositionHandle());
            AL.Listener(ALListener3f.Velocity, ref player.getVelocityHandle());
        }

        public static unsafe void playSoundAux(string soundName, float volume = 1.0F, float speed = 1.0F)
        {
            SoundManager.context.Dispose();
            //Initialize
            IntPtr device = Alc.OpenDevice(null);
            var context = Alc.CreateContext(device, (int*)null);

            Alc.MakeContextCurrent(context);
            var version = AL.Get(ALGetString.Version);
            var vendor = AL.Get(ALGetString.Vendor);
            var renderer = AL.Get(ALGetString.Renderer);
            Application.infoPrint("Open AL Extensions: " + AL.Get(ALGetString.Extensions));
            Application.debugPrint(version);
            Application.debugPrint(vendor);
            Application.debugPrint(renderer);

            //Process

            int sampleFreq = 44100;
            double dt = 2 * Math.PI / sampleFreq;
            var dataCount = 100;
            double amp = 0.5;


            int source;
            int buffers;
            AL.GenBuffers(1, out buffers);
            AL.GenSources(1, out source);
            Sound snd = SoundUtil.getSound(soundName);

            AL.BufferData(buffers, ALFormat.Mono16, snd.getData(), snd.getData().Length, snd.getSampleRate());
            AL.Source(source, ALSourcei.Buffer, buffers);
            AL.Source(source, ALSourceb.Looping, false);

            AL.SourcePlay(source);
            Thread.Sleep(2000);

            Application.debugPrint("fin");

            ///Dispose
            if (context != ContextHandle.Zero)
            {
                Alc.MakeContextCurrent(ContextHandle.Zero);
                Alc.DestroyContext(context);
            }
            context = ContextHandle.Zero;

            if (device != IntPtr.Zero)
            {
                Alc.CloseDevice(device);
            }
            device = IntPtr.Zero;

        }

        public static void playSoundAt(string soundName, Vector3 pos, float volume = 1.0F, float speed = 1.0F)
        {
           

        }

        public static void onFrame()
        {
           // context.Process();
        }

        public static void onClosing()
        {
            AL.DeleteSource(src);
            AL.DeleteBuffer(buf);
            SoundUtil.deleteAll();
            //context.Dispose();
        }
    }
}
