using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Bunifu.Dataviz.WinForms.BunifuDatavizAdvanced;

namespace Desktop_COVID_19
{
    public partial class DashForm : Form
    {
        public DashForm()
        {
            InitializeComponent();
        }
       
        string httpFeedback;
        WorldStat worldStat;

        private void gunaLabel11_Click(object sender, EventArgs e)
        {
            panel1.Location = new Point(gunaLabel11.Location.X + 5, 58);
        }

        private void gunaLabel12_Click(object sender, EventArgs e)
        {
            panel1.Location = new Point(gunaLabel12.Location.X + 5, 58);
            var a4 = new ProcessStartInfo("chrome.exe");
            a4.Arguments = "https://github.com/intiu/Desktop-COVID-19";
            Process.Start(a4);
        }

        private void gunaLabel13_Click(object sender, EventArgs e)
        {
            panel1.Location = new Point(gunaLabel13.Location.X + 10, 58);
            var a3 = new ProcessStartInfo("chrome.exe");
            a3.Arguments = "https://www.facebook.com/phuong.lethanh.3158";
            Process.Start(a3);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            GetWorldStat();
            if (backgroundWorker1.CancellationPending == true)
            {
                e.Cancel = true;
                return;
            }
        }

        private void DashForm_Load(object sender, EventArgs e)
        {
            toastControl1.Visible = true;
            backgroundWorker1.RunWorkerAsync();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toastControl1.Visible = false;
            timer1.Stop();
        }

        private void GetWorldStat()
        {
            var client = new RestClient("https://coronavirus-monitor.p.rapidapi.com/coronavirus/worldstat.php");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "coronavirus-monitor.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "dd29196058msh8c69bf9c3a1229bp1807ffjsned3c52cf3144");
            IRestResponse response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = response.Content;
                worldStat = JsonConvert.DeserializeObject<WorldStat>(content);
            }
            else
            {
                httpFeedback = response.ErrorMessage;
                backgroundWorker1.CancelAsync();
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                timer1.Start();
                MessageBox.Show(httpFeedback);
            }
            timer1.Start();
            wtBunifuLabel.Text = worldStat.total_cases;
            wRbunifuLabel.Text = worldStat.total_recovered;
            wDbunifuLabel.Text = worldStat.total_deaths;
        }

        private void guna2GradientButton3_Click(object sender, EventArgs e)
        {
            AllCountryForm allCountryForm = new AllCountryForm();
            //this.Hide();
            allCountryForm.Show();
            this.Show();
        }

        private void guna2GradientButton2_Click(object sender, EventArgs e)
        {
            var a1 = new ProcessStartInfo("chrome.exe");
            a1.Arguments = "https://ncov.moh.gov.vn";
            Process.Start(a1);
        }

        private void guna2GradientButton1_Click(object sender, EventArgs e)
        {
            var a2 = new ProcessStartInfo("chrome.exe");
            a2.Arguments = "https://www.youtube.com/watch?v=BtulL3oArQw";
            Process.Start(a2);
        }
    }
}
