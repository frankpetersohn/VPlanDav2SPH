using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;

namespace VPlanDav2SPH
{
    internal class Changes
    {
        public string id { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string absenceTimeId { get; set; }
        public string timeId { get; set; }
        public string information { get; set; }
        public string message { get; set; }
        public string teacherId { get; set; }
        public string skipStandInReasonId { get; set; }
        public string typ { get; set; }

        public string newsTeacher { get; set; }
        public string newsSubject { get; set; }

        public IEnumerable<XElement> newsElement { get; set; }

        static public DateTime getDateTime(string datestring)
        {
            if (string.IsNullOrWhiteSpace(datestring)) return DateTime.MinValue;
            return DateTime.ParseExact(datestring, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        }
        
    }
}
