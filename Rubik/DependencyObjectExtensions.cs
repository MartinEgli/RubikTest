using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup.Primitives;
using JetBrains.Annotations;

namespace Rubik
{
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
        /// Updates the binding.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="property">The dependency property.</param>
        /// <returns></returns>
        public static bool UpdateBinding([NotNull] this DependencyObject target,
            [NotNull] DependencyProperty property)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (property == null) throw new ArgumentNullException(nameof(property));
            var binding = BindingOperations.GetBinding(target, property);
            if (binding == null)
            {
                return false;
            }
            target.UpdateBinding(property, binding);
            return true;
        }


        /// <summary>
        /// Updates the binding.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="target">The target.</param>
        /// <param name="dependencyProperty">The dependency property.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static bool UpdateBinding(this DependencyObject target, DependencyProperty dependencyProperty, Binding binding)

        {
            if (binding == null || binding.Source != null || binding.RelativeSource != null ||
                binding.ElementName != null)
            {
                return false;
            }

            if (binding.Mode != BindingMode.OneWayToSource)
            {
                return true;
            }
            var value = target.GetValue(dependencyProperty);
            
            BindingOperations.ClearBinding(target, dependencyProperty);
            BindingOperations.SetBinding(target, dependencyProperty, binding);

            target.SetValue(dependencyProperty, value);

            return true;
        }

        /// <summary>
        /// Updates the binding base.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="dependencyProperty">The dependency property.</param>
        public static void UpdateBindingBase([NotNull] this DependencyObject target, [NotNull] DependencyProperty dependencyProperty)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (dependencyProperty == null) throw new ArgumentNullException(nameof(dependencyProperty));
            var bindingBase = BindingOperations.GetBindingBase(target, dependencyProperty);
            if (bindingBase == null) return;
            UpdateBindingBase(target, dependencyProperty, bindingBase);
        }

        /// <summary>
        /// Updates the binding base.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="dependencyProperty">The dependency property.</param>
        /// <param name="bindingBase">The binding base.</param>
        private static void UpdateBindingBase([NotNull] this DependencyObject target,
            [NotNull] DependencyProperty dependencyProperty,
            [NotNull] BindingBase bindingBase)
        {
            switch (bindingBase)
            {
                case Binding binding:
                    {
                        target.UpdateBinding(dependencyProperty, binding);
                        break;
                    }
                case MultiBinding multiBinding:
                    {
                        target.UpdateMultiBinding(dependencyProperty, multiBinding);
                        break;
                    }
                case PriorityBinding priorityBinding:
                    {
                        target.UpdatePriorityBinding(dependencyProperty, priorityBinding);
                        break;
                    }
            }
        }

        /// <summary>
        /// Updates the multi binding.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="dependencyProperty">The dependency property.</param>
        /// <param name="multiBinding">The multi binding.</param>
        private static void UpdateMultiBinding([NotNull]this DependencyObject target, [NotNull] DependencyProperty dependencyProperty,
            [NotNull] MultiBinding multiBinding)
        {
            foreach (var binding in multiBinding.Bindings)
            {
                target.UpdateBindingBase(dependencyProperty, binding);
            }
        }


        /// <summary>
        /// Updates the priority binding.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="dependencyProperty">The dependency property.</param>
        /// <param name="priorityBinding">The priority binding.</param>
        private static void UpdatePriorityBinding([NotNull]this DependencyObject target, [NotNull]DependencyProperty dependencyProperty,
            [NotNull] PriorityBinding priorityBinding)
        {
            foreach (var binding in priorityBinding.Bindings)
            {
                target.UpdateBindingBase(dependencyProperty, binding);
            }
        }
    }
}