// Decompiled with JetBrains decompiler
// Type: Hacknet.Magic.NAT
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace Hacknet.Magic
{
  public class NAT
  {
    private static TimeSpan _timeout = new TimeSpan(0, 0, 0, 3);
    private static string _descUrl;
    private static string _serviceUrl;
    private static string _eventUrl;

    public static TimeSpan TimeOut
    {
      get
      {
        return NAT._timeout;
      }
      set
      {
        NAT._timeout = value;
      }
    }

    public static bool Discover()
    {
      Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
      socket.ReceiveTimeout = 5000;
      string s = "M-SEARCH * HTTP/1.1\r\nHOST: 239.255.255.250:1900\r\nST:upnp:rootdevice\r\nMAN:\"ssdp:discover\"\r\nMX:3\r\n\r\n";
      DebugLog.add(s);
      byte[] bytes = Encoding.ASCII.GetBytes(s);
      IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Broadcast, 1900);
      byte[] numArray = new byte[4096];
      DateTime now = DateTime.Now;
      do
      {
        socket.SendTo(bytes, (EndPoint) ipEndPoint);
        socket.SendTo(bytes, (EndPoint) ipEndPoint);
        socket.SendTo(bytes, (EndPoint) ipEndPoint);
        int count;
        do
        {
          try
          {
            count = socket.Receive(numArray);
          }
          catch (Exception ex)
          {
            Console.WriteLine((object) ex);
            count = 0;
          }
          string lower = Encoding.ASCII.GetString(numArray, 0, count).ToLower();
          DebugLog.add(lower);
          if (lower.Contains("upnp:rootdevice"))
          {
            string str = lower.Substring(lower.ToLower().IndexOf("location:") + 9);
            string resp = str.Substring(0, str.IndexOf("\r")).Trim();
            if (!string.IsNullOrEmpty(NAT._serviceUrl = NAT.GetServiceUrl(resp)))
            {
              NAT._descUrl = resp;
              return true;
            }
          }
        }
        while (count > 0);
      }
      while (now.Subtract(DateTime.Now) < NAT._timeout);
      return false;
    }

    private static string GetServiceUrl(string resp)
    {
      try
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(WebRequest.Create(resp).GetResponse().GetResponseStream());
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
        nsmgr.AddNamespace("tns", "urn:schemas-upnp-org:device-1-0");
        if (!xmlDocument.SelectSingleNode("//tns:device/tns:deviceType/text()", nsmgr).Value.Contains("InternetGatewayDevice"))
          return (string) null;
        XmlNode xmlNode1 = xmlDocument.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:WANIPConnection:1\"]/tns:controlURL/text()", nsmgr);
        if (xmlNode1 == null)
        {
          XmlNode xmlNode2 = xmlDocument.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:WANPPPConnection:1\"]/tns:controlURL/text()", nsmgr);
          if (xmlNode2 == null)
            return (string) null;
          XmlNode xmlNode3 = xmlDocument.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:WANPPPConnection:1\"]/tns:eventSubURL/text()", nsmgr);
          NAT._eventUrl = NAT.CombineUrls(resp, xmlNode3.Value);
          return NAT.CombineUrls(resp, xmlNode2.Value);
        }
        XmlNode xmlNode4 = xmlDocument.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:WANIPConnection:1\"]/tns:eventSubURL/text()", nsmgr);
        NAT._eventUrl = NAT.CombineUrls(resp, xmlNode4.Value);
        return NAT.CombineUrls(resp, xmlNode1.Value);
      }
      catch
      {
        return (string) null;
      }
    }

    private static string CombineUrls(string resp, string p)
    {
      int num = resp.IndexOf("://");
      int length = resp.IndexOf('/', num + 3);
      return resp.Substring(0, length) + p;
    }

    public static void ForwardPort(int port, ProtocolType protocol, string description)
    {
      if (string.IsNullOrEmpty(NAT._serviceUrl))
        throw new Exception("No UPnP service available or Discover() has not been called");
      NAT.SOAPRequest(NAT._serviceUrl, "<u:AddPortMapping xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\"><NewRemoteHost></NewRemoteHost><NewExternalPort>" + port.ToString() + "</NewExternalPort><NewProtocol>" + protocol.ToString().ToUpper() + "</NewProtocol><NewInternalPort>" + port.ToString() + "</NewInternalPort><NewInternalClient>" + Dns.GetHostAddresses(Dns.GetHostName())[0].ToString() + "</NewInternalClient><NewEnabled>1</NewEnabled><NewPortMappingDescription>" + description + "</NewPortMappingDescription><NewLeaseDuration>0</NewLeaseDuration></u:AddPortMapping>", "AddPortMapping");
    }

    public static void DeleteForwardingRule(int port, ProtocolType protocol)
    {
      if (string.IsNullOrEmpty(NAT._serviceUrl))
        throw new Exception("No UPnP service available or Discover() has not been called");
      NAT.SOAPRequest(NAT._serviceUrl, "<u:DeletePortMapping xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\"><NewRemoteHost></NewRemoteHost><NewExternalPort>" + (object) port + "</NewExternalPort><NewProtocol>" + protocol.ToString().ToUpper() + "</NewProtocol></u:DeletePortMapping>", "DeletePortMapping");
    }

    public static IPAddress GetExternalIP()
    {
      if (string.IsNullOrEmpty(NAT._serviceUrl))
        throw new Exception("No UPnP service available or Discover() has not been called");
      XmlDocument xmlDocument = NAT.SOAPRequest(NAT._serviceUrl, "<u:GetExternalIPAddress xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\"></u:GetExternalIPAddress>", "GetExternalIPAddress");
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
      nsmgr.AddNamespace("tns", "urn:schemas-upnp-org:device-1-0");
      return IPAddress.Parse(xmlDocument.SelectSingleNode("//NewExternalIPAddress/text()", nsmgr).Value);
    }

    private static XmlDocument SOAPRequest(string url, string soap, string function)
    {
      string s = "<?xml version=\"1.0\"?><s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"><s:Body>" + soap + "</s:Body></s:Envelope>";
      WebRequest webRequest = WebRequest.Create(url);
      webRequest.Method = "POST";
      byte[] bytes = Encoding.UTF8.GetBytes(s);
      webRequest.Headers.Add("SOAPACTION", "\"urn:schemas-upnp-org:service:WANIPConnection:1#" + function + "\"");
      webRequest.ContentType = "text/xml; charset=\"utf-8\"";
      webRequest.ContentLength = (long) bytes.Length;
      webRequest.GetRequestStream().Write(bytes, 0, bytes.Length);
      XmlDocument xmlDocument = new XmlDocument();
      Stream responseStream = webRequest.GetResponse().GetResponseStream();
      xmlDocument.Load(responseStream);
      return xmlDocument;
    }
  }
}
