using RabbetGameEngine.Text;

namespace RabbetGameEngine
{
    public class GUIBoolButton : GUIButton
    {
        public bool boolValue = false;
        public Color trueTitleColor = Color.white;
        public Color falseTitleColor = Color.lightGrey;
        public GUIBoolButton(float posX, float posY, float sizeX, float sizeY, Color color, string title, FontFace font, ComponentAnchor alignment = ComponentAnchor.CENTER_LEFT, int renderLayer = 0, string texture = "white") : base(posX, posY, sizeX, sizeY, color, title, font, alignment, renderLayer, texture)
        {

        }
    }
}
