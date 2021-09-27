using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ToDue2.Properties;

namespace ToDue2
{
	public class DragAndDropHandler : IDragSource, IDropTarget
	{
		private readonly DefaultDragHandler _DragHandler = new DefaultDragHandler();
		private readonly DefaultDropHandler _DropHandler = new DefaultDropHandler();
		private readonly MainWindow _Window;

		public DragAndDropHandler(MainWindow window) => _Window = window;

		void IDropTarget.DragOver(IDropInfo dropInfo) => _DropHandler.DragOver(dropInfo);

		void IDropTarget.Drop(IDropInfo dropInfo) => _DropHandler.Drop(dropInfo);

		public void StartDrag(IDragInfo dragInfo) => _DragHandler.StartDrag(dragInfo);

		public bool CanStartDrag(IDragInfo dragInfo) => true;

		public void Dropped(IDropInfo dropInfo)
		{
			_DragHandler.Dropped(dropInfo);
			//var content = dropInfo.VisualTargetItem as ContentPresenter;
			//var grid = content.ContentTemplate.FindName("TodoGrid", content) as Grid;
			//grid.Background = Application.Current.Resources["HighlightBackground"] as SolidColorBrush;
		}

		public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
		{			
			if (Settings.Default.DoesReorderTodo)
			{
				if (Settings.Default.DoesShowPopup)
				{
					Settings.Default.DoesShowPopup = false;

					var result = MainWindow.ShowConfirmationMessage();

					if (result == MessageBoxResult.Yes)
					{
						Settings.Default.DoesReorderTodo = false;
					}
					else
					{
						Settings.Default.DoesReorderTodo = true;
						_Window.TodoItems.Reorder();
					}

					Settings.Default.Save();

				}
				else
				{
					_Window.TodoItems.Reorder();
				}
			}
			else
			{
				_Window.TodoItems.Refresh();
			}

			_Window.PinnedItems.Refresh();

			_Window.SavePinnedList();
			_Window.SaveTodoList();
		}

		public void DragCancelled() => _DragHandler.DragCancelled();

		public bool TryCatchOccurredException(Exception exception)
			=> _DragHandler.TryCatchOccurredException(exception);
	}
}
