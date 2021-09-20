using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Linq;
using GongSolutions.Wpf.DragDrop;
using System.Windows;
using System.Collections.ObjectModel;

namespace ToDue2
{
	public class ObservableTodoList : List<TodoItem>, INotifyCollectionChanged
	{
		public ObservableTodoList() : base() { }

		public ObservableTodoList(IEnumerable<TodoItem> items, bool doesAutoSort = true)
			: base(items)
		{
			DoesAutoSort = doesAutoSort;
		}

		public bool DoesAutoSort { get; set; }

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public new void Add(TodoItem content)
		{
			if (DoesAutoSort)
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
			else
			{
				base.Add(content);
				CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Add, content));
			}
		}

		public new void Remove(TodoItem content)
		{
			base.Remove(content);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, content));
		}

		public void Refresh()
		{
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public void Reorder()
		{
			var newList = this.OrderBy(todo => todo.DueDate).ToArray();
			this.Clear();
			this.AddRange(newList);
			Refresh();
		}
	}
}
