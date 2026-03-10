using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VPlanDav2SPH
{
    internal class StandIn
    {
        //Tag;Lehrer;Stunde;Klasse;Art;Vertreter;Fach;Raum;Hinweis;Raum_alt;Fach_alt;Klasse_alt;Hinweis2;Lerngruppe
        public string Tag { get; set; }
        public string Lehrer { get; set; }
        public string Stunde { get; set; }
        public string Klasse { get; set; }
        public string Art { get; set; }
        public string Vertreter { get; set; }
        public string Fach { get; set; }
        public string Raum { get; set; }
        public string Hinweis { get; set; }
        public string Raum_alt { get; set; }
        public string Fach_alt { get; set; }
        public string Klasse_alt { get; set; }
        public string Hinweis2 { get; set; }
        public string Lerngruppe { get; set; }
        static public void generateCSV(List<StandIn> vplan, string path)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Tag; Lehrer; Stunde; Klasse; Art; Vertreter; Fach; Raum; Hinweis; Raum_alt; Fach_alt; Klasse_alt; Hinweis2; Lerngruppe");
            foreach (StandIn v in vplan)
            {
                sb.AppendLine($"{v.Tag};{v.Lehrer ?? ""};{v.Stunde?.ToString() ?? ""};{v.Klasse ?? ""};{v.Art ?? ""};{v.Vertreter ?? ""};{v.Fach ?? ""};{v.Raum ?? ""};{v.Hinweis ?? ""};{v.Raum_alt ?? ""};{v.Fach_alt ?? ""};{v.Klasse_alt ?? ""};{v.Hinweis2 ?? ""};{v.Lerngruppe ?? ""}");
            }
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }
    }
    
}
