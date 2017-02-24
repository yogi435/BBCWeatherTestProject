using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace BBCWeather
{
    [TestClass]
    public class Weather
    {

        string browser = Properties.Settings.Default.Browser;

        IWebDriver Driver;

        string XPathForPressure = "";
        [TestMethod]
        public void GetPressureDifference()
        {

            var options = new InternetExplorerOptions();
            options.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
            options.UnexpectedAlertBehavior = InternetExplorerUnexpectedAlertBehavior.Accept;
            Driver = new InternetExplorerDriver(options);

            Driver.Navigate().GoToUrl("http://www.bbc.co.uk/weather/");

            Driver.FindElement(By.Name("search")).SendKeys("Reading,Reading");
            Driver.FindElement(By.Name("submitBtn")).Click();
            Thread.Sleep(TimeSpan.FromSeconds(2));
            Driver.FindElement(By.Id("detail-table-view")).Click();
            //Building XPath for todays ( date as Current Day)pressure , Can be modified to any day by changing the contains text to different date
                        
            XPathForPressure = "//*[@id='hourly']/div[3]/table[./caption[contains(text(),'"+DateTime.Now.ToString("dd")+"')]]/tfoot/tr[3]/td[" + GetPressureColumnIndex(21) + "]";
            int todayPressure = Int32.Parse(Driver.FindElement(By.XPath(XPathForPressure)).Text);
            Console.WriteLine("Todays Pressure is :"+ todayPressure);

            string tomorrow = DateTime.Now.AddDays(1).Date.ToString("yyyyMMdd");
            
            //Clicking on the 2017-02-23 report
            Driver.FindElement(By.XPath("//*[@id='blq-content']/div[7]/div[2]/ul/li[contains(@class,'"+ tomorrow + "')]/a")).Click();
            Thread.Sleep(TimeSpan.FromSeconds(2));


            //Building XPath for tomorrows ( date as 23)pressure , Can be modified to any day by changing the contains text to different date
            XPathForPressure = "//*[@id='hourly']/div[3]/table[./caption[contains(text(),'"+ DateTime.Now.AddDays(1).ToString("dd")+"')]]/tfoot/tr[3]/td[" + GetPressureColumnIndex(21) + "]";
            int tomorrowPressure = Int32.Parse(Driver.FindElement(By.XPath(XPathForPressure)).Text);
            Console.WriteLine("Tomorrows Pressure is :"+ tomorrowPressure);
            Console.WriteLine("Pressure difference between today and tomorrow: " + (todayPressure - tomorrowPressure));

            Driver.Quit();


        }



        /// <summary>
        /// Get the Pressure index/column number reference to the hour of the time
        /// </summary>
        /// <param name="hour"> hour of the time for which the pressure needed</param>
        /// <returns>Column index</returns>
        private int GetPressureColumnIndex(int hour)
        {
            IReadOnlyCollection<IWebElement> WeatherTimeElements = Driver.FindElements(By.XPath("//table/thead/tr/th[*]/span[1]"));


            for (int i = 2; i < WeatherTimeElements.Count - 1; i++)
            {

                string XPathPrefixForTime = "//table/thead/tr/th[" + (i) + "]/span[1]";
                try
                {
                    if (Driver.FindElement(By.XPath(XPathPrefixForTime)).Text.Contains(hour.ToString()))
                    {

                        return i - 1;
                    }
                }

                catch (Exception e)
                {

                }

            }
            return 0;
        }
    }
}
