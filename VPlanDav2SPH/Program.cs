using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace VPlanDav2SPH
{
    internal class Program
    {
        //List<Teacher> teachers = new List<Teacher>();

        static void Main(string[] args)
        {
            int daysToExport = 2;
            string pfad = "C:\\Users\\PetersohnFrank\\Nextcloud\\Davinci\\StPl_260310.davinci";
            //string pfad = "D:\\OneDrive - Berufliche Schulen Schwalmstadt\\Dokumente\\StPl-260310.daVinci";
            //string pfad = "C:\\Users\\PetersohnFrank\\OneDrive - Berufliche Schulen Schwalmstadt\\Schule\\Stundenplan\\StPl_260306.daVinci";
            string csvPath = "C:\\Users\\PetersohnFrank\\Nextcloud\\Davinci\\vplan.csv";
            //string csvPath = "C:\\Users\\Petersohn.VERWBSCAMPUS\\Downloads\\vplan.csv";


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
                            absenceReasonTyp = (string)item.Element("AbsenceReason")?.Attribute("Class"),
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
                var absenceReasons = doc
                       .Element("Storage")
                       ?.Element("Directories")
                       ?.Element("ClassAbsenceReasons")                     
                       ?.Elements("Items")
                       ?.Elements("Item")
                       .Select(item => new Reason
                       {
                           id = (string)item.Attribute("ID"),
                           typ = (string)item.Attribute("Class"),
                           name = (string)item.Element("Name"),
                           code = (string)item.Element("Code"),
                           key = (string)item.Element("Key"),
                       }).ToList();
                absenceReasons.AddRange( doc
                    .Element("Storage")
                    ?.Element("Directories")
                    ?.Element("TeacherAbsenceReasons")
                    ?.Elements("Items")
                    ?.Elements("Item")
                    .Select(item => new Reason
                    {
                        id = (string)item.Attribute("ID"),
                        typ = (string)item.Attribute("Class"),
                        name = (string)item.Element("Name"),
                        code = (string)item.Element("Code"),
                        key = (string)item.Element("Key"),
                    }).ToList());
                absenceReasons.AddRange(doc
                    .Element("Storage")
                    ?.Element("Directories")
                    ?.Element("ClassAbsenceReasons")
                    ?.Elements("Items")
                    ?.Elements("Item")
                    .Select(item => new Reason
                    {
                        id = (string)item.Attribute("ID"),
                        typ = (string)item.Attribute("Class"),
                        name = (string)item.Element("Name"),
                        code = (string)item.Element("Code"),
                        key = (string)item.Element("Key"),
                    }).ToList());
              

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
                             subjectId = (string)item.Element("Subject")?.Attribute("ID"),
                   
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
                         if (item.Elements("Times") != null)
                         {
                             IEnumerable<XElement> timeItems = item.Elements("Times")?.Elements("Items")?.Elements("Item");
                             foreach(XElement timeItem in timeItems)
                             {
                                 DateTime startTime = Event.getDateTime((string)timeItem.Element("Start"));
                                 DateTime endTime = Event.getDateTime((string)timeItem.Element("End"));
                                 string start = timeFrameRows.Find(a => a.start.TimeOfDay == startTime.TimeOfDay)?.name ?? null;
                                 string end = timeFrameRows.Find(a => a.end.TimeOfDay == endTime.TimeOfDay)?.name ?? null;
                                 string dayNo = (string)timeItem.Element("Weekday");
                                 string timeId = (string)timeItem.Attribute("ID");
                                 ev.lessonTimes.Add(new Event.LessonTime(dayNo, start, end, timeId));
                                 ev.addTeacherIds(timeItem.Elements("Teacher")?.Elements("Items")?.Elements("Item"));
                                 ev.addRoomIds(timeItem.Elements("Rooms")?.Elements("Items")?.Elements("Item"));

                             }
                         }
                         foreach (string id in ev.teacherIds) { 
                            ev.teachers.Add(teachers.Find(a => a.id == id).kuerzel);
                         }
                         foreach(string id in ev.roomIds)
                         {
                             ev.rooms.Add(rooms.Find(a => a.id == id).code);
                         }
                         foreach (string id in ev.classIds)
                         {
                             ev.classes.Add(classes.Find(a => a.id == id).code);
                         }
                         if(ev.subjectId != null) ev.subject = subjects.Find(a => a.id == ev.subjectId).name;
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

                    string start = timeFrameRows.Find(a => a.start.TimeOfDay == change.start.TimeOfDay)?.name ?? "";
                    string ende = timeFrameRows.Find(a => a.end.TimeOfDay == change.end.TimeOfDay)?.name ?? "";
                    if(start == ende)
                    {
                        entry.Stunde = start;
                    }
                    else
                    {
                        entry.Stunde = start + "-" + ende;
                    }



                    AbsenceTime absenceTime = absenceTimes.Find(a => a.id == change.absenceTimeId);
                    if (absenceTime != null)
                    {
                        if (absenceTime.teacherId != null)
                        {
                            entry.Lehrer = teachers.Find(a => a.id.Equals(absenceTime.teacherId))?.kuerzel ?? "";
                            entry.Hinweis = absenceReasons.Find(a => a.id.Equals(absenceTime.id))?.code ?? "";
                        }
                        if (absenceTime.typ == "ClassAbsenceTime" && absenceTime.schoolClassId != null)
                        {
                            entry.Klasse = classes.Find(a => a.id == absenceTime.schoolClassId)?.code ?? "";
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
                    entry.Hinweis = change.information;
                    entry.Hinweis2 = change.message;

                    if(change.timeId != null)
                    {
                        Event e = events.Find(a => a.lessonTimes.Exists(x => x.timeId == change.timeId));
                        if (e != null)
                        {
                            entry.Fach = e.subject;
                            entry.Raum = string.Join(", ", e.rooms);
                        }
                    }

                    vplan.Add(entry);
                }



                StandIn.generateCSV(vplan, csvPath);

                
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