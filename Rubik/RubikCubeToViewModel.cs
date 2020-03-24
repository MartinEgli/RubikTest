// -----------------------------------------------------------------------
//  <copyright file="RubikCubeToViewModel.cs" company="Anori Soft">
//      Copyright (c) Anori Soft Martin Egli. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Rubik
{
    using Microsoft.Xaml.Behaviors;

    #region

    using System.Windows;

    #endregion

    public class RubikCubeToViewModel : Behavior<RubikCube>
    {
        public static readonly DependencyProperty RubikCubeProperty = DependencyProperty.Register(
            "RubikCube",
            typeof(RubikCube),
            typeof(RubikCubeToViewModel),
            new PropertyMetadata(default(RubikCube)));

        public RubikCube RubikCube
        {
            get => (RubikCube)this.GetValue(RubikCubeProperty);
            set => this.SetValue(RubikCubeProperty, value);
        }

        //public RubikCube RubikCube { get; set; }

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
}