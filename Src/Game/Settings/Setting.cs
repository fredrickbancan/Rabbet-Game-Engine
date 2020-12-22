using System.Collections.Generic;

namespace RabbetGameEngine
{
    public enum SettingType
    {
        BOOL,
        INT,
        FLOAT,
        STRING,
        LIST_INT,
        LIST_FLOAT,
        LIST_STRING
    }

    public class Setting
    {
        public SettingType type;
        public string title;
        public string description = null;
        public string stringValue = "";
        public float floatValue = 0;
        public int intValue = 0;
        public bool boolValue = false;
        public float minFloatVal = 0;
        public float maxFloatVal = (float)int.MaxValue;
        public int minIntVal = 0;
        public int maxIntVal = int.MaxValue;
        public string[] listTitles;
        public string[] listStrings;
        public int[] listInts;
        public float[] listFloats;
        public int listIndex = 0;

        /// <summary>
        /// if true, this setting will be displayed in settings gui
        /// </summary>
        public bool editable;

        public Setting(string title, SettingType type, List<Setting> category, bool editable = true)
        {
            this.editable = editable;
            this.type = type;
            this.title = title;
            category.Add(this);
        }

        public Setting setDescription(string d)
        {
            description = d;
            return this;
        }
        public Setting setRange(float min, float max)
        {
            minFloatVal = min;
            maxFloatVal = max;
            return this;
        }
        public Setting setRange(int min, int max)
        {
            minIntVal = min;
            maxIntVal = max;
            return this;
        }
        public Setting setListTitles(string[] listTitles)
        {
            this.listTitles = listTitles;
            return this;
        }

        public Setting setListStrings(string[] values)
        {
            this.listStrings = values;
            return this;
        }

        public Setting setListInts(int[] values)
        {
            this.listInts = values;
            return this;
        }

        public Setting setListFloats(float[] values)
        {
            this.listFloats = values;
            return this;
        }

        public Setting setIntValue(int v)
        {
            intValue = v;
            return this;
        }

        public Setting setFloatValue(float v)
        {
            floatValue = v;
            return this;
        }

        public Setting setBoolValue(bool v)
        {
            boolValue = v;
            return this;
        }

        public Setting setListIndex(int i)
        {
            listIndex = i;
            return this;
        }

        /// <summary>
        /// Can be added to a gui value slider as a listener func
        /// </summary>
        public void applySliderValue(GUIValueSlider g)
        {
            if(type == SettingType.FLOAT)
            {
                floatValue = g.getFloatValue();
            }
            else
            {
                intValue = g.getIntValue();
            }
        }

        /// <summary>
        /// Can be added to a GUI drop down button as a listener func
        /// </summary>
        public void applyDropDownValue(GUIDropDownButton g)
        {
            listIndex = g.listIndex;
            switch (type)
            {
                case SettingType.LIST_STRING:
                    if(listStrings != null)
                    {
                        stringValue = listStrings[listIndex];
                    }
                    break;
                case SettingType.LIST_INT:
                    if (listInts != null)
                    {
                        intValue = listInts[listIndex];
                    }
                    break;
                case SettingType.LIST_FLOAT:
                    if (listFloats != null)
                    {
                        floatValue = listFloats[listIndex];
                    }
                    break;
            }
        }

        public float normalizedFloatValue { get => MathUtil.normalize(minFloatVal, maxFloatVal, floatValue); }
    }
}
