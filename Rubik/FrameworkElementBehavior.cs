using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup.Primitives;
using JetBrains.Annotations;
using Microsoft.Xaml.Behaviors;

namespace Rubik
{
    public abstract class FrameworkElementBehavior<TFrameworkElement> : Behavior<TFrameworkElement>
        where TFrameworkElement : FrameworkElement
    {

        /// <summary>
        /// Called when [framework element property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected static void OnFrameworkElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is FrameworkElement oldElement)
            {
                oldElement.DataContextChanged -= ((FrameworkElementBehavior<TFrameworkElement>)d).FrameworkElementOnDataContextChanged;
            }

            if (e.NewValue is FrameworkElement newElement)
            {
                newElement.DataContextChanged -= ((FrameworkElementBehavior<TFrameworkElement>)d).FrameworkElementOnDataContextChanged;
                newElement.DataContextChanged += ((FrameworkElementBehavior<TFrameworkElement>)d).FrameworkElementOnDataContextChanged;
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
            OnDataContextDetaching(e.OldValue);
            var properties = GetDependencyProperties(this);
            foreach (var dependencyProperty in properties)
            {
                var binding = BindingOperations.GetBinding(this, dependencyProperty);
                if (binding == null || binding.Source != null || binding.RelativeSource != null ||
                    binding.ElementName != null) continue;
                BindingOperations.ClearBinding(this, dependencyProperty);
                BindingOperations.SetBinding(this, dependencyProperty, binding);
            }
            OnDataContextAttached(e.NewValue);
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
            AssociatedObject.DataContextChanged += FrameworkElementOnDataContextChanged;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            AssociatedObject.DataContextChanged -= FrameworkElementOnDataContextChanged; ;
            base.OnDetaching();
        }

        /// <summary>
        /// Called when [data context attached].
        /// </summary>
        /// <param name="obj">The object.</param>
        protected virtual void OnDataContextAttached(object obj) { }

        /// <summary>
        /// Called when [data context detaching].
        /// </summary>
        /// <param name="obj">The object.</param>
        protected virtual void OnDataContextDetaching(object obj) { }


    }
}