using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RabbetGameEngine
{
    public class ControlBinding
    {
        public bool isMouseButton = false;

        /// <summary>
        /// integer value of this bindings enum. Either Keys or MouseButton
        /// </summary>
        public int code = 0;

        public EntityAction act;

        public ControlBinding(EntityAction act, Keys defaultKeyBinding)
        {
            this.act = act;
            code = (int)defaultKeyBinding;
            isMouseButton = false;
            GameSettings.bindings.Add(this);
        }


        public ControlBinding(EntityAction act,  MouseButton defaultButtonBinding)
        {
            this.act = act;
            code = (int)defaultButtonBinding;
            isMouseButton = true;
            GameSettings.bindings.Add(this);
        }
    }
}
