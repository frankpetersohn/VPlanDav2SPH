using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VPlanDav2SPH
{
    internal class Event
    {
        public string id { get; set; }
        public List<string> roomIds = new List<string>();
        public List<string> rooms = new List<string>();
        public List<string> teacherIds = new List<string>();
        public List<string> teachers = new List<string>();
        public List<string> classIds = new List<string>();
        public List<string> classes = new List<string>();
        public List<string> timeIds = new List<string>();
               
        public struct LessonTime
        {
            public string weekday;
            public string start;
            public string end;
            public string timeId;
            public LessonTime(string weekday, string start, string end, string timeId)
            {
                this.weekday = weekday;
                this.start = start;
                this.end = end;
                this.timeId = timeId;
            }
        } 
        public List<LessonTime> lessonTimes = new List<LessonTime>();
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string subjectId { get; set; }
        public string subject { get; set; }


        public void addRoomIds(IEnumerable<XElement> eList)
        {
            foreach (XElement e in eList)
            {
                if (!this.roomIds.Contains((string)e.Attribute("ID")))
                {
                    this.roomIds.Add((string)e.Attribute("ID"));
                }
            }
        }
        public void addTeacherIds(IEnumerable<XElement> eList)
        {
            foreach (XElement e in eList)
            {
                if (!this.teacherIds.Contains((string)e.Attribute("ID")))
                {
                    this.teacherIds.Add((string)e.Attribute("ID"));
                }
            }
        }
        public void addClassIds(IEnumerable<XElement> eList)
        {
            foreach (XElement e in eList)
            {
                if (!this.classIds.Contains((string)e.Attribute("ID")))
                {
                    this.classIds.Add((string)e.Attribute("ID"));
                }
            }
        }
        public void addTimeIds(IEnumerable<XElement> eList)
        {
            foreach (XElement e in eList)
            {
                if (!this.timeIds.Contains((string)e.Attribute("ID")))
                {
                    this.timeIds.Add((string)e.Attribute("ID"));
                }
            }
        }
        static public DateTime getDateTime(string datestring)
        {
            if (string.IsNullOrWhiteSpace(datestring)) return DateTime.MinValue;
            return DateTime.ParseExact(datestring, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        }


    }
}
