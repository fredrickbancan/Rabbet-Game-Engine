using OpenTK.Mathematics;


using System;
using System.Collections.Generic;

namespace RabbetGameEngine
{
    public class GUIValueSlider : GUIComponent
    {
        private List<System.Action<GUIValueSlider>> slideMoveListeners = new List<System.Action<GUIValueSlider>>();
        public bool sliderGrabbed = false;
        public bool isInteger;
        public float minFloat, maxFloat;
        public int minInt, maxInt;

        public float sliderPos = 0.5F;
        public string title = "";
        private string minDisplayValString = "";
        private string maxDisplayValString = "";
        private int minDisplayValInt, maxDisplayValInt;
        private float minDisplayValFloat, maxDisplayValFloat;
        private string currentValString = "";
        private Vector4 titleColor = Color.white.toNormalVec4();
        private Vector4 minValColor = Color.grey.toNormalVec4();
        private Vector4 maxValColor = Color.grey.toNormalVec4();
        private Vector4 currentValColor = Color.lightSkyBlue.toNormalVec4();
        private Model titleTextModel;
        private Model minValTextModel;
        private Model maxValTextModel;
        private Model currentValTextModel;
        private FontFace font;

        /// <summary>
        /// pos of the slider knob relative to the size and pos of this component
        /// </summary>
        private Vector2 sliderKnobPos;
        private Vector2 sliderKnobSize = new Vector2(0.05F, 0.9F);
        private Vector2 middleLineRectSize = new Vector2(1.0F, 0.1F);
        private Color middleLineRectColor = Color.darkGrey;
        private Color endRectColor = Color.darkGrey;
        /// <summary>
        /// text offsets are relative to the pos of the slider as a percentage of WINDOW size.
        /// </summary>
        private Vector2 titleOffset = new Vector2(0.0F, -0.055F);
        /// <summary>
        /// min/max text offsets are relative to the end/start of the slider as a percentage of WINDOW size.
        /// </summary>
        private Vector2 minValTextOffset = new Vector2(-0.0125F, 0.0F);
        private Vector2 maxValTextOffset = new Vector2(0.0125F, 0.0F);
        /// <summary>
        /// text offsets are relative to the pos of the slider as a percentage of WINDOW size.
        /// </summary>
        private Vector2 currentValTextOffset = new Vector2(0.0F, -0.03F);
        private float maxTextFontSize = 0.15F;
        private float minTextFontSize = 0.15F;
        private float currentTextFontSize = 0.2F;
        private float titleFontSize = 0.2F;

        private GUIButton sliderKnob;
        private GUITransparentRectangle middleLineRect;
        private GUITransparentRectangle leftEndRect;
        private GUITransparentRectangle rightEndRect;
        private GUITransparentRectangle background;
        private GUI parentGUI;

        public GUIValueSlider(GUI parentGUI, float posX, float posY, float sizeX, float sizeY, string title, FontFace font, bool isInteger = false, ComponentAnchor anchor = ComponentAnchor.CENTER, int renderLayer = 0) : base(posX, posY, renderLayer)
        {
            this.parentGUI = parentGUI;
            this.font = font;
            this.title = title;
            this.isInteger = isInteger;
            this.anchor = anchor;
            background = new GUITransparentRectangle(posX, posY, sizeX, sizeY, Color.black.setAlphaF(0.7F), anchor, renderLayer - 1);
            sliderKnob = new GUIButton(posX, posY, 0, 0, Color.grey, null, null, anchor, renderLayer).setHoverColor(Color.grey).clearHoverEnterListeners().clearClickListeners();
            middleLineRect = new GUITransparentRectangle(posX, posY, middleLineRectSize.X * sizeX, middleLineRectSize.Y * sizeY, middleLineRectColor, anchor, renderLayer - 1);
            Vector2 endRectSize = new Vector2(sliderKnobSize.X * 0.5F, sliderKnobSize.Y);
            leftEndRect = new GUITransparentRectangle(posX - sizeX * 0.5F, posY, endRectSize.X * sizeX, endRectSize.Y * sizeY, endRectColor, anchor, renderLayer - 1);
            rightEndRect = new GUITransparentRectangle(posX + sizeX * 0.5F, posY, endRectSize.X * sizeX, endRectSize.Y * sizeY, endRectColor, anchor, renderLayer - 1);
            setSize(sizeX, sizeY, true);
        }
        public override void updateRenderData()
        {
            base.updateRenderData();
            updateSlider();
            middleLineRect.updateRenderData();
            leftEndRect.updateRenderData();
            rightEndRect.updateRenderData();
            background.updateRenderData();
            titleTextModel = TextModelBuilder2D.convertStringToModel(title, font, titleColor, new Vector3(screenPixelPos.X + titleOffset.X * GameInstance.gameWindowWidth, screenPixelPos.Y - titleOffset.Y * GameInstance.gameWindowHeight, -0.2F), titleFontSize, ComponentAnchor.CENTER);
            minValTextModel = TextModelBuilder2D.convertStringToModel(minDisplayValString, font, minValColor, new Vector3(screenPixelPos.X - screenPixelSize.X*0.5F + minValTextOffset.X * GameInstance.gameWindowWidth, screenPixelPos.Y - minValTextOffset.Y * GameInstance.gameWindowHeight, -0.2F), minTextFontSize, ComponentAnchor.CENTER);
            maxValTextModel = TextModelBuilder2D.convertStringToModel(maxDisplayValString, font, maxValColor, new Vector3(screenPixelPos.X + screenPixelSize.X*0.5F + maxValTextOffset.X * GameInstance.gameWindowWidth, screenPixelPos.Y - maxValTextOffset.Y * GameInstance.gameWindowHeight, -0.2F), maxTextFontSize, ComponentAnchor.CENTER);
            updateDisplayedCurrentVal();
        }

        private void updateDisplayedCurrentVal()
        {
            currentValString = (isInteger ? ((int)((float)minDisplayValInt + ((float)maxDisplayValInt - (float)minDisplayValInt) * sliderPos)).ToString() : (minDisplayValFloat + (maxDisplayValFloat - minDisplayValFloat) * sliderPos).ToString("0.0"));
            currentValTextModel = TextModelBuilder2D.convertStringToModel(currentValString, font, currentValColor, new Vector3(screenPixelPos.X + currentValTextOffset.X * GameInstance.gameWindowWidth, screenPixelPos.Y - currentValTextOffset.Y * GameInstance.gameWindowHeight, -0.2F), currentTextFontSize, ComponentAnchor.CENTER);
        }
        public override void setSize(float width, float height, bool dpiRelative = true)
        {
            base.setSize(width, height, dpiRelative);
            sliderKnob.setSize(sliderKnobSize.X * size.X, sliderKnobSize.Y * size.Y, true);
        }

        private void updateSlider()
        {
            sliderKnobPos.X = (screenPos.X - size.X * 0.5F) + (size.X * sliderPos);
            sliderKnobPos.Y = screenPos.Y;
            sliderKnob.setPos(sliderKnobPos);
            sliderKnob.updateRenderData();
        }

        public override void onFrame()
        {
            base.onFrame();
            float actuateEpsilon = 0.0001F;
            float prevSliderPos = sliderPos;
            sliderKnob.onUpdate();

            if(sliderKnob.isHovered)
            {
                sliderKnob.isHovered = false;
                if(Input.mouseleftButtonDown())
                {
                    if(!sliderGrabbed)
                    {
                        onSliderKnobGrab();
                    }
                    sliderGrabbed = true;
                }
            }

            bool p = sliderGrabbed;
            if(p)
            sliderGrabbed = Input.mouseleftButtonDown();
            if (p && !sliderGrabbed) onSliderKnobRelease();

            if (sliderGrabbed)
            {
                float newPos = (GameInstance.get.MouseState.Position.X - GameInstance.gameWindowWidth * 0.5F - screenPixelPos.X) / screenPixelSize.X + 0.5F;
                int section = (int)(newPos * 20.0F);
                bool sound = section != (int)(sliderPos * 20.0F);
                float s = MathF.Abs(newPos - prevSliderPos);
                if (s >= actuateEpsilon)
                {
                    if(newPos > 1.0F)
                    {
                        newPos = 1.0F;
                        sliderPos = newPos;
                        updateSlider();
                        if (prevSliderPos < 1.0F) onSlideMoved(sound);
                    }
                    else if(newPos < 0.0F)
                    {
                        newPos = 0.0F;
                        sliderPos = newPos;
                        updateSlider();
                        if (prevSliderPos > 0.0F) onSlideMoved(sound);
                    }
                    else
                    {
                        sliderPos = newPos;
                        updateSlider();
                        onSlideMoved(sound);
                    }
                }
            }
        }

        private void onSliderKnobGrab()
        {
            sliderKnob.setColor(Color.darkGrey);
            SoundManager.playSound("buttonhover");
        }
        private void onSliderKnobRelease()
        {
            sliderKnob.setColor(Color.grey);
            SoundManager.playSound("buttonclick");
        }

        public GUIValueSlider setSliderPos(float p)
        {
            sliderPos = p;
            return this;
        }

        public GUIValueSlider setRange(float min, float max)
        {
            minFloat = min;
            maxFloat = max;
            return this;
        }

        public GUIValueSlider setRange(int min, int max)
        {
            minInt = min;
            maxInt = max;
            return this;
        }
        public GUIValueSlider setDisplayRange(float min, float max)
        {
            minDisplayValFloat = min;
            minDisplayValString = ((int)min).ToString();
            maxDisplayValFloat = max;
            maxDisplayValString = ((int)max).ToString();
            return this;
        }

        public GUIValueSlider setDisplayRange(int min, int max)
        {
            minDisplayValInt= min;
            minDisplayValString = min.ToString();
            maxDisplayValInt = max;
            maxDisplayValString = max.ToString();
            return this;
        }
        public GUIValueSlider setTitleFontSize(float s)
        {
            titleFontSize = s;
            titleTextModel = TextModelBuilder2D.convertStringToModel(title, font, Color.white.toNormalVec4(), new Vector3(screenPixelPos.X, screenPixelPos.Y, -0.2F), titleFontSize, ComponentAnchor.CENTER);
            return this;
        }

        /// <summary>
        /// Set the size of this slider knob as a percentage of the whole slider's size.
        /// </summary>
        public GUIValueSlider setSliderKnobSize(Vector2 s)
        {
            sliderKnobSize = s;
            sliderKnob.setSize(sliderKnobSize.X * size.X, sliderKnobSize.Y * size.Y, false);
            return this;
        }

        public GUIValueSlider setSliderKnobColor(Color c)
        {
            sliderKnob.setHoverColor(c);
            return this;
        }
        public GUIValueSlider setSliderKnobHoverColor(Color c)
        {
            sliderKnob.setColor(c);
            return this;
        }

        public float getFloatValue()
        {
            return minFloat + (maxFloat - minFloat) * sliderPos;
        }

        public int getIntValue()
        {
            return (int)((float)minInt + ((float)maxInt - (float)minInt) * sliderPos);
        }

        private void onSlideMoved(bool sound)
        {
            updateDisplayedCurrentVal();
            if(sound)
            SoundManager.playSound("tick");
            foreach (System.Action<GUIValueSlider> a in slideMoveListeners)
            {
                a(this);
            }
            parentGUI.onComponentValueChanged();
        }

        public GUIValueSlider addSlideMoveListener(System.Action<GUIValueSlider> a)
        {
            slideMoveListeners.Add(a);
            return this;
        }

        public override void requestRender()
        {
            if(!hidden)
            {
                background.requestRender();
                Renderer.requestRender(RenderType.guiText, font.texture, titleTextModel, renderLayer);
                Renderer.requestRender(RenderType.guiText, font.texture, minValTextModel, renderLayer);
                Renderer.requestRender(RenderType.guiText, font.texture, maxValTextModel, renderLayer);
                Renderer.requestRender(RenderType.guiText, font.texture, currentValTextModel, renderLayer);
                middleLineRect.requestRender();
                leftEndRect.requestRender();
                rightEndRect.requestRender();
                sliderKnob.requestRender();
            }
        }
    }
}
