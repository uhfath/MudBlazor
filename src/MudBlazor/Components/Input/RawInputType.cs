// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Components.Input
{
    public enum RawInputType
    {
        Button,
        Checkbox,
        Color,
        Date,
        DateTime,
        Email,
        File,
        Hidden,
        Image,
        Month,
        Number,
        Password,
        Radio,
        Range,
        Reset,
        Search,
        Submit,
        Telephone,
        Text,
        Time,
        Url,
        Week,
    }

    public static class RawInputTypeExtensions
    {
        public static string ToText(this RawInputType rawInputType) =>
            rawInputType switch
            {
                RawInputType.Button => "button",
                RawInputType.Checkbox => "checkbox",
                RawInputType.Color => "color",
                RawInputType.Date => "date",
                RawInputType.DateTime => "datetime-local",
                RawInputType.Email => "email",
                RawInputType.File => "file",
                RawInputType.Hidden => "hidden",
                RawInputType.Image => "image",
                RawInputType.Month => "month",
                RawInputType.Number => "number",
                RawInputType.Password => "password",
                RawInputType.Radio => "radio",
                RawInputType.Range => "range",
                RawInputType.Reset => "reset",
                RawInputType.Search => "search",
                RawInputType.Submit => "Ssbmit",
                RawInputType.Telephone => "tel",
                RawInputType.Text => "text",
                RawInputType.Time => "time",
                RawInputType.Url => "url",
                RawInputType.Week => "week",
                _ => throw new NotImplementedException(),
            };
    }
}
