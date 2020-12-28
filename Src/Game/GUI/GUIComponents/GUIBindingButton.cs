using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.Text;
using System;

namespace RabbetGameEngine
{
    //TODO: implement ability to change bindings
    public class GUIBindingButton : GUIButton
    {
        public GUI parentGUI = null;
        public ControlBinding bind = null;
        public GUITransparentRectangle backGround = null;
        public Model bindTitleModel = null;
        public GUITextPanel popupText = null;
        public GUITransparentOverlay popupOverlay = null;
        public GUITransparentRectangle popupBackground = null;
        public Vector2 bindTitleOffset = new Vector2(0.0F, 0.05F);
        public string bindingTitle = "";
        public bool bindPopupEnabled = false;
        public GUIBindingButton(GUI parentGUI, ControlBinding bind, float posX, float posY, float sizeX, float sizeY, Color color, FontFace font, ComponentAnchor anchor = ComponentAnchor.CENTER, int renderLayer = 0, string textureName = "white") : base(posX, posY, sizeX, sizeY, color, "", font, anchor, renderLayer, textureName)
        {
            this.bind = bind;
            this.parentGUI = parentGUI;
            addClickListener(onClick);
            title = Enum.GetName(bind.isMouseButton ? bind.mButtonValue.GetType() : bind.keyValue.GetType(), bind.isMouseButton ? (int)bind.mButtonValue : (int)bind.keyValue);
            bindingTitle = bind.title;
            setFontSize(0.15F);
            backGround = new GUITransparentRectangle(posX, posY + 0.015F, sizeX + 0.075F, sizeY + 0.05F, Color.black.setAlphaF(0.7F), ComponentAnchor.CENTER, renderLayer-1);
            popupOverlay = new GUITransparentOverlay(Color.black, 0.5F, renderLayer + 2);
            popupBackground = new GUITransparentRectangle(0, 0, 0.75F, 0.15F, Color.black.setAlphaF(0.8F), ComponentAnchor.CENTER, renderLayer + 3);
            popupText = new GUITextPanel(0, 0.01F, parentGUI.guiFont, ComponentAnchor.CENTER, renderLayer + 4);
            popupText.addLine("Press the key which you would like to use for \"" + bind.title + "\".");
            popupText.addLine("[press ESC to cancel]");
            updateRenderData();
        }

        public void onClick(GUIButton b)
        {
            bindPopupEnabled = true;
            parentGUI.pauseAllExcept(this);
        }

        public override void updateRenderData()
        {
            base.updateRenderData();
            backGround.updateRenderData();
            bindTitleModel = TextModelBuilder2D.convertStringToModel(bindingTitle, parentGUI.guiFont, Color.lightGrey.toNormalVec4(), new Vector3(screenPixelPos.X + bindTitleOffset.X * GameInstance.gameWindowHeight, screenPixelPos.Y + bindTitleOffset.Y * GameInstance.gameWindowHeight, -0.2F), 0.2F, ComponentAnchor.CENTER);
            popupOverlay.updateRenderData();
            popupBackground.updateRenderData();
            popupText.updateRenderData();
        }

        public override void requestRender()
        {
            backGround.requestRender();
            base.requestRender();
            Renderer.requestRender(RenderType.guiText, parentGUI.guiFont.texture, bindTitleModel, renderLayer + 1);

            if(bindPopupEnabled)
            {
                popupOverlay.requestRender();
                popupBackground.requestRender();
                popupText.requestRender();
            }
        }
    }
}
