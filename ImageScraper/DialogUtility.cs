using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageScraper
{
    public static class DialogUtility
    {
        /// <summary>
        /// Launches a dialog to select a folder.
        /// </summary>
        /// <returns>The selected folder's path or string.Empty if no folder was selected.</returns>
        public static string GetFolderPath()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.ShowDialog();

            return fbd.SelectedPath.ToString();
        }
    }
}
