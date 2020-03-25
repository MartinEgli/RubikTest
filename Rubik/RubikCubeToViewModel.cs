// -----------------------------------------------------------------------
//  <copyright file="RubikCubeToViewModel.cs" company="Anori Soft">
//      Copyright (c) Anori Soft Martin Egli. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Rubik
{
    using Microsoft.Xaml.Behaviors;
    using System;

    #region

    using System.Windows;
    using System.Windows.Controls;

    #endregion

    public class RubikCubeToViewModel : Behavior<RubikCube>
    {
        public static readonly DependencyProperty RubikCubeProperty = DependencyProperty.Register(
            "RubikCube",
            typeof(RubikCube),
            typeof(RubikCubeToViewModel),
            new PropertyMetadata(default(RubikCube), OnRubikCubePropertyChanged));

        private static void OnRubikCubePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public RubikCube RubikCube
        {
            get
            {
                return (RubikCube)this.GetValue(RubikCubeProperty);
            }

            set
            {
                this.SetValue(RubikCubeProperty, value);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.SetRubikCube();
        }

        private void SetRubikCube()
        {
            if ((!(this.AssociatedObject is RubikCube rubikCube)))
            {
                return;
            }

            if (rubikCube.Equals(this.RubikCube))
            {
                return;
            }

            this.RubikCube = rubikCube;
        }

        protected override void OnChanged()
        {
            base.OnChanged();
            this.SetRubikCube();
        }
    }

    public class GridToViewModel : Behavior<Grid>
    {
        public static readonly DependencyProperty GridProperty = DependencyProperty.Register(
            "Grid",
            typeof(Grid),
            typeof(GridToViewModel),
            new PropertyMetadata(default(Grid), OnGridPropertyChanged));

        private static void OnGridPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public Grid Grid
        {
            get
            {
                return (Grid)this.GetValue(GridProperty);
            }

            set
            {
                this.SetValue(GridProperty, value);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.SetRubikCube();
            AssociatedObject.DataContextChanged += OnDataContextChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.DataContextChanged -= OnDataContextChanged;
            base.OnDetaching();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.Grid = AssociatedObject;
        }

        private void SetRubikCube()
        {
            if ((!(this.AssociatedObject is Grid grid)))
            {
                return;
            }

            if (grid.Equals(this.Grid))
            {
                return;
            }

            this.Grid = grid;
        }

        protected override void OnChanged()
        {
            base.OnChanged();
            this.SetRubikCube();
        }
    }
}