using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace DeepSeekBox.Controls;

public class Badge : TemplatedControl
{
    public static readonly StyledProperty<object?> TextProperty = AvaloniaProperty.Register<Badge, object?>(nameof(Text));

    public object? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}