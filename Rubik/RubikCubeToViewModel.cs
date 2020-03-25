// -----------------------------------------------------------------------
//  <copyright file="RubikCubeToViewModel.cs" company="Anori Soft">
//      Copyright (c) Anori Soft Martin Egli. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup.Primitives;
using JetBrains.Annotations;

namespace Rubik
{
    using Microsoft.Xaml.Behaviors;
    using System;

    #region

    using System.Windows;

    #endregion

    public class RubikCubeToViewModel : Behavior<RubikCube>
    {
        public static readonly DependencyProperty DataContextProperty = DependencyProperty.Register(
            "DataContext",
            typeof(object),
            typeof(RubikCubeToViewModel),
            new PropertyMetadata(null, OnDataContextPropertyChanged));

        private static void OnDataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RubikCubeToViewModel)d).FrameworkElementOnDataContextChanged(d,e);
        }



        private void FrameworkElementOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnDataContextDetaching(e.OldValue);
            var properties = GetDependencyProperties(this);
            foreach (var dependencyProperty in properties)
            {
                if (dependencyProperty == DataContextProperty)
                {
                    continue;
                }
                var binding = BindingOperations.GetBinding(this, dependencyProperty);
                if (binding == null || binding.Source != null || binding.RelativeSource != null ||
                    binding.ElementName != null) continue;
                BindingOperations.ClearBinding(this, dependencyProperty);
                BindingOperations.SetBinding(this, dependencyProperty, binding);
            }
            OnDataContextAttached(e.NewValue);
        }

        private void OnDataContextAttached(object newValue)
        {
            this.RubikCube = AssociatedObject;

        }

        private void OnDataContextDetaching(object oldValue)
        {
            this.RubikCube = null;
        }

        public static IEnumerable<DependencyProperty> GetDependencyProperties([NotNull] object element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return MarkupWriter.GetMarkupObjectFor(element)
                .Properties.Where(mp => mp.DependencyProperty != null)
                .Select(mp => mp.DependencyProperty)
                .ToList();
        }


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

        public object DataContext
        {
            get => this.GetValue(DataContextProperty);
            set => this.SetValue( DataContextProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (DataContext == null || DataContext == DependencyProperty.UnsetValue)
            {
                var binding = new Binding();
                BindingOperations.SetBinding(this, DataContextProperty, binding);
            }
            this.SetRubikCube();
        }

        protected override void OnDetaching()
        {
            BindingOperations.ClearBinding(this, DataContextProperty);
            base.OnDetaching();
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
 //           this.SetRubikCube();
        }

    }
}