using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcsight_Health_Checker {
    public class Process {
        public string name = "??";
        public string status = "??";
        public string upTime = "??";
        public string cpuUsage = "??";
        public string memoryUsage = "??";
        public List<string> infoList = new List<string>();
        public Process(string allInfo) {
            this.infoList = allInfo.Replace("\r", "").Split('\n').ToList();
            this.name = infoList[0];
            this.status = infoList[1];
            this.upTime = infoList[2];
            this.cpuUsage = infoList[3];
            this.memoryUsage = infoList[4];
        }
        public void eprint() {
            //int ln = infoList.Select(ss => ss.Length).Max() + 10;
            name = name.PadRight(15, ' ');
            status = status.PadRight(15, ' ');
            upTime = upTime.PadRight(15, ' ');
            cpuUsage = cpuUsage.PadRight(8, ' ');
            memoryUsage = memoryUsage.PadRight(15, ' ');
            Console.WriteLine("|\t" + name + "   \t" + status + "\t" + upTime + "\t" + cpuUsage + "\t" + memoryUsage);
        }
    }
}
