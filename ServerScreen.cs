// Decompiled with JetBrains decompiler
// Type: Hacknet.ServerScreen
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.ExternalCounterparts;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class ServerScreen : GameScreen
  {
    private Color backgroundColor = new Color(6, 6, 6);
    public bool drawingWithEffects = false;
    private string mainIP;
    private List<string> messages;
    private ExternalNetworkedServer server;
    private bool canCloseServer;

    public ServerScreen()
    {
      this.mainIP = MultiplayerLobby.getLocalIP();
      this.messages = new List<string>();
      this.server = new ExternalNetworkedServer();
      this.server.initializeListener();
      this.server.messageReceived += new Action<string>(this.parseMessage);
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
      base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
      this.canCloseServer = !otherScreenHasFocus && !coveredByOtherScreen;
    }

    public override void HandleInput(InputState input)
    {
      base.HandleInput(input);
      GuiData.doInput(input);
    }

    public void parseMessage(string msg)
    {
      this.addDisplayMessage(msg);
      char[] chArray = new char[2]{ ' ', '\n' };
      string[] strArray = msg.Split(chArray);
      if (!strArray[0].Equals("cCDDrive"))
        return;
      if (strArray[2].Equals("open"))
        Programs.cdDrive(true);
      else
        Programs.cdDrive(false);
    }

    public void addDisplayMessage(string msg)
    {
      this.messages.Add(msg);
    }

    public override void Draw(GameTime gameTime)
    {
      base.Draw(gameTime);
      if (this.drawingWithEffects)
        PostProcessor.begin();
      GuiData.startDraw();
      Viewport viewport = this.ScreenManager.GraphicsDevice.Viewport;
      GuiData.spriteBatch.Draw(Utils.white, new Rectangle(0, 0, viewport.Width, viewport.Height), this.backgroundColor);
      int x = 80;
      int num1 = 80;
      TextItem.doFontLabel(new Vector2((float) x, (float) num1), "HACKNET RELAY SERVER", GuiData.titlefont, new Color?(), 500f, 50f, false);
      int y1 = num1 + 55;
      if (this.canCloseServer && Button.doButton(800, x, y1, 160, 30, "Shut Down Server", new Color?()))
      {
        this.server.closeServer();
        this.ExitScreen();
      }
      int num2 = y1 + 35;
      for (int index = 0; index < MultiplayerLobby.allLocalIPs.Count; ++index)
      {
        TextItem.doFontLabel(new Vector2((float) x, (float) num2), "IP: " + MultiplayerLobby.allLocalIPs[index], GuiData.smallfont, new Color?(), float.MaxValue, float.MaxValue, false);
        num2 += 20;
      }
      int y2 = num2 + 30;
      this.drawMessageLog(x, y2);
      GuiData.endDraw();
      if (!this.drawingWithEffects)
        return;
      PostProcessor.end();
    }

    public void drawMessageLog(int x, int y)
    {
      float num = 1f;
      Vector2 pos = new Vector2((float) x, (float) y);
      for (int index = 0; index < 20 && index < this.messages.Count; ++index)
      {
        if (index > 10)
          num = (float) (1.0 - (double) (index - 10) / 10.0);
        TextItem.doTinyLabel(pos, this.messages[this.messages.Count - 1 - index], new Color?(Color.White * num));
        pos.Y += 13f;
      }
    }
  }
}
