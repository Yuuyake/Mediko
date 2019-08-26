using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcsight_Health_Checker {
    public class Process {
        public string name;
        public string status;
        public string upTime;
        public string cpuUsage;
        public string memoryUsage;
        public List<string> infoList = new List<string>();
        public Process(string rawInfo) {
            this.infoList = rawInfo.Replace("\r", "").Split('\n').ToList();

            this.name     = infoList.ElementAtOrDefault(0) ?? "??";
            this.status   = infoList.ElementAtOrDefault(1) ?? "??";
            this.upTime   = infoList.ElementAtOrDefault(2) ?? "??";
            this.cpuUsage = infoList.ElementAtOrDefault(3) ?? "??"; 
            this.memoryUsage = infoList.ElementAtOrDefault(4) ?? "??";
        }
        public void eprint() {
            //int ln = infoList.Select(ss => ss.Length).Max() + 10;
            name    = name.PadRight(15, ' ');
            status  = status.PadRight(15, ' ');
            upTime  = upTime.PadRight(15, ' ');
            cpuUsage = cpuUsage.PadRight(8, ' ');
            memoryUsage = memoryUsage.PadRight(15, ' ');
            Console.WriteLine("|\t" + name + "   \t" + status + "\t" + upTime + "\t" + cpuUsage + "\t" + memoryUsage);
        }
    }
}
