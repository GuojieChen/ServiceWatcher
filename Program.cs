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
                    try {
                        ServiceController sc = new ServiceController(service);

                        Console.WriteLine(DateTime.Now + " Checking status of: {0}", service);
                        Console.WriteLine(DateTime.Now + " Status of service {0} is: {1}", service, sc.Status);

                        switch (sc.Status) {

                            case ServiceControllerStatus.Running:
                            Console.WriteLine(DateTime.Now + " Service {0} is running properly", service);
                            break;
                            
                            case ServiceControllerStatus.Stopped:
                            Console.WriteLine(DateTime.Now + " Start service {0}", sc.ServiceName);
                            sc.Start();
                            sc.WaitForStatus(ServiceControllerStatus.Running);
                            break;

                            case ServiceControllerStatus.Paused:
                            Console.WriteLine(DateTime.Now + " Start service {0}", sc.ServiceName);
                            sc.Start();
                            sc.WaitForStatus(ServiceControllerStatus.Running);
                            break;

                            case ServiceControllerStatus.PausePending:
                            Console.WriteLine(DateTime.Now + " Service {0} is pausing. Wait.", sc.ServiceName);
                            sc.WaitForStatus(ServiceControllerStatus.Paused);
                            Console.WriteLine(DateTime.Now + " Start service {0}", sc.ServiceName);
                            sc.Start();
                            sc.WaitForStatus(ServiceControllerStatus.Running);
                            break;

                            case ServiceControllerStatus.StopPending:
                            Console.WriteLine(DateTime.Now + " Service {0} is stopping. Wait.", sc.ServiceName);
                            sc.WaitForStatus(ServiceControllerStatus.Stopped);
                            Console.WriteLine(DateTime.Now + " Start service {0}", sc.ServiceName);
                            sc.Start();
                            sc.WaitForStatus(ServiceControllerStatus.Running);
                            break;

                            case ServiceControllerStatus.StartPending:
                            Console.WriteLine(DateTime.Now + " Service {0} start pending. Wait.", sc.ServiceName);
                            sc.WaitForStatus(ServiceControllerStatus.Running);
                            break;
                            
                            default:
                            Console.WriteLine(DateTime.Now + " Service {0} is running properly", service);
                            break;                    
                        }
                        sc.Dispose();
                        sc = null;
                    } catch (Exception e) {
                        Console.WriteLine("Error: {0}\n{1}", e.StackTrace,e.Message);
                    }
                }
                Console.WriteLine(DateTime.Now + " All done. Waiting for {0} sec", Properties.Settings.Default.Delay / 1000);
                System.Threading.Thread.Sleep(Properties.Settings.Default.Delay);
            }
        }
    }
}