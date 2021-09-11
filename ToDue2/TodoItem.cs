using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using ToDue2.Properties;

namespace ToDue2
{
	[Serializable]
	public class TodoItem : INotifyPropertyChanged
	{

		public TodoItem(DateTime dueDate, string content, string id = null)
		{
			DueDate = dueDate;
			Content = content;
			ID = id ?? GenerateID();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private DateTime _DueDate;
		public DateTime DueDate
		{
			get => _DueDate; 
			set
			{
				_DueDate = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DueDate)));
			}
		}

		private string _Content;
		public string Content
		{
			get => _Content;
			set
			{
				_Content = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Content)));
			}
		}

		public string ID { get; }

		private static string GenerateID() => Settings.Default.Counter++.ToString();

		public static explicit operator TodoItem(TodoStruct s)
		{
			return new TodoItem(s.DueDate, s.Content, s.ID);
		}

		public static explicit operator TodoStruct(TodoItem o)
		{
			return new TodoStruct(o.DueDate, o.Content, o.ID);
		}
	}

	[Serializable]
	public class TodoStruct
	{
		public TodoStruct(DateTime dueDate, string content, string id)
		{
			DueDate = dueDate;
			Content = content;
			ID = id;
		}

		public readonly DateTime DueDate;
		public readonly string Content;
		public readonly string ID;
	}
}
