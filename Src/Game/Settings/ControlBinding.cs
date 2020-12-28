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
        public Keys keyValue = 0;
        public MouseButton mButtonValue = 0;

        public string title = "";

        public ControlBinding(EntityAction act, Keys defaultKeyBinding)
        {
            this.act = act;
            keyValue = defaultKeyBinding;
            code = (int)defaultKeyBinding;
            isMouseButton = false;
            GameSettings.bindings.Add(this);
            title = bindingsTitles[(int)act];
        }


        public ControlBinding(EntityAction act,  MouseButton defaultButtonBinding)
        {
            this.act = act;
            mButtonValue = defaultButtonBinding;
            code = (int)defaultButtonBinding;
            isMouseButton = true;
            GameSettings.bindings.Add(this);
            title = bindingsTitles[(int)act];
        }

        public ControlBinding setMouseButton(MouseButton mb)
        {
            mButtonValue = mb;
            return this;
        }

        public ControlBinding setKeyValue(Keys mb)
        {
            keyValue = mb;
            return this;
        }

        /// <summary>
        /// can be used as listener func
        /// </summary>
        public void applyBindingButtonValue(GUIBindingButton b)
        {

        }
    }
    
}
