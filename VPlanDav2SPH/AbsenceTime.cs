using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPlanDav2SPH
{
    internal class AbsenceTime
    {
        public string id { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string teacherId { get; set; }
        public string absenceReasonID { get; set; }

        public string absenceReasonTyp { get; set; }
        public string schoolClassId { get; set; }
        public string eventId { get; set; }
        public string typ { get; set; }

        static public DateTime getDateTime(string datestring)
        {
            return DateTime.ParseExact(datestring, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        }
    }
}
