using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudFocusTrap : IDisposable
    {
        private bool _shiftDown;
        private bool _disabled;
        private bool _initialized;
        private bool _shouldRender = true;

        protected string Classname =>
            new CssBuilder("outline-none")
                .AddClass(Class)
                .Build();

        protected ElementReference _firstBumper;
        protected ElementReference _lastBumper;
        protected ElementReference _fallback;
        protected ElementReference _root;

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FocusTrap.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnRootKeyDown { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnRootKeyUp { get; set; }

        /// <summary>
        /// If true, the focus will no longer loop inside the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FocusTrap.Behavior)]
        public bool Disabled
        {
            get => _disabled;
            set
            {
                if (_disabled != value)
                {
                    _disabled = value;
                    _initialized = false;
                }
            }
        }

        /// <summary>
        /// Defines on which element to set the focus when the component is created or enabled.
        /// When DefaultFocus.Element is used, the focus will be set to the FocusTrap itself, so the user will have to press TAB key once to focus the first tabbable element.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FocusTrap.Behavior)]
        public DefaultFocus DefaultFocus { get; set; } = DefaultFocus.FirstChild;

        [Parameter]
        [Category(CategoryTypes.FocusTrap.Behavior)]
        public AutoRestoreFocus AutoRestoreFocus { get; set; } = AutoRestoreFocus.Last;

        private string TrapTabIndex => Disabled ? "-1" : "0";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await SaveFocusAsync();
            }

            if (!_initialized)
            {
                await InitializeFocusAsync();
            }
        }

        private Task OnBottomFocusAsync(FocusEventArgs args)
        {
            return FocusLastAsync();
        }

        private Task OnBumperFocusAsync(FocusEventArgs args)
        {
            return _shiftDown ? FocusLastAsync() : FocusFirstAsync();
        }

        private Task OnRootFocusAsync(FocusEventArgs args)
        {
            return FocusFallbackAsync();
        }

        private Task OnRootKeyDownInternal(KeyboardEventArgs args)
        {
            HandleKeyEvent(args);
            return OnRootKeyDown.InvokeAsync(args);
        }

        private Task OnRootKeyUpInternal(KeyboardEventArgs args)
        {
            HandleKeyEvent(args);
            return OnRootKeyUp.InvokeAsync(args);
        }

        private Task OnTopFocusAsync(FocusEventArgs args)
        {
            return FocusFirstAsync();
        }

        private Task InitializeFocusAsync()
        {
            _initialized = true;

            if (!_disabled)
            {
                switch (DefaultFocus)
                {
                    case DefaultFocus.Element: return FocusFallbackAsync();
                    case DefaultFocus.FirstChild: return FocusFirstAsync();
                    case DefaultFocus.LastChild: return FocusLastAsync();
                }
            }
            return Task.CompletedTask;
        }

        private Task FocusFallbackAsync()
        {
            return _fallback.FocusAsync().AsTask();
        }

        private Task FocusFirstAsync()
        {
            return _root.MudFocusFirstAsync(2, 4).AsTask();
        }

        private Task FocusLastAsync()
        {
            return _root.MudFocusLastAsync(2, 4).AsTask();
        }

        private void HandleKeyEvent(KeyboardEventArgs args)
        {
            _shouldRender = false;
            if (args.Key == "Tab")
            {
                _shiftDown = args.ShiftKey;
            }
        }

        private Task RestoreFocusAsync()
        {
            return _root.MudRestoreFocusAsync().AsTask();
        }

        private Task ClearSavedFocusAsync()
        {
            return _root.MudClearSavedFocusAsync().AsTask();
        }

        private Task RestoreFocusToPreviousAsync()
        {
            return _root.MudRestoreFocusToPreviousAsync().AsTask();
        }

        private Task RestoreFocusToNextAsync()
        {
            return _root.MudRestoreFocusToNextAsync().AsTask();
        }

        private Task SaveFocusAsync()
        {
            return _root.MudSaveFocusAsync().AsTask();
        }

        protected override bool ShouldRender()
        {
            if (_shouldRender)
            {
                return true;
            }

            _shouldRender = true; // auto-reset _shouldRender to true

            return false;
        }

        public ValueTask FocusNextAsync()
        {
            return _root.MudFocusNextAsync();
        }

        public void Dispose()
        {
            if (!_disabled)
            {
                switch (AutoRestoreFocus)
                {
                    case AutoRestoreFocus.None:
                        ClearSavedFocusAsync().AndForget(ignoreExceptions: true);
                        break;

                    case AutoRestoreFocus.Last:
                        RestoreFocusAsync().AndForget(ignoreExceptions: true);
                        break;

                    case AutoRestoreFocus.Previous:
                        RestoreFocusToPreviousAsync().AndForget(ignoreExceptions: true);
                        break;

                    case AutoRestoreFocus.Next:
                        RestoreFocusToNextAsync().AndForget(ignoreExceptions: true);
                        break;
                }
            }
        }
    }
}
