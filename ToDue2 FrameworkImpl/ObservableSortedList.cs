using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Linq;

namespace ToDue2
{
	public class ObservableSortedList : List<TodoItem>, INotifyCollectionChanged
	{
		public ObservableSortedList() : base() { }

		public ObservableSortedList(IEnumerable<TodoItem> items)
			: base(items) { }

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public new void Add(TodoItem content)
		{
			int pos = 0;
			for (; pos < base.Count; pos++)
			{
				if (content.DueDate <= base[pos].DueDate) break;
			}
			base.Insert(pos, content);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
				NotifyCollectionChangedAction.Add, content, pos));
		}

		public new void Remove(TodoItem content)
		{
			base.Remove(content);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
				NotifyCollectionChangedAction.Remove, content));
		}

		public void Refresh()
		{
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
	}
}
