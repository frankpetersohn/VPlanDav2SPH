using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace VPlanDav2SPH
{
    internal class VplanUploader
    {
        private readonly IConfigurationRoot _settings;
        private readonly HttpClient _httpClient;

        public VplanUploader(IConfigurationRoot settings)
        {
            _settings = settings;
            _httpClient = BuildHttpClient();
        }

        /// <summary>
        /// Lädt die angegebene CSV-Datei ins Lanis-Schulportal hoch.
        /// </summary>
        /// <param name="csvFilePath">Vollständiger Pfad zur hochzuladenden CSV-Datei</param>
        public async Task UploadAsync(string csvFilePath)
        {
            if (!File.Exists(csvFilePath))
                throw new FileNotFoundException("CSV-Datei nicht gefunden.", csvFilePath);

         //   Console.WriteLine($"Starte Lanis-Upload für: {csvFilePath}");

            await ResetAsync();
            await UploadFileAsync(csvFilePath);
            await UnlockAsync();

    //        Console.WriteLine("Lanis-Upload abgeschlossen.");
        }

        // --- Schritte ------------------------------------------------------------

        private async Task ResetAsync()
        {
          //  Console.WriteLine("[1/3] Reset...");
            await PostFormAsync(
                ("c", _settings["Vertretungscode"]),
                ("i", _settings["Schulnummer"]),
                ("reset", "1"),
                ("a", _settings["Quelle"]),
                ("upload", "1")
            );
        }

        private async Task UploadFileAsync(string csvFilePath)
        {
           // Console.WriteLine("[2/3] Datei-Upload...");

            using (var content = BuildBaseForm(
                ("c", _settings["Vertretungscode"]),
                ("i", _settings["Schulnummer"]),
                ("a", _settings["Quelle"]),
                ("upload", "1")
            )) 
            { 

            var fileBytes = await Task.Run(() => File.ReadAllBytes(csvFilePath));
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
            content.Add(fileContent, "d", Path.GetFileName(csvFilePath));

                await SendAsync(content);
                    }
        }

        private async Task UnlockAsync()
        {
           // Console.WriteLine("[3/3] Unlock...");
            await PostFormAsync(
                ("c", _settings["Vertretungscode"]),
                ("i", _settings["Schulnummer"]),
                ("unlock", "1"),
                ("a", _settings["Quelle"]),
                ("upload", "1")
            );
        }

        // --- Hilfsmethoden -------------------------------------------------------

        private async Task PostFormAsync(params (string key, string value)[] fields)
        {
            using (var content = BuildBaseForm(fields)) { 
            await SendAsync(content);
            }
        }

        private static MultipartFormDataContent BuildBaseForm(
            params (string key, string value)[] fields)
        {
            var content = new MultipartFormDataContent();
            foreach (var (key, value) in fields)
                content.Add(new StringContent(value), key);
            return content;
        }

        private async Task SendAsync(MultipartFormDataContent content)
        {
            var response = await _httpClient.PostAsync(_settings["Url"], content);
          //  Console.WriteLine($"    → HTTP {(int)response.StatusCode} {response.StatusCode}");
            response.EnsureSuccessStatusCode();
        }

        private HttpClient BuildHttpClient()
        {
            var handler = new HttpClientHandler();

            if (_settings["IgnoreInsecureSSL"] == "true")
            {
          //      Console.WriteLine("WARNUNG: SSL-Zertifikatsprüfung ist deaktiviert!");
                handler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            }

            if (!string.IsNullOrWhiteSpace(_settings["ProxyUrl"]))
            {
                //handler.Proxy = new WebProxy(_settings.ProxyUrl, bypassOnLocal: false);
                handler.Proxy = new WebProxy(_settings["ProxyUrl"], false);
                handler.UseProxy = true;
          //      Console.WriteLine($"Proxy: {_settings["ProxyUrl"]}");
            }
            else
            {
                handler.UseProxy = false;
            }

            return new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(60) };
        }
    }
}
