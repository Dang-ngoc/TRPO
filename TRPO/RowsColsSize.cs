using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TRPO
{
    public partial class RowsColsSize : Form
    {
        private int cols;
        private int rows;

        private Form1 main;

        public RowsColsSize(Form1 main)
        {
            InitializeComponent();
            this.main = main;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cols = (int) numericUpDown1.Value;
            rows = (int)numericUpDown2.Value;
            this.Close();
            main.onSizeEntered(rows + 1, cols + 1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
