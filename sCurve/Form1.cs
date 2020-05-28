using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sCurve
{

    /*
     * Original Code by Gayan Madushan Kandethanthri
     * from youttube video - Gayan Madushan Kandethanthri
     * Mar 30, 2020
     * 
     * Edited by Jason Mitcell
     * May 28, 2020
     * Changes include:
     * - Curve generation by SCurve (BELL), Front Loaded, Back Loaded, or linear  
     * per activity
     * - Changed start Month and End month to dates instead of Month numbers
     * - Updated Graph YAXIS Label to illustrate either % or $ selection
     * - Ability to have data and curve based upon percentages or values
     * - Changed month select Switch statement to singular for loop
     */



    public partial class Form1 : Form
    {

        private readonly double[] arrMonthSum = new double[12];
        private readonly double[] arrMonthPt = new double[12];

        public Form1()
        {
            InitializeComponent();

            this.comboBox1.SelectedIndex = 0;

            cartesianChart2.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Months",
                
            });

            this.dataGridView1.Select();
            
        }

        void CumAmt()
        {
            double amt = 0;

            for (int i = 0; i < dataGridView1.Rows.Count;i++)
            {
                try
                {
                    amt += double.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString());
                }
                catch
                {
                }
                lblAmount.Text = amt.ToString("N2",null);
                lblAmount.Visible = true;
            }

        } 

        private void reset() {
            Array.Clear(arrMonthPt, 0, arrMonthPt.Length);
            Array.Clear(arrMonthSum, 0, arrMonthSum.Length);
        }

        private void GtNum_Click_1(object sender, EventArgs e)
        {
            cartesianChart2.Visible = false;
            reset();

            try
            {
                cartesianChart2.AxisY.RemoveAt(0);

            }
            catch {
            }


            if (comboBox1.SelectedIndex == 1)
            {
                cartesianChart2.AxisY.Add(new LiveCharts.Wpf.Axis
                {
                    Title = "Cumulative Progress $",

                });
            } else
            {
                cartesianChart2.AxisY.Add(new LiveCharts.Wpf.Axis
                {
                    Title = "Cumulative Progress %",

                });
            }



            //Duration
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                try
                {

                    dataGridView1.Rows[i].Cells[5].Value = int.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString()) - int.Parse(dataGridView1.Rows[i].Cells[3].Value.ToString()) + 1;
                }
                catch
                {

                }

            }

            //%

            double amt = 0;

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                try
                {
                    amt += double.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString());
                }
                catch
                {
                }

            }

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                try
                {
                    for (int k = 8; k < 20; k++)
                    {
                        dataGridView1.Rows[i].Cells[k].Value = DBNull.Value;
                    }
                    double dValue = 0;
                    dValue = double.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString());
                    dataGridView1.Rows[i].Cells[6].Value = Math.Round((dValue / amt) *100,2);
                    DateTime d1 = DateTime.Parse(dataGridView1.Rows[i].Cells[3].Value.ToString());
                    DateTime d2 = DateTime.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString());
                    int duration = Forecast.duration(d1, d2);
                    dataGridView1.Rows[i].Cells[5].Value = duration;
                    dataGridView1.Rows[i].Cells[7].Value = Math.Round(dValue * 100 / (amt * (duration)));

                    int startMonth = d1.Month;

                    Forecast f = new Forecast();
                    string sSelectedText = Convert.ToString((dataGridView1.Rows[i].Cells[0] as DataGridViewComboBoxCell).FormattedValue.ToString());

                    for (int k = 8+ startMonth - 1; k < ((duration) + 8 + startMonth - 1); k++)
                    {
                        try
                        {
                            dataGridView1.Rows[i].Cells[k].Value = f.load(k-8-startMonth+1, duration, dValue, sSelectedText, comboBox1.SelectedIndex);
                        }
                        catch
                        {

                        }
                    }

                }
                catch
                {
                }

            }


            //Month -01
            if(dataGridView1.RowCount > 1)
            {
                for (int i = 0; i < 12; i++)
                {
                    for (int j = 0; j < dataGridView1.Rows.Count-1; j++)
                    {
                        try
                        {
                            arrMonthSum[i] += double.Parse(dataGridView1.Rows[j].Cells[(8+i)].Value.ToString());
                        }
                        catch { }
                    }

                    for (int j=0; j<=i; j++)
                    {
                        arrMonthPt[i] += arrMonthSum[j];
                    }
                }
            }           

            cartesianChart2.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<ObservablePoint>
                    {

                        new ObservablePoint(1, arrMonthPt[0]),
                        new ObservablePoint(2, arrMonthPt[1]),
                        new ObservablePoint(3, arrMonthPt[2]),
                        new ObservablePoint(4, arrMonthPt[3]),
                        new ObservablePoint(5, arrMonthPt[4]),
                        new ObservablePoint(6, arrMonthPt[5]),
                        new ObservablePoint(7, arrMonthPt[6]),
                        new ObservablePoint(8, arrMonthPt[7]),
                        new ObservablePoint(9, arrMonthPt[8]),
                        new ObservablePoint(10, arrMonthPt[9]),
                        new ObservablePoint(11, arrMonthPt[10]),
                        new ObservablePoint(12, arrMonthPt[11]),
                    },
                    PointGeometrySize = 15,
                }

            };

            if (dataGridView1.RowCount > 1)
            {
                cartesianChart2.Visible = true;
            }

        }

        private void Button1_Click_2(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void DataGridView1_CellEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            CumAmt();
        }

        private void DataGridView1_CellLeave_1(object sender, DataGridViewCellEventArgs e)
        {
            CumAmt();
        }
    }


    }
 

