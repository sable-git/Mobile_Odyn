using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RuleModule_Odyn
{
    public partial class Form2 : Form
    {
        List<string> variables = null;
        public Dictionary<string, double> values = new Dictionary<string, double>();
        public Form2(List<string> data)
        {
            variables = data;
            InitializeComponent();

            int counter = 0;
            foreach (var item in data)
            {
                Label l = new Label();
                l.Text = item;
                l.Location = new Point(10, 10 + counter * 40);

                TextBox t = new TextBox();
                t.Size = new Size(40, 20);
                t.Name = item;
                t.Location = new Point(70, 10 + counter * 40);
                t.Text = "0";

                this.Controls.Add(t);
                this.Controls.Add(l);

                counter++;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var item in variables)
            {
                try
                {
                    double v = Double.Parse(this.Controls[item].Text);
                    values.Add(item, v);
                }
#pragma warning disable CS0168 // Zmienna „ex” jest zadeklarowana, lecz nie jest nigdy używana
                catch(Exception ex)
#pragma warning restore CS0168 // Zmienna „ex” jest zadeklarowana, lecz nie jest nigdy używana
                {
                    MessageBox.Show("Wrong number for variable: " + item);
                    this.DialogResult = DialogResult.None;
                    return;
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
