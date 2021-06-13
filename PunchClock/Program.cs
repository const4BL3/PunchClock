using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System;
using System.Configuration;
using System.Threading;

namespace PunchClock
{
    class Program
    {
        static void Main(string[] args)
        { 
            EdgeOptions options = new EdgeOptions
            {
                PageLoadStrategy = PageLoadStrategy.Normal
            };
            EdgeDriver _driver = new EdgeDriver(options);
            Login(_driver);
            CheckTime(_driver);
            _driver.Quit();
        }

        public static void Login(EdgeDriver driver)
        {
            // Replace with your own test logic\
            driver.Url = "https://login.clicktime.com";
            driver.FindElement(By.Id("email")).SendKeys(ConfigurationManager.AppSettings.Get("loginName"));
            driver.FindElement(By.Id("password")).SendKeys(ConfigurationManager.AppSettings.Get("password"));
            driver.FindElement(By.Id("loginbutton")).Click();
        }

        public static void CheckTime(EdgeDriver driver)
        {
            var day = DateTime.Today.DayOfWeek;

            var timesheetStatus = driver.FindElement(By.Id("TimesheetStatusText")).Text;

            if (timesheetStatus.Contains("Open"))
            {
                var MondayTotal = double.Parse(driver.FindElement(By.Id("DayTotal_C0")).Text);
                var TuesdayTotal = double.Parse(driver.FindElement(By.Id("DayTotal_C1")).Text);
                var WednesdayTotal = double.Parse(driver.FindElement(By.Id("DayTotal_C2")).Text);
                var ThursdayTotal = double.Parse(driver.FindElement(By.Id("DayTotal_C3")).Text);
                var FridayTotal = double.Parse(driver.FindElement(By.Id("DayTotal_C4")).Text);



                if (day >= DayOfWeek.Monday && MondayTotal < 8.0)
                {
                    double addTime = 8.0 - MondayTotal;
                    driver.FindElement(By.Id("AT0_C0Hours-inputEl")).SendKeys(addTime.ToString());
                }
                if (day >= DayOfWeek.Tuesday && TuesdayTotal < 8.0)
                {
                    double addTime = 8.0 - TuesdayTotal;
                    driver.FindElement(By.Id("AT0_C1Hours-inputEl")).SendKeys(addTime.ToString());
                }
                if (day >= DayOfWeek.Wednesday && WednesdayTotal != 8.0)
                {
                    double addTime = 8.0 - WednesdayTotal;
                    driver.FindElement(By.Id("AT0_C2Hours-inputEl")).SendKeys(addTime.ToString());
                }
                if (day >= DayOfWeek.Thursday && ThursdayTotal < 8.0)
                {
                    double addTime = 8.0 - ThursdayTotal;
                    driver.FindElement(By.Id("AT0_C3Hours-inputEl")).SendKeys(addTime.ToString());
                }
                if (day >= DayOfWeek.Friday && FridayTotal < 8.0)
                {
                    double addTime = 8.0 - FridayTotal;
                    driver.FindElement(By.Id("AT0_C4Hours-inputEl")).SendKeys(addTime.ToString());
                }
                driver.FindElement(By.Id("SaveButtonBottom-btnInnerEl")).Click();

                double GrandTotal = double.Parse(driver.FindElement(By.Id("GrandTotalCell")).Text);
                if (day >= DayOfWeek.Friday && GrandTotal == 40.0)
                {
                    Console.Write("Confirm Ready to Send(Y/N): ");
                    string send = Console.ReadLine();
                    if (send[0].ToString().ToUpper() == "Y")
                    {
                        driver.FindElement(By.Id("SubmitTimesheetLink")).Click();
                        Console.WriteLine("Timesheet has been submitted.  Press any key to end program.");
                        Console.Read();
                    }
                    else
                    {
                        Console.WriteLine("Timesheet is ready for manual review.  Press any key to end program.");
                        Console.Read();
                    }
                }
            }
            else
            {
                Console.WriteLine("This Timesheet has already been submitted.  Press any key to end program.");
                Console.Read();
            }
        }

        public static void WaitForElementDisplayed(EdgeDriver driver, WebElement element)
        {
            if (!element.Displayed)
            {
                Thread.Sleep(500);
                WaitForElementDisplayed(driver, element);
            }
        }
    }
}
