// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Components.Dialog
{
    public interface IMudDialogInstanceGetter
    {
        MudDialogInstance MudDialogInstance { get; }
    }
}
