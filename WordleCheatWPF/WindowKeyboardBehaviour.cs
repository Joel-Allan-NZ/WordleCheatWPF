using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WordleCheatWPF
{
    class WindowKeyboardBehaviour : Behavior<Window>
    {
        protected override void OnAttached()
        {
            AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;
        }

        private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                if (e.Key == Key.Back)
                {
                    DeleteCommand.Execute(null);
                    e.Handled = true;
                }
                else if (e.Key >= Key.A && e.Key <= Key.Z)
                {
                    TypeCommand.Execute(e.Key.ToString());
                    e.Handled = true;
                }
                else if (e.Key == Key.Enter)
                {
                    EnterCommand.Execute(null);
                    e.Handled = true;
                }
            }           
        }


        /// <summary>
        /// Bindable command that fires on backspace press 
        /// </summary>
        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeleteCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(WindowKeyboardBehaviour), new PropertyMetadata(null));


        /// <summary>
        /// Bindable command that fires on alphabetical key presses (without modifiers)
        /// </summary>
        public ICommand TypeCommand
        {
            get { return (ICommand)GetValue(TypeCommandProperty); }
            set { SetValue(TypeCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TypeCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeCommandProperty =
            DependencyProperty.Register("TypeCommand", typeof(ICommand), typeof(WindowKeyboardBehaviour), new PropertyMetadata(null));

        /// <summary>
        /// Bindable command that fires on keyboard Enter press.
        /// </summary>
        public ICommand EnterCommand
        {
            get { return (ICommand)GetValue(EnterCommandProperty); }
            set { SetValue(EnterCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnterCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnterCommandProperty =
            DependencyProperty.Register("EnterCommand", typeof(ICommand), typeof(WindowKeyboardBehaviour), new PropertyMetadata(null));




    }
}
