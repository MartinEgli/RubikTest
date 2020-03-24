// -----------------------------------------------------------------------
//  <copyright file="RubikView.xaml.cs" company="Anori Soft">
//      Copyright (c) Anori Soft Martin Egli. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace RubikDemo
{
    #region

    using System.Windows;

    #endregion

    /// <summary>
    /// Interaction logic for RubikView.xaml
    /// </summary>
    public partial class RubikView
    {
        public RubikView()
        {
            this.InitializeComponent();
            this.Loaded += this.MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            this.view1.ZoomExtents();
        }
    }
}