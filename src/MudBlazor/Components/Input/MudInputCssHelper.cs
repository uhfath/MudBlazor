using System;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    internal static class MudInputCssHelper
    {
        public static string GetClassname<T>(MudRawInput<T> rawInput, bool shrinkWhen) =>
            new CssBuilder("mud-input")
                .AddClass($"mud-input-{rawInput.Variant.ToDescriptionString()}")
                .AddClass("mud-input-adorned-start", rawInput.HasStartAdornments)
                .AddClass("mud-input-adorned-end", rawInput.HasEndAdornments)
                .AddClass($"mud-input-margin-{rawInput.Margin.ToDescriptionString()}", when: () => rawInput.Margin != Margin.None)
                .AddClass("mud-input-underline", when: () => rawInput.DisableUnderLine == false && rawInput.Variant != Variant.Outlined)
                .AddClass("mud-shrink", shrinkWhen)
                .AddClass("mud-disabled", rawInput.Disabled)
                .AddClass("mud-input-error", rawInput.HasError)
                .AddClass(rawInput.Class)
                .Build();

        public static string GetClassname<T>(MudBaseInput<T> baseInput, Func<bool> shrinkWhen) =>
            new CssBuilder("mud-input")
                .AddClass($"mud-input-{baseInput.Variant.ToDescriptionString()}")
                .AddClass($"mud-input-adorned-{baseInput.Adornment.ToDescriptionString()}", baseInput.Adornment != Adornment.None)
                .AddClass($"mud-input-margin-{baseInput.Margin.ToDescriptionString()}", when: () => baseInput.Margin != Margin.None)
                .AddClass("mud-input-underline", when: () => baseInput.DisableUnderLine == false && baseInput.Variant != Variant.Outlined)
                .AddClass("mud-shrink", when: shrinkWhen)
                .AddClass("mud-disabled", baseInput.Disabled)
                .AddClass("mud-input-error", baseInput.HasErrors)
                .AddClass("mud-ltr", baseInput.GetInputType() == InputType.Email || baseInput.GetInputType() == InputType.Telephone)
                .AddClass(baseInput.Class)
                .Build();

        public static string GetInputClassname<T>(MudRawInput<T> rawInput, bool isDiv) =>
            new CssBuilder("mud-input-slot")
                .AddClass("mud-input-root")
                .AddClass($"mud-input-root-{rawInput.Variant.ToDescriptionString()}")
                .AddClass("mud-input-adorned-start", rawInput.HasStartAdornments)
                .AddClass("mud-input-adorned-end", rawInput.HasEndAdornments)
                .AddClass($"mud-input-root-margin-{rawInput.Margin.ToDescriptionString()}", when: () => rawInput.Margin != Margin.None)
                .AddClass("mud-input-placeholder", isDiv)
                .AddClass(rawInput.Class)
                .Build();

        public static string GetInputClassname<T>(MudBaseInput<T> baseInput) =>
            new CssBuilder("mud-input-slot")
                .AddClass("mud-input-root")
                .AddClass($"mud-input-root-{baseInput.Variant.ToDescriptionString()}")
                .AddClass($"mud-input-root-adorned-{baseInput.Adornment.ToDescriptionString()}", baseInput.Adornment != Adornment.None)
                .AddClass($"mud-input-root-margin-{baseInput.Margin.ToDescriptionString()}", when: () => baseInput.Margin != Margin.None)
                .AddClass(baseInput.Class)
                .Build();

        public static string GetAdornmentClassname<T>(MudBaseInput<T> baseInput) =>
            new CssBuilder("mud-input-adornment")
                .AddClass($"mud-input-adornment-{baseInput.Adornment.ToDescriptionString()}", baseInput.Adornment != Adornment.None)
                .AddClass("mud-text", !string.IsNullOrEmpty(baseInput.AdornmentText))
                .AddClass($"mud-input-root-filled-shrink", baseInput.Variant == Variant.Filled)
                .AddClass(baseInput.Class)
                .Build();
    }
}
