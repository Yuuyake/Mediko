using System;
using System.Collections.Generic;
using System.Drawing;
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
                name    = rowList[0];
                day     = rowList[1];
                month   = rowList[2];
                year    = rowList[3];
                date    = this.day + "/" + this.month + "/" + this.year;
                storageGroup = rowList[4];
                status  = rowList[5];
                indexStatus  = rowList[6];
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
                date    = this.day + "/" + this.month + "/" + this.year;
                storageGroup = row.FindElement((By.CssSelector("[id*=storagegroup-]"))).Text;
                indexStatus  = row.FindElement((By.CssSelector("[id*=indexstatus-]"))).Text;
            }
            catch (Exception ee) {
                Console.WriteLineFormatted("\t > Exception: row " + row.Text.Replace("\t", " ") + " is problematic . . .", Color.Red);
                var error = ee.Message;
                Console.WriteLineFormatted("\t\t> " + error, Color.Orange);
            }
        }
    }
}