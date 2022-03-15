using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using CC_Library;
using CC_Library.Datatypes;
using CC_Library.Predictions;

namespace DataAnalysis
{
    public class Form1 : Form
    {
        private System.ComponentModel.IContainer components = null;
        Chart chart1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            chart1.Series.Add("series1");
            chart1.Series.Add("series2");

            OpenFileDialog ofd = new OpenFileDialog()
            {
                FileName = "Select a text file",
                Title = "Open txt file"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                chart1.Series["series1"].Enabled = true;
                chart1.Series["series2"].Enabled = true;
                chart1.Visible = true;
                int runs = 0;
                double er = 0;
                double acc = 0;
                var filepath = ofd.FileName;
                var dir = Path.GetDirectoryName(filepath);
                var Files = Directory.GetFiles(dir);
                Random random = new Random();
                for (int i = 0; i < 1000; i++)
                {
                    string f = Files[random.Next(Files.Count())];
                    try
                    {
                        //var error = Predictionary.Propogate(f, write);

                        Sample s = f.ReadFromBinaryFile<Sample>();
                        string datatype = s.Datatype;
                        var error = MasterformatNetwork.Propogate(s, CMDLibrary.WriteNull, true);

                        //var error = new MasterformatNetwork(s).PropogateSingle(s, write, true);
                        if (error[0] > 0)
                        {
                            runs++;
                            er += error[0];
                            acc += error[1];
                            chart1.Series["series1"].Points.AddXY(i, er);
                            chart1.Series["series2"].Points.AddXY(i, error[0]);
                            chart1.Invalidate();
                        }
                    }
                    catch (Exception exc) { exc.OutputError(); }

                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            ChartArea chartArea1 = new ChartArea();
            Legend legend1 = new Legend();
            this.chart1 = new Chart();
            //((System.ComponentModel.ISupportInitialize)(chart1)).BeginInit();
            this.SuspendLayout();

            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Dock = DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(0, 50);
            this.chart1.Name = "chart1";
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.chart1);
            this.Name = "Form1";
            this.Text = "FakeChart";
            this.Load += new System.EventHandler(this.Form1_Load);
            //((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
        }
    }
}