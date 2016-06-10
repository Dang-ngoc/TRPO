using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TRPO
{
    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();
            fileInputMenu.Click += new EventHandler(this.fileInput);
            keyboardInputMenu.Click += new EventHandler(this.keyboardInput);
            button1.Click += new EventHandler(this.Ok);
            minItemInColumnMenu.Click += new EventHandler(this.minItemColumnProcessor);
            lebedevMenu.Click += new EventHandler(this.lebedevProcessor);
            MethodSquareMenu.Click += new EventHandler(this.MethodSquareProcessor);
           
         
            dataGridView2.ReadOnly = true;
            dataGridView3.ReadOnly = true;

            exitMenu.Click += new EventHandler((o, args) => 
            {
                if (MessageBox.Show("Вы точно хотите выйти?", "Выход", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                {
                    Close();
                }
            });
        }
       
       
        public void onSizeEntered(int rows, int cols)
        {
            onSizeEntered(dataGridView1, rows, cols);
        }

        private void onSizeEntered(DataGridView grid, int rows, int cols)
        {
            if (cols > 0 && rows > 0 && cols <= 11 && rows <= 11)
            {
               
                grid.RowCount = rows;
                grid.ColumnCount = cols;
                disableSort(grid);
            }
            else
            {
                MessageBox.Show("Введены некорректные данные количества единиц груза", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void disableSort(DataGridView grid)
        {
            foreach (DataGridViewColumn col in grid.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void fileInput(object sender, EventArgs args)
        {
            lblInfor.Text = "";
            button1.Visible = false;
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog(this) == DialogResult.OK)
            {
                Stream stream;
                if ((stream = file.OpenFile()) != null)
                {
                    StreamReader reader = new StreamReader(stream);
                    List<string> lines = new List<string>();
                    string tmp;
                    while ((tmp = reader.ReadLine()) != null)
                    {
                        lines.Add(tmp);
                    }
                    reader.Close();
                    try
                    {
                        Matrix matrix = new Matrix(lines.ToArray());
                        if (!validateMatrix(matrix))
                        {
                            throw new ArgumentException();
                        }
                        groupBox1.Visible = true;
                        txtNumA.ReadOnly = true;
                        txtNumB.ReadOnly = true;
                        inputDataControl.Visible = true;
                        drawMatrix(dataGridView1, matrix);
                        inputDataControl.SelectTab(tabPage1);
                        clearDataGridView(dataGridView2);
                        clearDataGridView(dataGridView3);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Введенные данные из файла неверны", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void Ok(object sender, EventArgs args)
        {
            clearDataGridView(dataGridView1);
            lblInfor.Text = "";
            try
            {
                onSizeEntered(Convert.ToInt32(txtNumA.Text) + 1, Convert.ToInt32(txtNumB.Text) + 1);
                inputDataControl.SelectTab(tabPage1);

                clearDataGridView(dataGridView2);

                clearDataGridView(dataGridView3);

                dataGridView1.Rows[0].Cells[0].Value = "Ai/Bj";
                dataGridView1.Rows[0].Cells[0].ReadOnly = true;
                for(int i=0;i < Convert.ToInt32(txtNumB.Text)+1;i++)
                {
                    dataGridView1.Rows[0].Cells[i].Style.Font = new Font(this.Font, FontStyle.Bold);
                    dataGridView1.Rows[0].Cells[i].Style.ForeColor = System.Drawing.Color.Navy;
                }
                for (int i = 1; i < Convert.ToInt32(txtNumA.Text) + 1; i++)
                {
                    dataGridView1.Rows[i].Cells[0].Style.Font = new Font(this.Font, FontStyle.Bold);
                    dataGridView1.Rows[i].Cells[0].Style.ForeColor = System.Drawing.Color.Navy;
                }
                dataGridView1.Rows[0].Cells[0].Style.BackColor = System.Drawing.Color.Khaki;
            }
           catch(Exception)
            {
                MessageBox.Show("Введены некорректные данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

           // inputDataControl.Visible = true;

            //RowsColsSize form = new RowsColsSize(this);
            //form.ShowDialog(this);
           

        }

        private void keyboardInput(object sender, EventArgs args)
        {
            clearDataGridView(dataGridView1);
            clearDataGridView(dataGridView2);
            clearDataGridView(dataGridView3);
            //inputDataControl.SelectedTab(tabPage1);
            inputDataControl.SelectTab(tabPage1);
            button1.Visible = true;
            txtNumA.Text = "";
            txtNumB.Text="";
            lblInfor.Text = "";
            txtNumB.ReadOnly = false;
            txtNumA.ReadOnly = false;
            
            groupBox1.Visible = true;
            inputDataControl.Visible = true;

        }

        private void drawMatrix(DataGridView grid, int[,] matrix, bool drawZero)
        {
          
            onSizeEntered(grid, matrix.GetLength(0), matrix.GetLength(1));
            txtNumA.Text = (matrix.GetLength(0)-1).ToString();
            txtNumB.Text = (matrix.GetLength(1)-1).ToString();
           

                for (int i = 0; i < matrix.GetLength(0); i++)
            {
                DataGridViewRow row = grid.Rows[i];
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    row.Cells[j].Style.BackColor = System.Drawing.Color.White;
                    if (i==0||j==0)
                    {
                        
                        row.Cells[0].Value = "Ai \\ Bj"+"\r\n";
                        row.Cells[j].Style.ForeColor = System.Drawing.Color.Navy;
                        row.Cells[j].ReadOnly = true;
                        row.Cells[j].Style.Font = new Font("Arial",11, FontStyle.Bold);
                        row.Cells[j].Style.BackColor = System.Drawing.Color.Khaki;
                        

                    }
                   
                    Object value = row.Cells[j].Value;
                    if (i == 0 && j == 0 && !drawZero)
                        continue;
                    
                    if (matrix[i, j] > 0 && grid != dataGridView1&&i!=0&&j!=0)
                    {
                        row.Cells[j].Style.BackColor = System.Drawing.Color.Thistle;
                        row.Cells[j].Style.Font = new Font(this.Font, FontStyle.Bold);
                    }
                    if (grid != dataGridView1 && i != 0 && j != 0)
                    {
                        Matrix mat = getMatrix(dataGridView1);
                        int[,] data = new int[matrix.GetLength(0), matrix.GetLength(1)];
                        data = mat.getData();
                        row.Cells[j].Style.Alignment = DataGridViewContentAlignment.TopLeft;
                        grid.Columns[0].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                        row.Cells[j].Value = data[i-1,j-1].ToString() + Environment.NewLine + "          " + matrix[i, j].ToString();

                    }
                    else
                    row.Cells[j].Value = matrix[i, j];
                    
                }
            }
        }

        private void drawMatrix(DataGridView grid, Matrix matrix)
        {
            int[,] res = new int[matrix.getData().GetLength(0) + 1, matrix.getData().GetLength(1) + 1];
            for (int i = 0; i < res.GetLength(0); i++)
            {
                for (int j = 0; j < res.GetLength(1); j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    if (i == 0)
                    {
                        res[i, j] = matrix.getProposal()[j - 1];
                    }
                    else if (j == 0)
                    {
                        res[i, j] = matrix.getDemand()[i - 1];
                    }
                    else
                    {

                        res[i, j] = matrix.getData()[i - 1, j - 1];
                    }
                }
            }
            drawMatrix(grid, res, false);
        }

        private void minItemColumnProcessor(object sender, EventArgs args)
        {
            clearDataGridView(dataGridView3);
            try
            {
                Matrix matrix = getMatrix(this.dataGridView1);
               
                int[,] data = minItemInColumn(matrix);
                Matrix m1 = new Matrix(matrix, data);

                drawMatrix(dataGridView2, m1);
                inputDataControl.SelectTab(tabPage2);
                lblInfor.Text = "Базисный план = "+strSumBasic+" = " + SumBasic.ToString();
            }
            catch (Exception)
            {
                MessageBox.Show("Введены некорректные данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lebedevProcessor(object sender, EventArgs args)
        {
            clearDataGridView(dataGridView3);
            try
            {
                Matrix matrix = getMatrix(this.dataGridView1);

                int[,] data = lebedev(matrix);
                Matrix m1 = new Matrix(matrix, data);
                drawMatrix(dataGridView2, m1);
                inputDataControl.SelectTab(tabPage2);
                lblInfor.Text = "Базисный план = "+strSumBasic+" = " + SumBasic.ToString();
            }
            catch (Exception)
            {
                MessageBox.Show("Введены некорректные данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void MethodSquareProcessor(object sender, EventArgs args)
        {
            try
            {
                Matrix matrix1 = getMatrix(this.dataGridView1);
            
                int[,] data = MethodSquare(matrix1,dataGridView2);
                Matrix m1 = new Matrix(matrix1, data);
                drawMatrix(dataGridView3, m1);
                inputDataControl.SelectTab(tabPage3);
                lblInfor.Text = "Базисный план = " + strSumBasic + " = " + SumBasic.ToString()
                    + "\nОптимальный план =" +StrSumOpt+" = "+ SumOpt.ToString();


            }
            catch (Exception)
            {
                MessageBox.Show("Введены некорректные данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Matrix getMatrix(DataGridView grid)
        {
            string[] lines = new string[grid.RowCount];
            int i = 0;
            foreach (DataGridViewRow row in grid.Rows)
            {
                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    Object value = row.Cells[j].Value;
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }
                    else if (value == null)
                    {
                        throw new ArgumentException();
                    }
                    sb.Append(row.Cells[j].Value.ToString()).Append(" ");
                }
                lines[i++] = sb.ToString();
            }
            return new Matrix(lines);
        }

        private int[,] minItemInColumn(Matrix matrix)
        {
            clearDataGridView(dataGridView2);
            int[,] data = new int[matrix.getData().GetLength(0), matrix.getData().GetLength(1)];
            int[] proposal = (int[])matrix.getProposal().Clone();
            int[] demand = (int[])matrix.getDemand().Clone();
            for (int i = 0; i < proposal.Length; i++)
            {
                int[] asc = sortElements(getRow(matrix.getData(), i));
                int j = 0;
                while (proposal[i] != 0)
                {
                    int min = Math.Min(proposal[i], demand[asc[j]]);
                    data[asc[j], i] = min;
                    proposal[i] -= min;
                    demand[asc[j]] -= min;
                    j++;
                }
            }
            SumBasic = 0;
            strSumBasic = "";
           for(int i=0;i< matrix.getData().GetLength(0);i++)
            {
                for (int j = 0; j < matrix.getData().GetLength(1); j++)
                    if (data[i, j] != 0)
                    {
                        if (strSumBasic != "")
                        {
                            strSumBasic += "+" + matrix.getData()[i, j].ToString() + "*" + data[i, j].ToString();
                        }
                        else
                        {
                            strSumBasic += matrix.getData()[i, j].ToString() + "*" + data[i, j].ToString();
                        }
                        SumBasic += data[i, j] * matrix.getData()[i, j];
                    }
            }
            return data;
        }
        private int SumOpt = 0;
        private string StrSumOpt = "";
        private int[,] MethodSquare(Matrix matrix1,DataGridView dgr)
        {
            int[,] S = matrix1.getData(); // SourceData 
            int rows = matrix1.getDemand().Length;
            int col = matrix1.getProposal().Length;
            int[,] D =new int[rows,col]; // DerivativesData : basic plan
            for(int i=0;i< rows;i++)
            {
                for (int j = 0; j < col; j++)
                {
                    string s = dgr[j + 1, i + 1].Value.ToString();

                    int found1 = s.IndexOf("          ");
                    s = s.Remove(0, found1 + 8);
                    s = s.Trim();
                    
                    D[i, j] = Convert.ToInt32(s);
                }
            }
            int n = S.GetLength(0); // n Rows
            int m = S.GetLength(1); // m Columns

            bool opti = false; // Optimization
            while (!opti)
            {
                opti = true;
                for (int i = 0; i < n; i++)
                {
                    for(int j = 0; j < m; j++)
                    {
                        if(D[i, j] != 0)
                        {
                            for(int k = 0; k < n; k++)
                            {
                                for(int l = 0; l < m; l++)
                                {
                                    if(D[k, l] != 0 && k != i && l != j && S[i, j] + S[k, l] > S[i, l] + S[k, j])//&&(D[i,l]*D[k,j]!=0))
                                    {
                                        int Min = Math.Min(D[i, j], D[k, l]);
                                        D[i, j] -= Min;
                                        D[k, l] -= Min;
                                        D[i, l] += Min;
                                        D[k, j] += Min;
                                        opti = false;
                                    }
                                }
                            }
                        }
                    }
                }
                SumOpt = 0;
                StrSumOpt = "";
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        if (D[i, j] != 0)
                        {

                            if (StrSumOpt != "")
                            {
                                StrSumOpt +="+"+ S[i, j].ToString()+"*" + D[i, j].ToString();
                            }
                            else
                            {
                                StrSumOpt += S[i, j].ToString()+"*"+ D[i, j].ToString();
                            }
                            SumOpt += D[i, j] * S[i, j];
                        }
                    }
                }
               
               
            }
            // return S;
            return D;
        }
        int SumBasic;
        string strSumBasic;
        private int[,] lebedev(Matrix matrix)
        {
            clearDataGridView(dataGridView2);
            double[] columnAvg = new double[matrix.getDemand().Length];
            double[] rowAvg = new double[matrix.getProposal().Length];

            for (int i = 0; i < columnAvg.Length; i++)
            {
                columnAvg[i] = getAverage(getColumn(matrix.getData(), i));
            }
            for (int i = 0; i < rowAvg.Length; i++)
            {
                rowAvg[i] = getAverage(getRow(matrix.getData(), i));
            }

            double[,] lebedevData = new double[columnAvg.Length, rowAvg.Length];
            for (int i = 0; i < columnAvg.Length; i++)
            {
                for (int j = 0; j < rowAvg.Length; j++)
                {
                    lebedevData[i, j] = columnAvg[i] + rowAvg[j] - matrix.getData()[i, j];
                }
            }
            int[] proposal = (int[])matrix.getProposal().Clone();
            int[] demand = (int[])matrix.getDemand().Clone();
            int[,] res = new int[columnAvg.Length, rowAvg.Length];

            Tuple<int, int>[] sortedCells = sortLebedevElements(lebedevData);
            for (int i = 0; i < sortedCells.Length; i++)
            {
                Tuple<int, int> cell = sortedCells[i];
                int min = Math.Min(demand[cell.Item1], proposal[cell.Item2]);
                res[cell.Item1, cell.Item2] = min;
                demand[cell.Item1] -= min;
                proposal[cell.Item2] -= min;
            }
            SumBasic = 0;
            strSumBasic = "";
            for (int i = 0; i < columnAvg.Length; i++)
            {
                for (int j = 0; j < rowAvg.Length; j++)
                {
                    if (res[i, j] != 0)
                    {
                        if(strSumBasic!="")
                        {
                            strSumBasic += "+" + matrix.getData()[i, j].ToString() + "*" + res[i, j].ToString();
                        }
                        else
                        {
                            strSumBasic += matrix.getData()[i, j].ToString() + "*" + res[i, j].ToString();
                        }
                        SumBasic += res[i, j] * matrix.getData()[i, j];
                    }
                }
            }
            return res;
        }

        Tuple<int, int>[] sortLebedevElements(double[,] data)
        {
            List<Tuple<int, int>> res = new List<Tuple<int, int>>();
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    res.Add(new Tuple<int, int>(i, j));
                }
            }
            for (int i = 0; i < res.Count - 1; i++)
            {
                for (int j = 0; j < res.Count - i - 1; j++)
                {
                    if (data[res[j].Item1, res[j].Item2] < data[res[j + 1].Item1, res[j + 1].Item2])
                    {
                        var tmp = res[j + 1];
                        res[j + 1] = res[j];
                        res[j] = tmp;
                    }
                }
            }
            return res.ToArray();
        }

        private int[] sortElements<T>(T[] data) where T : IComparable
        {
            int[] res = new int[data.Length];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = i;
            }
            for (int i = 0; i < data.Length ; i++)
            {
                for (int j = i+1; j < data.Length ; j++)
                {
                    if (data[i].CompareTo(data[j]) > 0)
                    {
                        swap(ref data[i], ref data[j]);
                        swap(ref res[i], ref res[j]);
                    }
                }
            }
            return res;
        }

        private void swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }

        private T[] getRow<T>(T[,] data, int row)
        {
            T[] res = new T[data.GetLength(0)];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = data[i, row];
            }
            return res;
        }

        private T[] getColumn<T>(T[,] data, int column)
        {
            T[] res = new T[data.GetLength(1)];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = data[column, i];
            }
            return res;
        }

        // второй способ
        private double getAverage(int[] column)
        {
            int sum = 0;
            foreach (int v in column)
                sum += v;
            return (double) sum / column.Length;
        }

        private bool validateMatrix(Matrix matrix)
        {
            int proposalSum = 0;
            int demandSum = 0;
            foreach (int demandItem in matrix.getDemand())
            {
                demandSum += demandItem;
            }

            foreach (int proposalItem in matrix.getProposal())
            {
                proposalSum += proposalItem;
            }
            return proposalSum == demandSum;
        }

        private void clearDataGridView(DataGridView grid)
        {
            //grid.ColumnCount = 0;
            //grid.RowCount = 0;
            grid.Rows.Clear();
        }

        private void txtNumA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtNumB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }

    class Matrix
    {
        private int[] demand; // по вертикали
        private int[] proposal; // по горизонтали
        private int[,] data;
        public Matrix(Matrix M1, int[,] data1)
        {
            data = data1;
            demand = M1.getDemand();
            proposal = M1.getProposal();
        }

        public Matrix(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++) // string line in lines
            {
                string line = lines[i];
                int[] lineValues = parseLine(line);
                if (proposal == null)
                {
                    proposal = lineValues;
                    demand = new int[lines.Length - 1];
                    data = new int[demand.Length, lineValues.Length];
                    continue;
                }

                if (lineValues.Length - 1 != proposal.Length)
                {
                    throw new ArgumentException("Несоответствие размеров матрицы");
                }

                demand[i - 1] = lineValues[0];
                for (int j = 1; j < lineValues.Length; j++)
                {
                    data[i - 1, j - 1] = lineValues[j];
                }
            }
        }

        public int[,] getData()
        {
            return data;
        }

        public int[] getDemand()
        {
            return demand;
        }

        public int[] getProposal()
        {
            return proposal;
        }

        private int[] parseLine(string line)
        {
            string[] lines = Regex.Split(line.Trim(), "\\s+");
            if (lines.Length == 0)
            {
                throw new ArgumentException();
            }
            int[] nums = new int[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                nums[i] = Convert.ToInt32(lines[i]);
                if (nums[i] < 0)
                {
                    throw new ArgumentException();
                }
            }
            return nums;
        }
    }
}