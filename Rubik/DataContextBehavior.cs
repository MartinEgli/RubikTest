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
    public class DataContextBehavior<T> : Behavior<T>
        where T : DependencyObject
    {
        /// <summary>
        ///     The data context property
        /// </summary>
        public static readonly DependencyProperty DataContextProperty = DependencyProperty.Register(
            "DataContext",
            typeof(object),
            typeof(DataContextBehavior<T>),
            new PropertyMetadata(null, OnDataContextPropertyChanged));

        /// <summary>
        ///     Gets or sets the data context.
        /// </summary>
        /// <value>
        ///     The data context.
        /// </value>
        public object DataContext
        {
            get => GetValue(DataContextProperty);
            set => SetValue(DataContextProperty, value);
        }

        /// <summary>
        ///     Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        ///     Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();
            if (DataContext != null && DataContext != DependencyProperty.UnsetValue) return;
            var binding = new Binding();
            BindingOperations.SetBinding(this, DataContextProperty, binding);
        }

        /// <summary>
        ///     Called when [data context attached].
        /// </summary>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnDataContextAttached(object newValue)
        {
        }

        /// <summary>
        ///     Called when [data context detaching].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        protected virtual void OnDataContextDetaching(object oldValue)
        {
        }

        /// <summary>
        ///     Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        ///     Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            BindingOperations.ClearBinding(this, DataContextProperty);
            base.OnDetaching();
        }

        /// <summary>
        ///     Called when [data context property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnDataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataContextBehavior<T>) d).FrameworkElementOnDataContextChanged(d, e);
        }

        /// <summary>
        ///     Frameworks the element on data context changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private void FrameworkElementOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnDataContextDetaching(e.OldValue);
            var properties = this.GetDependencyProperties();
            foreach (var dependencyProperty in properties)
            {
                if (dependencyProperty == DataContextProperty) continue;
                this.UpdateBinding(dependencyProperty);
            }

            OnDataContextAttached(e.NewValue);
        }
    }

    public static class DependencyObjectExtensions
    {
        /// <summary>
        ///     Gets the dependency properties.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">element</exception>
        public static IEnumerable<DependencyProperty> GetDependencyProperties([NotNull] this object element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            return MarkupWriter.GetMarkupObjectFor(element)
                .Properties.Where(markupProperty => markupProperty.DependencyProperty != null)
                .Select(markupProperty => markupProperty.DependencyProperty)
                .ToList();
        }

        /// <summary>
        ///     Updates the binding.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="dependencyProperty">The dependency property.</param>
        /// <returns></returns>
        public static bool UpdateBinding(this DependencyObject target, DependencyProperty dependencyProperty)
        {
            var binding = BindingOperations.GetBinding(target, dependencyProperty);
            var value = target.GetValue(dependencyProperty);

            if (binding == null || binding.Source != null || binding.RelativeSource != null ||
                binding.ElementName != null) return false;
            BindingOperations.ClearBinding(target, dependencyProperty);
            var expression = BindingOperations.SetBinding(target, dependencyProperty, binding);
            switch (binding.Mode)
            {
                case BindingMode.Default:
                case BindingMode.TwoWay:
                case BindingMode.OneWay:
                case BindingMode.OneTime:
                    expression.UpdateTarget();
                    break;

                case BindingMode.OneWayToSource:
                    target.SetValue(dependencyProperty, value);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }
    }
}