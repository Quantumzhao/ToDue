using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToDue.Properties;

namespace ToDue
{
	public partial class Form2 : Form
	{
		public Form2(Form1 parent)
		{
			InitializeComponent();

			_Parent = parent;
		}

		private Form1 _Parent;

		private void button1_Click(object sender, EventArgs e)
		{
			var content = textBox1.Text;
			var date = dateTimePicker1.Value;
			_Parent.ListView.Add(content, date);
			_Parent.RedrawAll();
			_Parent.SaveTodoList();
			this.Close();
		}
	}
}
