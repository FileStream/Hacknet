// Decompiled with JetBrains decompiler
// Type: Hacknet.MultiplayerLobby
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Hacknet.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Hacknet
{
  internal class MultiplayerLobby : GameScreen
  {
    public static List<string> allLocalIPs = new List<string>();
    private string destination = "192.168.1.1";
    private string myIP = "?";
    private string externalIP = "Loading...";
    private bool shouldAddServer = false;
    private bool isConnecting = false;
    private string chatIP = "";
    private string messageString = "Test Message";
    private Color darkgrey = new Color(9, 9, 9);
    private Color dark_ish_gray = new Color(20, 20, 20);
    private bool connectingToServer = false;
    private Rectangle fullscreen;
    private TcpListener listener;
    private Thread listenerThread;
    private byte[] buffer;
    private ASCIIEncoding encoder;
    private TcpClient client;
    private NetworkStream clientStream;
    private List<string> messages;
    private TcpClient chatclient;
    private NetworkStream chatclientStream;

    public MultiplayerLobby()
    {
      this.TransitionOnTime = TimeSpan.FromSeconds(0.1);
      this.TransitionOffTime = TimeSpan.FromSeconds(0.3);
    }

    public override void LoadContent()
    {
      base.LoadContent();
      try
      {
        this.listener = new TcpListener(IPAddress.Any, Multiplayer.PORT);
        this.listener.Start();
        this.listener.BeginAcceptTcpClient(new AsyncCallback(this.DoAcceptTcpClientCallback), (object) this.listener);
        this.buffer = new byte[4096];
        this.encoder = new ASCIIEncoding();
        if (MultiplayerLobby.allLocalIPs.Count == 0)
        {
          this.myIP = MultiplayerLobby.getLocalIP();
          new Thread(new ThreadStart(this.getExternalIP)).Start();
          Console.WriteLine("Started Multiplayer IP Getter Thread");
        }
        else
          this.myIP = MultiplayerLobby.allLocalIPs[0];
        this.messages = new List<string>();
        Viewport viewport = this.ScreenManager.GraphicsDevice.Viewport;
        this.fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
      }
      catch (Exception ex)
      {
        this.ScreenManager.RemoveScreen((GameScreen) this);
        this.ScreenManager.AddScreen((GameScreen) new MainMenu(), new PlayerIndex?(this.ScreenManager.controllingPlayer));
      }
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
      base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
      if (!this.shouldAddServer || this.ScreenState == ScreenState.TransitionOff)
        return;
      this.ExitScreen();
      this.ScreenManager.AddScreen((GameScreen) new OS(this.client, this.clientStream, true, this.ScreenManager), new PlayerIndex?(this.ScreenManager.controllingPlayer));
      if (this.chatclient != null)
        this.chatclient.Close();
    }

    public override void HandleInput(InputState input)
    {
      base.HandleInput(input);
      GuiData.doInput(input);
    }

    public override void Draw(GameTime gameTime)
    {
      base.Draw(gameTime);
      GuiData.startDraw();
      this.ScreenManager.SpriteBatch.Draw(Utils.white, this.fullscreen, this.isConnecting ? Color.Gray : Color.Black);
      this.doGui();
      GuiData.endDraw();
      this.ScreenManager.FadeBackBufferToBlack((int) byte.MaxValue - (int) this.TransitionAlpha);
    }

    public void drawConnectingGui()
    {
      PatternDrawer.draw(new Rectangle(100, 200, 600, 250), 1.4f, Color.DarkGreen * 0.3f, Color.DarkGreen, GuiData.spriteBatch);
      TextItem.doLabel(new Vector2(110f, 210f), "Connecting...", new Color?());
    }

    public void doGui()
    {
      PatternDrawer.draw(new Rectangle(180, 160, 500, 85), 1f, this.darkgrey, this.dark_ish_gray, GuiData.spriteBatch);
      TextItem.doSmallLabel(new Vector2(200f, 170f), "IP To Connect to:", new Color?());
      this.destination = TextBox.doTextBox(100, 200, 200, 300, 1, this.destination, GuiData.smallfont);
      if (Button.doButton(123, 510, 201, 120, 23, "Connect", new Color?()) || TextBox.BoxWasActivated)
      {
        this.ConnectToServer(this.destination);
      }
      else
      {
        if (this.isConnecting)
          this.drawConnectingGui();
        TextItem.doLabel(new Vector2(200f, 300f), "Local IPs: " + this.myIP, new Color?());
        float y = 340f;
        for (int index = 0; index < MultiplayerLobby.allLocalIPs.Count - 1; ++index)
        {
          TextItem.doLabel(new Vector2(351f, y), MultiplayerLobby.allLocalIPs[index], new Color?());
          y += 40f;
        }
        TextItem.doLabel(new Vector2(200f, y + 40f), "Extrn IP: " + this.externalIP, new Color?());
        Vector2 pos = new Vector2(610f, 280f);
        TextItem.doLabel(pos, "Info:", new Color?());
        pos.Y += 40f;
        string text = DisplayModule.cleanSplitForWidth("To Begin a multiplayer session, type in the IP of the computer you want to connect to and press enter or connect. Both players must be on this screen. To connect over the internet, use the extern IP address and ensure port 3030 is open.", 400);
        TextItem.doFontLabel(pos, text, GuiData.tinyfont, new Color?(Color.DarkGray), float.MaxValue, float.MaxValue, false);
        if (!Button.doButton(999, 10, 10, 200, 30, "<- Back to Menu", new Color?(Color.Gray)))
          return;
        this.ExitScreen();
        this.ScreenManager.AddScreen((GameScreen) new MainMenu(), new PlayerIndex?(this.ScreenManager.controllingPlayer));
      }
    }

    public void ConnectToServer(string ip)
    {
      this.connectingToServer = true;
      try
      {
        TcpClient socket = new TcpClient();
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), Multiplayer.PORT);
        socket.Connect(remoteEP);
        NetworkStream stream = socket.GetStream();
        this.buffer = this.encoder.GetBytes("connect Client Connecting");
        stream.Write(this.buffer, 0, this.buffer.Length);
        stream.Flush();
        stream.Read(this.buffer, 0, this.buffer.Length);
        this.ExitScreen();
        this.ScreenManager.AddScreen((GameScreen) new OS(socket, stream, false, this.ScreenManager), new PlayerIndex?(this.ScreenManager.controllingPlayer));
      }
      catch (Exception ex)
      {
        DebugLog.add(ex.ToString());
        this.isConnecting = false;
      }
    }

    public void chatToServer(string ip, string msg)
    {
      try
      {
        if (this.chatclientStream == null)
        {
          this.chatclient = new TcpClient();
          this.chatclient.Connect(new IPEndPoint(IPAddress.Parse(ip), Multiplayer.PORT));
          this.chatclientStream = this.chatclient.GetStream();
        }
        this.buffer = this.encoder.GetBytes("Chat " + msg);
        this.messages.Add("Me: " + msg);
        this.chatclientStream.Write(this.buffer, 0, this.buffer.Length);
        this.chatclientStream.Flush();
        this.messageString = "";
      }
      catch (Exception ex)
      {
        Console.WriteLine((object) ex);
      }
    }

    public void DoAcceptTcpClientCallback(IAsyncResult ar)
    {
      TcpListener asyncState = (TcpListener) ar.AsyncState;
      try
      {
        this.client = asyncState.EndAcceptTcpClient(ar);
        this.clientStream = this.client.GetStream();
        bool flag = true;
        while (flag)
        {
          int num = 0;
          try
          {
            num = this.clientStream.Read(this.buffer, 0, 4096);
          }
          catch (Exception ex)
          {
            Console.WriteLine((object) ex);
            if (Game1.threadsExiting)
              break;
          }
          if (num == 0)
          {
            this.client.Close();
            if (Game1.threadsExiting)
              break;
          }
          string str = this.encoder.GetString(this.buffer);
          char[] chArray1 = new char[1]{ ' ' };
          if (str.Split(chArray1)[0].Equals("Chat"))
          {
            char[] chArray2 = new char[3]{ ' ', '\n', char.MinValue };
            this.messages.Add(str.Substring(4).Trim(chArray2).Replace("\0", ""));
            if (Game1.threadsExiting)
              break;
          }
          else
          {
            this.buffer = this.encoder.GetBytes("Replying - Hello World");
            this.clientStream.Write(this.buffer, 0, this.buffer.Length);
            this.clientStream.Flush();
            this.shouldAddServer = true;
            break;
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine((object) ex);
      }
    }

    private void listenForConnections()
    {
      try
      {
        this.listener = new TcpListener(IPAddress.Any, Multiplayer.PORT);
        this.listener.Start();
        this.client = this.listener.AcceptTcpClient();
        this.clientStream = this.client.GetStream();
        bool flag = true;
        while (flag)
        {
          int num = 0;
          try
          {
            num = this.clientStream.Read(this.buffer, 0, 4096);
          }
          catch (Exception ex)
          {
            Console.WriteLine((object) ex);
            if (Game1.threadsExiting)
              break;
          }
          if (num == 0)
          {
            this.client.Close();
            if (Game1.threadsExiting)
              break;
          }
          string str = this.encoder.GetString(this.buffer);
          char[] chArray1 = new char[1]{ ' ' };
          if (str.Split(chArray1)[0].Equals("Chat"))
          {
            char[] chArray2 = new char[3]{ ' ', '\n', char.MinValue };
            this.messages.Add(str.Substring(4).Trim(chArray2).Replace("\0", ""));
            if (Game1.threadsExiting)
              break;
          }
          else
          {
            this.buffer = this.encoder.GetBytes("Replying - Hello World");
            this.clientStream.Write(this.buffer, 0, this.buffer.Length);
            this.clientStream.Flush();
            this.shouldAddServer = true;
            break;
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine((object) ex);
      }
    }

    public static string getLocalIP()
    {
      string str = "?";
      foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
      {
        if (address.AddressFamily == AddressFamily.InterNetwork)
        {
          str = address.ToString();
          MultiplayerLobby.allLocalIPs.Add(address.ToString());
        }
      }
      return str;
    }

    public void getExternalIPwithPF()
    {
      try
      {
        this.externalIP = "Attempting automated port-fowarding...";
        try
        {
          if (NAT.Discover())
          {
            Console.WriteLine("Attempting port foward");
            NAT.ForwardPort(Multiplayer.PORT, ProtocolType.Tcp, "Hacknet (TCP)");
            this.externalIP = NAT.GetExternalIP().ToString();
          }
          else
            this.ScreenManager.ShowPopup("You dont have UPNP enabled - Internet play will not work");
        }
        catch (Exception ex)
        {
          Console.WriteLine((object) ex);
        }
        if (!this.externalIP.Equals("Attempting automated port-fowarding..."))
          return;
        this.externalIP = "Automated port-fowarding Failed - Internet Play Disabled";
      }
      catch (Exception ex)
      {
        Console.WriteLine((object) ex);
        this.externalIP = "Automated port-fowarding Failed - Internet Play Disabled";
      }
    }

    public void getExternalIP()
    {
      try
      {
        this.externalIP = "Unknown...";
        this.externalIP = new WebClient().DownloadString("http://icanhazip.com/");
      }
      catch (Exception ex)
      {
        Console.WriteLine((object) ex);
        this.externalIP = "Could not Find Conection";
      }
    }
  }
}
