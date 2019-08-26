using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenQA.Selenium;
using Console = Colorful.Console;

namespace Arcsight_Health_Checker {
    class Archive {
        public string name     = "??";
        public string day      = "??";
        public string month    = "??";
        public string year     = "??";
        public string status   = "??";
        public string date     = "??";
        public string indexStatus  = "??";
        public string storageGroup = "??";

        public Archive(string rowInfo) {
            var rowList = rowInfo.Split(' ');
            try {
                name    = rowList.ElementAtOrDefault(0) ?? "??"; 
                day     = rowList.ElementAtOrDefault(1) ?? "??";
                month   = rowList.ElementAtOrDefault(2) ?? "??";
                year    = rowList.ElementAtOrDefault(3) ?? "??";
                status  = rowList.ElementAtOrDefault(5) ?? "??";
                date = this.day + "." + this.month + "." + this.year;
                storageGroup = rowList.ElementAtOrDefault(4) ?? "??";
                indexStatus  = rowList.ElementAtOrDefault(6) ?? "??";
            }
            catch (Exception ee) {
                Console.WriteLineFormatted("\t > Exception: row " + rowInfo.Replace("\t", " ") + " is problematic . . .", Color.Red);
                var error = ee.Message;
                Console.WriteLineFormatted("\t\t> " + error, Color.Orange);
            }
        }

        public Archive(IWebElement row) {
            try {
                name    = row.FindElement((By.CssSelector("[id*=name-]"))).Text;
                day     = row.FindElement((By.CssSelector("[id*=day-]"))).Text;
                month   = row.FindElement((By.CssSelector("[id*=month-]"))).Text;
                year    = row.FindElement((By.CssSelector("[id*=year-]"))).Text;
                status  = row.FindElement((By.CssSelector("[id*=status-]"))).Text;
                date    = this.day + "." + this.month + "." + this.year;
                storageGroup = row.FindElement((By.CssSelector("[id*=storagegroup-]"))).Text;
                indexStatus  = row.FindElement((By.CssSelector("[id*=indexstatus-]"))).Text;
            }
            catch (Exception ee) {
                Console.WriteLineFormatted("\t > Exception: row " + row.Text.Replace("\t", " ") + " is problematic . . .", Color.Red);
                var error = ee.Message;
                Console.WriteLineFormatted("\t\t> " + error, Color.Orange);
            }
        }

        public void eprint() {
            if( status != "Archived" || indexStatus != "Indexed") {
                Console.WriteLineFormatted("\t" +
                    this.name.PadRight(60) +
                    (this.day + "." + this.month + "." + this.year).PadRight(13) +
                    this.storageGroup.PadRight(30) +
                    this.status.PadRight(12) +
                    this.indexStatus.PadRight(12),Color.Red);
            }
            Console.WriteLine("\t" +
                                this.name.PadRight(60) +
                                (this.day + "." + this.month + "." + this.year).PadRight(13) +
                                this.storageGroup.PadRight(30) +
                                this.status.PadRight(12) +
                                this.indexStatus.PadRight(12));
        }
    }
}