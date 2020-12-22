using RabbetGameEngine.Text;
using System;
using System.Collections.Generic;

namespace RabbetGameEngine
{
    public class GUIDropDownButton : GUIButton
    {
        public int listIndex = 0;
        private GUIButton[] dropDownButtons = null;
        private GUIButton dropDownBackground = null;
        private List<Action<GUIDropDownButton>> listeners;
        public bool listEnabled = false;
        private string baseTitle = "";
        private string[] listTitles;
        private GUI parentGUI = null;
        public GUIDropDownButton(GUI parentGUI, float posX, float posY, float sizeX, float sizeY, Color color, string title, string[] listTitles, FontFace font, ComponentAnchor anchor = ComponentAnchor.CENTER, int renderLayer = 0, string textureName = "white") : base(posX, posY, sizeX, sizeY, color, title, font, anchor, renderLayer, textureName)
        {
            baseTitle = title;
            this.parentGUI = parentGUI;
            setHoverColor(Color.black.setAlphaF(0.8F));
            listeners = new List<Action<GUIDropDownButton>>();
            this.listTitles = listTitles;
            addHoverEnterListener(onHoverEnter);
            addClickListener(onClick);
            float listButtonSizeX = sizeX;
            float listButtonSizeY = 0.049F;
            dropDownBackground = new GUIButton(posX, posY, listButtonSizeX + 0.05F, listButtonSizeY * listTitles.Length + 0.05F, Color.black.setAlphaF(0.8F), "", font, anchor, renderLayer + 1).setHoverColor(Color.black.setAlphaF(0.8F));
            dropDownBackground.updateRenderData();
            dropDownButtons = new GUIButton[listTitles.Length];
            for(int i = 0; i < listTitles.Length; i++)
            {
                float yPos = dropDownBackground.getScreenPos().Y + (listTitles.Length * listButtonSizeY * 0.5F - i * listButtonSizeY - listButtonSizeY * 0.5F);
                dropDownButtons[i] = new GUIButton(dropDownBackground.getScreenPos().X, yPos, listButtonSizeX, listButtonSizeY, color.setAlphaF(1.0F), listTitles[i], font, ComponentAnchor.CENTER, renderLayer + 2).addClickListener(onDropDownButtonClick).addHoverEnterListener(GUI.defaultOnButtonHoverEnter);
            }
            updateRenderData();
        }

        public override void updateRenderData()
        {
            base.updateRenderData();
            dropDownBackground.updateRenderData();
            foreach (GUIButton g in dropDownButtons)
            {
                g.updateRenderData();
            }

        }

        private void onClick(GUIButton g)
        {
            if(!listEnabled)
            {
                listEnabled = true;
                GUI.defaultOnButtonClick();
            }
        }
        private void onHoverEnter()
        {
            if(!listEnabled)
            {
                GUI.defaultOnButtonHoverEnter();
            }
        }

        public override void onUpdate()
        {

            if (listEnabled)
            {
                parentGUI.pauseAllExcept(this);
                foreach (GUIButton b in dropDownButtons)
                {
                    b.onUpdate();
                }
                dropDownBackground.onUpdate();
                listEnabled = dropDownBackground.isHovered;
            }
            else
            {
                parentGUI.unPauseAll();
            base.onUpdate();
            }
        }

        public override void requestRender()
        {
            base.requestRender();
            if(listEnabled)
            {
                dropDownBackground.requestRender();
                foreach(GUIButton b in dropDownButtons)
                {
                    b.requestRender();
                }
            }
        }

        private void onDropDownButtonClick(GUIButton b)
        {
            GUI.defaultOnButtonClick();
            for(int i = 0; i < dropDownButtons.Length; i++)
            {
                dropDownButtons[i].enable();
                if(b == dropDownButtons[i])
                {
                    setDropDownIndex(i);
                }
            }
        }

        public GUIDropDownButton addValueChangeListener(Action<GUIDropDownButton> a)
        {
            listeners.Add(a);
            return this;
        }

        public GUIDropDownButton setDropDownIndex(int i)
        {
            listIndex = i;
            dropDownButtons[i].disable();
            title = baseTitle + ": " + listTitles[i];
            updateRenderData();
            return this;
        }
    }
}

