using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace lolKillItSelf
{
    class Program
    {
        
        static void Main(string[] args)
        {

            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            if (!isAdmin)
            {
                Console.WriteLine("Please start as admin 請使用系統管理者開啟");
                Console.WriteLine("按任意鍵結束....");
                //Console.ReadLine();// 使畫面停住
                Console.ReadKey();  //可按任意鍵結束畫面
                return;
            }


            Program pg = new Program();
            while (true)
            {
                pg.DoLoopJob();
                Thread.Sleep(500);
            }
             

        
            
            

            

        }
      
        public void DoLoopJob()
        {
            Program pg = new Program();
            var rawJson = pg.GetRawJson();

            if (rawJson != null)
            {
                bool isInGame = true;
                Evendata eventdata = new Evendata();

                eventdata = JsonConvert.DeserializeObject<Evendata>(rawJson);

                foreach (EventDetail detail in eventdata.Events)
                {
                    if (detail.EventName == "GameEnd")
                    {
                        pg.killLoLSelf();

                        isInGame = false;
                    }

                }
                if (isInGame)
                {
                    Console.WriteLine("遊戲中");
                }
            }
            else
            {

            }

        }
        public void killLoLSelf()
        {
            using (Process p = new Process())
            {
                p.StartInfo = new ProcessStartInfo()
                {
                    FileName = "taskkill",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = $"/F /IM \"{"League of Legends.exe"}\""
                };
                p.Start();
                p.WaitForExit(5000);
                Console.WriteLine("KILLED ONE!!");
            }
        }
        public string GetRawJson()
        {
            Task<string> result = GetResponseJson();
            string finalResult = result.Result;
            return finalResult;
        }
       public async Task<string> GetResponseJson()
        {
            string teststring = "";

            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => { return true; };
            using (var httpClientHandler = new HttpClientHandler())
            {
                //X509Certificate2 certificate = new X509Certificate2(@"riotgames.pem");
                //httpClientHandler.ClientCertificates.Clear();
                //httpClientHandler.ClientCertificates.Add(certificate);
                //httpClientHandler.SslProtocols = SslProtocols.None;
                //httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;

                using (var client = new HttpClient(httpClientHandler))
                {
                    try
                    {
                        HttpResponseMessage result = client.GetAsync("https://127.0.0.1:2999/liveclientdata/eventdata").Result;

                        if (result.IsSuccessStatusCode)
                        {
                            teststring = await result.Content.ReadAsStringAsync();
                            return teststring;
                        }
                        return null;
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine("未偵測到進行中的遊戲");
                    }

                    return null;

                }
            }

        }

        public bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        public class Evendata
        {
            public List<EventDetail> Events = new List<EventDetail>();

        }
        public class EventDetail
        {
            public string EventName { get; set; }
            //public string EventID { get; set; }
            //public string EventTime { get; set; }
            //public List< string> Assisters { get; set; }
            //public string KillerName { get; set; }
            //public string TurretKilled { get; set; }
        }
    }
}
