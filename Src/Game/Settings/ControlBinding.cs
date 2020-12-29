using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RabbetGameEngine
{
    public enum ScrollDirection
    {
        MWUp,
        MWDown
    };
    public enum BindingType
    {
        MOUSEBUTTON,
        KEY,
        MWHEEL
    };

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


        /// <summary>
        /// integer value of this bindings enum. Either Keys or MouseButton
        /// </summary>
        public int code = 0;

        public EntityAction act;
        public Keys keyValue = 0;
        public MouseButton mButtonValue = 0;
        public BindingType type;
        public ScrollDirection mWheelValue;
        public string title = "";

        public ControlBinding(EntityAction act, Keys defaultKeyBinding)
        {
            this.act = act;
            keyValue = defaultKeyBinding;
            code = (int)defaultKeyBinding;
            type = BindingType.KEY;
            GameSettings.bindings.Add(this);
            title = bindingsTitles[(int)act];
        }


        public ControlBinding(EntityAction act,  MouseButton defaultButtonBinding)
        {
            this.act = act;
            mButtonValue = defaultButtonBinding;
            code = (int)defaultButtonBinding;
            type = BindingType.MOUSEBUTTON;
            GameSettings.bindings.Add(this);
            title = bindingsTitles[(int)act];
        }

        public ControlBinding setMouseButton(MouseButton mb)
        {
            type = BindingType.MOUSEBUTTON;
            mButtonValue = mb;
            code = (int)mb;
            return this;
        }

        public ControlBinding setKeyValue(Keys mb)
        {
            type = BindingType.KEY;
            keyValue = mb;
            code = (int)mb;
            return this;
        }
        public ControlBinding setScrollValue(ScrollDirection sd)
        {
            type = BindingType.MWHEEL;
            this.mWheelValue = sd;
            code = (int)sd;
            return this;
        }
    }
    
}
