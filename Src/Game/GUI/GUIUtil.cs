using RabbetGameEngine.Sound;
namespace RabbetGameEngine
{
    public static class GUIUtil
    {
        public static void defaultOnButtonHoverEnter()
        {
            SoundManager.playSound("buttonhover");
        }

        public static void defaultOnButtonHoverExit()
        {
        }

        public static void defaultOnButtonClick(GUIButton b)
        {
            SoundManager.playSound("buttonclick");
        }

        public static void loadSliderValueFromSetting(GUIValueSlider gvs, Setting s)
        {
            if (gvs.isInteger)
            {
                gvs.sliderPos = MathUtil.normalizeClamped(s.minIntVal, s.maxIntVal, s.intValue);
            }
            else
            {
                gvs.sliderPos = MathUtil.normalizeClamped(s.minFloatVal, s.maxFloatVal, s.floatValue);
            }
        }

        public static void addAudioSettingsChangersToGUI(GUI g)
        {

        }
        public static void addVideoSettingsChangersToGUI(GUI g)
        {
            int displayedSettingsCount = 0;
            for(int i = 0; i < GameSettings.videoSettings.Count; i++)
            {
                if (GameSettings.videoSettings[i].editable) displayedSettingsCount++;
            }
            int componentsWide = 4;
            int bottomRowCount = displayedSettingsCount % componentsWide;
            float componentWidth = 0.25F;
            float componentHeight = 0.05F;
            float componentSpacing = 0.1F;
            float halfComponentSpacing = componentSpacing * 0.5F;
            float halfComponentWidth = componentWidth * 0.5F;
            float halfComponentHeight = componentHeight * 0.5F;

            for(int i = 0; i < GameSettings.videoSettings.Count; i++)
            {
                Setting curSet = GameSettings.videoSettings[i];
                if (!curSet.editable) continue;
                GUIComponent newComp = null;

                float compPosY = 0.5F - componentHeight * 4.0F;
                float compPosX = (-(componentsWide * (componentWidth + componentSpacing) - componentSpacing)) * 0.5F + halfComponentWidth; 

                if(i >= displayedSettingsCount - bottomRowCount)
                {
                    compPosY -= (i / componentsWide) * (componentHeight + componentSpacing);
                    compPosX = (-(bottomRowCount/2 * (componentWidth + componentSpacing))) + (bottomRowCount - (displayedSettingsCount - i)) * (componentWidth + componentSpacing);
                }
                else
                {
                    compPosY -= (i / componentsWide) * (componentHeight + componentSpacing);
                    compPosX += (i % componentsWide) * (componentWidth + componentSpacing);
                }

                switch(curSet.type)
                {
                    case SettingType.BOOL:
                        newComp = new GUIBoolButton(g, compPosX, compPosY, componentWidth, componentHeight, Color.grey.setAlphaF(0.7F), curSet.title, g.guiFont, ComponentAnchor.CENTER, 2).setBoolValue(curSet.boolValue).addValueChangedListener(curSet.applyBoolButtonValue);
                        break;
                    case SettingType.FLOAT:
                        newComp = new GUIValueSlider(g, compPosX, compPosY, componentWidth, componentHeight, curSet.title, g.guiFont, false, ComponentAnchor.CENTER, 2).setRange(curSet.minFloatVal, curSet.maxFloatVal).setDisplayRange(curSet.minFloatDisplayVal, curSet.maxFloatDisplayVal).setSliderPos(curSet.normalizedFloatValue).addSlideMoveListener(curSet.applySliderValue);
                        break;
                    case SettingType.INT:
                        newComp = new GUIValueSlider(g, compPosX, compPosY, componentWidth, componentHeight, curSet.title, g.guiFont, true, ComponentAnchor.CENTER, 2).setRange(curSet.minIntVal, curSet.maxIntVal).setDisplayRange(curSet.minIntDisplayVal, curSet.maxIntDisplayVal).setSliderPos(curSet.normalizedIntValue).addSlideMoveListener(curSet.applySliderValue);
                        break;
                    case SettingType.STRING:
                        break;
                    case SettingType.LIST_FLOAT:
                        newComp = new GUIDropDownButton(g, compPosX, compPosY, componentWidth, componentHeight, Color.grey.setAlphaF(0.7F), curSet.title, curSet.listTitles, g.guiFont, ComponentAnchor.CENTER, 2).setDropDownIndex(curSet.listIndex).addValueChangeListener(curSet.applyDropDownValue);
                        break;
                    case SettingType.LIST_INT:
                        newComp = new GUIDropDownButton(g, compPosX, compPosY, componentWidth, componentHeight, Color.grey.setAlphaF(0.7F), curSet.title, curSet.listTitles, g.guiFont, ComponentAnchor.CENTER, 2).setDropDownIndex(curSet.listIndex).addValueChangeListener(curSet.applyDropDownValue);
                        break;
                    case SettingType.LIST_STRING:
                        newComp = new GUIDropDownButton(g, compPosX, compPosY, componentWidth, componentHeight, Color.grey.setAlphaF(0.7F), curSet.title, curSet.listTitles, g.guiFont, ComponentAnchor.CENTER, 2).setDropDownIndex(curSet.listIndex).addValueChangeListener(curSet.applyDropDownValue);
                        break;
                }
                if(newComp != null)
                {
                    g.addGuiComponent(curSet.title, newComp);
                }
            }
        }
        public static void addAdvVideoSettingsChangersToGUI(GUI g)
        {

        }
        public static void addControlsSettingsChangersToGUI(GUI g)
        {

        }
        public static void addBindingsSettingsChangersToGUI(GUI g)
        {

        }
    }
}
