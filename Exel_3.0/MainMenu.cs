using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Exel_3._0
{
    public partial class MainMenu : Form
    {
        private const int col_d_row = 65536;
        private const int coordinates = 30;
        private const int width_size = 40;           //розміри клітинки
        private const int height_correct = 30;       //відступи по висоті
        private const int alphabet = 65;
        private const int number_of_letters = 26;
        private const int start_number = 10;         //кількість початкових стовпчиків/рядків
        public int col = 0;
        public string[,] massFormulas; //масив формул в парсер
        public MainMenu()
        {
            InitializeComponent();
            ChangeSize();
            for (int i = 0; i < start_number; i++)
            {
                AddColumn();
                AddRow();
            }            
        }

        private void FillingMass()
        {
            massFormulas = new string [dataGridFormula.ColumnCount, dataGridFormula.RowCount];
            for (int i = 0; i < dataGridFormula.ColumnCount; i++)
            { 
                for (int j = 0; j < dataGridFormula.RowCount; j++)
                {
                    if (dataGridFormula[i, j].Value != null)
                        massFormulas[i, j] = dataGridFormula[i, j].Value.ToString();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddColumn();
        }

        public string Formulas(int i, int j)
        {
            if (dataGridFormula[i, j].Value != null)
            {
                string s = dataGridFormula[0, 0].Value.ToString();
                return s;
            }
            else 
                return "0";
        }

        private void AddColumn()
        {
            int temp = 0;
            dataGridValue.ColumnCount++;
            temp = dataGridFormula.ColumnCount++;
            dataGridFormula.Columns[temp].HeaderText = Convert.ToString(temp + 1);
            dataGridFormula.Columns[temp].Width = width_size;
            dataGridValue.Columns[temp].HeaderText = Convert.ToString(temp + 1);
            dataGridValue.Columns[temp].Width = width_size;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddRow();
        }

        private void AddRow()
        {
            int temp = 0;
            if ((dataGridFormula.RowCount == 0) && (dataGridFormula.ColumnCount == 0))
            {
                AddColumn();
            }
            dataGridValue.RowCount++;
            temp = dataGridFormula.RowCount++;
            if (temp < number_of_letters)
            {
                dataGridFormula.Rows[temp].HeaderCell.Value = Convert.ToString((char)(temp + alphabet));
                dataGridValue.Rows[temp].HeaderCell.Value = Convert.ToString((char)(temp + alphabet));
            }
            else
                if (temp < (number_of_letters * (number_of_letters + 1)))
                {
                    int secondT = temp % number_of_letters;
                    int firstT = temp / number_of_letters - 1;
                    string s;
                    s = Convert.ToString((char)(firstT + alphabet));
                    s += Convert.ToString((char)(secondT + alphabet));
                    dataGridFormula.Rows[temp].HeaderCell.Value = s;
                    dataGridValue.Rows[temp].HeaderCell.Value = s;
                }
                else
                    if (temp < (number_of_letters * (number_of_letters * (number_of_letters + 1) + 1)))
                    {

                        int thirdT = temp % number_of_letters;
                        int secondT = temp / number_of_letters - number_of_letters - 1;
                        int firstT = temp / number_of_letters / number_of_letters - 1;
                        string s;
                        s = Convert.ToString((char)(firstT + alphabet));
                        s += Convert.ToString((char)(secondT + alphabet));
                        s += Convert.ToString((char)(thirdT + alphabet));
                        dataGridFormula.Rows[temp].HeaderCell.Value = s;
                        dataGridValue.Rows[temp].HeaderCell.Value = s;
                    }
                    else
                    {
                        dataGridFormula.RowCount--; dataGridValue.RowCount--;
                    }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            ChangeSide();           
        }
        
        void ChangeSide()
        {
            for (int j = 0; j < dataGridValue.RowCount; j++)
            {
                for (int i = 0; i < dataGridValue.ColumnCount; i++)
                {
                    if (dataGridFormula[i, j].Value == null)
                        dataGridFormula[i, j].Value = dataGridValue[i, j].Value;
                }
            }
            Recalculation();
            dataGridValue.Width = dataGridFormula.Width;
            dataGridValue.Height = dataGridFormula.Height;
            dataGridFormula.Visible = false;
            dataGridValue.Visible = true;
        }

        private void Recalculation()
        {
            FillingMass();
            for (int j = 0; j < dataGridFormula.RowCount; j++)
            {
                for (int i = 0; i < dataGridFormula.ColumnCount; i++)
                {
                    if (dataGridFormula[i, j].Value != null)
                    {
                        int ColumnCount = dataGridFormula.ColumnCount;
                        int RowCount = dataGridValue.RowCount;
                        Parser a = new Parser(massFormulas, ColumnCount, RowCount);
                        string s = dataGridFormula[i, j].Value.ToString();
                        double rez = a.StartEvaluate(s);
                        dataGridValue[i, j].Value = Convert.ToString(rez);
                    }
                }
            }
        }

        private void MainMenu_SizeChanged(object sender, EventArgs e)
        {
            ChangeSize();   
        }

        private void ChangeSize()
        {
            int delta_width = 0;
            int delta_height = 0;
            dataGridFormula.Location = new Point(coordinates, coordinates);
            dataGridValue.Location = new Point(coordinates, coordinates);
            delta_width = -button1.Width - width_size / 2;
            delta_height = -button1.Height - button2.Height - height_correct;
            button1.Location = new Point(this.Size.Width + delta_width, this.Size.Height + delta_height);
            delta_width -= button4.Width;
            button4.Location = new Point(this.Size.Width + delta_width, this.Size.Height + delta_height);
            delta_width -= button2.Width;
            button2.Location = new Point(this.Size.Width + delta_width, this.Size.Height + delta_height);
            delta_width -= button5.Width;
            button5.Location = new Point(this.Size.Width + delta_width, this.Size.Height + delta_height);
            delta_width -= button3.Width;
            button3.Location = new Point(this.Size.Width + delta_width, this.Size.Height + delta_height);
            dataGridFormula.Width = this.Size.Width - width_size - coordinates;
            dataGridFormula.Height = this.Size.Height - 3 * height_correct - coordinates;
            dataGridValue.Width = this.Size.Width - width_size - coordinates;
            dataGridValue.Height = this.Size.Height - 3 * height_correct - coordinates;
        }

        private void Ntab (int ColumnC, int RowC, string[,] mass)
        {
            dataGridFormula.ColumnCount = 0;
            dataGridValue.ColumnCount = 0;
            for (int i=0; i< ColumnC; i++)
            {
                AddColumn();
            }
            dataGridFormula.RowCount = 0;
            dataGridValue.RowCount = 0;
            for (int i=0; i<RowC; i++)
            {
                AddRow();
            }
            for (int i=0; i<ColumnC; i++)
            {
                for (int j=0; j<RowC; j++)
                {
                    dataGridFormula[i, j].Value =mass[i, j];
                }
            }
            Recalculation();
        }

        private void проПрограмуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Created by me");
        }
        private void відкритиToolStripMenuItem_Click(object sender, EventArgs e)
        {
             OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = "c:\\";
            ofd.Filter = "Exel 3.0 files (*.exel3)|*.exel3";
            ofd.FilterIndex = 2;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.Cancel)
                return;
            StreamReader sr = new StreamReader(ofd.FileName);
            int ColumnCount_ = 0;
            int RowCount_ = 0;
            string[,] mass;
            if (sr.Peek() >= 0) ColumnCount_ = Convert.ToInt32(sr.ReadLine());
            else
            {
                Show a = new Show();
                a.FILEFAIL();
                return;
            }
            if (sr.Peek() >= 0) RowCount_=Convert.ToInt32(sr.ReadLine());
            else
            {
                Show a = new Show();
                a.FILEFAIL();
                return;
            }
            if (RowCount_*ColumnCount_>col_d_row)
            {
                Show a = new Show();
                a.FILEFAIL();
                return;
            }
            mass = new string [ColumnCount_,RowCount_];
            for (int i=0; i<ColumnCount_; i++)
            {
                for (int j=0; j<RowCount_; j++)
                {
                    if (sr.Peek() >= 0)
                    {
                        mass[i, j] = sr.ReadLine();
                        if (mass[i, j] == "0") mass[i, j] = "";
                    }
                    else
                    {
                        Show a = new Show();
                        a.FILEFAIL();
                        return;
                    }
                }
            }
            Ntab(ColumnCount_, RowCount_, mass);
            sr.Close();
        }

        private void зберегтиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Exel 3.0 files (*.exel3)|*.exel3";
            sfd.FilterIndex = 2;
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == DialogResult.Cancel)
                return;
            StreamWriter sw = new StreamWriter(sfd.FileName);
            sw.WriteLine(dataGridFormula.ColumnCount);
            sw.WriteLine(dataGridFormula.RowCount);
            for (int i=0; i<dataGridFormula.ColumnCount; i++)
            {
                for (int j=0; j<dataGridFormula.RowCount; j++)
                {
                   if (dataGridFormula[i,j].Value!=null)
                       sw.WriteLine(Convert.ToString(dataGridFormula[i, j].Value));
                }
            }
            sw.Close();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            DeleteCount();
        }

        private void DeleteCount()
        {
            if (dataGridFormula.ColumnCount > 0)
            {
                int k = dataGridValue.CurrentCell.ColumnIndex;
                dataGridFormula.Columns.RemoveAt(k);
                dataGridValue.Columns.RemoveAt(k);
                for (int i = dataGridValue.ColumnCount - 1; i >= k; i--)
                {
                    dataGridFormula.Columns[i].HeaderText = Convert.ToString(i + 1);
                    dataGridValue.Columns[i].HeaderText = Convert.ToString(i + 1);
                }
            }
            ChangeSide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DeleteRow();
        }

        private void DeleteRow()
        {
            if (dataGridFormula.RowCount > 0)
            {
                int k = dataGridValue.CurrentRow.Index;
                for (int i = dataGridValue.RowCount - 1; i >= k; i--)
                {
                    dataGridFormula.Rows[i].HeaderCell.Value = dataGridFormula.Rows[i - 1].HeaderCell.Value;
                    dataGridValue.Rows[i].HeaderCell.Value = dataGridValue.Rows[i - 1].HeaderCell.Value;
                    //for(int j = 0; j < dataGridFormula.ColumnCount; j++)
                    //{
                    //    dataGridFormula[i,j].Value = dataGridFormula[i - 1,j].Value;
                    //}
                }
                dataGridFormula.Rows.RemoveAt(k);
                dataGridValue.Rows.RemoveAt(k);

            }
            if (dataGridFormula.RowCount == 0)
            {
                dataGridFormula.ColumnCount = 0;
                dataGridValue.ColumnCount = 0;
            }
            ChangeSide();
        }
        private void dataGridValue_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1)
            {
                var a = dataGridFormula[e.ColumnIndex, e.RowIndex].Value;
                if (a != null)
                    textBox1.Text = a.ToString();
                else
                    textBox1.Text = "null";
            }
        }
        private void dataGridValue_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            dataGridValue[e.ColumnIndex, e.RowIndex].Value = dataGridFormula[e.ColumnIndex, e.RowIndex].Value;
        }

        private void dataGridValue_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dataGridFormula[e.ColumnIndex, e.RowIndex].Value = dataGridValue[e.ColumnIndex, e.RowIndex].Value;
            ChangeSide();
        }
    }
    public partial class Show : Form
    {
        public void INCORRECT()
        {
            MessageBox.Show("Некоректне посилання на клітинку");
        }
        public void FILEFAIL()
        {
            MessageBox.Show("Помилка у читаннi файла");
        }
        public void CYCLE()
        {
            MessageBox.Show("Зациклення");
        }
        public void DIVBYZERO()
        {
            MessageBox.Show("Дiлення на 0");
        }
    }

}
