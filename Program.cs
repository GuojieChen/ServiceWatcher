using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;


namespace ServiceWatcher {
    class Program {
        static void Main(string[] args) {

            List<String> services = new List<string>();

            services.AddRange(Properties.Settings.Default.Services.Split(';'));

            while (true) {

                foreach (string service in services) {
                    Console.WriteLine(DateTime.Now +" Checking status of: {0}",service);
                    ServiceController sc = new ServiceController(service);
                    Console.WriteLine(DateTime.Now + " Status of service {0} is: {1}", service, sc.Status);
                    if (sc.Status != ServiceControllerStatus.Running) {
                        Console.WriteLine(DateTime.Now + " Start service {0}", sc.DisplayName);
                        sc.Start();
                        sc.WaitForStatus(ServiceControllerStatus.Running);
                        if (sc.Status != ServiceControllerStatus.Running && sc.Status != ServiceControllerStatus.StartPending) {
                            Console.WriteLine(DateTime.Now + " Service {0} not started properly - retry", sc.DisplayName);
                            sc.Start();
                        }

                    }
                    sc.Dispose();
                    sc = null;
                }
                Console.WriteLine(DateTime.Now + " All done. Waiting for {0} sec", Properties.Settings.Default.Delay / 1000);
                System.Threading.Thread.Sleep(ServiceWatcher.Properties.Settings.Default.Delay); 
        }
    }
}
