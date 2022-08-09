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
    public enum RawInputMode
    {
        None,
        Text,
        Decimal,
        Numeric,
        Telephone,
        Search,
        Email,
        Url,
    }

    public static class RawInputModeExtensions
    {
        public static string ToText(this RawInputMode rawInputMode) =>
            rawInputMode switch
            {
                RawInputMode.None => "none",
                RawInputMode.Text => "text",
                RawInputMode.Decimal => "decimal",
                RawInputMode.Numeric => "numeric",
                RawInputMode.Telephone => "tel",
                RawInputMode.Search => "search",
                RawInputMode.Email => "email",
                RawInputMode.Url => "url",
                _ => throw new NotImplementedException(),
            };
    }
}
