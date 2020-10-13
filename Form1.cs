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

namespace ToDue
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private Point _ItemsCollectionAnchor = new Point(26, 130);
		private SortedList<DateTime, string> _Items = new SortedList<DateTime, string>(new DateTimeComparer());
	}

	class DateTimeComparer : IComparer<DateTime>
	{
		public int Compare(DateTime x, DateTime y) => DateTime.Compare(x, y);
	}
}
