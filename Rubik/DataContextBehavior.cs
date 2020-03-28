using System.Windows;
using System.Windows.Data;
using Microsoft.Xaml.Behaviors;

namespace Rubik
{
    public class DataContextBehavior<T> : Behavior<T>
        where T : DependencyObject
    {
        /// <summary>
        ///     The data context property
        /// </summary>
        protected static readonly DependencyProperty DataContextProperty = DependencyProperty.Register(
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
            protected set => SetValue(DataContextProperty, value);
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
            if (DataContext != null && DataContext != DependencyProperty.UnsetValue)
            {
                return;
            }
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
        /// <param name="dependencyObject">The dependencyObject.</param>
        /// <param name="eventArgs">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnDataContextPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            ((DataContextBehavior<T>) dependencyObject).OnDataContextChanged(dependencyObject, eventArgs);
        }

        /// <summary>
        ///     Frameworks the element on data context changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            OnDataContextDetaching(eventArgs.OldValue);
            var properties = this.GetDependencyProperties();
            foreach (var dependencyProperty in properties)
            {
                if (dependencyProperty == DataContextProperty)
                {
                    continue;
                }
                this.UpdateBindingBase(dependencyProperty);
            }
            OnDataContextAttached(eventArgs.NewValue);
        }
    }
}