using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageScraper
{
    public class ImageScraperPresenter
    {
        public readonly string UrlErrorMessage = "An Error occured while validating the given Url: ";
        public readonly string DestinationErrorMessage = "An Error occured while validating the given Destination: ";
        public readonly string IMGProcessingErrorMessage = "An Error occured while processing the following image source: ";

        private IScraper _view;

        public ImageScraperPresenter(IScraper view)
        {
            _view = view;
        }

        /// <summary>
        /// Takes images from the given url and saves them to the given desination.
        /// </summary>
        /// <param name="url">The url to scrape images from</param>
        /// <param name="destination">The destination for the images</param>
        public void ScrapeImages(string url, string destination)
        {
            //Make sure the url and destinaiton folder are valid before moving on
            if (UrlIsValid(url) && DestinationIsValid(destination))
            {
                List<string> imageSources = GetImageSourcesFromUrl(url);

                AppendUrl(url, ref imageSources);
                RemoveQueryStrings(ref imageSources);

                DownloadImages(imageSources, destination);
            }
        }

        /// <summary>
        /// Checks the passed in url to make sure it is valid.
        /// Calls DisplayErrorMessage from the view if an error occurs.
        /// </summary>
        /// <param name="url">The url whose validity is to be determined</param>
        /// <returns>True if the url is valid and false if it is not</returns>
        private bool UrlIsValid(string url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

                //Use the HEAD request to check that the site is there without being too costly in terms of bandwidth
                request.Method = "HEAD";

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                response.Close();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    //Provide the view with a message to indicate failure
                    _view.DisplayErrorMessage(UrlErrorMessage + url + "\n" + response.StatusDescription);
                    return false;
                }
            }
            catch (Exception e)
            {
                //Provide the view with a message to indicate failure
                _view.DisplayErrorMessage(UrlErrorMessage + url + "\n" + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Checks the passed in destination to make sure it is valid.
        /// Creates a folder at the given destination if it does not exist.
        /// Calls DisplayErrorMessage from the view if an error occurs.
        /// </summary>
        /// <param name="destination"></param>
        /// <returns>True if the destination is valid and false if it is not</returns>
        private bool DestinationIsValid(string destination)
        {
            try
            {
                // If the directory doesn't exist, create it.
                if (!Directory.Exists(destination))
                {
                    Directory.CreateDirectory(destination);
                }

                return true;
            }
            catch (Exception e)
            {
                //Provide the view with a message to indicate failure
                _view.DisplayErrorMessage(DestinationErrorMessage + destination + "\n" + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Downloads the HTML from the given url and extracts raw image sources from it.
        /// </summary>
        /// <param name="url">The url to scrape images from</param>
        /// <returns>A list of raw image sources</returns>
        private List<string> GetImageSourcesFromUrl(string url)
        {
            List<string> imageSources = new List<string>();

            //Retrieve the raw html code for the url
            string html = new WebClient().DownloadString(url);

            //Separate the raw html into lines by the img tag
            string[] lines = Regex.Split(html, "<img");

            foreach (string line in lines)
            {
                if (line.Contains("src"))
                {
                    string source = line;

                    try
                    {
                        //Cut off the start of the img tag up to the point of where the source declaration begins
                        source = line.Substring(line.IndexOf("src"));

                        //Search for the opening quote and cut off everything before that, including the opening quote
                        int start = line.IndexOf("\"") + 1;
                        source = line.Substring(start);

                        //Search for the closing quote and cut off the rest of the string
                        int end = source.IndexOf("\"");
                        source = source.Substring(0, end);

                        if (source != string.Empty)
                        {
                            imageSources.Add(source);
                        }
                    }
                    //The only time this should be hit is if the img tag does not contain a src declaration or opening and closing quotes
                    catch
                    {
                        _view.DisplayErrorMessage(imageSources + source);
                    }
                }
            }

            return imageSources;
        }

        /// <summary>
        /// Some image sources may have been referencing the css from the web page rather than specifying the whole url.
        /// Adds the missing url to the beggining of the image source.
        /// </summary>
        /// <param name="url">The original url which the images came from</param>
        /// <param name="imageSources">Sources for the scrubbed images</param>
        private void AppendUrl(string url, ref List<string> imageSources)
        {
            for (int i = 0; i < imageSources.Count; i++)
            {
                if (!imageSources[i].Contains("http"))
                {
                    //Add '/' after the url because while it won't mind if there are two, it will if there are none
                    imageSources[i] = url + '/' + imageSources[i];
                }
            }
        }

        /// <summary>
        /// Some image sources may have a query string added to the extension.
        /// Removes the query string so that we can save the original image later.
        /// </summary>
        /// <param name="imageSources">Sources for the scrubbed images</param>
        private void RemoveQueryStrings(ref List<string> imageSources)
        {
            for (int i = 0; i < imageSources.Count; i++)
            {
                string currentSource = imageSources[i];

                //Find the first '?' indicating the start of the query string
                int queryStringStart = currentSource.IndexOf('?');

                //If the query string indicator exists, remove the query string
                if (queryStringStart != -1)
                {
                    imageSources[i] = currentSource.Substring(0, queryStringStart);
                }
            }
        }

        /// <summary>
        /// Stores the image sources that are valid to the specified destination.
        /// Displays
        /// </summary>
        /// <param name="imageSources">Sources for the scrubbed images</param>
        /// <param name="destination">The destination to which the images will be saved</param>
        private void DownloadImages(List<string> imageSources, string destination)
        {
            foreach (string url in imageSources)
            {
                if (UrlIsValid(url))
                {
                    //Now that we know this is a valid url
                    //Identify the end of the url which is the image name and extension
                    string extension = url.Substring(url.LastIndexOf('/'));
                    
                    //Finally, save our image to our destination
                    new WebClient().DownloadFile(new Uri(url), destination + extension);
                }
            }
        }
    }
}
