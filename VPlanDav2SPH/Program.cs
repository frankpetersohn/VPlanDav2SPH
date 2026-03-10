using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VPlanDav2SPH
{
    internal class Program
    {
        //List<Teacher> teachers = new List<Teacher>();

        static void Main(string[] args)
        {
            int daysToExport = 2;
            string pfad = "C:\\Users\\PetersohnFrank\\Nextcloud\\Davinci\\StPl_20260309.davinci";

            //string pfad = "C:\\Users\\PetersohnFrank\\OneDrive - Berufliche Schulen Schwalmstadt\\Schule\\Stundenplan\\StPl_260306.daVinci";
            try
            {
                XDocument doc = XDocument.Load(pfad);
                var teachers = doc
                             .Element("Storage")
                             ?.Element("Teacher")
                             ?.Element("Items")
                             ?.Elements("Item")
                             .Select(item => new Teacher
                             {
                                 id = (string)item.Attribute("ID"),
                                 kuerzel = (string)item.Element("Code")
                             }).ToList();
                var rooms = doc
                           .Element("Storage")
                           ?.Element("Rooms")
                           ?.Element("Items")
                           ?.Elements("Item")
                           .Select(item => new Room
                           {
                               id = (string)item.Attribute("ID"),
                               code = (string)item.Element("Code"),
                               name = (string)item.Element("Name"),

                           }).ToList();
                var subjects = doc
                          .Element("Storage")
                          ?.Element("Subjects")
                          ?.Element("Items")
                          ?.Elements("Item")
                          .Select(item => new Room
                          {
                              id = (string)item.Attribute("ID"),
                              code = (string)item.Element("Code"),
                              name = (string)item.Element("Name"),

                          }).ToList();
                var classes = doc
                         .Element("Storage")
                         ?.Element("Classes")
                         ?.Element("Items")
                         ?.Elements("Item")
                         .Select(item => new SchoolClass
                         {
                             id = (string)item.Attribute("ID"),
                             code = (string)item.Element("Code")
                         }).ToList();
                var absenceTimes = doc
                        .Element("Storage")
                        ?.Element("AbsenceTimes")
                        ?.Element("Items")
                        ?.Elements("Item")
                        .Select(item => new AbsenceTime
                        {
                            id = (string)item.Attribute("ID"),
                            start = AbsenceTime.getDateTime((string)item.Element("Start")),
                            end = AbsenceTime.getDateTime((string)item.Element("End")),
                            absenceReasonID = (string)item.Element("AbsenceReason")?.Attribute("ID"),
                            teacherId = (string)item.Element("Teacher")?.Attribute("ID"),
                            schoolClassId = (string)item.Element("Class")?.Attribute("ID"),
                            eventId = (string)item.Element("Event")?.Attribute("ID"),
                            typ = (string)item.Attribute("Class")
                        }).ToList();
                var timeFrameRows = doc
                        .Element("Storage")
                        ?.Element("Settings")
                        ?.Element("TimeFrame")
                        ?.Elements("Rows")
                        ?.Elements("Items")
                        ?.Elements("Item")
                        .Select(item => new TimeFrameRow
                        {
                            id = (string)item.Attribute("ID"),
                            start = AbsenceTime.getDateTime((string)item.Element("Start")),
                            end = AbsenceTime.getDateTime((string)item.Element("End")),
                            name = (string)item.Element("Name"),
                        }).ToList();

                var events = doc
                     .Element("Storage")
                     ?.Element("Events")
                     ?.Elements("Items")
                     ?.Elements("Item")
                     .Select(item =>
                     {
                         var ev = new Event
                         {
                             id = (string)item.Attribute("ID"),
                             subjectId = (string)item.Element("Subject")?.Attribute("ID")
                         };
                         if (item.Elements("Rooms") != null)
                         {
                             ev.addRoomIds(item.Elements("Rooms")?.Elements("Items")?.Elements("Item"));
                         }
                         if (item.Elements("Classes") != null)
                         {
                             ev.addClassIds(item.Elements("Classes")?.Elements("Items")?.Elements("Item"));
                         }
                         if (item.Elements("Teacher") != null)
                         {
                             ev.addTeacherIds(item.Elements("Teacher")?.Elements("Items")?.Elements("Item"));
                         }

                         return ev;
                     })
                     .ToList();

                var futureAbsences = absenceTimes.Where(a => a.start >= DateTime.Today).ToList();
                var changes = doc
                        .Element("Storage")
                        ?.Element("Changes")
                        ?.Elements("Items")
                        ?.Elements("Item")
                        .Select(item =>
                        {
                            var ch = new Changes
                            {
                                id = (string)item.Attribute("ID"),
                                typ = (string)item.Attribute("Class"),
                                start = Changes.getDateTime((string)item?.Element("Start")),
                                end = Changes.getDateTime((string)item?.Element("End")),
                                absenceTimeId = (string)item?.Element("AbsenceTime")?.Attribute("ID"),
                                timeId = (string)item?.Element("Time")?.Attribute("ID"),
                                information = (string)item?.Element("Information"),
                                message = (string)item?.Element("Message"),
                                skipStandInReasonId = (string)item?.Element("SkipStandInReason")?.Attribute("ID"),
                                newsElement = (item?.Element("New")?.Element("Items")?.Elements("Item")),
                                newsTeacher = (string)item?.Element("New")
                              ?.Element("Items")
                              ?.Elements("Item")
                              .FirstOrDefault(n => (string)n.Attribute("Class") == "Teacher")
                              ?.Attribute("ID"),
                                newsSubject = (string)item?.Element("New")
                              ?.Element("Items")
                              ?.Elements("Item")
                              .FirstOrDefault(n => (string)n.Attribute("Class") == "Subject")
                              ?.Attribute("ID"),
                            };
                            

                            return ch;
                        }).ToList();

                //var relevatChanges = changes.Where(a => ((a.start >= DateTime.Today) || (a.start <= DateTime.Today && a.end >= DateTime.Today))).ToList();
                var today = DateTime.Today;
                var in7Days = today.AddDays(daysToExport);

                var relevantChanges = changes.Where(a => a.start <= in7Days && a.end >= today).ToList();





                List<StandIn> vplan = new List<StandIn>();
                foreach (var change in relevantChanges)
                {
                    StandIn entry = new StandIn();
                    AbsenceTime absenceTime = absenceTimes.Find(a => a.id == change.absenceTimeId);
                    if (absenceTime != null)
                    {
                        if (absenceTime.teacherId != null)
                        {
                            entry.Lehrer = teachers.Find(a => a.id.Equals(absenceTime.teacherId)).kuerzel;
                        }
                        if (absenceTime.typ == "ClassAbsenceTime" && absenceTime.schoolClassId != null)
                        {
                            entry.Klasse = classes.Find(a => a.id == absenceTime.schoolClassId).code;
                            entry.Hinweis = "Klasse fehlt";
                        }

                    }


                    if(change.newsTeacher != null)
                    {
                        entry.Vertreter = teachers.Find(a => a.id == change.newsTeacher).kuerzel;
                    }
                    if(change.newsSubject != null)
                    {
                        entry.Fach = subjects.Find(a => a.id == change.newsSubject).code;
                    }


                    TimeFrameRow timeFrameRow = timeFrameRows.Find(a => (a.start.TimeOfDay == change.start.TimeOfDay));
                    if (timeFrameRow != null) entry.Stunde = timeFrameRow.name;


                    entry.Tag = change.start.Day + "." + change.start.Month + "." + change.start.Year;


                    vplan.Add(entry);
                }



                StandIn.generateCSV(vplan, "C:\\Users\\PetersohnFrank\\Nextcloud\\Davinci\\vplan.csv");

                
                Console.ReadLine();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }




            Console.ReadLine();
        }
    }

}