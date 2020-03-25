using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup.Primitives;
using JetBrains.Annotations;
using Microsoft.Xaml.Behaviors;

namespace Rubik
{
    public class GridToViewModel : Behavior<Grid>
    {
        /// <summary>
        /// The grid property
        /// </summary>
        public static readonly DependencyProperty GridProperty = DependencyProperty.Register(
            "Grid",
            typeof(Grid),
            typeof(GridToViewModel),
            new PropertyMetadata(default(Grid), OnGridPropertyChanged));

        /// <summary>
        /// Called when [grid property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnGridPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is FrameworkElement oldElement)
            {
                oldElement.DataContextChanged -= ((GridToViewModel)d).FrameworkElementOnDataContextChanged;
            }

            if (e.NewValue is FrameworkElement newElement)
            {
                newElement.DataContextChanged += ((GridToViewModel)d).FrameworkElementOnDataContextChanged;
            }

        }

        /// <summary>
        /// Gets the dependency properties.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">element</exception>
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

        /// <summary>
        /// Frameworks the element on data context changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void FrameworkElementOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var properties = GetDependencyProperties(this);
            foreach (var dependencyProperty in properties)
            {
                var binding = BindingOperations.GetBinding(this, dependencyProperty);
                if (binding != null && binding.Source == null && binding.RelativeSource == null &&
                    binding.ElementName == null)
                {
                    BindingOperations.ClearBinding(this, dependencyProperty);
                    BindingOperations.SetBinding(this, dependencyProperty, binding);
                }
            }

            if ((!(this.AssociatedObject is Grid grid)))
            {
                return;
            }

            this.Grid = grid;
        }

        /// <summary>
        /// Gets or sets the grid.
        /// </summary>
        /// <value>
        /// The grid.
        /// </value>
        public Grid Grid
        {
            get => (Grid)this.GetValue(GridProperty);

            set => this.SetValue(GridProperty, value);
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.SetRubikCube();
            AssociatedObject.DataContextChanged += FrameworkElementOnDataContextChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.DataContextChanged -= FrameworkElementOnDataContextChanged; ;
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
            //           this.SetRubikCube();
        }
    }
}