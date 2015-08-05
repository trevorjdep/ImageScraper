using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageScraper;
using System.Collections.Generic;

namespace UnitTests
{
    /// <summary>
    /// A few simple tests I used while debugging my code.
    /// </summary>
    [TestClass]
    public class ImageScraperTests
    {
        private readonly string url = "http://cleanmyrivers.org/";
        private readonly string destination = @"C:\Users\trevor.depender\Downloads";

        private IScraper _view;
        private ImageScraperPresenter _presenter;

        [TestInitialize]
        public void TestInitialize()
        {
            _view = new TestScraperView();
            _presenter = new ImageScraperPresenter(_view);
        }

        [TestMethod]
        public void ScrapeTest_ValidUrl_ValidDest()
        {
            _presenter.ScrapeImages(url, destination);
            Assert.IsTrue((_view as TestScraperView).results.Count == 0);
        }

        [TestMethod]
        public void ScrapeTest_InValidUrl_ValidDest()
        {
            _presenter.ScrapeImages(string.Empty, destination);
            Assert.IsTrue((_view as TestScraperView).results.Count > 0);
        }

        [TestMethod]
        public void ScrapeTest_ValidUrl_InValidDest()
        {
            _presenter.ScrapeImages(url, string.Empty);
            Assert.IsTrue((_view as TestScraperView).results.Count > 0);
        }
    }

    internal class TestScraperView : IScraper
    {
        public List<string> results = new List<string>();
        
        public void DisplayErrorMessage(string message)
        {
            results.Add(message);
        }
    }
}
