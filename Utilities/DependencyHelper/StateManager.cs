﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Utilities.DependencyHelper
{
    /// <summary>
    /// Using the VisualStateManager with the Model-View-Viewmodel (MVVM) pattern in WPF or Silverlight
    /// How to use: In Xaml "VisualStateName" is a string
    /// <UserControl x:Class="MyApp.SomeView" MyApp:StateManager.VisualStateProperty="{Binding VisualStateName}"></UserControl>
    /// </summary>
    public class StateManager : DependencyObject
    {
        public static string GetVisualStateProperty(DependencyObject obj)
        {
            return (string)obj.GetValue(VisualStatePropertyProperty);
        }

        public static void SetVisualStateProperty(DependencyObject obj, string value)
        {
            obj.SetValue(VisualStatePropertyProperty, value);
        }

        public static readonly DependencyProperty VisualStatePropertyProperty =
            DependencyProperty.RegisterAttached(
            "VisualStateProperty",
            typeof(string),
            typeof(StateManager),
            new PropertyMetadata((s, e) =>
            {
                var propertyName = (string)e.NewValue;
                var ctrl = s as Control;
                if (ctrl == null)
                    throw new InvalidOperationException("This attached property only supports types derived from Control.");
                System.Windows.VisualStateManager.GoToState(ctrl, (string)e.NewValue, true);
            }));
    }
}
