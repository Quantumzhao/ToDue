using System;
using System.Collections.Generic;
using System.Text;
using ToDue2.Properties;

namespace ToDue2
{
	[Serializable]
	public class TodoItem
	{
		public TodoItem(DateTime dueDate, string content)
		{
			DueDate = dueDate;
			Content = content;
			ID = GenerateID();
		}

		public DateTime DueDate { get; set; }
		public string Content { get; set; }
		public string ID { get; set; }

		public string ToDateString()
		{
			return $"{DueDate.Month:00}{DueDate.Day:00}";
		}

		private static string GenerateID() => Settings.Default.Counter++.ToString();
	}
}
