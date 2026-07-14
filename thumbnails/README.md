# Thumbnails

Generate 1600x800 release thumbnails for FiveM scripts — pick a theme, set your
text and images, then export a PNG. 30+ built-in themes, optional grid/grain/
gradient, and a live 1600x800 preview.

Based on [cfx-portal/rk-thumbnails](https://github.com/cfx-portal/rk-thumbnails).

## What's here

The WPF source for the Thumbnails workspace. It shipped inside FiveOS and now
lives here as an optional addon.

- `ViewModels/ThumbnailViewModel.cs` — themes, layout, and state
- `Views/ThumbnailView.xaml` — the 1600x800 render surface + side controls
- `Views/ThumbnailView.xaml.cs` — image picking, film grain, PNG export

## Using it

Drop the files back into a FiveOS build (`ViewModels/` and `Views/`) and add the
view to the sidebar. Exported PNGs land in `Downloads/FiveOS_thumbs`.

## Credit

Ported from [cfx-portal/rk-thumbnails](https://github.com/cfx-portal/rk-thumbnails).
