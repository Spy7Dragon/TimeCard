using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using TimeCard.Properties;

namespace TimeCard
{
    public class Externals
    {
        private const string REQUEST_LINK = "https://docs.google.com/forms/d/e/1FAIpQLSfdr1PF51-6JAyiJ45pUmOa_R69kzgDiBEXxQ67ao_jGJgh_A/viewform";

        private const string PRE_FILLED_LINK =
                "https://docs.google.com/forms/d/e/1FAIpQLSfdr1PF51-6JAyiJ45pUmOa_R69kzgDiBEXxQ67ao_jGJgh_A/viewform?usp=pp_url&entry.851319557=Bug&entry.1355839897=Automated+GUI+Error&entry.865993389"
            ;
        private static string s_Browser = "chrome";
        private static ToolStripMenuItem s_BrowserMenu;
        private static TextBox s_StatusBox;
        private static IWebDriver s_Driver;


        public static IWebDriver Driver { get; set; }

        public static string Browser
        {
            get { return s_Browser; }
            set
            {
                if (s_Browser != value)
                {
                    s_Browser = value;
                }
            }
        }

        public static ToolStripMenuItem BrowserMenu
        {
            get { return s_BrowserMenu; }
            set
            {
                if (s_BrowserMenu != value)
                {
                    s_BrowserMenu = value;
                }
            }
        }

        public static TextBox StatusBox
        {
            get { return s_StatusBox; }
            set
            {
                if (s_StatusBox != value)
                {
                    s_StatusBox = value;
                }
            }
        }

        static Externals()
        {
            Browser = Settings.Default.browser;
        }

        public static void SetBrowser(object sender, EventArgs e)
        {
            ToolStripMenuItem menu_item = sender as ToolStripMenuItem;
            if (menu_item != null)
            {
                Browser = menu_item.Text.ToLower();
                foreach (ToolStripMenuItem item in BrowserMenu.DropDownItems)
                {
                    if (Browser == item.Text.ToLower())
                    {
                        item.Checked = true;
                    }
                    else
                    {
                        item.Checked = false;
                    }
                }
            }

            Settings.Default.browser = s_Browser;
            Settings.Default.Save();
        }

        public static void SubmitRequest(object sender, EventArgs e)
        {
            try
            {
                Process.Start(s_Browser, REQUEST_LINK);
            }
            catch (Exception exp)
            {
                string message = "Selected browser:" + s_Browser.ToUpper() + " is unavailable.\n"
                                 + exp.Message + "\n" + exp.StackTrace;
                Logger.Log(message);
                MessageBox.Show(message);
            }
        }

        public static void SubmitGuiError(object sender, EventArgs e)
        {
            GetHiddenDriver();

            string ending_message;
            try
            {
                Driver.Navigate().GoToUrl(PRE_FILLED_LINK);

                WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30.0));
                ReadOnlyCollection<IWebElement> request = wait.Until(drv => drv.FindElements(By.TagName("textarea")).Count > 1 ? 
                drv.FindElements(By.TagName("textarea")) : null);

                request[1].SendKeys(FrmTimeCard.Instance.SaveGui());
                IWebElement submit = Driver.FindElement(By.XPath("//span[text()='Submit']"));

                ClickElement(submit);

                ending_message = "Successfully submitted GUI Error.";
            }
            catch (Exception exp)
            {
                ending_message = "Error submission failed.\n"
                                 + exp.Message + "\n" + exp.StackTrace;
            }
            finally
            {
                Driver.Quit();
            }

            Logger.Log(ending_message);
            MessageBox.Show(ending_message);
        }

        internal static void ShowAbout(object sender, EventArgs e)
        {
            AboutBox about_box = new AboutBox();
            about_box.ShowDialog();
        }

        public static bool SubmitDay(DataTable data, double total)
        {
            bool succesfully_submitted = false;
            if (data == null)
            {
                return false;
            }

            GetDriver();
            string ending_message;

            try
            {
                Driver.Navigate().GoToUrl("http://win.rockwellcollins.com/TimeEntry/TEWeeklyTimesheet.aspx?lp=true&ctzo=240");

                IWebElement element = GetElement(".//input[@value = 'Edit This Week']");
                element.Click();

                int count = 0;
                ReadOnlyCollection<IWebElement> rows = Driver.FindElements(By.XPath("//tr"));
                foreach (IWebElement row in rows)
                {
                    ReadOnlyCollection<IWebElement> tds = row.FindElements(By.XPath(".//td"));
                    if (tds[0].Text.Contains("Network Activities"))
                    {
                        DateTime day_of = DateTime.Parse(FrmTimeCard.Instance.lblDateTime.Text);
                        string day = day_of.ToString("MM/dd");
                        Logger.Log(String.Format("Looking for date: {0}", day));
                        int start = 0;
                        for (int i = 0; i < tds.Count; i++)
                        {
                            Logger.Log(String.Format("Checking {0}", tds[i].Text));
                            if (tds[i].Text.Contains("Network Activities"))
                            {
                                start = i;
                                continue;
                            }
                            if (tds[i].Text == day)
                            {
                                count = i - start + 2;
                                break;
                            }
                        }
                        Logger.Log(String.Format("The day is the number {0} column", count));
                        break;
                    }
                }
 
                List<string> identifiers = data.AsEnumerable().Select(t => t.Field<string>("Identification")).ToList();
                Logger.Log(String.Format("Starting to iterate {0} rows.", rows.Count));
                foreach (IWebElement row in rows)
                {
                    ReadOnlyCollection<IWebElement> tds = row.FindElements(By.XPath(".//td"));
                    string identifier_td;
                    if (tds.Count > 1)
                    {
                        identifier_td = tds[1].Text;
                    }
                    else
                    {
                        continue;
                    }
                    Logger.Log("Rows identifier table data is " + identifier_td);
                    string match = null;
                    foreach (string identity in identifiers)
                    {
                        if (identifier_td.Contains(identity))
                        {
                            match = identity;
                            break;
                        }
                    }
 
                    // GridUpdate row on matches.
                    if (match != null)
                    {
                        Logger.Log("Starting match: " + match);
                        string search = String.Format("Identification = '{0}'", match);
                        DataRow[] found_rows = data.Select(search);
                        string hours = "0.0";
                        if (found_rows.Length > 0)
                        {
                            hours = String.Format("{0:0.0}", found_rows[0]["Hours"]);
                        }
                        if (hours != "0.0")
                        {
                            Logger.Log("Inserting " + hours + " into row " + count + " of " + match);
                            ClearAndSend(tds[count], hours);
                        }
                    }
                }

                IWebElement submit = GetElement(".//input[@value = 'Save']");
                ClickElement(submit);

                ReadOnlyCollection<IWebElement> final_rows = Driver.FindElements(By.XPath("//tr"));
                IWebElement last_total = null;
                foreach (IWebElement row in final_rows)
                {
                    ReadOnlyCollection<IWebElement> tds = row.FindElements(By.XPath(".//td"));
                    if (tds.Count < 1)
                    {
                        continue;
                    }
                    if (tds[0].Text.Contains("Totals"))
                    {
                        last_total = tds[count];
                    }
                }

                if (last_total == null)
                {
                    ending_message = "Last total was not found.";
                }
                else
                {
                    double last_total_value;
                    try
                    {
                        last_total_value = Double.Parse(last_total.Text);
                    }
                    catch (Exception e)
                    {
                        Exception exception = new Exception("Received text: " + last_total.Text, e);
                        throw exception;
                    }
                    
                    if (Math.Abs(last_total_value - total) < 0.05)
                    {
                        ending_message = "Successfully submitted time!";
                        succesfully_submitted = true;
                    }
                    else
                    {
                        ending_message = String.Format("Error: Submitted {0} hours. Reported {1} hours.",
                            last_total_value, total);
                    }
                }
            }
            catch (Exception exp)
            {
                ending_message = "Error submission failed.\n"
                                 + exp.Message + "\n" + exp.StackTrace;
            }
            finally
            {
                Driver.Quit();
            }

            Logger.Log(ending_message);
            MessageBox.Show(ending_message);
            return succesfully_submitted;
        }

        private static void ClearAndSend(IWebElement webElement, string keys)
        {
            webElement.Click();
            IJavaScriptExecutor js = (IJavaScriptExecutor) Driver;
            js.ExecuteScript("arguments[0].value = arguments[1];", webElement, keys);
        }

        private static IWebElement GetElement(string xpath)
        {
            WebDriverWait submit_wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(60.0));
            IWebElement element = submit_wait.Until(x => x.FindElement(By.XPath(xpath)));
            return element;
        }

        private static void ClearElement(IWebElement element)
        {
            Actions actions = new Actions(Driver);
            actions.MoveToElement(element);
            actions.Click();
            // Diffent ways to clear.
            actions.KeyDown(OpenQA.Selenium.Keys.Control).SendKeys("a");
            actions.SendKeys(OpenQA.Selenium.Keys.Delete);
            actions.SendKeys(OpenQA.Selenium.Keys.Clear);
            actions.Build().Perform();
        }

        private static void SendKeysToElement(string keys, IWebElement element)
        {
            Actions actions = new Actions(Driver);
            actions.MoveToElement(element);
            actions.Click();
            actions.SendKeys(keys);
            actions.Build().Perform();
        }

        private static void ClickElement(IWebElement element)
        {
            Actions actions = new Actions(Driver);
            actions.MoveToElement(element);
            actions.Click();
            actions.Perform();
        }

        public static void GetDriver()
        {
            string driver_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drivers");
            switch (Browser)
            {
                case "chrome":
                    ChromeDriverService chrome_service = ChromeDriverService.CreateDefaultService(driver_path);
                    chrome_service.HideCommandPromptWindow = true;
                    Driver = new ChromeDriver(chrome_service);
                    break;
                case "firefox":
                    FirefoxDriverService firefox_service = FirefoxDriverService.CreateDefaultService();
                    firefox_service.HideCommandPromptWindow = true;
                    Driver = new FirefoxDriver(firefox_service);
                    break;
            }
        }

        public static void GetHiddenDriver()
        {

            string driver_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drivers");
            switch (Browser)
            {
                case "chrome":
                    ChromeDriverService service = ChromeDriverService.CreateDefaultService(driver_path);
                    service.HideCommandPromptWindow = true;
                    ChromeOptions chrome_options = new ChromeOptions();
                    chrome_options.AddArgument("--window-position=-32000,-32000");
                    Driver = new ChromeDriver(service, chrome_options);
                    break;
                case "firefox":
                    Driver = new FirefoxDriver();
                    break;
            }
        }

        public static void ExitApplication(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
