using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Bunifu.Dataviz.WinForms.BunifuDatavizAdvanced;

namespace Desktop_COVID_19
{
    public partial class AllCountryForm : Form
    {
        public AllCountryForm()
        {
            InitializeComponent();
        }
        DataTable countriesDataTable = new DataTable();
        DataTable historyDataTable = new DataTable();
        string httpFeedback, countryName;
        WorldStat worldStat;

        //Dataviz datapoints
        DataPoint totalCasesdataPoint = new DataPoint(_type.Bunifu_bar);
        DataPoint newCasesdataPoint = new DataPoint(_type.Bunifu_column);
        DataPoint criticalCasesdataPoint = new DataPoint(_type.Bunifu_column);
        DataPoint deathCasesdataPoint = new DataPoint(_type.Bunifu_column);
        DataPoint recoveredCasesdataPoint = new DataPoint(_type.Bunifu_column);

        private void AllCountryForm_Load(object sender, EventArgs e)
        {
            toastControl1.Visible = true;
            //bunifuDataGridView2.PopulateWithSampleData();
            backgroundWorker1.RunWorkerAsync();           
        }

        private void countryBunifuButton_Click(object sender, EventArgs e)
        {
            CountryForm countryForm = new CountryForm();
            countryForm.countryData = countriesDataTable;
            /*countryForm.Location = new Point(countryBunifuButton.Location.X + 25, countryBunifuButton.Location.Y + 55);*/
            countryForm.ShowDialog();
            if (countryForm.result == DialogResult.OK)
            {
                //countryBunifuLabel.Text = countryForm.selectedCountry;
                countryBunifuLabel.Text = countryForm.selectedCountry;
                countryName = countryForm.selectedCountry;
                toastControl1.Visible = true;


                //clear history datatable if data is not null
                if (historyDataTable.Columns.Count > 0)
                {
                    historyDataTable.Columns.Clear();
                }
                if (historyDataTable.Rows.Count > 0)
                {
                    historyDataTable.Rows.Clear();
                }
                bunifuDataGridView1.DataSource = null;
                //clear datapoints 
                totalCasesdataPoint.clear();
                newCasesdataPoint.clear();
                recoveredCasesdataPoint.clear();
                deathCasesdataPoint.clear();
                criticalCasesdataPoint.clear();
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!string.IsNullOrEmpty(countryName))
            {
                GetCountryCovid19History(countryName);

            }
            else
            {
                GetAllCountries();
                GetWorldStat();
            }


            if (backgroundWorker1.CancellationPending == true)
            {
                e.Cancel = true;
                return;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toastControl1.Visible = false;
            timer1.Stop();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                timer1.Start();
                MessageBox.Show(httpFeedback);

                #region default_zero_values
                acBunifuLabel.Text = "0";
                tcBunifuLabel.Text = "0";
                ccBunifuLabel.Text = "0";
                dcBunifuLabel.Text = "0";
                tcBunifuLabel2.Text = "0";
                tcBunifuLabel3.Text = "0";

                bunifuCircleProgress1.Value = 0;
                bunifuRadialGauge1.Value = 0;
                bunifuCircleProgress3.Value = 0;
                #endregion
            }
            else
            {
                Console.WriteLine(historyDataTable.Rows.Count);
                if (historyDataTable.Rows.Count > 0)
                {
                    bunifuDataGridView1.DataSource = historyDataTable;
                    //expand the first column
                    bunifuDataGridView1.Columns[0].Width = 110;

                    //Assign the first row data of the dataTable to Labels showing a country's latest data


                    #region label_assignments
                    acBunifuLabel.Text = historyDataTable.Rows[0]["Active cases"].ToString();
                    tcBunifuLabel.Text = historyDataTable.Rows[0]["Total cases"].ToString();
                    ccBunifuLabel.Text = historyDataTable.Rows[0]["Critical cases"].ToString();
                    dcBunifuLabel.Text = historyDataTable.Rows[0]["Total deaths"].ToString();
                    tcBunifuLabel2.Text = historyDataTable.Rows[0]["Total cases"].ToString();
                    tcBunifuLabel3.Text = historyDataTable.Rows[0]["Total cases"].ToString();
                    #endregion

                    //calculate percentages and assign the values to the CircleProgressBars and the Radial Gauge
                    int activeCaseNumbers = int.Parse(acBunifuLabel.Text, System.Globalization.NumberStyles.AllowThousands);
                    int criticalCaseNumbers = int.Parse(ccBunifuLabel.Text, System.Globalization.NumberStyles.AllowThousands);
                    int deathCaseNumbers = int.Parse(dcBunifuLabel.Text, System.Globalization.NumberStyles.AllowThousands);
                    int totalCaseNumbers = int.Parse(tcBunifuLabel.Text, System.Globalization.NumberStyles.AllowThousands);

                    int activeCasePercentage = (100 * activeCaseNumbers) / totalCaseNumbers;
                    int criticalCasePercentage = (100 * criticalCaseNumbers) / totalCaseNumbers;
                    int deathCasePercentage = (100 * deathCaseNumbers) / totalCaseNumbers;


                    //assign the values to the controls
                    bunifuCircleProgress1.Value = activeCasePercentage;
                    bunifuRadialGauge1.Value = criticalCaseNumbers;
                    bunifuCircleProgress3.Value = deathCasePercentage;
                 
                    //Add datapoints to canvas
                    Canvas statisticsCanvas = new Canvas();
                    statisticsCanvas.addData(totalCasesdataPoint);
                    /*statisticsCanvas.addData(newCasesdataPoint);
                    statisticsCanvas.addData(criticalCasesdataPoint);
                    statisticsCanvas.addData(recoveredCasesdataPoint);
                    statisticsCanvas.addData(deathCasesdataPoint);*/

                    // add colors to the datapoints
                    bunifuDatavizAdvanced1.colorSet.Add(Color.FromArgb(254, 62, 88));
                    bunifuDatavizAdvanced1.colorSet.Add(Color.FromArgb(0, 255, 255));
                    bunifuDatavizAdvanced1.colorSet.Add(Color.FromArgb(246, 195, 90));
                    bunifuDatavizAdvanced1.colorSet.Add(Color.FromArgb(255, 255, 255));
                    bunifuDatavizAdvanced1.colorSet.Add(Color.FromArgb(146, 84, 205));
                    bunifuDatavizAdvanced1.colorSet.Add(Color.FromArgb(87, 166, 74));
                    bunifuDatavizAdvanced1.colorSet.Add(Color.FromArgb(255, 94, 19));

                    //render canvas to dataviz
                    bunifuDatavizAdvanced1.Render(statisticsCanvas);
                }
                timer1.Start();
                //MessageBox.Show("Data loaded");
                //assign worldwide stats to the labels
                wtBunifuLabel.Text = worldStat.total_cases;
                wRbunifuLabel.Text = worldStat.total_recovered;
                wDbunifuLabel.Text = worldStat.total_deaths;
            }
        }

        private void GetAllCountries()
        {
            var client = new RestClient("https://restcountries-v1.p.rapidapi.com/all");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "restcountries-v1.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "dd29196058msh8c69bf9c3a1229bp1807ffjsned3c52cf3144");
            IRestResponse response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = response.Content;
                var countries = JsonConvert.DeserializeObject<List<Country>>(content);
                // add data to the countries dataTable
                countriesDataTable.Columns.Add("Name");
                foreach (var country in countries)
                {
                    countriesDataTable.Rows.Add(country.name);
                }
            }
            else
            {
                httpFeedback = response.ErrorMessage;
                backgroundWorker1.CancelAsync();
            }
        }

        private void bunifuButton2_Click(object sender, EventArgs e)
        {

        }

        private void GetCountryCovid19History(string country)
        {
            var client = new RestClient($"https://coronavirus-monitor.p.rapidapi.com/coronavirus/cases_by_particular_country.php?country={country}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "coronavirus-monitor.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "dd29196058msh8c69bf9c3a1229bp1807ffjsned3c52cf3144");
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = response.Content;
                var historyData = JsonConvert.DeserializeObject<History_stats>(content);
                if (historyData != null)
                {
                    //Add data to datatable
                    //Creating some columns
                    DataColumn[] dataColumns = new DataColumn[]
                    {
                        new DataColumn("Record date"),
                        new DataColumn("Total cases"),
                        new DataColumn("New cases"),
                        new DataColumn("Active cases"),
                        new DataColumn("Total deaths"),
                        new DataColumn("Total recovered"),
                        new DataColumn("Critical cases")
                    };
                    //Add columns to data table
                    historyDataTable.Columns.AddRange(dataColumns);

                    //reverse the List
                    historyData.stat_by_country.Reverse();

                    //Get a 7-day history data

                    //get last 7 dates from today
                    DateTime[] last_seven_days = Enumerable.Range(0, 7).Select(i => DateTime.Now.Date.AddDays(-i)).ToArray();
                    //creating dictionaries for the dataviz datapoints that will later help us reverse the list for the datapoints
                    Dictionary<string, int> totalCasesDictionary = new Dictionary<string, int>();
                    Dictionary<string, int> newCasesDictionary = new Dictionary<string, int>();
                    Dictionary<string, int> recoveredCasesDictionary = new Dictionary<string, int>();
                    Dictionary<string, int> deathCasesDictionary = new Dictionary<string, int>();
                    Dictionary<string, int> criticalCasesDictionary = new Dictionary<string, int>();
                    foreach (var day in last_seven_days)
                    {
                        //Add data to the dataTable
                        foreach (var data in historyData.stat_by_country)
                        {
                            #region convert_to_zero_data
                            //convert the empty strings into zero's
                            if (data.new_cases == "")
                            {
                                data.new_cases = "0";
                            }
                            if (data.new_deaths == "")
                            {
                                data.new_deaths = "0";
                            }
                            if (data.serious_critical == "")
                            {
                                data.serious_critical = "0";
                            }
                            if (data.total_recovered == "")
                            {
                                data.total_recovered = "0";
                            }
                            if (data.total_deaths == "")
                            {
                                data.total_deaths = "0";
                            }
                            #endregion
                            if (data.record_date.Contains($"{day:yyyy-MM-dd}"))
                            {
                                //get the day
                                DateTime dateTime = new DateTime(day.Date.Year, day.Date.Month, day.Date.Day);
                                historyDataTable.Rows.Add($"{day:dd-MM-yyyy}" + " " + dateTime.ToString("ddd"), data.total_cases, data.new_cases, data.active_cases, data.total_deaths, data.total_recovered, data.serious_critical);
                                //add data to the dictionaries
                                totalCasesDictionary.Add(dateTime.ToString("ddd"), int.Parse(data.total_cases, System.Globalization.NumberStyles.AllowThousands));
                                newCasesDictionary.Add(dateTime.ToString("ddd"), int.Parse(data.new_cases, System.Globalization.NumberStyles.AllowThousands));
                                recoveredCasesDictionary.Add(dateTime.ToString("ddd"), int.Parse(data.total_recovered, System.Globalization.NumberStyles.AllowThousands));
                                deathCasesDictionary.Add(dateTime.ToString("ddd"), int.Parse(data.total_deaths, System.Globalization.NumberStyles.AllowThousands));
                                criticalCasesDictionary.Add(dateTime.ToString("ddd"), int.Parse(data.serious_critical, System.Globalization.NumberStyles.AllowThousands));

                                //break out of the loop
                                break;
                            }
                        }
                    }

                    //reverse data in the dictionaries
                    var reversedTotalCases = totalCasesDictionary.Reverse();
                    var reversedNewCases = newCasesDictionary.Reverse();
                    var reversedRecoveredCases = recoveredCasesDictionary.Reverse();
                    var reversedDeathCases = deathCasesDictionary.Reverse();
                    var reversedCriticalCases = criticalCasesDictionary.Reverse();

                    //Add reversed data to BunifuDataviz datapoints

                    foreach (var totalCases in reversedTotalCases)
                    {
                        totalCasesdataPoint.addLabely(totalCases.Key, totalCases.Value);
                    }
                    foreach (var newCases in reversedNewCases)
                    {
                        newCasesdataPoint.addLabely(newCases.Key, newCases.Value);
                    }
                    foreach (var recoveredCases in reversedRecoveredCases)
                    {
                        recoveredCasesdataPoint.addLabely(recoveredCases.Key, recoveredCases.Value);
                    }
                    foreach (var deathCases in reversedDeathCases)
                    {
                        deathCasesdataPoint.addLabely(deathCases.Key, deathCases.Value);
                    }
                    foreach (var criticalCases in reversedCriticalCases)
                    {
                        criticalCasesdataPoint.addLabely(criticalCases.Key, criticalCases.Value);
                    }
                }
                else
                {

                    httpFeedback = "No data";
                    backgroundWorker1.CancelAsync();
                }
            }
            else
            {
                httpFeedback = response.StatusDescription;
                backgroundWorker1.CancelAsync();
            }
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            
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

    }
}
