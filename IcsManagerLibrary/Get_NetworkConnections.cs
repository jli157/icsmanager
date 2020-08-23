using System.Management.Automation;
using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace IcsManagerLibrary
{
    [Cmdlet(VerbsCommon.Get, "NetworkConnections")]
    public class Get_NetworkConnections: PSCmdlet
    {
        protected override void ProcessRecord()
        {
            foreach (var nic in IcsManager.GetAllIPv4Interfaces())
            {
                Console.WriteLine(
                            "Name .......... : {0}", nic.Name);
                Console.WriteLine(
                            "GUID .......... : {0}", nic.Id);
                Console.WriteLine(
                            "Status ........ : {0}", nic.OperationalStatus);

                Console.WriteLine(
                            "InterfaceType . : {0}", nic.NetworkInterfaceType);

                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    var ipprops = nic.GetIPProperties();
                    foreach (var a in ipprops.UnicastAddresses)
                    {
                        if (a.Address.AddressFamily == AddressFamily.InterNetwork)
                            Console.WriteLine(
                                "Unicast address : {0}/{1}", a.Address, a.IPv4Mask);
                    }
                    foreach (var a in ipprops.GatewayAddresses)
                    {
                        Console.WriteLine(
                            "Gateway ....... : {0}", a.Address);
                    }
                }
                try
                {
                    var connection = IcsManager.GetConnectionById(nic.Id);
                    if (connection != null)
                    {
                        var props = IcsManager.GetProperties(connection);
                        Console.WriteLine(
                            "Device ........ : {0}", props.DeviceName);
                        var sc = IcsManager.GetConfiguration(connection);
                        if (sc.SharingEnabled)
                            Console.WriteLine(
                                "SharingType ... : {0}", sc.SharingConnectionType);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Please run this program with Admin rights to see all properties");
                }
                catch (NotImplementedException e)
                {
                    Console.WriteLine("This feature is not supported on your operating system.");
                    Console.WriteLine(e.StackTrace);
                }
                Console.WriteLine();
            }
        }
    }
}
