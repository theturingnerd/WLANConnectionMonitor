using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WLANConnectionMonitor
{


    public class WLANMonitor
    {
        readonly Timer _timer;
        NativeWifi.WlanClient cli;
        List<string> ssidList;

        public WLANMonitor()
        {
            _timer = new Timer(10000) { AutoReset = true };
            
            _timer.Elapsed += _timer_Elapsed;
            cli = new NativeWifi.WlanClient();
            ssidList = new List<string>();


            try
            {
                //boostrap settings
               string wlanList= System.Configuration.ConfigurationManager.AppSettings["NetworkList"];

               foreach(var network in wlanList.Split(','))
                {
                    ssidList.Add(network);
                }
            }
            catch(Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry(ex.ToString());
            }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("It is {0} and all is well", DateTime.Now);



            foreach (var thing in cli.Interfaces)
            {
                //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(thing));



                if (thing.InterfaceState != NativeWifi.Wlan.WlanInterfaceState.Disconnected && (  ssidList.Any(p=>p==thing.CurrentConnection.profileName)))
                {
                    Console.WriteLine("Already connected to preferred network!");

                    foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        if(ni.Name == "Mobile Broadband Connection")
                        {
                            if(ni.OperationalStatus == OperationalStatus.Up)
                            {
                                Console.WriteLine("--> !! Disable broadband adapter now... ");
                                MobileAPIHelper.Disconnect();
                                //SysInterfaceHelper.Disable("Mobile Broadband Connection");
                            }
                        }
                    }


                        Ping p1 = new Ping();
                    PingReply PR = p1.Send("10.66.18.31");
                    if(PR.Status == IPStatus.Success)
                    {
                        Console.WriteLine("-->Connectivity is Good, latency {0}", PR.RoundtripTime);
                    }
                    else
                    {
                        Console.WriteLine("-->Connectivity is NOT GOOD, data: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(PR));
                    }
                }
                else
                {
                    Console.WriteLine("Not IH5 Connected. Searching with Wifi Radio!");
                    thing.Scan();
                        var broadcastNets = thing.GetAvailableNetworkList(NativeWifi.Wlan.WlanGetAvailableNetworkFlags.IncludeAllAdhocProfiles);
                    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(broadcastNets));

                    bool flagFound = false;

                    foreach (var availNetwork in broadcastNets)
                    {
                        if ( ssidList.Any(p=>p==availNetwork.profileName) )
                        {
                            Console.WriteLine("-->INTEGRIS Network Detected!");

                            if (availNetwork.wlanSignalQuality >= 50)
                            {

                                thing.Connect(NativeWifi.Wlan.WlanConnectionMode.Profile, NativeWifi.Wlan.Dot11BssType.Any, "IH5");
                                Console.WriteLine("-->Should connect now!");
                                Console.WriteLine("--> !! Disable broadband adapter now... ");
                                //SysInterfaceHelper.Disable("Mobile Broadband Connection");
                                flagFound = true;
                            }
                            else
                            {
                                Console.WriteLine("-->Signal Strength is too weak to connect! ({0}%)", availNetwork.wlanSignalQuality);
                            }

                        }
                        
                    }

                    if(!flagFound)
                    {
                        Console.WriteLine("--> ## Enable broadband adapter now... ");
                        SysInterfaceHelper.Enable("Mobile Broadband Connection");
                        Console.WriteLine("@@ Connecting to AT&T...");
                        MobileAPIHelper.Connect();
                    }
                }

            }

        }

        public void Start() { _timer.Start(); }
        public void Stop() { _timer.Stop(); }




    }
}
