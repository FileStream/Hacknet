// Decompiled with JetBrains decompiler
// Type: Hacknet.ExternalCounterparts.ExternalNetworkedServer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Hacknet.ExternalCounterparts
{
  public class ExternalNetworkedServer
  {
    private const int BUFFER_SIZE = 4096;
    public Action<string> messageReceived;
    private static ASCIIEncoding encoder;
    private List<TcpClient> connections;
    private Dictionary<NetworkStream, byte[]> buffers;
    private TcpListener listener;

    public ExternalNetworkedServer()
    {
      this.connections = new List<TcpClient>();
      this.buffers = new Dictionary<NetworkStream, byte[]>();
      if (ExternalNetworkedServer.encoder != null)
        return;
      ExternalNetworkedServer.encoder = new ASCIIEncoding();
    }

    public void initializeListener()
    {
      this.listener = new TcpListener(new IPEndPoint(IPAddress.Any, Multiplayer.PORT));
      this.listener.Start();
      this.listener.BeginAcceptTcpClient(new AsyncCallback(this.AcceptTcpConnectionCallback), (object) this.listener);
    }

    public void closeServer()
    {
      this.listener.Stop();
      foreach (TcpClient connection in this.connections)
      {
        connection.GetStream().Close();
        connection.Close();
      }
    }

    private void AcceptTcpConnectionCallback(IAsyncResult ar)
    {
      TcpListener asyncState = ar.AsyncState as TcpListener;
      if (asyncState == null)
        return;
      try
      {
        TcpClient tcpClient = this.listener.EndAcceptTcpClient(ar);
        if (tcpClient != null)
        {
          this.connections.Add(tcpClient);
          NetworkStream stream = tcpClient.GetStream();
          this.buffers.Add(stream, new byte[4096]);
          if (this.messageReceived != null)
            this.messageReceived("Connection");
          tcpClient.GetStream().BeginRead(this.buffers[stream], 0, 4096, new AsyncCallback(this.TcpReadCallback), (object) stream);
          asyncState.BeginAcceptTcpClient(new AsyncCallback(this.AcceptTcpConnectionCallback), (object) asyncState);
        }
      }
      catch (Exception ex)
      {
      }
    }

    private void TcpReadCallback(IAsyncResult ar)
    {
      NetworkStream asyncState = ar.AsyncState as NetworkStream;
      if (asyncState == null)
        return;
      try
      {
        int count = asyncState.EndRead(ar);
        if (count == 0)
          return;
        byte[] buffer = this.buffers[asyncState];
        string str = ExternalNetworkedServer.encoder.GetString(buffer, 0, count);
        if (this.messageReceived != null)
          this.messageReceived(str);
        asyncState.BeginRead(this.buffers[asyncState], 0, 4096, new AsyncCallback(this.TcpReadCallback), (object) asyncState);
      }
      catch (Exception ex)
      {
      }
    }
  }
}
