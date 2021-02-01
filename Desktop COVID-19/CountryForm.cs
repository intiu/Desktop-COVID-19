using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Desktop_COVID_19
{
    public partial class CountryForm : Form
    {
        public DataTable countryData { get; set; }
        public DialogResult result { get; set; }
        public string selectedCountry { get; set; }
        public CountryForm()
        {
            InitializeComponent();
        }

        private void CountryForm_Load(object sender, EventArgs e)
        {
            //bunifuDataGridView1.PopulateWithSampleData();
            countryBunifuTextBox.Focus();
            bunifuDataGridView1.DataSource = countryData;
            bunifuVScrollBar1.Maximum = countryData.Rows.Count;
        }

        private void countryBunifuTextBox_TextChange(object sender, EventArgs e)
        {
            countryData.DefaultView.RowFilter = string.Format("name LIKE '{0}*'", countryBunifuTextBox.Text);
            if (countryData.DefaultView.Count == 0)
            {
                bunifuVScrollBar1.Enabled = false;
            }
            else
            {
                bunifuVScrollBar1.Enabled = true;

                bunifuVScrollBar1.Maximum = countryData.DefaultView.Count;
            }
        }

        private void bunifuVScrollBar1_Scroll(object sender, Bunifu.UI.WinForms.BunifuVScrollBar.ScrollEventArgs e)
        {
            bunifuDataGridView1.FirstDisplayedScrollingRowIndex = e.Value;
        }

        private void bunifuDataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedCountry = bunifuDataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            result = DialogResult.OK;
            this.Hide();
        }
    }
}
