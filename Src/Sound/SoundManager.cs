using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;
namespace RabbetGameEngine.Sound
{
    //TODO: set up automated random playing of sound files with number suffixes
    public static class SoundManager
    {
        private static ALContext context;
        static int buf = 0;
        static int src = 0;
        public static void init()
        {
            //GameInstance.get.
          //  ALC.GetCurrentContext();
           /* if(!ALC.MakeContextCurrent(context))
            {
                Application.warn("Could not instantiate OpenAL context!");
            }*/
           /* Application.infoPrint("Created audio context for device: ");
            Application.infoPrint("Open AL version: " + AL.Get(ALGetString.Version));
            Application.infoPrint("Open AL Vendor: " + AL.Get(ALGetString.Vendor));
            Application.infoPrint("Open AL Renderer: " + AL.Get(ALGetString.Renderer));
            Application.infoPrint("Open AL Extensions: " + AL.Get(ALGetString.Extensions));
            SoundUtil.loadAllFoundSoundFiles();
            Application.infoPrint("Loaded " + SoundUtil.getSoundFileCount() + " sound files.");*/
       //     buf = AL.GenBuffer();
    //        src = AL.GenSource();

        }

        public static void setListener(EntityPlayer player)
        {
           // AL.Listener(ALListener3f.Position, ref player.getPositionHandle());
            //AL.Listener(ALListener3f.Velocity, ref player.getVelocityHandle());
        }

        public static void playSoundAux(string soundName, float volume = 1.0F, float speed = 1.0F)
        {
          //  int source;
          //  int buffers;
          /*  AL.GenBuffers(1, out buffers);
            AL.GenSources(1, out source);
            Sound snd = SoundUtil.getSound(soundName);

            AL.BufferData(buffers, ALFormat.Mono16, snd.getData(), snd.getData().Length, snd.getSampleRate());
            AL.Source(source, ALSourcei.Buffer, buffers);
            AL.Source(source, ALSourceb.Looping, false);

            AL.SourcePlay(source);
            Thread.Sleep(2000);
*/
          //  Application.debugPrint("fin");
/*
            ///Dispose
            if (context != ContextHandle.Zero)
            {
                ALC.MakeContextCurrent(ContextHandle.Zero);
                ALC.DestroyContext(context);
            }
            context = ContextHandle.Zero;

            if (device != IntPtr.Zero)
            {
                ALC.CloseDevice(device);
            }
            device = IntPtr.Zero;*/

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
        //    AL.DeleteSource(src);
        //    AL.DeleteBuffer(buf);
           // SoundUtil.deleteAll();
            //context.Dispose();
        }
    }
}
