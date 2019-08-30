/*                                              
│      Emre Ekinci
│      Arcsight Logger Health Checker                              
│        
        DEFAULT PROFILE OLUSTUR, PROFILES sız HER SEFERINDE OLUSTURUYOR OLABILIR

*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using Arcsight_Health_Checker.Properties;
using OpenQA.Selenium.Firefox;
using Console = Colorful.Console;

namespace Arcsight_Health_Checker {
    public static class Program {
        static string banner    = Resources.banner;
        static string geckoLoc  = Directory.GetCurrentDirectory() + @"\geckodriver.exe";
        static string firefoxLoc= Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "FOREFOX_LOC";
        static List<Logger> loggers   = new List<Logger>() {
            new Logger("LOGGER_URLS"),/**/
        };
        static void Main(string[] args) {
            string username = ""; // Console.ReadLine();
            SecureString password   = null;
            Console.OutputEncoding  = Encoding.UTF8;
            Console.Title           = "Mediko";

            // ===================================================================== Set up browser driver configs, binary paths
            Console.WriteLine("\n\n > Initializing browser . . .");
            if (File.Exists(firefoxLoc) == false) {
                Console.WriteFormatted("\n\t| Cannot found firefox.exe in path: " + firefoxLoc, Color.Red);
                Console.ReadLine();
                Environment.Exit(0);
            }
            if (File.Exists(geckoLoc) == false) {
                Console.WriteFormatted("\n\t| Cannot found geckodriver.exe in path: " + geckoLoc, Color.Red);
                Console.ReadLine();
                Environment.Exit(0);
            }
            Environment.SetEnvironmentVariable("webdriver.firefox.bin", firefoxLoc);
            FirefoxOptions options = new FirefoxOptions()
            {
                BrowserExecutableLocation = firefoxLoc,
                AcceptInsecureCertificates = true,
                LogLevel = FirefoxDriverLogLevel.Info,
            };
            options.SetPreference("webdriver.gecko.driver", Directory.GetCurrentDirectory() + @"\geckodriver.exe");
            //options.AddArgment("--headless");
            Driver.ffoxDriver = new FirefoxDriver(options);
            Driver.ffoxDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(8);

            //Logger.Blissadmin();
            while (true) {
                printMenu();
                Console.Clear();
                Console.WriteLineFormatted(Resources.banner, Color.LightGoldenrodYellow);
                Console.WriteFormatted("\n\t|Enter Username: ", Color.White); username = username == "" ? Console.ReadLine() : username;
                Console.WriteFormatted("\t|Enter Password: ",   Color.White); password = password == null ? Darker() : password;
                // ===================================================================== Run Driver service, go pages scrap infos
                Console.WriteLine("\n\n\t> Opening login pages . . .");
                loggers.ForEach(lg => lg.OpenLogInPage());
                Console.WriteLine("\t> Logging in . . .");
                loggers.ForEach(lg => lg.LogInToPage(username, password));
                Console.WriteLine("\t> Scraping Process Status datas . . .");
                loggers.ForEach(lg => lg.GetProcInfos());
                Console.WriteLine("\t> Scraping Event Archives datas . . .");
                loggers[0].GetArchiveInfos();
                Console.WriteFormatted("\n\n".PadRight(127, '▬'),Color.Gray);
                Console.WriteFormatted("\n".PadRight(57, '▬') + "   SUMMARY   " + "▬".PadRight(57, '▬') + "n\n", Color.Gray);

                loggers.ForEach(lg => lg.GetSummary());
                Console.WriteLineFormatted("\n\n\t\t\t\t─────────────────────   ALL DONE  ─────────────────────", Color.Red);
                Console.WriteFormatted("\n\n\t\t> Press \"r\" to redo and anything else to quit: ", Color.Yellow);
                if (Console.ReadLine() != "r") 
                    break;
            }
            Driver.ffoxDriver.Dispose();
            password.Dispose();
        }
        static void printMenu() {
            List<string> menuItems = new List<string>();
            List<string> descItems = new List<string>();
            string descTemplate = "\t use arrows(↑↓) and d=delete | a=add | enter=finish        ";
            for (int i = 0; i < loggers.Count; i++) {
                menuItems.Add(loggers[i].mainPage.PadRight(25, ' '));
                descItems.Add("                                             ");
            }
            //new SoundPlayer(URLbliss.Properties.Resources.gurg).Play();
            Console.ForegroundColor = Color.FromArgb(0, 255, 0);
            Console.Clear();
            int currChoice = 0;
            Console.WriteLineFormatted(banner, Color.LightGoldenrodYellow);
            Console.Write("\n ╔═════════════════════════════════════════════════════════════════════════════════════════════════\n ║");
            while (true) {
                if(menuItems.Count == 0) {
                    Console.WriteLineFormatted("\t No logger No cry . . .", Color.Red);
                    Console.ReadLine();
                    Environment.Exit(0);
                }
                //menuItems = menuItems.Count == 0 ? menuItems.Add(" No logger No cry . . .");
                currChoice = currChoice < 0 ? currChoice + menuItems.Count : currChoice % menuItems.Count;
                //setting cursor position to 0 gives slightly higher performance than Console.Clear()
                Console.SetCursorPosition(0, banner.Count(f => f == '\n') + 2); // normally 18 is the height of URLBLISS banner
                descItems[currChoice] = descTemplate;
                for (int i = 0; i < menuItems.Count; i++) { // 6 element is the length of selective menu
                    if (currChoice -1 == i)
                        Console.Write("\n ╟  " + menuItems[i]);
                    else if (currChoice == i)
                        Console.WriteFormatted("\n ╟►  " + menuItems[i], Color.White);
                    else
                        Console.WriteFormatted("\n ╟ " + menuItems[i], Color.FromArgb(0, 255, 0));
                    Console.WriteFormatted(descItems[i], Color.FromArgb(255, 0, 0));
                }
                descItems[currChoice] = "                                             ";

                ConsoleKeyInfo pressed = Console.ReadKey(true);
                if (pressed.Key == ConsoleKey.DownArrow) {
                    currChoice++;
                    //new SoundPlayer(URLbliss.Properties.Resources.slideup).Play();
                }
                else if (pressed.Key == ConsoleKey.UpArrow) {
                    currChoice--;
                    //new SoundPlayer(URLbliss.Properties.Resources.slidedown).Play();
                }
                else if (pressed.Key == ConsoleKey.D) {
                    menuItems.RemoveAt(currChoice);
                    loggers.RemoveAt(currChoice);
                    Console.Clear();
                    Console.WriteLineFormatted(banner, Color.LightGoldenrodYellow);
                    Console.Write("\n ╔═════════════════════════════════════════════════════════════════════════════════════════════════\n ║");
                    //new SoundPlayer(URLbliss.Properties.Resources.slidedown).Play();
                }
                else if (pressed.Key == ConsoleKey.A) {
                    Console.SetCursorPosition(0, banner.Count(f => f == '\n') + 2 + menuItems.Count + 2); // normally 18 is the height of URLBLISS banner
                    Console.Write("\t► Enter logger URL: ");
                    var tlogger = new Logger(Console.ReadLine());
                    menuItems.Add(tlogger.mainPage.PadRight(25, ' '));
                    loggers.Add(tlogger);
                    descItems.Add("                                             ");
                    Console.Clear();
                    Console.WriteLineFormatted(banner, Color.LightGoldenrodYellow);
                    Console.Write("\n ╔═════════════════════════════════════════════════════════════════════════════════════════════════\n ║");
                    //new SoundPlayer(URLbliss.Properties.Resources.slidedown).Play();
                }
                else if (pressed.Key == ConsoleKey.UpArrow) {
                    currChoice--;
                    //new SoundPlayer(URLbliss.Properties.Resources.slidedown).Play();
                }
                else if (pressed.Key == ConsoleKey.Enter) {
                    //new SoundPlayer(URLbliss.Properties.Resources.select).Play();
                    break; // if Enters exit from Choice screen and call proper function
                }
            } // while 
            switch ((currChoice + 1).ToString()) {
                case "1":
                    break;
                default:
                    break;
            } // switch 
        }
        private static string LnkToFile(string fileLink) {
            string link = File.ReadAllText(fileLink);
            int i1 = link.IndexOf("DATA\0");
            if (i1 < 0)
                return null;
            i1 += 5;
            int i2 = link.IndexOf("\0", i1);
            if (i2 < 0)
                return link.Substring(i1);
            else
                return link.Substring(i1, i2 - i1);
        }
        static public SecureString Darker() {
            SecureString securePwd = new SecureString();
            ConsoleKeyInfo key;
            do {
                key = Console.ReadKey(true);
                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter) {
                    securePwd.AppendChar(key.KeyChar);
                    Console.Write("*");
                }
                else {
                    if (key.Key == ConsoleKey.Backspace && securePwd.Length > 0) {
                        securePwd = new NetworkCredential("", securePwd.ToString().Substring(0, (securePwd.Length - 1))).SecurePassword;
                        Console.Write("\b \b");
                    }
                    else if (key.Key == ConsoleKey.Enter) {
                        break;
                    }
                }
            } while (true);
            return securePwd;
        }
    }

}
