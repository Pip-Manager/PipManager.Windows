using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace PipManager.Controls
{
    public class ToastOptions
    {
        public double ToastWidth { get; set; }
        public double ToastHeight { get; set; }
        public double TextWidth { get; set; }
        public int Time { get; set; } = 2000;
        public SymbolRegular Icon { get; set; } = SymbolRegular.Info24;
        public Brush Foreground { get; set; } = new SolidColorBrush(Colors.Black);
        public Brush IconForeground { get; set; } = new SolidColorBrush(Colors.Black);
        public FontFamily FontFamily { get; set; } = new("Microsoft YaHei");
        public FontWeight FontWeight { get; set; } = SystemFonts.MenuFontWeight;
        public Brush BorderBrush { get; set; } = new SolidColorBrush(Color.FromRgb(229, 229, 229));
        public Thickness BorderThickness { get; set; } = new(2);
        public Brush Background { get; set; } = new SolidColorBrush(Color.FromArgb(40, 0, 255, 0));
        public ApplicationTheme Theme { get; set; } = ApplicationTheme.Light;
        public ToastType ToastType { get; set; } = ToastType.Info;
        public EventHandler<EventArgs>? Closed { get; internal set; }
        public EventHandler<EventArgs>? Click { get; internal set; }
        public Thickness ToastMargin { get; set; } = new(0, 120, 0, 0);
    }

    public enum ToastType
    {
        Info,
        Warning,
        Error,
        Success
    }

    /// <summary>
    /// Toast.xaml 的交互逻辑
    /// </summary>
    public partial class Toast
    {
        private readonly Window? _owner;
        private Popup? _popup;
        private DispatcherTimer? _timer;
        private readonly ArgumentOutOfRangeException _argumentOutOfRangeException = new();

        private Toast()
        {
            InitializeComponent();
            DataContext = this;
        }

        private Toast(Window? owner, string title, string message, ToastOptions? options = null)
        {
            Title = title;
            Message = message;
            InitializeComponent();
            if (options != null)
            {
                if (options.ToastWidth != 0) ToastWidth = options.ToastWidth;
                if (options.ToastHeight != 0) ToastHeight = options.ToastHeight;
                if (options.TextWidth != 0) TextWidth = options.TextWidth;

                Time = options.Time;
                Closed += options.Closed;
                Click += options.Click;
                FontFamily = options.FontFamily;
                FontWeight = options.FontWeight;
                BorderThickness = options.BorderThickness;
                ToastMargin = options.ToastMargin;

                // Theme

                Icon = options.ToastType switch
                {
                    ToastType.Info or ToastType.Warning => SymbolRegular.Info24,
                    ToastType.Error => SymbolRegular.DismissCircle24,
                    ToastType.Success => SymbolRegular.Checkmark24,
                    _ => throw _argumentOutOfRangeException
                };

                IconForeground = options.ToastType switch
                {
                    ToastType.Info => options.Theme switch
                    {
                        ApplicationTheme.Light => new SolidColorBrush(Color.FromRgb(0, 120, 212)),
                        ApplicationTheme.Dark => new SolidColorBrush(Color.FromRgb(76, 194, 255)),
                        _ => throw _argumentOutOfRangeException
                    },
                    ToastType.Warning => options.Theme switch
                    {
                        ApplicationTheme.Light => new SolidColorBrush(Color.FromRgb(157, 93, 0)),
                        ApplicationTheme.Dark => new SolidColorBrush(Color.FromRgb(252, 225, 0)),
                        _ => throw _argumentOutOfRangeException
                    },
                    ToastType.Error => options.Theme switch
                    {
                        ApplicationTheme.Light => new SolidColorBrush(Color.FromRgb(196, 43, 28)),
                        ApplicationTheme.Dark => new SolidColorBrush(Color.FromRgb(255, 153, 164)),
                        _ => throw _argumentOutOfRangeException
                    },
                    ToastType.Success => options.Theme switch
                    {
                        ApplicationTheme.Light => new SolidColorBrush(Color.FromRgb(15, 123, 15)),
                        ApplicationTheme.Dark => new SolidColorBrush(Color.FromRgb(108, 203, 95)),
                        _ => throw _argumentOutOfRangeException
                    },
                    _ => throw _argumentOutOfRangeException
                };

                Background = options.ToastType switch
                {
                    ToastType.Info => options.Theme switch
                    {
                        ApplicationTheme.Light => new SolidColorBrush(Color.FromRgb(244, 244, 244)),
                        ApplicationTheme.Dark => new SolidColorBrush(Color.FromRgb(39, 39, 39)),
                        _ => throw _argumentOutOfRangeException
                    },
                    ToastType.Warning => options.Theme switch
                    {
                        ApplicationTheme.Light => new SolidColorBrush(Color.FromRgb(255, 244, 206)),
                        ApplicationTheme.Dark => new SolidColorBrush(Color.FromRgb(67, 53, 25)),
                        _ => throw _argumentOutOfRangeException
                    },
                    ToastType.Error => options.Theme switch
                    {
                        ApplicationTheme.Light => new SolidColorBrush(Color.FromRgb(253, 231, 233)),
                        ApplicationTheme.Dark => new SolidColorBrush(Color.FromRgb(68, 39, 38)),
                        _ => throw _argumentOutOfRangeException
                    },
                    ToastType.Success => options.Theme switch
                    {
                        ApplicationTheme.Light => new SolidColorBrush(Color.FromRgb(223, 246, 221)),
                        ApplicationTheme.Dark => new SolidColorBrush(Color.FromRgb(57, 61, 27)),
                        _ => throw _argumentOutOfRangeException
                    },
                    _ => throw _argumentOutOfRangeException
                };

                BorderBrush = options.Theme switch
                {
                    ApplicationTheme.Light => new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                    ApplicationTheme.Dark => new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                    _ => throw _argumentOutOfRangeException
                };

                Foreground = options.Theme switch
                {
                    ApplicationTheme.Light => new SolidColorBrush(Colors.Black),
                    ApplicationTheme.Dark => new SolidColorBrush(Colors.White),
                    _ => throw _argumentOutOfRangeException
                };
            }

            DataContext = this;
            _owner = owner ?? Application.Current.MainWindow;
            if (_owner != null) _owner.Closed += Owner_Closed;
        }

        private void Owner_Closed(object? sender, EventArgs e)
        {
            Close();
        }

        public static void Show(string title, string msg, ToastOptions? options = null)
        {
            var toast = new Toast(null, title, msg, options);
            var time = toast.Time;
            ShowToast(toast, time);
        }

        private static void ShowToast(Toast toast, int time)
        {
            toast._popup = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                toast._popup = new Popup
                {
                    PopupAnimation = PopupAnimation.Fade,
                    AllowsTransparency = true,
                    StaysOpen = true,
                    Placement = PlacementMode.Top,
                    IsOpen = false,
                    Child = toast,
                    MinWidth = toast.MinWidth,
                    MaxWidth = toast.MaxWidth,
                    MinHeight = toast.MinHeight,
                    MaxHeight = toast.MaxHeight,
                };

                if (toast.ToastWidth != 0)
                {
                    toast._popup.Width = toast.ToastWidth;
                }

                if (toast.ToastHeight != 0)
                {
                    toast._popup.Height = toast.ToastHeight;
                }

                toast._popup.PlacementTarget = GetPopupPlacementTarget(toast);
                if (toast._owner != null)
                {
                    toast._owner.LocationChanged += toast.UpdatePosition;
                    toast._owner.SizeChanged += toast.UpdatePosition;
                }

                toast._popup.Closed += Popup_Closed;

                toast._popup.IsOpen = true;
                SetPopupOffset(toast._popup, toast);
                toast._popup.IsOpen = false;
                toast._popup.IsOpen = true;
            });

            toast._timer = new DispatcherTimer();
            toast._timer.Tick += OnTimerTick;
            toast._timer.Interval = new TimeSpan(0, 0, 0, 0, time);
            toast._timer.Start();
            return;

            void OnTimerTick(object? sender, EventArgs e)
            {
                if (toast._popup != null) toast._popup.IsOpen = false;
                if (toast._owner != null)
                {
                    toast._owner.LocationChanged -= toast.UpdatePosition;
                    toast._owner.SizeChanged -= toast.UpdatePosition;
                }

                toast._timer.Tick -= OnTimerTick;
            }
        }

        private void UpdatePosition(object? sender, EventArgs e)
        {
            var up = typeof(Popup).GetMethod("UpdatePosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (up == null)
            {
                return;
            }
            SetPopupOffset(_popup, this);
            up.Invoke(_popup, null);
        }

        private static void Popup_Closed(object? sender, EventArgs e)
        {
            if (sender is not Popup popup)
            {
                return;
            }

            if (popup.Child is not Toast toast)
            {
                return;
            }
            toast.RaiseClosed(e);
        }

        private void UserControl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                RaiseClick(e);
            }
        }

        private static Window? GetPopupPlacementTarget(Toast toast)
        {
            return toast._owner;
        }

        private static void SetPopupOffset(Popup? popup, Toast toast)
        {
            if (toast._owner == null) return;
            var ownerWidth = toast._owner.ActualWidth;
            var popupWidth = (popup?.Child as FrameworkElement)?.ActualWidth ?? 0;
            var margin = toast.ToastMargin;

            if (popup == null) return;
            popup.HorizontalOffset = (ownerWidth - popupWidth) / 2;
            popup.VerticalOffset = margin.Top;
        }

        public void Close()
        {
            _timer?.Stop();
            _timer = null;
            if (_popup != null) _popup.IsOpen = false;
            if (_owner == null) return;
            _owner.LocationChanged -= UpdatePosition;
            _owner.SizeChanged -= UpdatePosition;
        }

        private event EventHandler<EventArgs>? Closed;

        private void RaiseClosed(EventArgs e)
        {
            Closed?.Invoke(this, e);
        }

        private event EventHandler<EventArgs>? Click;

        private void RaiseClick(EventArgs e)
        {
            Click?.Invoke(this, e);
        }

        #region Dependency Properties

        private string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        private static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(Toast), new PropertyMetadata(string.Empty));

        private string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        private static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(Toast), new PropertyMetadata(string.Empty));

        private new Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        private new static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(Toast), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(229, 229, 229))));

        private new Thickness BorderThickness
        {
            get => (Thickness)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        private new static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(nameof(BorderThickness), typeof(Thickness), typeof(Toast), new PropertyMetadata(new Thickness(0)));

        private new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        private new static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(Toast), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(40, 0, 255, 0))));

        private double ToastWidth
        {
            get => (double)GetValue(ToastWidthProperty);
            set => SetValue(ToastWidthProperty, value);
        }

        private static readonly DependencyProperty ToastWidthProperty =
            DependencyProperty.Register(nameof(ToastWidth), typeof(double), typeof(Toast), new PropertyMetadata(0.0));

        private double ToastHeight
        {
            get => (double)GetValue(ToastHeightProperty);
            set => SetValue(ToastHeightProperty, value);
        }

        private static readonly DependencyProperty ToastHeightProperty =
            DependencyProperty.Register(nameof(ToastHeight), typeof(double), typeof(Toast), new PropertyMetadata(0.0));

        private SymbolRegular Icon
        {
            get => (SymbolRegular)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        private static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(SymbolRegular), typeof(Toast), new PropertyMetadata(SymbolRegular.Info20));

        private int Time
        {
            get => (int)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }

        private static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register(nameof(Time), typeof(int), typeof(Toast), new PropertyMetadata(2000));

        public double TextWidth
        {
            get => (double)GetValue(TextWidthProperty);
            set => SetValue(TextWidthProperty, value);
        }

        public static readonly DependencyProperty TextWidthProperty =
            DependencyProperty.Register(nameof(TextWidth), typeof(double), typeof(Toast), new PropertyMetadata(double.MaxValue));

        public Thickness ToastMargin
        {
            get => (Thickness)GetValue(ToastMarginProperty);
            set => SetValue(ToastMarginProperty, value);
        }

        public static readonly DependencyProperty ToastMarginProperty =
            DependencyProperty.Register(nameof(ToastMargin), typeof(Thickness), typeof(Toast), new PropertyMetadata(new Thickness(0)));

        private Brush IconForeground
        {
            get => (Brush)GetValue(IconForegroundProperty);
            set => SetValue(IconForegroundProperty, value);
        }

        private static readonly DependencyProperty IconForegroundProperty =
            DependencyProperty.Register(nameof(IconForeground), typeof(Brush), typeof(Toast), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        #endregion Dependency Properties
    }
}