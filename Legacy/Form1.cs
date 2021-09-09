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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace ToDue
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			this.Icon = Resources.icon__256x256_;

			InitContextMenu();
			InitFonts();

			ListView = new ListViewWrapper(this, _ItemsCollectionAnchor, 52);

			label1.Font = new Font(Fonts.Families[0], 50);
			label2.Font = new Font(Fonts.Families[0], 40);
			label3.Font = new Font(Fonts.Families[0], 40);
			label4.Font = new Font(Fonts.Families[0], 40);
			label5.Font = new Font(Fonts.Families[0], 40);
			label6.Font = new Font(Fonts.Families[0], 40);
		}

		private bool _IsMouseDown;
		private Point _LastLocation;
		private readonly Point _ItemsCollectionAnchor = new Point(6, 141);
		public readonly ListViewWrapper ListView;
		private Point _SeparaterAnchor;
		private Point _SeparaterEnd;
		private Pen _Pen = new Pen(MyColors.DefaultColor, 0.5f);
		private bool _IsLayoutChangedFlag = false;

		//Create your private font collection object.
		public readonly PrivateFontCollection Fonts = new PrivateFontCollection();

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
			Form2 form = new Form2(this);
			form.ShowDialog(this);
		}
		#endregion

		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			if (_IsLayoutChangedFlag)
			{
				Refresh();
				_Pen.Color = MyColors.DefaultColor;
				e.Graphics.DrawLine(_Pen, _SeparaterAnchor, _SeparaterEnd);
				_IsLayoutChangedFlag = false;
			}
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
			Settings.Default.StartupLocation = Location;
			Settings.Default.Save();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			var settings = Settings.Default;
			try
			{
				this.Location = settings.StartupLocation;
			}
			catch
			{
				settings.Reset();
				settings.Save();
			}

			Location = settings.StartupLocation;
			Opacity = settings.Opacity;

			using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Settings.Default.TodoItems)))
			{
				BinaryFormatter bf = new BinaryFormatter();
				ListView.Items = bf.Deserialize(ms) as List<TodoItem>;
			}

			RedrawAll();
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			_Pen.Dispose();
		}

		private void Form1_VisibleChanged(object sender, EventArgs e)
		{
			if (this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;
		}

		private void Form1_Deactivate(object sender, EventArgs e)
		{
			this.Activate();
		}

		private void Form1_SizeChanged(object sender, EventArgs e)
		{
			if (this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;
		}

		private void InitContextMenu()
		{
			var transparency = new ToolStripMenuItem("Transparency");
			for (int i = 1; i <= 10; i++)
			{
				var text = $"{i}0%";
				var percentage = i / 10f;
				var menuItem = new ToolStripMenuItem(text);
				menuItem.Click += (s, e) =>
				{
					this.Opacity = percentage;
					Settings.Default.Opacity = percentage;
					Settings.Default.Save();
				};
				transparency.DropDownItems.Add(menuItem);
			}
			contextMenuStrip1.Items.Add(transparency);

			var theme = new ToolStripComboBox("Theme");
			theme.Items.Add("Light");
			theme.Items.Add("Dark");
			theme.SelectedIndex = Settings.Default.IsLightMode ? 0 : 1;
			theme.SelectedIndexChanged += (s, e) =>
			{
				MyColors.SetTheme((s as ToolStripComboBox).SelectedIndex == 0);
				RedrawAll();
			};
			contextMenuStrip1.Items.Add(theme);

			var sep = new ToolStripSeparator();
			contextMenuStrip1.Items.Add(sep);

			var exit = new ToolStripMenuItem("Exit");
			exit.Click += (s, e) => Application.Exit();
			contextMenuStrip1.Items.Add(exit);
		}

		private void InitFonts()
		{
			//Select your font from the resources.
			//My font here is "Digireu.ttf"
			int fontLength = Resources.ConservativeSimplicity.Length;

			// create a buffer to read in to
			byte[] fontdata = Resources.ConservativeSimplicity;

			// create an unsafe memory block for the font data
			IntPtr data = Marshal.AllocCoTaskMem(fontLength);

			// copy the bytes to the unsafe memory block
			Marshal.Copy(fontdata, 0, data, fontLength);

			// pass the font to the font collection
			Fonts.AddMemoryFont(data, fontLength);

			int fontLength2 = Resources.segmdl2.Length;
			byte[] fontdata2 = Resources.segmdl2;
			IntPtr data2 = Marshal.AllocCoTaskMem(fontLength2);
			Marshal.Copy(fontdata2, 0, data2, fontLength2);
			Fonts.AddMemoryFont(data2, fontLength2);
		}

		public void RedrawAll()
		{
			_SeparaterAnchor = ListView.RedrawItems() + new Size(15, 0);
			_SeparaterEnd = new Point(415, _SeparaterAnchor.Y);
			_IsLayoutChangedFlag = true;

			label1.ForeColor = MyColors.DefaultColor;
			label2.ForeColor = MyColors.DefaultColor;
			label3.ForeColor = MyColors.DefaultColor;
			label4.ForeColor = MyColors.DefaultColor;
			label5.ForeColor = MyColors.DefaultColor;
			label6.ForeColor = MyColors.DefaultColor;
			button1.ForeColor = MyColors.DefaultColor;
			this.BackColor = MyColors.BackColor;
			this.TransparencyKey = MyColors.BackColor;

			button1.Location = _SeparaterAnchor + new Size(0, 13);
		}

		public void SaveTodoList()
		{
			using (MemoryStream ms = new MemoryStream())
			{
				BinaryFormatter bf = new BinaryFormatter();
				bf.Serialize(ms, ListView.Items);
				ms.Position = 0;
				byte[] buffer = new byte[(int)ms.Length];
				ms.Read(buffer, 0, buffer.Length);
				Settings.Default.TodoItems = Convert.ToBase64String(buffer);
				Settings.Default.Save();
			}
		}

		private void label6_Click(object sender, EventArgs e)
		{
			if (this.FormBorderStyle == FormBorderStyle.None)
			{
				this.FormBorderStyle = FormBorderStyle.Sizable;
			}
			else if (this.FormBorderStyle == FormBorderStyle.Sizable)
			{
				this.FormBorderStyle = FormBorderStyle.None;
			}
		}
	}

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

	static class MyColors
	{
		internal static Color DefaultColor => Settings.Default.IsLightMode ? _DefaultColorLight : _DefaultColorDark;
		internal static Color AddBtnHighlightColor => Settings.Default.IsLightMode ? _AddBtnHighlightColorLight : _AddBtnHighlightColorDark;
		internal static Color DefaultHighlightColor => Settings.Default.IsLightMode ? _DefaultHighlightColorLight : _DefaultHighlightColorDark;
		internal static Color BackColor => Settings.Default.IsLightMode ? _BackColorLight : _BackColorDark;

		private static readonly Color _DefaultColorDark = Color.FromArgb(200, 255, 255, 255);
		private static readonly Color _AddBtnHighlightColorDark = Color.FromArgb(255, 179, 255, 213);
		private static readonly Color _DefaultHighlightColorDark = Color.FromArgb(255, 255, 255, 255);
		private static readonly Color _BackColorDark = Color.FromArgb(255, 240, 240, 255);

		private static readonly Color _DefaultColorLight = Color.FromArgb(200, 0, 0, 0);
		private static readonly Color _AddBtnHighlightColorLight = Color.FromArgb(255, 60, 84, 70);
		private static readonly Color _DefaultHighlightColorLight = Color.FromArgb(255, 0, 0, 0);
		private static readonly Color _BackColorLight = Color.FromArgb(255, 0, 0, 16);

		internal static void SetTheme(bool isLight)
		{
			if (Settings.Default.IsLightMode != isLight)
			{
				Settings.Default.IsLightMode = isLight;
				Settings.Default.Save();
			}
		}
	}

	public class ListViewWrapper
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
		public List<TodoItem> Items = new List<TodoItem>();

		public void Add(string content, DateTime dueDate)
		{
			int pos = 0;
			for (; pos < Items.Count; pos++)
			{
				if (dueDate <= Items[pos].DueDate) break;
			}
			Items.Insert(pos, new TodoItem(dueDate, content));
		}

		public Point RedrawItems()
		{
			void DrawItem(TodoItem item, int anchorX, int anchorY)
			{
				var baseAnchor = new Point(anchorX, anchorY);
				var contentAnchor = baseAnchor + new Size(20, -11);
				var dateAnchor = contentAnchor + new Size(327, 0);

				var removeLbl = new Label()
				{
					ForeColor = MyColors.DefaultColor,
					BackColor = Color.Transparent,
					FlatStyle = FlatStyle.Flat,
					Text = "",
					Height = 20,
					Width = 20,
					Tag = item.ID,
					Location = baseAnchor
				};
				removeLbl.Font = new Font(_Me.Fonts.Families[1], 8);
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
					var rm = Items.Single(i => i.ID == (s as Label).Tag as string);
					Items.Remove(rm);
					_Me.SaveTodoList();
					_Me.RedrawAll();
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
					AutoSize = true,
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
			foreach (var item in Items)
			{
				DrawItem(item, _Anchor.X, currentY);
				currentY += _ItemOffsetY;
			}

			return new Point(_Anchor.X, currentY);
		}
	}
}
