// Copyright (c) 2026 FiveOS. All rights reserved. See LICENSE.
// https://github.com/w3bportal/FiveOS
// Adapted from cfx-portal/rk-thumbnails - https://github.com/cfx-portal/rk-thumbnails

using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using FiveOS.ViewModels;

namespace FiveOS.Views;

/// <summary>
/// Thumbnails plugin view — port of cfx-portal/rk-thumbnails. The 1600x800
/// preview surface drives both on-screen rendering (scaled by the
/// surrounding Viewbox) and PNG export.
/// </summary>
public partial class ThumbnailView : UserControl
{
    private WriteableBitmap? _grainBitmap;

    public ThumbnailView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is INotifyPropertyChanged oldVm)
            oldVm.PropertyChanged -= OnVmPropertyChanged;
        if (e.NewValue is INotifyPropertyChanged newVm)
            newVm.PropertyChanged += OnVmPropertyChanged;

        UpdateGrain();
    }

    private void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ThumbnailViewModel.ShowGrain))
            UpdateGrain();
    }

    private void UpdateGrain()
    {
        if (DataContext is not ThumbnailViewModel vm) return;
        if (!vm.ShowGrain) { GrainLayer.Source = null; return; }

        if (_grainBitmap is null)
        {
            const int W = 1600, H = 800;
            var wb = new WriteableBitmap(W, H, 96, 96, PixelFormats.Bgra32, null);
            var rng = new Random(1337);
            var pixels = new byte[W * H * 4];
            for (int i = 0; i < pixels.Length; i += 4)
            {
                var v = (byte)rng.Next(0, 256);
                pixels[i + 0] = v;
                pixels[i + 1] = v;
                pixels[i + 2] = v;
                pixels[i + 3] = 255;
            }
            wb.WritePixels(new Int32Rect(0, 0, W, H), pixels, W * 4, 0);
            wb.Freeze();
            _grainBitmap = wb;
        }
        GrainLayer.Source = _grainBitmap;
    }

    private static string? PickImageFile(string title)
    {
        var dlg = new OpenFileDialog
        {
            Title = title,
            Filter = "Image files (*.png;*.jpg;*.jpeg;*.webp;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.webp;*.gif;*.bmp|All files (*.*)|*.*",
            CheckFileExists = true,
        };
        return dlg.ShowDialog() == true ? dlg.FileName : null;
    }

    private void OnPickLogo(object sender, RoutedEventArgs e)
    {
        if (DataContext is not ThumbnailViewModel vm) return;
        if (PickImageFile("Choose logo image") is { } p) vm.LogoPath = p;
    }

    private void OnPickImage(object sender, RoutedEventArgs e)
    {
        if (DataContext is not ThumbnailViewModel vm) return;
        if (PickImageFile("Choose main image") is { } p) vm.ImagePath = p;
    }

    private void OnPickFooterLogo(object sender, RoutedEventArgs e)
    {
        if (DataContext is not ThumbnailViewModel vm) return;
        if (PickImageFile("Choose footer-right logo") is { } p) vm.FooterRightLogoPath = p;
    }

    private void OnExport(object sender, RoutedEventArgs e)
    {
        if (DataContext is not ThumbnailViewModel vm) return;

        try
        {
            var surface = ThumbnailSurface;
            surface.UpdateLayout();

            var rtb = new RenderTargetBitmap(
                pixelWidth: 1600,
                pixelHeight: 800,
                dpiX: 96,
                dpiY: 96,
                pixelFormat: PixelFormats.Pbgra32);
            rtb.Render(surface);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            var folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads", "FiveOS_thumbs");
            Directory.CreateDirectory(folder);

            var safeTitle = MakeSafeFileName(vm.TitleText);
            var name = $"{safeTitle}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            var path = Path.Combine(folder, name);

            using (var fs = File.Create(path))
                encoder.Save(fs);

            vm.Summary = $"Exported → {path}";
            vm.SetStatus(vm.Summary);

            try
            {
                Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{path}\"") { UseShellExecute = true });
            }
            catch { /* ignore — file is saved either way */ }
        }
        catch (Exception ex)
        {
            vm.Summary = $"Export failed: {ex.Message}";
            vm.SetStatus(vm.Summary);
        }
    }

    private static string MakeSafeFileName(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "thumbnail";
        foreach (var ch in Path.GetInvalidFileNameChars())
            s = s.Replace(ch, '_');
        s = s.Replace(' ', '_');
        return s.Length > 48 ? s.Substring(0, 48) : s;
    }
}
