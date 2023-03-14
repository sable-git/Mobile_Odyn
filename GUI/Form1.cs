using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RuleModule_Odyn;

namespace GUI
{
    public partial class Form1 : Form
    {
        double scale = 1.0;
        GraphicsSchemaFrame gs;
        readonly RuleModule mod = new RuleModule();
        readonly List<string> variablesName = null;        
        GraphNode active = null;
        int dx, dy;
        public Form1()
        {
            InitializeComponent();
            try
            {
                Thread t = new Thread(InData);
                //t.Start();
                
                //mod.ReadSchema("C:\\Projects\\COVID\\schema\\sterowanieVC-OS-c1.txt");
                //mod.ReadSchema("C:\\Projects\\COVID\\schema\\sterowanieVC-OS-a6.txt");
                if (File.Exists("C:\\Projects\\COVID\\schema\\inicjalizacjaVC-OS-c6.txt"))
                {
                    listBox1.Items.Add("C:\\Projects\\COVID\\schema\\inicjalizacjaVC-OS-c6.txt");
                    ShowSchema((string)listBox1.Items[0]);                   
                    addToolStripMenuItem.Enabled = true;
                    listBox1.SelectedIndex = 0;
                }
                RuleModule.CheckAnswer = SugestionAnswer;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        CheckResult SugestionAnswer(string msg)
        {
            DialogResult res;
            res=MessageBox.Show(msg);

            if (res == DialogResult.OK)
                return CheckResult.YES;

            return CheckResult.NO;
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
            mod.ActiveSchema = x[0];
            Schema sc = mod.GetActiveSchema();
            gs = new GraphicsSchemaFrame(sc.schema, sc.startNode);
            gs.PrepareSchemaDraw();

            pictureBox1.Image = gs.bitmap;
            pictureBox1.Size = new Size(gs.bitmap.Width, gs.bitmap.Height);
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

        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void LoadToolStripMenuItem1_Click(object sender, EventArgs e)
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
        public async void InData()
        {
            while (true)
            {
                if (mod.inputData != null && mod.inputData.Count > 0)
                {
                    InputDataObject k;
                    mod.inputData.TryDequeue(out k);
                }
                await Task.Delay(1000);
            }

        }
        void UpdateBase()
        {
            dataGridView1.Rows.Clear();
            foreach(var item in mod.GetVariables())
            {
                string[] row = new string[] { item.Key, item.Value.ToString()};
                dataGridView1.Rows.Add(row);
            }
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            mod.ActiveSchema = (string)listBox1.SelectedItem;
            Schema sc = mod.GetActiveSchema();
            sc.Start();
            Node node = sc.RunNext();
            if (node == null)
                button2.Enabled = false;

            richTextBox1.Text = "";
            if (node.sugestions != null)
                foreach (var item in node.sugestions)
                    richTextBox1.Text += item + "\n" ;
            gs.DrawSchema(pictureBox1);
            
            SizeM pos = gs.ActiveBox(sc.nextNode);
            UpdateBase();
            panel1.AutoScrollPosition = new Point((int)pos.Width - (int)pos.Width / 2,(int) pos.Height);
            /*using (Control c = new Control() { Parent = panel1,Height=1, Top = pos.Height })
            {
                panel1.ScrollControlIntoView(c);
            }*/
            pictureBox1.Refresh();
            button2.Enabled = true;

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            mod.ActiveSchema = (string)listBox1.SelectedItem;
            Schema sc = mod.GetActiveSchema();
            Node node = sc.RunNext();
            if (node==null)
                button2.Enabled = false;

            richTextBox1.Text = "";
            if (node.sugestions != null)
                foreach (var item in node.sugestions)
                    richTextBox1.Text += item + "\n";
            
            gs.DrawSchema(pictureBox1);
            SizeM pos=gs.ActiveBox(sc.nextNode);
            panel1.AutoScrollPosition = new Point((int)(pos.Width-pos.Width/2), (int)pos.Height);
            UpdateBase();
            pictureBox1.Refresh();
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex>=0)
            ShowSchema((string)listBox1.Items[listBox1.SelectedIndex]);
        }

        private void AddToolStripMenuItem_Click(object sender, EventArgs e)
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
                    mod.ActiveSchema = fileName;
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
//                    ShowSchema((string)listBox1.Items[index]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
//            List<string> k = new List<string>(mod.rules.Keys);

           // mod.AddSchemaToThread(mod.rules[k[0]], mod.schemaThread);
        }

        private void Panel1_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
           
        }

        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            if (active == null)
            {
                active = gs.GetPointedNode(x, y);
                if (active != null)
                {
                    dx = x - (int)active.x.x;
                    dy = y - (int)active.x.y;
                }
            }
            else
                active = null;
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (active != null)
            {

                active.MoveShape(e.X-dx, e.Y-dy);
                pictureBox1.Invalidate();
            }
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if(gs!=null)
                gs.DrawSchema(pictureBox1);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {  
            if(e.KeyChar==43)
            {
                scale += 0.1;
                gs.Scale(scale);
                pictureBox1.Invalidate();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
           
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            
            scale = 1.1;
            gs.Scale(scale);
            //gs.PrepareSchemaDraw();
            //gs.SetGraphNodeGraphics();
            //pictureBox1.Size = new Size(gs.bitmap.Width, gs.bitmap.Height);
            //pictureBox1.Image = gs.bitmap;
           
            pictureBox1.Invalidate();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            scale = 0.9;
            gs.Scale(scale);
            //gs.PrepareSchemaDraw();
            //gs.SetGraphNodeGraphics();
            //pictureBox1.Size = new Size(gs.bitmap.Width, gs.bitmap.Height);
            //pictureBox1.Image = gs.bitmap;

            pictureBox1.Invalidate();
        }

        private void SavePositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res=saveFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                try
                {
                    gs.SavePositions((string)listBox1.SelectedItem, saveFileDialog1.FileName);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Ups: " + ex.Message);
                }
            }
        }
    }
}
