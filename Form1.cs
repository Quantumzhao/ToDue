using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToDue.Properties;
using System.Reflection;

namespace ToDue
{
	public partial class Form1 : Form
	{
		private bool _IsMouseDown;
		private Point _LastLocation;

		public Form1()
		{
			InitializeComponent();

			_ListView = new ListViewWrapper(this, _ItemsCollectionAnchor, 52);
		}

		private readonly Point _ItemsCollectionAnchor = new Point(6, 141);
		private readonly ListViewWrapper _ListView;

		#region Add Button
		private void button1_MouseEnter(object sender, EventArgs e)
		{
			var button = sender as Button;
			button.ForeColor = MyColors.AddBtnHighlightColor;
		}

		private void button1_MouseLeave(object sender, EventArgs e)
		{
			var button = sender as Button;
			button.ForeColor = MyColors.DefaultColor;
		}
		private void button1_Click(object sender, EventArgs e)
		{
			_ListView.Add("Test", new DateTime());
		}
		#endregion

		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			//var graphics = e.Graphics;
			//var pen = new Pen(MyColors.DefaultColor, 0.5f);
			//graphics.DrawLine(pen, new Point(0, 0), new Point(50, 50));

			//TextFormatFlags flags = TextFormatFlags.Bottom | TextFormatFlags.EndEllipsis;
			//TextRenderer.DrawText(e.Graphics, "This is some text that will be clipped at the end.", this.Font,
			//	new Rectangle(10, 10, 100, 50), SystemColors.ControlText, flags);
		}

		private void Form1_MouseDown(object sender, MouseEventArgs e)
		{
			_IsMouseDown = true;
			_LastLocation = e.Location;
		}

		private void Form1_MouseMove(object sender, MouseEventArgs e)
		{
			if (_IsMouseDown)
			{
				this.Location = new Point(
					(this.Location.X - _LastLocation.X) + e.X, (this.Location.Y - _LastLocation.Y) + e.Y);

				this.Update();
			}
		}

		private void Form1_MouseUp(object sender, MouseEventArgs e)
		{
			_IsMouseDown = false;
		}
	}

	class DateTimeComparer : IComparer<DateTime>
	{
		public int Compare(DateTime x, DateTime y) => DateTime.Compare(x, y);
	}

	class TodoItem
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

		private static int _Count = 0;
		private static string GenerateID() => _Count++.ToString();
	}

	static class MyColors
	{
		internal static readonly Color DefaultColor = Color.FromArgb(200, 255, 255, 255);
		internal static readonly Color AddBtnHighlightColor = Color.FromArgb(255, 179, 255, 213);
		internal static readonly Color DefaultHighlightColor = Color.FromArgb(255, 255, 255, 255);
	}

	class ListViewWrapper
	{
		public ListViewWrapper(Form1 form, Point anchor, int itemOffsetY)
		{
			_Me = form;
			_Anchor = anchor;
			_ItemOffsetY = itemOffsetY;
		}

		private Form1 _Me;
		private Point _Anchor;
		private int _ItemOffsetY;
		private HashSet<Control> _Controls = new HashSet<Control>();
		private readonly List<TodoItem> _Items = new List<TodoItem>();

		public void Add(string content, DateTime dueDate)
		{
			int pos = 0;
			for (; pos < _Items.Count; pos++)
			{
				if (dueDate <= _Items[pos].DueDate) break;
			}
			_Items.Insert(pos, new TodoItem(dueDate, content));

			RedrawItems();
		}

		public void RedrawItems()
		{
			void DrawItem(TodoItem item, int anchorX, int anchorY)
			{
				var baseAnchor = new Point(anchorX, anchorY);
				var contentAnchor = baseAnchor + new Size(20, -11);
				var dateAnchor = contentAnchor + new Size(327, 0);

				var removeLbl = new Label()
				{
					ForeColor = MyColors.DefaultColor,
					BackColor = Color.FromArgb(1, 255, 255, 255),
					FlatStyle = FlatStyle.Flat,
					Text = "",
					Height = 20,
					Width = 20,
					Tag = item.ID,
					Location = baseAnchor
				};
				removeLbl.Font = new Font("Segoe MDL2 Assets", 8);
				removeLbl.MouseEnter += (s, e) =>
				{
					(s as Label).Text = "";
				};
				removeLbl.MouseLeave += (s, e) =>
				{
					(s as Label).Text = "";
				};
				removeLbl.MouseClick += (s, e) =>
				{
					object tag = (s as Label).Tag;
					//var rmList = _Controls.Where(c => c.Tag == tag).ToArray();
					//foreach (var con in rmList)
					//{
					//	_Controls.Remove(con);
					//	_Me.Controls.Remove(con);
					//}
					var rm = _Items.Single(i => i.ID == tag as string);
					_Items.Remove(rm);
					RedrawItems();
				};
				_Me.Controls.Add(removeLbl);
				_Controls.Add(removeLbl);

				var content = new Label()
				{
					Text = item.Content,
					ForeColor = MyColors.DefaultColor,
					BackColor = Color.Transparent,
					FlatStyle = FlatStyle.Flat,
					Height = 35,
					Tag = item.ID,
					Location = contentAnchor
				};
				content.Font = new Font("Source Han Sans SC ExtraLight", 12);
				_Me.Controls.Add(content);
				_Controls.Add(content);

				var date = new Label()
				{
					Text = item.ToDateString(),
					ForeColor = MyColors.DefaultColor,
					BackColor = Color.Transparent,
					FlatStyle = FlatStyle.Flat,
					Height = 35,
					Tag = item.ID,
					Location = dateAnchor
				};
				date.Font = new Font("Source Han Sans SC", 12);
				_Me.Controls.Add(date);
				_Controls.Add(date);
			}

			foreach (var c in _Controls)
			{
				_Me.Controls.Remove(c);
			}
			_Controls.Clear();

			int currentY = _Anchor.Y;
			foreach (var item in _Items)
			{
				DrawItem(item, _Anchor.X, currentY);
				currentY += 50;
			}
		}
	}
}
