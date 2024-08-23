using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Cyclone.LicenseManagement.Server.Controls
{
    public class FormItem : ContentControl
    {
        #region Label

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(object), typeof(FormItem), new FrameworkPropertyMetadata(default(object)));

        public object Label
        {
            get => (object)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        #endregion Label

        #region SharedName

        public static readonly DependencyProperty SharedNameProperty =
                            DependencyProperty.Register(nameof(SharedName), typeof(string), typeof(FormItem), new PropertyMetadata(default(string), OnSharedNameChanged));

        public string SharedName
        {
            get => (string)GetValue(SharedNameProperty);
            set => SetValue(SharedNameProperty, value);
        }

        private static void FormItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is FormItem formItem && formItem.Parent is Panel panel)
            {
                panel.SetValue(Grid.IsSharedSizeScopeProperty, true);
            }
        }

        private static void OnSharedNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FormItem formItem)
            {
                if (!string.IsNullOrEmpty(e.NewValue?.ToString()))
                {
                    formItem.Loaded += FormItem_Loaded;
                }
                else
                {
                    formItem.Loaded -= FormItem_Loaded;
                }
            }
        }

        #endregion SharedName

        #region AttachedObject

        public static readonly DependencyProperty AttachedObjectProperty =
            DependencyProperty.Register(nameof(AttachedObject), typeof(object), typeof(FormItem), new FrameworkPropertyMetadata(default(object)));

        public object AttachedObject
        {
            get => (object)GetValue(AttachedObjectProperty);
            set => SetValue(AttachedObjectProperty, value);
        }

        #endregion AttachedObject

        #region LabelHorizontalAlignment

        public static readonly DependencyProperty LabelHorizontalAlignmentProperty = StackPanel.HorizontalAlignmentProperty.AddOwner(typeof(FormItem), new FrameworkPropertyMetadata(HorizontalAlignment.Left));

        public HorizontalAlignment LabelHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(LabelHorizontalAlignmentProperty);
            set => SetValue(LabelHorizontalAlignmentProperty, value);
        }

        #endregion LabelHorizontalAlignment

        #region IsRequired

        public static readonly DependencyProperty IsRequiredProperty =
            DependencyProperty.Register(nameof(IsRequired), typeof(bool), typeof(FormItem), new PropertyMetadata(default(bool)));

        public bool IsRequired
        {
            get => (bool)GetValue(IsRequiredProperty);
            set => SetValue(IsRequiredProperty, value);
        }

        #endregion IsRequired
    }
}