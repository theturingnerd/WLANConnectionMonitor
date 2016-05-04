using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace WLANConnectionMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>                                
            {
                x.Service<WLANMonitor>(s =>                       
                {
                    s.ConstructUsing(name => new WLANMonitor());     
                    s.WhenStarted(tc => tc.Start());             
                    s.WhenStopped(tc => tc.Stop());               
                });
                x.RunAsLocalSystem();                           

                x.SetDescription("Does the monitoring");        
                x.SetDisplayName("WLAN Monitor");                       
                x.SetServiceName("IH.WLANMonitor");                       
            });
        }
    }

   
}
