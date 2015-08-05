using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageScraper
{
    public interface IScraper
    {
        //If an error occurs while scraping images, a message with a description of the error will be returned here
        void DisplayErrorMessage(string message);
    }
}
