using RabbetGameEngine.Text;

namespace RabbetGameEngine
{
    public class GUIBindingButton : GUIButton
    {
        public GUI parentGUI = null;
        public GUIBindingButton(GUI parentGUI, float posX, float posY, float sizeX, float sizeY, Color color, string title, FontFace font, ComponentAnchor anchor = ComponentAnchor.CENTER, int renderLayer = 0, string textureName = "white") : base(posX, posY, sizeX, sizeY, color, title, font, anchor, renderLayer, textureName)
        {
            this.parentGUI = parentGUI;
            //TODO: implement
        }
    }
}
