// Copyright (c) 2026 FiveOS. All rights reserved. See LICENSE.
// https://github.com/w3bportal/FiveOS

using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FiveOS.ViewModels;

/// <summary>
/// View-model for the Thumbnails plugin — port of cfx-portal/rk-thumbnails.
/// Generates 1600x800 thumbnail images for FiveM script releases.
/// </summary>
public partial class ThumbnailViewModel : ObservableObject
{
    private readonly Action<string> _setStatus;

    public ThumbnailViewModel(Action<string> setStatus)
    {
        _setStatus = setStatus;

        Themes = new ObservableCollection<ThumbnailTheme>(BuildThemes());
        SelectedTheme = Themes.First(t => t.Key == "default");
    }

    public ObservableCollection<ThumbnailTheme> Themes { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PageBgBrush))]
    [NotifyPropertyChangedFor(nameof(ControlsBgBrush))]
    [NotifyPropertyChangedFor(nameof(InputBgBrush))]
    [NotifyPropertyChangedFor(nameof(InputBorderBrush))]
    [NotifyPropertyChangedFor(nameof(ThumbnailBgBrush))]
    [NotifyPropertyChangedFor(nameof(ThumbnailGridColor))]
    [NotifyPropertyChangedFor(nameof(MainTextBrush))]
    [NotifyPropertyChangedFor(nameof(SecondaryTextBrush))]
    private ThumbnailTheme _selectedTheme = null!;

    public Brush PageBgBrush => SelectedTheme.PageBg;
    public Brush ControlsBgBrush => SelectedTheme.ControlsBg;
    public Brush InputBgBrush => SelectedTheme.InputBg;
    public Brush InputBorderBrush => SelectedTheme.InputBorder;
    public Brush ThumbnailBgBrush => SelectedTheme.ThumbnailBg;
    public Color ThumbnailGridColor => SelectedTheme.ThumbnailGridColor;
    public Brush MainTextBrush => SelectedTheme.MainText;
    public Brush SecondaryTextBrush => SelectedTheme.SecondaryText;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDefaultLayout))]
    private bool _isTextOnly;

    public bool IsDefaultLayout => !IsTextOnly;

    [ObservableProperty] private bool _showGrid = true;
    [ObservableProperty] private bool _showGrain;
    [ObservableProperty] private double _grainOpacity = 0.15;

    [ObservableProperty] private string _handleText = "";
    [ObservableProperty] private string _titleText = "Input Data";
    [ObservableProperty] private string _rightHeaderText = "Input Data";
    [ObservableProperty] private string _footerText = "Input Data";
    [ObservableProperty] private string _footerLeftText = "Invite Only";
    [ObservableProperty] private string _footerRightText = "Not Affiliated With Cfx.re/Rockstar Games.";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LogoSource))]
    [NotifyPropertyChangedFor(nameof(HasLogo))]
    private string _logoPath = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ImageSource))]
    [NotifyPropertyChangedFor(nameof(HasImage))]
    private string _imagePath = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FooterLogoSource))]
    [NotifyPropertyChangedFor(nameof(HasFooterLogo))]
    private string _footerRightLogoPath = string.Empty;

    public ImageSource? LogoSource => TryLoadImage(LogoPath);
    public ImageSource? ImageSource => TryLoadImage(ImagePath);
    public ImageSource? FooterLogoSource => TryLoadImage(FooterRightLogoPath);

    public bool HasLogo => LogoSource is not null;
    public bool HasImage => ImageSource is not null;
    public bool HasFooterLogo => FooterLogoSource is not null;

    private static ImageSource? TryLoadImage(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;
        try
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            if (Uri.TryCreate(path, UriKind.Absolute, out var uri) && uri.Scheme is "http" or "https" or "file")
            {
                bmp.UriSource = uri;
            }
            else if (File.Exists(path))
            {
                bmp.UriSource = new Uri(Path.GetFullPath(path), UriKind.Absolute);
            }
            else
            {
                return null;
            }
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }
        catch
        {
            return null;
        }
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ImageCardSize))]
    [NotifyPropertyChangedFor(nameof(ImageReflectionHeight))]
    private double _imageScale = 100;

    public double ImageCardSize => 600 * (ImageScale / 100.0);
    public double ImageReflectionHeight => ImageCardSize * 0.35;

    [ObservableProperty] private bool _gradientEnabled;
    [ObservableProperty] private string _gradientColorStart = "#1a1e2a";
    [ObservableProperty] private string _gradientColorEnd = "#2a1020";

    [ObservableProperty] private string _summary = "Tweak text + theme on the left, then click EXPORT PNG.";

    public void SetStatus(string s) => _setStatus(s);

    private static IEnumerable<ThumbnailTheme> BuildThemes()
    {
        var rows = new (string, string, string, string, string, string, string, string, string, string, string)[]
        {
            ("default",          "Default",          "Dark",     "#10121a", "#222533", "#2a2c3a", "#2a2c3a", "#1a1e2a", "#26FFFFFF", "#ffffff", "#a9b1d6"),
            ("midnight-blue",    "Midnight Blue",    "Dark",     "#0a0f1d", "#101830", "#1c2849", "#1c2849", "#0c142d", "#1AADD8E6", "#e0eafc", "#8a9fcf"),
            ("slate-gray",       "Slate Gray",       "Dark",     "#2d3748", "#4a5568", "#718096", "#718096", "#1a202c", "#0FFFFFFF", "#e2e8f0", "#a0aec0"),
            ("charcoal",         "Charcoal",         "Dark",     "#232B2B", "#36454F", "#414a4c", "#505a5f", "#202020", "#0DFFFFFF", "#ffffff", "#c0c0c0"),
            ("deep-space",       "Deep Space",       "Dark",     "#0a0a1a", "#1a1a2e", "#2a2a4d", "#3d3d66", "#0f0f24", "#264B0082", "#c9c9ff", "#9370db"),
            ("obsidian-dark",    "Obsidian Dark",    "Dark",     "#0d0d0d", "#1d1d1d", "#2d2d2d", "#3d3d3d", "#0f0f0f", "#08FFFFFF", "#ffffff", "#a0a0a0"),
            ("silver-metallic",  "Silver Metallic",  "Dark",     "#1a1a1a", "#2a2a2a", "#3a3a3a", "#4a4a4a", "#0f0f0f", "#14C0C0C0", "#e0e0e0", "#a0a0a0"),
            ("platinum-elegant", "Platinum Elegant", "Dark",     "#0f0f0f", "#1f1f1f", "#2f2f2f", "#3f3f3f", "#0a0a0a", "#0FE5E4E2", "#f5f5f5", "#c0c0c0"),
            ("arctic-light",     "Arctic Light",     "Light",    "#757575", "#ffffff", "#f0f4f8", "#d1d9e6", "#f0f4f8", "#12000000", "#24292e", "#586069"),
            ("matrix-green",     "Matrix Green",     "Colorful", "#050a05", "#0a140a", "#0f200f", "#0f200f", "#0d130d", "#2600FF00", "#33ff33", "#00cc00"),
            ("crimson-red",      "Crimson Red",      "Colorful", "#1f0d0d", "#2b1212", "#401a1a", "#401a1a", "#1a0a0a", "#26DC143C", "#ffcdd2", "#dc143c"),
            ("sunset-orange",    "Sunset Orange",    "Colorful", "#2a1a05", "#3f280a", "#5c3a0f", "#5c3a0f", "#2a1a0a", "#1AFFA500", "#ffd700", "#ffa500"),
            ("cyberpunk-neon",   "Cyberpunk Neon",   "Colorful", "#0a0a0f", "#1a1a2e", "#16213e", "#0f3460", "#0f0f23", "#1A00FFFF", "#00ffff", "#ff00ff"),
            ("synthwave-purple", "Synthwave Purple", "Colorful", "#1a0d26", "#2d1b3d", "#3d2352", "#4a2c5a", "#1f1129", "#268B5CF6", "#f0c0ff", "#c084fc"),
            ("forest-green",     "Forest Green",     "Colorful", "#0d1b0d", "#1a2e1a", "#2d4a2d", "#3d5a3d", "#0f1f0f", "#1A228B22", "#90ee90", "#7cfc00"),
            ("ocean-blue",       "Ocean Blue",       "Colorful", "#0c1821", "#1e3a5f", "#2a4d6b", "#3d6b8c", "#0f1f2a", "#1A4682B4", "#87ceeb", "#4682b4"),
            ("rose-gold",        "Rose Gold",        "Colorful", "#2a1a1f", "#3d2832", "#4a3240", "#5a3d4f", "#2d1d25", "#1ACD919E", "#f4c2c2", "#cd919e"),
            ("mint-fresh",       "Mint Fresh",       "Colorful", "#0f1f1a", "#1a332a", "#2a4d3d", "#3d6650", "#12241e", "#1A20B2AA", "#98fb98", "#20b2aa"),
            ("cosmic-purple",    "Cosmic Purple",    "Colorful", "#0b0b1f", "#1a1a3a", "#2d2d5a", "#3d3d6b", "#0f0f2a", "#266A5ACD", "#dda0dd", "#9370db"),
            ("amber-glow",       "Amber Glow",       "Colorful", "#1f1a0d", "#332a1a", "#4d3d2a", "#66503d", "#241e12", "#1AFF8C00", "#ffd700", "#ff8c00"),
            ("ice-blue",         "Ice Blue",         "Colorful", "#0d1a1f", "#1a2d33", "#2a404d", "#3d5366", "#0f1e24", "#1A4169E1", "#b0e0e6", "#87ceeb"),
            ("volcanic-red",     "Volcanic Red",     "Colorful", "#1f0a0a", "#331a1a", "#4d2a2a", "#663d3d", "#240f0f", "#1AFF6B6B", "#ff6b6b", "#ff4757"),
            ("neon-lime",        "Neon Lime",        "Colorful", "#0a1f0a", "#1a331a", "#2a4d2a", "#3d663d", "#0f240f", "#2600FF00", "#32ff32", "#00ff00"),
            ("golden-hour",      "Golden Hour",      "Colorful", "#2a1f0d", "#3d2f1a", "#4d3f2a", "#66523d", "#2d2412", "#1AFDCB6E", "#ffeaa7", "#fdcb6e"),
            ("teal-ocean",       "Teal Ocean",       "Colorful", "#0a2e2e", "#1a4a4a", "#2a6a6a", "#3a8a8a", "#0f3535", "#1F20B2AA", "#afeeee", "#48d1cc"),
            ("magenta-neon",     "Magenta Neon",     "Colorful", "#2e0a2e", "#4a1a4a", "#6a2a6a", "#8a3a8a", "#350f35", "#26FF1493", "#ff69b4", "#ff1493"),
            ("copper-warm",      "Copper Warm",      "Colorful", "#2e1a0a", "#4a2a1a", "#6a3a2a", "#8a4a3a", "#351f0f", "#1FCD853F", "#daa520", "#cd853f"),
            ("bronze-classic",   "Bronze Classic",   "Colorful", "#2a1a0a", "#3a2a1a", "#4a3a2a", "#5a4a3a", "#2f1f0f", "#1FB8860B", "#cd7f32", "#b8860b"),
            ("jade-natural",     "Jade Natural",     "Colorful", "#0a2e1a", "#1a4a2a", "#2a6a3a", "#3a8a4a", "#0f351f", "#1F00A86B", "#98fb98", "#00a86b"),
            ("coral-reef",       "Coral Reef",       "Colorful", "#2e0a1a", "#4a1a2a", "#6a2a3a", "#8a3a4a", "#350f1f", "#1FFF7F50", "#ff7f50", "#ff6347"),
            ("indigo-mystic",    "Indigo Mystic",    "Colorful", "#0a0a2e", "#1a1a4a", "#2a2a6a", "#3a3a8a", "#0f0f35", "#266A0DAD", "#e6e6fa", "#9370db"),
            ("golden-royal",     "Golden Royal",     "Colorful", "#2e2a0a", "#4a461a", "#6a662a", "#8a863a", "#35350f", "#1FDAA520", "#ffd700", "#daa520"),
        };

        foreach (var r in rows)
        {
            yield return new ThumbnailTheme
            {
                Key = r.Item1,
                Name = r.Item2,
                Category = r.Item3,
                PageBg = ParseBrush(r.Item4),
                ControlsBg = ParseBrush(r.Item5),
                InputBg = ParseBrush(r.Item6),
                InputBorder = ParseBrush(r.Item7),
                ThumbnailBg = ParseBrush(r.Item8),
                ThumbnailGridColor = (Color)ColorConverter.ConvertFromString(r.Item9),
                MainText = ParseBrush(r.Item10),
                SecondaryText = ParseBrush(r.Item11),
            };
        }
    }

    private static SolidColorBrush ParseBrush(string hex)
    {
        var c = (Color)ColorConverter.ConvertFromString(hex);
        var b = new SolidColorBrush(c);
        b.Freeze();
        return b;
    }
}

public sealed class ThumbnailTheme
{
    public string Key { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public Brush PageBg { get; init; } = Brushes.Black;
    public Brush ControlsBg { get; init; } = Brushes.Black;
    public Brush InputBg { get; init; } = Brushes.Black;
    public Brush InputBorder { get; init; } = Brushes.Black;
    public Brush ThumbnailBg { get; init; } = Brushes.Black;
    public Color ThumbnailGridColor { get; init; } = Colors.White;
    public Brush MainText { get; init; } = Brushes.White;
    public Brush SecondaryText { get; init; } = Brushes.White;

    public override string ToString() => $"{Name} · {Category}";
}
