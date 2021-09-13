using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ToDue2_FrameworkImpl
{
	public class MyDatePicker : DatePicker
	{
        public Popup Popup { get; private set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template.FindName("PART_Button", this) is Button button)
            {
                button.Visibility = System.Windows.Visibility.Collapsed;
            }

            Popup = Template.FindName("PART_Popup", this) as Popup;
            var textbox = Template.FindName("PART_TextBox", this) as TextBox;
            textbox.GotMouseCapture += (s, e) => Popup.IsOpen = true;
            SelectedDateChanged += (s, e) =>
			{
                var todo = (Parent as Grid).DataContext as TodoItem;
				if (todo == null)
				{
                    (App.Current.MainWindow as MainWindow).DisplayedDueDate = (DateTime)SelectedDate;
				}
				else
				{
                    todo.DueDate = (DateTime)SelectedDate;
                }
			};
        }
	}
}
