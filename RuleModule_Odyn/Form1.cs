using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RuleModule_Odyn
{
    public partial class Form1 : Form
    {
        GraphicsSchema gs;
        RuleModule mod = new RuleModule();
        List<string> variablesName = null;        
        HashSet<string> aVar = new HashSet<string>();
        public Form1()
        {
            InitializeComponent();
            try
            {
                Schema.inData = GetRequestedValues;
                //mod.ReadSchema("C:\\Projects\\COVID\\schema\\sterowanieVC-OS-c1.txt");
                //mod.ReadSchema("C:\\Projects\\COVID\\schema\\sterowanieVC-OS-a6.txt");
                if (File.Exists("C:\\Projects\\COVID\\schema\\inicjalizacjaVC-OS-c6.txt"))
                {
                    listBox1.Items.Add("C:\\Projects\\COVID\\schema\\inicjalizacjaVC-OS-c6.txt");
                    ShowSchema((string)listBox1.Items[0]);                   
                    addToolStripMenuItem.Enabled = true;
                    listBox1.SelectedIndex = 0;
                }               
                RuleModule.cc = MessageBox.Show;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void ShowSchema(string fileName)
        {
            List<string> x = new List<string>();
            if (fileName.Contains("+"))
            {
                string[] aux = fileName.Split('+');
                foreach (var item in aux)
                    x.Add(item);
            }
            else
                x.Add(fileName);
            
            mod.NewSchema(x[0],x[0]);
            for(int i=1;i<x.Count;i++)
                mod.AddSchema(x[0],x[i]);
            gs = new GraphicsSchema(mod.rules[x[0]].schema, mod.rules[x[0]].startNode);
            gs.PrepareSchemaDraw();
            gs.DrawSchema(pictureBox1);
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (gs != null)
            {
                gs.DrawSchema(pictureBox1);
                pictureBox1.Refresh();
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void loadToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DialogResult res = openFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                try
                {
                    listBox1.Items.Add(fileName);
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    if (listBox1.Items.Count == 1)                    
                        ShowSchema(fileName);
                    addToolStripMenuItem.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        void DeleteVariables()
        {
            foreach(var item in variablesName)
            {
                this.Controls.RemoveByKey(item);
            }
        }
        public Dictionary<string,double> GetRequestedValues(List<string> variables)
        {
            Dictionary<string, double> dic = null;
            Form2 dial = new Form2(variables);
            DialogResult res=dial.ShowDialog();
            if (res == DialogResult.OK)
            {
                dic = dial.values;
            }
            else
                button2.Enabled = false;

            return dic;
        }
        void UpdateBase()
        {
            dataGridView1.Rows.Clear();
            foreach(var item in Node.variables)
            {
                string[] row = new string[] { item.Key, item.Value.ToString()};
                dataGridView1.Rows.Add(row);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            mod.rules[(string)listBox1.SelectedItem].Start();
            Tuple<bool, List<string>> nodeRes = mod.rules[(string)listBox1.SelectedItem].RunNext();
            if (!nodeRes.Item1)
                button2.Enabled = false;

            richTextBox1.Text = "";
            if (nodeRes.Item2 != null)
                foreach (var item in nodeRes.Item2)
                    richTextBox1.Text += item + "\n" ;
            gs.DrawSchema(pictureBox1);
            
            Size pos = gs.ActiveBox(mod.rules[(string)listBox1.SelectedItem].nextNode);
            UpdateBase();
            panel1.AutoScrollPosition = new Point(pos.Width - pos.Width / 2, pos.Height);
            /*using (Control c = new Control() { Parent = panel1,Height=1, Top = pos.Height })
            {
                panel1.ScrollControlIntoView(c);
            }*/
            pictureBox1.Refresh();
            button2.Enabled = true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Tuple<bool, List<string>> nodeRes = mod.rules[(string)listBox1.SelectedItem].RunNext();
            if (!nodeRes.Item1)
                button2.Enabled = false;

            richTextBox1.Text = "";
            if (nodeRes.Item2 != null)
                foreach (var item in nodeRes.Item2)
                    richTextBox1.Text += item + "\n";
            
            gs.DrawSchema(pictureBox1);
            Size pos=gs.ActiveBox(mod.rules[(string)listBox1.SelectedItem].nextNode);
            panel1.AutoScrollPosition = new Point(pos.Width-pos.Width/2, pos.Height);           
            pictureBox1.Refresh();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex>=0)
            ShowSchema((string)listBox1.Items[listBox1.SelectedIndex]);
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = openFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                try
                {
                    int index = listBox1.SelectedIndex;
                    string rem = (string)listBox1.SelectedItem;
                    string s = listBox1.SelectedItem+"+"+fileName;
                    
                    listBox1.Items.Remove(rem);
                    listBox1.Items.Insert(index, s);
                    string[] aux = s.Split('+');
                    mod.AddSchema(aux[0],fileName);
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
//                    ShowSchema((string)listBox1.Items[index]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<string> k = new List<string>(mod.rules.Keys);

            mod.AddSchemaToThread(mod.rules[k[0]], mod.sugesionThread);
        }
    }
}
