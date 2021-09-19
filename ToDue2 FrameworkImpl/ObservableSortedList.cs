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
	public class ObservableTodoList : ObservableCollection<TodoItem>, IDropTarget
	{
		public ObservableTodoList() : base() { }

		public ObservableTodoList(IEnumerable<TodoItem> items, bool doesAutoSort = false)
			: base(items) 
		{
			_DoesAutoSort = doesAutoSort;
		}

		private bool _DoesAutoSort;

		public new void Add(TodoItem content)
		{
			if (_DoesAutoSort)
			{
				int pos = 0;
				for (; pos < base.Count; pos++)
				{
					if (content.DueDate <= base[pos].DueDate) break;
				}
				base.Insert(pos, content);
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Add, content, pos));
			}
			else
			{
				base.Add(content);
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Add, content, this[this.Count - 1]));
			}
		}

		public new void Remove(TodoItem content)
		{
			base.Remove(content);
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
				NotifyCollectionChangedAction.Remove, content));
		}

		public void Refresh()
		{
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public void DragOver(IDropInfo dropInfo)
		{
			ObservableTodoList sourceItem = dropInfo.Data as ObservableTodoList;
			ObservableTodoList targetItem = dropInfo.TargetItem as ObservableTodoList;

			if (sourceItem != null && targetItem != null)
			{
				dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
				dropInfo.Effects = DragDropEffects.Move;
			}
		}

		public void Drop(IDropInfo dropInfo)
		{
			var sourceItem = dropInfo.Data as TodoItem;
			var targetItem = dropInfo.TargetItem as TodoItem;
			//targetItem.Add(sourceItem);
		}

		public new void Move(int oldIndex, int newIndex)
		{

		}

		public new void InsertItem(int index, TodoItem item) { }
		public new void MoveItem(int oldIndex, int newIndex) { }

	}
}
