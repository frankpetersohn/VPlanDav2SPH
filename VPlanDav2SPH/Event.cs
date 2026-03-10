using System;
using System.Collections.Generic;
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
        public List<string> teacherIds = new List<string>();
        public List<string> classIds = new List<string>();
        public List<string> timeIds = new List<string>();

        public string subjectId { get; set; }


        public void addRoomIds(IEnumerable<XElement> eList)
        {
            foreach (XElement e in eList)
            {
                this.roomIds.Add((string)e.Attribute("ID")); 
            }
        }
        public void addTeacherIds(IEnumerable<XElement> eList)
        {
            foreach (XElement e in eList)
            {
                this.teacherIds.Add((string)e.Attribute("ID"));
            }
        }
        public void addClassIds(IEnumerable<XElement> eList)
        {
            foreach (XElement e in eList)
            {
                this.classIds.Add((string)e.Attribute("ID"));
            }
        }
        public void addTimeIds(IEnumerable<XElement> eList)
        {
            foreach (XElement e in eList)
            {
                this.timeIds.Add((string)e.Attribute("ID"));
            }
        }


    }
}
