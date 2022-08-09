// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Components.Input
{
    public class MudRawAdornment
    {
        public string Class { get; set; }
        public string AdornmentIcon { get; set; }
        public string AdornmentText { get; set; }
        public Adornment Adornment { get; set; } = Adornment.None;
        public Color AdornmentColor { get; set; } = Color.Default;
        public string AdornmentAriaLabel { get; set; } = string.Empty;
        public Size AdornmentIconSize { get; set; } = Size.Medium;
        public EventCallback<MouseEventArgs> OnAdornmentClick { get; set; }
    }
}
