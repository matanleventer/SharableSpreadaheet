using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using q1;

namespace SpreadsheetApp
{
    public partial class Form1 : Form
    {
        SharableSpreadaheet sb = new SharableSpreadaheet(1, 1);
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = textBox1.Text;
                sb.load(filename);
                MessageBox.Show("Successeful Load");
            }
            catch
            {
                MessageBox.Show("Error! please insert valid filename");
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int r = Convert.ToInt32(textBox2.Text);
                int c = Convert.ToInt32(textBox3.Text);
                if (r > sb.get_row())
                    MessageBox.Show("Error! row number out of range");
                else if (c > sb.get_col())
                    MessageBox.Show("Error! col number out of range");
                else
                {
                    string res = sb.getCell(r, c);
                    MessageBox.Show(res);
                }
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
            }
            catch
            {
                MessageBox.Show("Error! please insert correct numbers");
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            int r = 0;
            int c = 0;
            bool res = sb.searchString(textBox1.Text, ref r, ref c);
            if (res)
            {
                MessageBox.Show($"String {textBox1.Text} found in cell [{r},{c}]");
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
            }
            else
            {
                MessageBox.Show($"String {textBox1.Text} was not found");
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                int r = Convert.ToInt32(textBox2.Text);
                int c = Convert.ToInt32(textBox3.Text);
                if (r > sb.get_row())
                    MessageBox.Show("Error! row number out of range");
                else if (c > sb.get_col())
                    MessageBox.Show("Error! col number out of range");
                else
                {
                    bool res = sb.setCell(r, c, textBox1.Text);
                    MessageBox.Show($"Cell [{r},{c}] is set to {textBox1.Text} ");
                    textBox1.Text = "";
                    textBox2.Text = "";
                    textBox3.Text = "";
                }
            }
            catch
            {
                MessageBox.Show("Error! please insert correct numbers");
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

  
