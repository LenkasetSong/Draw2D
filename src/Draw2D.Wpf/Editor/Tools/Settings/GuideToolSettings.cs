﻿using System;
using Draw2D.Models.Style;

namespace Draw2D.Editor.Tools
{
    public class GuideToolSettings : SettingsBase
    {
        private DrawStyle _guideStyle;
        
        public DrawStyle GuideStyle
        {
            get => _guideStyle;
            set => Update(ref _guideStyle, value);
        }
    }
}
