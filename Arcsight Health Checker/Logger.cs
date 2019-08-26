using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Threading;
using OpenQA.Selenium;
using Console = Colorful.Console;

namespace Arcsight_Health_Checker {
    public class Logger {
        public int runProblems  = -1;// determined at SetStatus method
        public int cpuProblems  = -1;// determined at SetStatus method
        public int memProblems  = -1;// determined at SetStatus method
        public string name      = "unknown";// determined at SetStatus method
        public string statue    = "unknown";// determined at SetStatus method
        public string tabID     = "unknown";// determined at loging page
        public string mainPage  = "unknown";// determined at first decleration
        public string adminPage = "unknown";// determined at first decleration
        public string archivePage = "unknown";// determined at first decleration
        public List<Process> procList = new List<Process>();

        static List<Archive> yestArchives  = new List<Archive>();
        static List<Archive> olderArchives = new List<Archive>();
        public Logger(string mainPage) {
            this.mainPage    = mainPage;
            this.adminPage   = mainPage + "/logger/sys_admin.ftl?action=ProcStat";
            this.archivePage = mainPage + "/logger/config_home.ftl?config-page=event_archive_config";
        }

        public void SetStatus(List<string> statuses) {
            try {
                this.name = statuses[0].Substring(0, statuses[0].IndexOf(".int")).ToUpper();
                statuses.RemoveAt(0);
                statuses.ForEach(st => this.procList.Add(new Process(st)));
                runProblems = procList.Count(pr => pr.status != "running");
                cpuProblems = procList.Count(pr => double.Parse(pr.cpuUsage.Replace("%", "").Replace(" ", ""), CultureInfo.InvariantCulture) > 90.0);
                memProblems = procList.Count(pr => double.Parse(pr.memoryUsage.Split(' ')[0].Replace("%", ""), CultureInfo.InvariantCulture) > 90.0);
                if (runProblems > 0 || cpuProblems > 0 || memProblems > 0 || procList.Count < 11)
                    statue = "BAD";
                else
                    statue = "OK ";
            }
            catch (Exception ee) {
                Console.WriteLineFormatted("\t> Exception: Page " + this.mainPage + " is problematic . . .", Color.Red);
                var error = ee.Message;
                Console.WriteLineFormatted("\t\t> " + error, Color.Orange);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void OpenLogInPage() {
            try {
                Driver.ffoxDriver.ExecuteJS("window.open('" + this.mainPage + "')");
                var tabs = Driver.ffoxDriver.WindowHandles;
                Driver.ffoxDriver.SwitchTo().Window(tabs[++(Driver.currTab)]);
                Driver.ffoxDriver.ExecuteJS("window.open('" + this.mainPage + "')");
                tabID = Driver.ffoxDriver.CurrentWindowHandle;
                Console.WriteLineFormatted("\t > DONE: " + this.mainPage, Color.Green);
            }
            catch (Exception ee) {
                Console.WriteLineFormatted("\t > Exception: Page " + this.mainPage + " is problematic . . .", Color.Red);
                var error = ee.Message;
                Console.WriteLineFormatted("\t\t> " + error, Color.Orange);
            }
            Thread.Sleep(500);
        }
        /// <summary>
        /// executes to login operation for the login page
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="tabs"></param>
        public void LogInToPage(string username, SecureString password /*ReadOnlyCollection<string> tabs*/) {
            try {
                var ntCreds = new System.Net.NetworkCredential(string.Empty, password);
                Driver.ffoxDriver.SwitchTo().Window(this.tabID);
                Driver.ffoxDriver.FindElement(By.XPath("//*[@id=\"userName-input\"]")).SendKeys(username);
                Driver.ffoxDriver.FindElement(By.XPath("//*[@id=\"password-input\"]")).SendKeys(ntCreds.Password);
                Driver.ffoxDriver.FindElement(By.XPath("//*[@type=\"button\"]")).Click();
                Console.WriteLineFormatted("\t > DONE: " + this.mainPage, Color.Green);
            }
            catch (Exception ee) {
                Console.WriteLineFormatted("\t > Exception: Page " + this.mainPage + " is problematic . . .", Color.Red);
                var error = ee.Message;
                Console.WriteLineFormatted("\t\t> " + error, Color.Orange);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tabs"></param>
        public void GetArchiveInfos() {
            Console.WriteLineFormatted("\t > Going archive page \"" + this.archivePage + "\" . . .",Color.Green);
            try {
                Driver.ffoxDriver.SwitchTo().Window(this.tabID);
                Driver.ffoxDriver.Navigate().GoToUrl(this.archivePage);
                Console.WriteLineFormatted("\t > Waiting to load archives . . .", Color.Green);
                while (true) {
                    if (Driver.ffoxDriver.PageSource.Contains("items-table-container") == false) {
                        Thread.Sleep(1000);
                        // put time limit like 20 seconds at most
                    }
                    else
                        break;
                }
                var statusRow = Driver.ffoxDriver.FindElements(
                    By.XPath("//*[contains(@id, 'row-0504403158265') and not(contains(@style, 'display: none;'))]")).ToList();
                var yestDate = DateTime.Now.AddDays(-1).ToString("dd/M/yyyy");
                foreach (IWebElement rr in statusRow) {
                    var tempArc = new Archive(rr);
                    if (yestDate == tempArc.date)
                        yestArchives.Add(new Archive(rr));
                    else
                        olderArchives.Add(new Archive(rr));
                }
                //print them
                Console.WriteLineFormatted("\n\tYesterday Archives: \n",Color.Cyan);
                string arcHeader = 
                    "Archive Name".PadRight(60) +
                    "Date".PadRight(12) +
                    "Storage Group".PadRight(30) +
                    "Status".PadRight(12) +
                    "Index Status".PadRight(12);
                Console.WriteLineFormatted("\t" + arcHeader, Color.LightGoldenrodYellow);
                yestArchives.ForEach(aa => aa.eprint());
                Console.WriteLineFormatted("\n\tOlder Archives: \n", Color.Cyan);
                Console.WriteLineFormatted("\t" + arcHeader,Color.LightGoldenrodYellow);
                olderArchives.ForEach(aa => aa.eprint());
            }
            catch (Exception ee) {
                Console.WriteLineFormatted("\t > Exception: Page " + this.archivePage + " is problematic . . .", Color.Red);
                var error = ee.Message;
                Console.WriteLineFormatted("\t\t> " + error, Color.Orange);
            }
            Console.WriteLineFormatted("\n\t > DONE: " + this.archivePage, Color.Green);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tabs"></param>
        public void GetProcInfos() {
            Console.WriteLineFormatted("\t > Going admin page \"" + this.adminPage + "\" . . .",Color.Green);
            try {
                Driver.ffoxDriver.SwitchTo().Window(this.tabID);
                Driver.ffoxDriver.Navigate().GoToUrl(this.adminPage);
            }
            catch (Exception ee) {
                Console.WriteLineFormatted("\t > Exception: Page " + this.adminPage + " is problematic . . .", Color.Red);
                var error = ee.Message;
                Console.WriteLineFormatted("\t\t> " + error, Color.Orange);
                return;
            }
            while (true) {
                try {
                    Driver.ffoxDriver.SwitchTo().Frame("sysadmin-content");
                    Driver.ffoxDriver.FindElements(By.CssSelector("[id*=sysadminmenu__sysadminmenu_x-auto-]")).First(webE => webE.Text == "Process Status").Click();
                    //Console.WriteLine(proccessButton.Text);
                    var statuses = Driver.ffoxDriver.FindElements(By.XPath("//*[@class=\"x-grid3-row-table\"]")).Select(ss => ss.Text).ToList();
                    this.SetStatus(statuses);
                    this.printProcStat();
                }
                catch (Exception ee) {
                    Console.WriteLineFormatted("\t > Exception: Page " + this.adminPage + " is problematic . . .", Color.Red);
                    var error = ee.Message;
                    if (error.Contains("is stale"))
                        continue;
                    Console.WriteLineFormatted("\t\t> " + error, Color.Orange);
                }
                break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void GetSummary() {
            Console.WriteLineFormatted("\t> " + statue + " : " + this.name, statue == "OK " ? Color.Green : Color.Red);
            if (runProblems > 0)
                Console.WriteLineFormatted("\t\t# of Not Running Services: " + runProblems, Color.Red);
            if (cpuProblems > 0)
                Console.WriteLineFormatted("\t\t# of Cpu Problems        : " + cpuProblems, Color.Red);
            if (memProblems > 0)
                Console.WriteLineFormatted("\t\t# of Memory Problems     : " + memProblems, Color.Red);
            if (procList.Count < 11)
                Console.WriteLineFormatted("\t\t# of Avaliable Services  : " + procList.Count + "\\11", Color.Red);
        }
        /// <summary>
        /// Extension method to execute a script over current driver
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        public void printProcStat() {
            Console.WriteLineFormatted("===========================================================================================================", Color.LightGray);
            Console.WriteLineFormatted("\n\t  " + this.name + "\n", Color.Cyan);
            Console.WriteLineFormatted(
                "\t  " + "Process".PadRight(15, ' ') +
                "\t" + "Status".PadRight(15, ' ') +
                "\t" + "UpTime".PadRight(15, ' ') +
                "\t" + "CpuUsage".PadRight(8, ' ') +
                "\t" + "MemoryUsage".PadRight(15, ' '),
                Color.LightGoldenrodYellow);
            procList.ForEach(pp => pp.eprint());
            Console.WriteLineFormatted("\n\t> DONE: " + this.mainPage, Color.Green);
        }
    }
}
