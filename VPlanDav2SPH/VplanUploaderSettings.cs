using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPlanDav2SPH
{
    internal class VplanUploaderSettings
    {
        /// <summary>Schulnummer im Lanis-Portal (Parameter "i")</summary>
        public string Schulnummer { get; set; } = string.Empty;

        /// <summary>Vertretungs-Upload-Code (Parameter "c")</summary>
        public string Vertretungscode { get; set; } = string.Empty;

        /// <summary>Upload-Quelle, z.B. "csv-kuerzel" (Parameter "a")</summary>
        public string Quelle { get; set; } = "csv-kuerzel";

        /// <summary>Ziel-URL des Schulportals</summary>
        public string Url { get; set; } = "https://start.schulportal.hessen.de/vertretungsplan.php";

        /// <summary>Optionaler HTTP-Proxy, z.B. "http://10.9.131.101:80"</summary>
        public string ProxyUrl { get; set; }

        /// <summary>SSL-Zertifikatsprüfung deaktivieren (nur für Test/Entwicklung!)</summary>
        public bool IgnoreInsecureSSL { get; set; } = false;

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Schulnummer))
                throw new InvalidOperationException("LanisUploader: Schulnummer darf nicht leer sein.");
            if (string.IsNullOrWhiteSpace(Vertretungscode))
                throw new InvalidOperationException("LanisUploader: Vertretungscode darf nicht leer sein.");
        }
    }
}
