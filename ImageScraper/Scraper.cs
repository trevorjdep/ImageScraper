using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageScraper
{
    /// <summary>
    /// Implementation of a view for the ImageScraperPresenter
    /// </summary>
    public partial class Scraper : Form, IScraper
    {
        private ImageScraperPresenter _scrapePresenter;
        private string _destinationFolder = string.Empty;

        public Scraper()
        {
            InitializeComponent();

            _scrapePresenter = new ImageScraperPresenter(this);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            _destinationFolder = DialogUtility.GetFolderPath();
            txtDestination.Text = _destinationFolder.ToString();
        }

        private void btnScrub_Click(object sender, EventArgs e)
        {
            string url = txtUrl.Text.Trim();

            _scrapePresenter.ScrapeImages(url, _destinationFolder);

            MessageBox.Show("Image scrub completed.");
        }

        public void DisplayErrorMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}
