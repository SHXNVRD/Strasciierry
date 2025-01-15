﻿using Microsoft.UI.Xaml.Controls;

using Strasciierry.UI.ViewModels;

namespace Strasciierry.UI.Views;

public sealed partial class ImageConverterPage : Page
{
    public ImageConverterViewModel ViewModel
    {
        get;
    }

    public ImageConverterPage()
    {
        ViewModel = App.GetService<ImageConverterViewModel>();
        InitializeComponent();
    }
}
