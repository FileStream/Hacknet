// Decompiled with JetBrains decompiler
// Type: Hacknet.ExternalCounterpart
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Hacknet
{
  public class ExternalCounterpart
  {
    public bool isConnected = false;
    private static Dictionary<string, string> networkIPList;
    private static ASCIIEncoding encoder;
    public string idName;
    public string connectionIP;
    private TcpClient connection;
    private byte[] buffer;

    public ExternalCounterpart(string idName, string ipEndpoint)
    {
      this.idName = idName;
      this.connectionIP = ipEndpoint;
      this.buffer = new byte[4096];
      if (ExternalCounterpart.encoder != null)
        return;
      ExternalCounterpart.encoder = new ASCIIEncoding();
    }

    public static string getIPForServerName(string serverName)
    {
      if (ExternalCounterpart.networkIPList == null)
        ExternalCounterpart.loadNetIPList();
      if (ExternalCounterpart.networkIPList.ContainsKey(serverName))
        return ExternalCounterpart.networkIPList[serverName];
      throw new InvalidOperationException("Server Name Not Found");
    }

    private static void loadNetIPList()
    {
      ExternalCounterpart.networkIPList = new Dictionary<string, string>();
      string str1 = Utils.readEntireFile("Content/Network/NetworkIPList.txt");
      string[] separator = new string[2]{ "\n\r", "\r\n" };
      int num = 1;
      foreach (string str2 in str1.Split(separator, (StringSplitOptions) num))
      {
        string[] strArray = str2.Split(Utils.spaceDelim);
        ExternalCounterpart.networkIPList.Add(strArray[0], strArray[1]);
      }
    }

    public void sendMessage(string message)
    {
      if (!this.isConnected)
        return;
      this.writeMessage(message);
    }

    public void disconnect()
    {
      if (!this.isConnected)
        return;
      this.writeMessage("Disconnecting");
      this.connection.Close();
      this.isConnected = false;
    }

    public void testConnection()
    {
      this.establishConnection();
      while (!this.isConnected)
        Thread.Sleep(5);
      this.writeMessage("Test Message From " + this.idName);
    }

    public void establishConnection()
    {
      TcpClient tcpClient = new TcpClient();
      tcpClient.BeginConnect(IPAddress.Parse(this.connectionIP), Multiplayer.PORT, new AsyncCallback(this.DoTcpConnectionCallback), (object) tcpClient);
    }

    private void DoTcpConnectionCallback(IAsyncResult ar)
    {
      TcpClient asyncState = ar.AsyncState as TcpClient;
      if (asyncState == null || !asyncState.Connected)
        return;
      this.isConnected = true;
      this.connection = asyncState;
      asyncState.EndConnect(ar);
    }

    public void writeMessage(string message)
    {
      if (this.isConnected)
      {
        NetworkStream stream = this.connection.GetStream();
        this.buffer = ExternalCounterpart.encoder.GetBytes(message);
        stream.BeginWrite(this.buffer, 0, this.buffer.Length, new AsyncCallback(this.TCPWriteMessageCallback), (object) stream);
      }
      else
        this.establishConnection();
    }

    private void TCPWriteMessageCallback(IAsyncResult ar)
    {
      NetworkStream asyncState = ar.AsyncState as NetworkStream;
      if (asyncState == null)
        return;
      try
      {
        asyncState.EndWrite(ar);
      }
      catch (Exception ex)
      {
      }
    }
  }
}
