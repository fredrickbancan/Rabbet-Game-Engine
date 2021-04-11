using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RabbetGameEngine;
using RabbetGameEngine.Text;
using System;

namespace RabbetGameEngine
{
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
            clearHoverEnterListeners();
            addHoverEnterListener(onHoverEnter);
            addClickListener(onClick);
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
        public void onHoverEnter()
        {
            if (!bindPopupEnabled)
            {
                GUIUtil.defaultOnButtonHoverEnter();
            }
        }
        public void onClick(GUIButton b)
        {
            if (!bindPopupEnabled)
            {
                bindPopupEnabled = true;
                parentGUI.pauseAllExcept(this);
                Input.pause();
            }
        }

        public override void onFrame()
        {
            base.onFrame();
            if(bindPopupEnabled)
            {
                isHovered = false;
                Input.pause();
            }
            else
            {
                Input.unPause();
                parentGUI.unPauseAll();
            }
        }

        public override void onKeyDown(KeyboardKeyEventArgs e)
        {
            base.onKeyDown(e);
            if (bindPopupEnabled)
            {
                if(e.Key == Keys.Escape)
                {
                    bindPopupEnabled = false;
                }
                else if(e.Key < Keys.F1 || e.Key > Keys.F25)
                {
                    bindPopupEnabled = false;
                    bind.setKeyValue(e.Key);
                    updateRenderData();
                }
            }
        }
        public override void onMouseDown(MouseButtonEventArgs e)
        {
            base.onMouseDown(e);
            if (bindPopupEnabled)
            {
                bindPopupEnabled = false;
                bind.setMouseButton(e.Button);
                updateRenderData();
            }
        }

        public override void onMouseWheel(float scrolldelta)
        {
            base.onMouseWheel(scrolldelta);
            if (bindPopupEnabled)
            {
                bindPopupEnabled = false;
                Application.debugPrint(scrolldelta);
                if(scrolldelta < -0.00001F)
                {
                    bind.setScrollValue(ScrollDirection.MWDown);
                }
                else if(scrolldelta > 0.00001F)
                {
                    bind.setScrollValue(ScrollDirection.MWUp);
                }
                updateRenderData();
            }
        }

        public override void updateRenderData()
        {
            switch (bind.type)
            {
                case BindingType.MOUSEBUTTON:
                    title = Enum.GetName(bind.mButtonValue.GetType(), (int)bind.mWheelValue);
                    break;
                case BindingType.KEY:
                    title = Enum.GetName(bind.keyValue.GetType(), (int)bind.keyValue);
                    break;
                case BindingType.MWHEEL:
                    title = Enum.GetName(bind.mWheelValue.GetType(), (int)bind.mWheelValue);
                    break;
            }
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
