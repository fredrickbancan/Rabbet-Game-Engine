using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RabbetGameEngine
{
    public class ControlBinding
    {
        public static readonly string[] bindingsTitles = new string[]
           {
            "none",
            "Walk Fowards",
            "Strafe Left",
            "Walk Backwards",
            "Strafe Right",
            "Jump",
            "Attack",
            "Duck",
            "Sprint",
            "Interact"
           };

        public bool isMouseButton = false;

        /// <summary>
        /// integer value of this bindings enum. Either Keys or MouseButton
        /// </summary>
        public int code = 0;

        public EntityAction act;

        public string title = "";

        public ControlBinding(EntityAction act, Keys defaultKeyBinding)
        {
            this.act = act;
            code = (int)act;
            isMouseButton = false;
            GameSettings.bindings.Add(this);
            title = bindingsTitles[code];
        }


        public ControlBinding(EntityAction act,  MouseButton defaultButtonBinding)
        {
            this.act = act;
            code = (int)act;
            isMouseButton = true;
            GameSettings.bindings.Add(this);
            title = bindingsTitles[code];
        }
    }
    
}
