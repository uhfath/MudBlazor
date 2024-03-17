using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Components.Input;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudRawInput<T> : MudComponentBase, IDisposable
    {
        private readonly string RawInputElementId = $"raw-input-{Guid.NewGuid()}";
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        private static readonly TypeConverter TypeConverter = TypeDescriptor.GetConverter(typeof(T));
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

        public delegate string FormatToStringDelegate(in T value);
        public delegate bool ParseFromStringDelegate(string text, out T value);

        [Parameter]
        public RenderFragment<T> ChildContent { get; set; }

        [Parameter]
        public T Value { get; set; }

        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }

        [Parameter]
        public int DebounceIntervalMilliseconds { get; set; }

        [Parameter]
        public int DebounceMinLength { get; set; }

        [Parameter]
        public FormatToStringDelegate FormatToString { get; set; }

        [Parameter]
        public ParseFromStringDelegate ParseFromString { get; set; }

        [Parameter]
        public FieldIdentifier? FieldIdentifier { get; set; }

        [Parameter]
        public bool AutoValidate { get; set; } = true;

        [Parameter]
        public bool HasError { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool ReadOnly { get; set; }

        [Parameter]
        public bool FullWidth { get; set; }

        [Parameter]
        public bool AutoGrow { get; set; }

        [Parameter]
        public int MaxLines { get; set; }

        [Parameter]
        public int Lines { get; set; } = 1;

        [Parameter]
        public bool DisableUnderLine { get; set; }

        [Parameter]
        public bool ForceChangeOnBlur { get; set; }

        [Parameter]
        public bool ForceChangeOnEnter { get; set; }

        [Parameter]
        public Variant Variant { get; set; } = Variant.Text;

        [Parameter]
        public Margin Margin { get; set; } = Margin.None;

        [Parameter]
        public string Placeholder { get; set; }

        [Parameter]
        public RawInputMode InputMode { get; set; } = RawInputMode.Text;

        [Parameter]
        public RawInputType InputType { get; set; } = RawInputType.Text;

        [Parameter]
        public int? MaxLength { get; set; }

        [Parameter]
        public string AdornmentIcon { get; set; }

        [Parameter]
        public string AdornmentText { get; set; }

        [Parameter]
        public Adornment Adornment { get; set; } = Adornment.None;

        [Parameter]
        public Color AdornmentColor { get; set; } = Color.Default;

        [Parameter]
        public string AdornmentClass { get; set; }

        [Parameter]
        public string AdornmentStyle { get; set; }

        [Parameter]
        public string AdornmentAriaLabel { get; set; } = string.Empty;

        [Parameter]
        public Size AdornmentIconSize { get; set; } = Size.Medium;

        [Parameter]
        public bool AdornmentShowFocus { get; set; } = true;

        [Parameter]
        public bool AdornmentDisabled { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnAdornmentClick { get; set; }

        [Parameter]
        public IEnumerable<MudRawAdornment> AdditionalAdornments { get; set; }

        [Parameter]
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Start;

        [Parameter]
        public bool AutoFocus { get; set; }

        [Parameter]
        public bool SelectOnFocus { get; set; }

        [Parameter]
        public bool FocusOnClear { get; set; }

        [Parameter]
        public bool ShowSpinners { get; set; }

        [Parameter]
        public bool HasActiveLabel { get; set; }

        [Parameter]
        public string SpinnerUpIcon { get; set; } = Icons.Material.Filled.KeyboardArrowUp;

        [Parameter]
        public string SpinnerDownIcon { get; set; } = Icons.Material.Filled.KeyboardArrowDown;

        [Parameter]
        public EventCallback<MouseEventArgs> OnSpinnerIncrement { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnSpinnerDecrement { get; set; }

        [Parameter]
        public bool Clearable { get; set; }

        [Parameter]
        public string ClearIcon { get; set; } = Icons.Material.Filled.Clear;

        [Parameter]
        public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        [Parameter]
        public bool KeyDownPreventDefault { get; set; }

        [Parameter]
        public bool KeyUpPreventDefault { get; set; }

        [Parameter]
        public EventCallback<FocusEventArgs> OnFocus { get; set; }

        [Parameter]
        public EventCallback<FocusEventArgs> OnBlur { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnKeyUp { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public EventCallback<ChangeEventArgs> OnInput { get; set; }

        [Parameter]
        public EventCallback<ChangeEventArgs> OnChange { get; set; }

        [Parameter]
        public EventCallback<ClipboardEventArgs> OnPaste { get; set; }

        [Parameter]
        public EditContext EditContext { get; set; }

        public string InputElementId => UserAttributes.TryGetValue("id", out var _id) ? _id.ToString() : RawInputElementId;

        [CascadingParameter]
        private EditContext CascadingEditContext { get; set; }

        public ElementReference InputElementReference { get; private set; }
        public string CurrentInputText { get; private set; }
        public bool IsFocused { get; private set; }

        private string _currentValueText;
        private T _previousValue;
        private bool _invalidateValue;
        private bool _invalidateRender;
        private bool _invalidateFocus;
        private IEnumerable<MudRawAdornment> _rawAdornments;
        private Timer _debounceTimer;
        private bool _isDisposed;
        private bool _isInputDirty;
        private bool _wasAutoFocused;

        protected EditContext _currentEditContext =>
            EditContext ?? CascadingEditContext;

        private Size _spinnerButtonSize =>
            Margin == Margin.Dense
                ? Size.Small
                : Size.Medium;

        private string _containerClass =>
            MudInputCssHelper.GetClassname(this, !string.IsNullOrEmpty(CurrentInputText) || HasActiveLabel || HasStartAdornments || !string.IsNullOrWhiteSpace(Placeholder));
        private string _inputClass =>
            MudInputCssHelper.GetInputClassname(this, false);
        private string _inputDivClass =>
            MudInputCssHelper.GetInputClassname(this, true);

        private string _inputStyle => new StyleBuilder()
            .AddStyle("text-align", HorizontalAlignment.ToDescriptionString())
            .AddStyle(Style)
            .Build();

        private string _inputDivStyle => new StyleBuilder()
            .AddStyle("text-align", HorizontalAlignment.ToDescriptionString())
            .AddStyle(Style)
            .Build();

        private string _clearButtonClass =>
            new CssBuilder()
                .AddClass("me-n1", HasEndAdornments && ShowSpinners)
                .AddClass("mud-icon-button-edge-end", HasEndAdornments && !ShowSpinners)
                .AddClass("me-6", !HasEndAdornments && ShowSpinners)
                //.AddClass("mud-icon-button-edge-margin-end", !HasEndAdornments && !ShowSpinners)
                .AddClass("align-self-start")
                .Build();

        public string GetAdornmentClassname(MudRawAdornment rawAdornment) =>
            new CssBuilder("mud-input-adornment")
                .AddClass($"mud-input-adornment-{rawAdornment.Adornment.ToDescriptionString()}", rawAdornment.Adornment != Adornment.None)
                .AddClass($"mud-text", !string.IsNullOrEmpty(rawAdornment.AdornmentText))
                .AddClass($"mud-input-root-filled-shrink", Variant == Variant.Filled)
                .AddClass("align-self-center")
                .AddClass(rawAdornment.Class)
                .Build();

        public string GetAdornmentStyle(MudRawAdornment rawAdornment) =>
            new StyleBuilder()
                .AddStyle(rawAdornment.Style)
                .Build();

        private bool _showClearIcon =>
            Clearable && !string.IsNullOrEmpty(_currentValueText);

        private string _inputMode =>
            InputMode.ToText();

        private string _inputType =>
            InputType.ToText();

        private IEnumerable<MudRawAdornment> _startAdornments =>
            _rawAdornments
                .Where(a => a.Adornment == Adornment.Start)
            ;

        private IEnumerable<MudRawAdornment> _endAdornments =>
            _rawAdornments
                .Where(a => a.Adornment == Adornment.End)
            ;

        public bool HasStartAdornments =>
            _startAdornments.Any();

        public bool HasEndAdornments =>
            _endAdornments.Any();

        public static bool IsValueEquals(in T previous, in T current) =>
            EqualityComparer<T>.Default.Equals(previous, current);

        public string GetStringFromValue(in T value) =>
            FormatToString?.Invoke(value) ?? value?.ToString();

        public bool GetValueFromString(string text, out T value)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                value = default;
                return true;
            }

            if (ParseFromString != null)
            {
                return ParseFromString(text, out value);
            }

            if (TypeConverter.CanConvertFrom(typeof(string)))
            {
                value = (T)TypeConverter.ConvertFromString(text);
                return true;
            }

            value = default;
            return false;
        }

        public bool GetValueFrom(object source, out T value)
        {
            if (source == null)
            {
                value = default;
                return true;
            }

            if (TypeConverter.CanConvertFrom(source.GetType()))
            {
                value = (T)TypeConverter.ConvertFrom(source);
                return true;
            }

            value = default;
            return false;
        }

        public ValueTask FocusAsync() =>
            InputElementReference.FocusAsync();

        public void InvalidateFocus() =>
            _invalidateFocus = true;

        public virtual Task ClearValueAsync() =>
            SetValueAsync(default);

        public virtual void NotifyFieldValueChanged()
        {
            if (AutoValidate && _currentEditContext != null && FieldIdentifier != null)
            {
                _currentEditContext.NotifyFieldChanged(FieldIdentifier.Value);
            }
        }

        public virtual async Task SetValueAsync(T value)
        {
            await ValueChanged.InvokeAsync(value);

            NotifyFieldValueChanged();
            StateHasChanged();
        }

        public virtual void InvalidateAdornments()
        {
            _rawAdornments = Enumerable.Concat(
                new[]
                {
                    new MudRawAdornment
                    {
                        AdornmentIcon = AdornmentIcon,
                        AdornmentText = AdornmentText,
                        Adornment = Adornment,
                        AdornmentColor = AdornmentColor,
                        Class = AdornmentClass,
                        Style = AdornmentStyle,
                        AdornmentAriaLabel = AdornmentAriaLabel,
                        AdornmentIconSize = AdornmentIconSize,
                        ShowFocus = AdornmentShowFocus,
                        Disabled = AdornmentDisabled,
                        OnAdornmentClick = OnAdornmentClick,
                    }
                },
                AdditionalAdornments ?? Enumerable.Empty<MudRawAdornment>());
        }

        private void StopDebounceTimer()
        {
            _debounceTimer?.Dispose();
            _debounceTimer = null;
        }

        private void StartDebounceTimer()
        {
            StopDebounceTimer();
            if (DebounceIntervalMilliseconds > 0)
            {
                _debounceTimer = new Timer(_ => InvokeAsync(() => ForceOnChange()), null, DebounceIntervalMilliseconds, Timeout.Infinite);
            }
        }

        private void ForceOnChange() =>
            InputElementReference.MudDispatchEvent("change");

        protected virtual async Task OnClearButtonClickInternal(MouseEventArgs eventArgs)
        {
            await OnClearButtonClick.InvokeAsync(eventArgs);
            await ClearValueAsync();

            if (FocusOnClear)
            {
                InvalidateFocus();
            }
        }

        protected virtual async Task OnFocusInternal(FocusEventArgs eventArgs)
        {
            if (SelectOnFocus)
            {
                await InputElementReference.MudSelectAsync();
            }

            await OnFocus.InvokeAsync(eventArgs);
            IsFocused = true;
        }

        protected virtual async Task OnBlurInternal(FocusEventArgs eventArgs)
        {
            await OnBlur.InvokeAsync(eventArgs);
            IsFocused = false;

            if (ForceChangeOnBlur && _isInputDirty && !Disabled && !ReadOnly)
            {
                ForceOnChange();
            }
        }

        protected virtual Task OnPasteInternal(ClipboardEventArgs eventArgs)
        {
            if (Disabled || ReadOnly)
            {
                return Task.CompletedTask;
            }

            return OnPaste.InvokeAsync(eventArgs);
        }

        protected virtual async Task OnInputInternal(ChangeEventArgs eventArgs)
        {
            if (Disabled || ReadOnly)
            {
                return;
            }

            await OnInput.InvokeAsync(eventArgs);
            CurrentInputText = eventArgs.Value?.ToString();
            _isInputDirty = true;

            if (DebounceIntervalMilliseconds > 0 && CurrentInputText?.Length >= DebounceMinLength)
            {
                StartDebounceTimer();
            }
        }

        protected virtual Task OnChangeInternal(ChangeEventArgs eventArgs)
        {
            StopDebounceTimer();
            _isInputDirty = false;
            return OnChange.InvokeAsync(eventArgs);
        }

        protected virtual async Task OnKeyDownInternal(KeyboardEventArgs eventArgs)
        {
            if (!Disabled && !ReadOnly)
            {
                await OnKeyDown.InvokeAsync(eventArgs);

                if (ForceChangeOnEnter && _isInputDirty && (eventArgs.Code == "Enter" || eventArgs.Code == "NumpadEnter"))
                {
                    ForceOnChange();
                }
            }
        }

        protected virtual Task OnKeyUpInternal(KeyboardEventArgs eventArgs) =>
            (Disabled || ReadOnly)
                ? Task.CompletedTask
                : OnKeyUp.InvokeAsync(eventArgs);
        protected virtual Task OnClickInternal(MouseEventArgs eventArgs) =>
            (Disabled || ReadOnly)
                ? Task.CompletedTask
                : OnClick.InvokeAsync(eventArgs);

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (!IsValueEquals(_previousValue, Value) || _invalidateValue)
            {
                _invalidateRender = true;
                _invalidateValue = false;
                _previousValue = Value;

                CurrentInputText = _currentValueText = GetStringFromValue(Value);
            }

            if (AutoFocus != _wasAutoFocused)
            {
                _wasAutoFocused = AutoFocus;
                if (AutoFocus)
                {
                    InvalidateFocus();
                }
            }

            InvalidateAdornments();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (AutoGrow)
            {
                if (firstRender)
                {
                    InputElementReference.GetJSInProcessRuntime()?.InvokeVoid("mudInputAutoGrow.initAutoGrow", InputElementReference, MaxLines);
                }
                else
                {
                    if (_invalidateRender)
                    {
                        _invalidateRender = false;
                        InputElementReference.GetJSInProcessRuntime()?.InvokeVoid("mudInputAutoGrow.adjustHeight", InputElementReference);
                    }
                }
            }

            base.OnAfterRender(firstRender);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_invalidateFocus)
            {
                _invalidateFocus = false;
                await FocusAsync();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    StopDebounceTimer();
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
