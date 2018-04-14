//Copyright 2017 Looking Glass Factory Inc.
//All rights reserved.
//Unauthorized copying or distribution of this file, and the source code contained herein, is strictly prohibited.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace HoloPlaySDK
{
    [Serializable]
    public class HoloPlayConfig
    {
        [Serializable]
        public class ConfigValue
        {
            public readonly bool isInt;
            [SerializeField]
            float value;
            public float Value
            {
                get { return value; }
                set
                {
                    this.value = isInt ? Mathf.Round(value) : value;
                    this.value = Mathf.Clamp(this.value, min, max);
                }
            }
            public readonly float defaultValue;
            public readonly float min;
            public readonly float max;
            public readonly string name;
            public ConfigValue(float defaultValue, float min, float max, string name, bool isInt = false)
            {
                this.defaultValue = defaultValue;
                this.min = min;
                this.max = max;
                this.Value = defaultValue;
                this.name = name;
                this.isInt = isInt;
            }

            public static implicit operator float(ConfigValue configValue)
            {
                return configValue.Value;
            }
        }

        public ConfigValue pitch = new ConfigValue(49.91f, 1f, 200, "Pitch");
        public ConfigValue slope = new ConfigValue(6, -30, 30, "Slope");
        public ConfigValue center = new ConfigValue(0, -1, 1, "Center");
        public ConfigValue viewCone = new ConfigValue(40, -180, 180, "View Cone");
        public ConfigValue verticalAngle = new ConfigValue(0, -20, 20, "Vert Angle");
        public ConfigValue numViews = new ConfigValue(32, 2, 128, "Num Views", true);
        public ConfigValue DPI = new ConfigValue(338, 1, 1000, "DPI", true);
        public ConfigValue screenW = new ConfigValue(2560, 640, 6400, "Screen Width", true);
        public ConfigValue screenH = new ConfigValue(1600, 480, 4800, "Screen Height", true);
        public ConfigValue flipImage = new ConfigValue(0, 0, 1, "Flip Image X", true);
        public ConfigValue flipImageY = new ConfigValue(0, 0, 1, "Flip Image Y", true);
        public ConfigValue flipSubp = new ConfigValue(0, 0, 1, "Flip Subpixels", true);
        public ConfigValue test = new ConfigValue(0, 0, 2, "Test Mode", true);

        public RealsenseCalibrator.CalibrationValues realsense = new RealsenseCalibrator.CalibrationValues();
    }
}