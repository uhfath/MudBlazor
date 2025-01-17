﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudInputControl : MudComponentBase
    {
        protected string Classname =>
           new CssBuilder("mud-input-control")
             .AddClass("mud-input-required", when: () => Required)
             .AddClass($"mud-input-control-margin-{Margin.ToDescriptionString()}", when: () => Margin != Margin.None)
             .AddClass("mud-input-control-full-width", FullWidth)
             .AddClass("mud-input-error", Error)
             .AddClass(Class)
           .Build();

        protected string HelperContainer =>
           new CssBuilder("mud-input-control-helper-container")
           .AddClass($"px-1", Variant == Variant.Filled)
           .AddClass($"px-2", Variant == Variant.Outlined)
           .Build();

        protected string HelperClass =>
           new CssBuilder("mud-input-helper-text")
             .AddClass("mud-input-helper-onfocus", HelperTextOnFocus)
             .AddClass("mud-input-error", !string.IsNullOrEmpty(ErrorText))
           .Build();

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Should be the Input
        /// </summary>
        [Parameter]
        public RenderFragment? InputContent { get; set; }

        /// <summary>
        ///  Will adjust vertical spacing. 
        /// </summary>
        [Parameter]
        public Margin Margin { get; set; } = Margin.None;

        /// <summary>
        /// If true, will apply mud-input-required class to the output div
        /// </summary>
        [Parameter]
        public bool Required { get; set; }

        /// <summary>
        /// If true, the label will be displayed in an error state.
        /// </summary>
        [Parameter]
        public bool Error { get; set; }

        /// <summary>
        /// The ErrorText that will be displayed if Error true
        /// </summary>
        [Parameter]
        public string? ErrorText { get; set; }

        /// <summary>
        /// The ErrorId that will be used by aria-describedby if Error true
        /// </summary>
        [Parameter]
        public string? ErrorId { get; set; }

        /// <summary>
        /// The HelperText will be displayed below the text field.
        /// </summary>
        [Parameter]
        public string? HelperText { get; set; }

        /// <summary>
        /// If true, the helper text will only be visible on focus.
        /// </summary>
        [Parameter]
        public bool HelperTextOnFocus { get; set; }

        /// <summary>
        /// The current character counter, displayed below the text field.
        /// </summary>
        [Parameter]
        public string? CounterText { get; set; }

        /// <summary>
        /// If true, the input will take up the full width of its container.
        /// </summary>
        [Parameter]
        public bool FullWidth { get; set; }

        /// <summary>
        /// If string has value the label text will be displayed in the input, and scaled down at the top if the input has value.
        /// </summary>
        [Parameter]
        public string? Label { get; set; }

        /// <summary>
        /// Variant can be Text, Filled or Outlined.
        /// </summary>
        [Parameter]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// If true, the input element will be disabled.
        /// </summary>
        [Parameter]
        public bool Disabled { get; set; }

        /// <summary>
        /// If string has value the label "for" attribute will be added.
        /// </summary>
        [Parameter]
        public string ForId { get; set; } = string.Empty;

        [Parameter]
        public bool KeyDownPreventDefault { get; set; }

        [Parameter]
        public bool KeyPressPreventDefault { get; set; }

        [Parameter]
        public bool KeyUpPreventDefault { get; set; }

        [Parameter]
        public bool MouseDownPreventDefault { get; set; }

        [Parameter]
        public bool ClickPreventDefault { get; set; }

        [Parameter]
        public bool MouseUpPreventDefault { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnKeyPress { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnKeyUp { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnMouseDown { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnMouseUp { get; set; }

        private Task OnKeyDownInternal(KeyboardEventArgs eventArgs) =>
            Disabled
                ? Task.CompletedTask
                : OnKeyDown.InvokeAsync(eventArgs);
        private Task OnKeyPressInternal(KeyboardEventArgs eventArgs) =>
            Disabled
                ? Task.CompletedTask
                : OnKeyPress.InvokeAsync(eventArgs);
        private Task OnKeyUpInternal(KeyboardEventArgs eventArgs) =>
            Disabled
                ? Task.CompletedTask
                : OnKeyUp.InvokeAsync(eventArgs);
        private Task OnMouseDownInternal(MouseEventArgs eventArgs) =>
            Disabled
                ? Task.CompletedTask
                : OnMouseDown.InvokeAsync(eventArgs);
        private Task OnClickInternal(MouseEventArgs eventArgs) =>
            Disabled
                ? Task.CompletedTask
                : OnClick.InvokeAsync(eventArgs);
        private Task OnMouseUpInternal(MouseEventArgs eventArgs) =>
            Disabled
                ? Task.CompletedTask
                : OnMouseUp.InvokeAsync(eventArgs);
    }
}
