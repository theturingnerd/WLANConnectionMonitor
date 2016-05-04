using MbnApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WLANConnectionMonitor
{
    public static class MobileAPIHelper
    {
        public static void Connect()
        {
            Console.WriteLine("COMAPI::MBNAPI==>{0}", "Invoke.Connect();");
            MbnConnectionManager mbnConnectionMgr = new MbnConnectionManager();

            IMbnConnectionManager connMgr =
            (IMbnConnectionManager)mbnConnectionMgr;

            

            IMbnConnection[] arrConn =
            (IMbnConnection[])connMgr.GetConnections();

            MbnInterfaceManager mbnInfMgr = new MbnInterfaceManager();
            IMbnInterfaceManager mbnInfMgrInterface = mbnInfMgr as IMbnInterfaceManager;
            IMbnInterface[] mobileInterfaces = mbnInfMgrInterface.GetInterfaces() as IMbnInterface[];
            MbnConnectionProfileManager mbnConnProfileMgr = new MbnConnectionProfileManager();
            IMbnConnectionProfileManager mbnConnProfileMgrInterface = mbnConnProfileMgr as IMbnConnectionProfileManager;
            string profileName = String.Empty;

            if (mbnConnProfileMgrInterface != null)
            {
                bool connProfileFound = false;


                IMbnConnectionProfile[] mbnConnProfileInterfaces =
                    mbnConnProfileMgrInterface.GetConnectionProfiles(mobileInterfaces[0]) as IMbnConnectionProfile[];

                foreach (IMbnConnectionProfile profile in mbnConnProfileInterfaces)
                {
                  //Console.WriteLine(profile.GetProfileXmlData());
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(profile.GetProfileXmlData());

                    profileName = xdoc["MBNProfile"]["Name"].InnerText;
                    connProfileFound = true;

                }
            }

            foreach (var thing in arrConn)
            {
                MBN_ACTIVATION_STATE state;
                string prof = "";
                thing.GetConnectionState(out state, out prof);

                Console.WriteLine("Current state: {0}", state.ToString());

                

                if (state != MBN_ACTIVATION_STATE.MBN_ACTIVATION_STATE_ACTIVATED | state == MBN_ACTIVATION_STATE.MBN_ACTIVATION_STATE_ACTIVATING)
                {
                    uint rid;
                    thing.Connect(MBN_CONNECTION_MODE.MBN_CONNECTION_MODE_PROFILE, profileName, out rid);
                }
            }
        }

        public static void Disconnect()
        {
            Console.WriteLine("COMAPI::MBNAPI==>{0}", "Invoke.Disconnect();");

            MbnConnectionManager mbnConnectionMgr = new MbnConnectionManager();

            IMbnConnectionManager connMgr =
            (IMbnConnectionManager)mbnConnectionMgr;

            IMbnConnection[] arrConn =
            (IMbnConnection[])connMgr.GetConnections();

             foreach (var thing in arrConn)
            {
                MBN_ACTIVATION_STATE state;
                string prof = "";
                thing.GetConnectionState(out state, out prof);

                Console.WriteLine("Current status: {0}", state.ToString());

                if (state == MBN_ACTIVATION_STATE.MBN_ACTIVATION_STATE_ACTIVATED)
                {
                    uint rid;
                    thing.Disconnect(out rid);
                }
            }

        }
    }
}
