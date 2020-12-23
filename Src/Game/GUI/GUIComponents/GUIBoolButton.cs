using RabbetGameEngine.Text;
using System;
using System.Collections.Generic;

namespace RabbetGameEngine
{
    public class GUIBoolButton : GUIButton
    {
        public bool boolValue = false;
        private List<Action<GUIBoolButton>> valueChangedListeners;
        private string trueString = "Enabled";
        private string falseString = "Disabled";
        private string originalTitle = "";
        public Color trueTitleColor = Color.white;
        public Color falseTitleColor = Color.lightGrey;
        private GUI parentGUI;
        public GUIBoolButton(GUI parentGUI, float posX, float posY, float sizeX, float sizeY, Color color, string title, FontFace font, ComponentAnchor alignment = ComponentAnchor.CENTER_LEFT, int renderLayer = 0, string texture = "white") : base(posX, posY, sizeX, sizeY, color, title, font, alignment, renderLayer, texture)
        {
            this.parentGUI = parentGUI;
            originalTitle = title;
            valueChangedListeners = new List<Action<GUIBoolButton>>();
            addClickListener(onClick);
        }

        private void onClick(GUIButton b)
        {
            boolValue = !boolValue;
            foreach(Action<GUIBoolButton> a in valueChangedListeners)
            {
                a(this);
            }
            parentGUI.onComponentValueChanged();
            updateRenderData();
        }
        public override void updateRenderData()
        {
            title = originalTitle + ": " + (boolValue ? trueString : falseString);
            base.updateRenderData();
        }

        public GUIBoolButton setTrueString(string s)
        {
            trueString = s;
            updateRenderData();
            return this;
        }

        public GUIBoolButton setFalseString(string s)
        {
            falseString = s;
            updateRenderData();
            return this;
        }

        public GUIBoolButton setBoolValue(bool flag)
        {
            boolValue = flag;
            updateRenderData();
            return this;
        }

        public GUIBoolButton addValueChangedListener(Action<GUIBoolButton> a)
        {
            valueChangedListeners.Add(a);
            return this;
        }

    }
}
