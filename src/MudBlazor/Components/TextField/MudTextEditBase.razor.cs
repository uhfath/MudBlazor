// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    public partial class MudTextEditBase<T> : MudRawInput<T>
    {
        [Parameter]
        public string InputControlClass { get; set; }

        [Parameter]
        public string InputControlStyle { get; set; }

        [Parameter]
        public RenderFragment<T> InputContent { get; set; }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string HelperText { get; set; }

        [Parameter]
        public bool HelperTextOnFocus { get; set; }

        [Parameter]
        public bool Error { get; set; }

        [Parameter]
        public string ErrorText { get; set; }

        [Parameter]
        public Func<string, string> ErrorTextPreProcessor { get; set; }

        [Parameter]
        public bool Required { get; set; }

        [Parameter]
        public int? Counter { get; set; }

        [Parameter]
        public bool ControlKeyDownPreventDefault { get; set; }

        [Parameter]
        public bool ControlKeyUpPreventDefault { get; set; }

        [Parameter]
        public bool ControlMouseDownPreventDefault { get; set; }

        [Parameter]
        public bool ControlClickPreventDefault { get; set; }

        [Parameter]
        public bool ControlMouseUpPreventDefault { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnControlKeyDown { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnControlKeyPress { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnControlKeyUp { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnControlMouseDown { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnControlClick { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnControlMouseUp { get; set; }

        private IEnumerable<string> _validationMessages = Enumerable.Empty<string>();
        private bool _isDisposed;

        private RenderFragment RenderBaseInput() =>
            base.BuildRenderTree;

        private string _counterText =>
            Counter == null
                ? null
                : Counter == 0
                    ? (base.CurrentInputText?.Length ?? 0).ToString()
                    : $"{base.CurrentInputText?.Length ?? 0} / {Counter}";

        private IEnumerable<string> _errorTextLines =>
            ErrorText?.Split(Environment.NewLine) ?? Enumerable.Empty<string>();

        private bool _hasErrors =>
            Error || _allErrors.Any();

        private IEnumerable<string> _allErrors =>
            (Error ? _errorTextLines : Enumerable.Empty<string>())
                .Concat(_validationMessages)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Select(e => (ErrorTextPreProcessor?.Invoke(e) ?? e).Trim())
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .ToArray()
            ;

        private string _errorText =>
            string.Join(Environment.NewLine, _allErrors);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!_isDisposed)
            {
                if (disposing)
                {
                    if (base._currentEditContext != null && base.FieldIdentifier != null)
                    {
                        base._currentEditContext.OnValidationStateChanged -= OnValidationStateChanged;
                    }
                }

                _isDisposed = true;
            }
        }

        private void OnValidationStateChanged(object sender, ValidationStateChangedEventArgs _)
        {
            _validationMessages = ((EditContext)sender).GetValidationMessages(base.FieldIdentifier.Value);
            StateHasChanged();
        }

        protected override Task OnInputInternal(ChangeEventArgs eventArgs) =>
            base.OnInputInternal(eventArgs);

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (base._currentEditContext != null && base.FieldIdentifier != null)
            {
                base._currentEditContext.OnValidationStateChanged += OnValidationStateChanged;
                OnValidationStateChanged(base._currentEditContext, ValidationStateChangedEventArgs.Empty);
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            base.HasError = _hasErrors;
        }
    }
}
