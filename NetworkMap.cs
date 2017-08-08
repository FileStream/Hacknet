// Decompiled with JetBrains decompiler
// Type: Hacknet.NetworkMap
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Hacknet
{
  internal class NetworkMap : CoreModule
  {
    public static int NODE_SIZE = 26;
    public static float ADMIN_CIRCLE_SCALE = 0.62f;
    public static float PULSE_DECAY = 0.5f;
    public static float PULSE_FREQUENCY = 0.8f;
    private float rotation = 0.0f;
    private float pulseFade = 1f;
    private float pulseTimer = NetworkMap.PULSE_FREQUENCY;
    public bool DimNonConnectedNodes = false;
    public NetmapSortingAlgorithm SortingAlgorithm = NetmapSortingAlgorithm.Scatter;
    public List<Corporation> corporations;
    public List<Computer> nodes;
    public List<int> visibleNodes;
    private Texture2D circle;
    private Texture2D circleOutline;
    private Texture2D adminCircle;
    private Texture2D nodeCircle;
    private Texture2D adminNodeCircle;
    private Texture2D nodeGlow;
    private Texture2D homeNodeCircle;
    private Texture2D targetNodeCircle;
    private Texture2D assetServerNodeOverlay;
    private string label;
    private Vector2 circleOrigin;
    public ConnectedNodeEffect nodeEffect;
    public ConnectedNodeEffect adminNodeEffect;
    public Computer mailServer;
    public Computer academicDatabase;
    public Computer lastAddedNode;

    public NetworkMap(Rectangle location, OS operatingSystem)
      : base(location, operatingSystem)
    {
    }

    public override void LoadContent()
    {
      this.label = "Network Map";
      this.visibleNodes = new List<int>();
      if (OS.WillLoadSave || Settings.IsInExtensionMode)
      {
        this.nodes = new List<Computer>();
        this.corporations = new List<Corporation>();
      }
      else if (this.os.multiplayer)
      {
        this.nodes = this.generateNetwork(this.os);
        this.corporations = new List<Corporation>();
      }
      else
      {
        this.nodes = new List<Computer>();
        List<Computer> gameNodes = this.generateGameNodes();
        this.nodes.Clear();
        this.nodes.AddRange((IEnumerable<Computer>) gameNodes);
        if (Settings.isDemoMode)
          this.nodes.AddRange((IEnumerable<Computer>) this.generateDemoNodes());
        this.nodes.Insert(0, this.generateSPNetwork(this.os)[0]);
        this.corporations = this.generateCorporations();
      }
      this.nodeEffect = new ConnectedNodeEffect(this.os);
      this.adminNodeEffect = new ConnectedNodeEffect(this.os);
      this.adminNodeEffect.color = new Color(60, 65, 75, 19);
      this.circle = TextureBank.load("Circle", this.os.content);
      this.nodeCircle = TextureBank.load("NodeCircle", this.os.content);
      this.adminNodeCircle = TextureBank.load("AdminNodeCircle", this.os.content);
      this.homeNodeCircle = TextureBank.load("HomeNodeCircle", this.os.content);
      this.targetNodeCircle = TextureBank.load("TargetNodeCircle", this.os.content);
      this.assetServerNodeOverlay = TextureBank.load("AssetServerNodeOverlay", this.os.content);
      this.circleOutline = TextureBank.load("CircleOutline", this.os.content);
      this.adminCircle = TextureBank.load("AdminCircle", this.os.content);
      this.nodeGlow = TextureBank.load("RadialGradient", this.os.content);
      this.circleOrigin = new Vector2((float) (this.circleOutline.Width / 2), (float) (this.circleOutline.Height / 2));
    }

    public override void Update(float t)
    {
      this.rotation += t / 2f;
      if ((double) this.pulseFade > 0.0)
      {
        this.pulseFade -= t * NetworkMap.PULSE_DECAY;
      }
      else
      {
        this.pulseTimer -= t;
        if ((double) this.pulseTimer <= 0.0)
        {
          this.pulseFade = 1f;
          this.pulseTimer = NetworkMap.PULSE_FREQUENCY;
        }
      }
      for (int index = 0; index < this.nodes.Count; ++index)
      {
        if (this.nodes[index].disabled)
          this.nodes[index].bootupTick(t);
      }
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.doGui(t);
    }

    public void CleanVisibleListofDuplicates()
    {
      List<int> intList = new List<int>();
      for (int index = 0; index < this.visibleNodes.Count; ++index)
      {
        if (!intList.Contains(this.visibleNodes[index]))
          intList.Add(this.visibleNodes[index]);
      }
      this.visibleNodes = intList;
    }

    public string getSaveString()
    {
      string str = string.Format("<NetworkMap sort=\"{0}\" >\n", (object) this.SortingAlgorithm) + this.getVisibleNodesString() + "\n" + "<network>\n";
      for (int index = 0; index < this.nodes.Count; ++index)
        str += this.nodes[index].getSaveString();
      return str + "</network>\n" + "</NetworkMap>";
    }

    public string getVisibleNodesString()
    {
      string str = "<visible>";
      for (int index = 0; index < this.visibleNodes.Count; ++index)
        str = str + (object) this.visibleNodes[index] + (index != this.visibleNodes.Count - 1 ? (object) " " : (object) "");
      return str + "</visible>";
    }

    public void load(XmlReader reader)
    {
      this.nodes.Clear();
      while (reader.Name != "NetworkMap")
        reader.Read();
      if (reader.MoveToAttribute("sort"))
      {
        string str = reader.ReadContentAsString();
        if (!Enum.TryParse<NetmapSortingAlgorithm>(str, out this.SortingAlgorithm))
        {
          Console.WriteLine("Error parsing netmap sorting algorithm: " + str);
          Utils.AppendToErrorFile("Error parsing netmap sorting algorithm: " + str);
        }
      }
      while (reader.Name != "visible")
        reader.Read();
      foreach (string str in reader.ReadElementContentAsString().Split())
        this.visibleNodes.Add(Convert.ToInt32(str));
      while (reader.Name != "network")
        reader.Read();
      reader.Read();
      while (reader.Name != "network")
      {
        while (reader.Name == "computer" && reader.NodeType != XmlNodeType.EndElement)
          this.nodes.Add(Computer.load(reader, this.os));
        while ((!(reader.Name == "computer") || reader.NodeType == XmlNodeType.EndElement) && reader.Name != "network")
          reader.Read();
      }
      for (int index1 = 0; index1 < this.nodes.Count; ++index1)
      {
        Computer node = this.nodes[index1];
        for (int index2 = 0; index2 < node.daemons.Count; ++index2)
          node.daemons[index2].loadInit();
      }
      this.loadAssignGameNodes();
      Console.WriteLine("Done loading");
    }

    private void loadAssignGameNodes()
    {
      this.mailServer = Programs.getComputer(this.os, "jmail");
      this.academicDatabase = Programs.getComputer(this.os, "academic");
    }

    public List<Corporation> generateCorporations()
    {
      List<Corporation> corporationList = new List<Corporation>();
      int num = 0;
      for (int index = 0; index < num; ++index)
        corporationList.Add(new Corporation(this.os));
      return corporationList;
    }

    public List<Computer> generateNetwork(OS os)
    {
      List<Computer> computerList1 = new List<Computer>();
      List<Computer> computerList2 = new List<Computer>();
      List<Computer> computerList3 = new List<Computer>();
      int num1 = 2;
      float num2 = 0.5f;
      int num3 = 4;
      int num4 = 0;
      float num5 = (float) num1;
      bool flag1 = false;
      float x = (float) ((this.bounds.Width - 40) / (num3 * 2 + 3));
      float num6 = (float) (this.bounds.Height - 30);
      float y = 10f;
      Vector2 compLocation = new Vector2(x, y);
      bool flag2 = false;
      while (num4 >= 0 || !flag1)
      {
        float num7 = num6 / (num5 + 1f);
        compLocation.Y = num7;
        for (int index1 = 0; index1 < (int) num5; ++index1)
        {
          Computer computer = new Computer(NameGenerator.generateName(), NetworkMap.generateRandomIP(), compLocation, num4, Utils.flipCoin() ? (byte) 1 : (byte) 2, os);
          Utils.random.NextDouble();
          int index2 = Math.Min(Math.Max(num4, 0), PortExploits.services.Count - 1);
          computer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[PortExploits.portNums[index2]], PortExploits.cracks[PortExploits.portNums[index2]]));
          computerList3.Add(computer);
          computerList1.Add(computer);
          compLocation.Y += num7;
        }
        for (int index = 0; index < computerList2.Count; ++index)
        {
          bool flag3 = index - 1 >= 0 && index < computerList2.Count - 1;
          bool flag4 = index < computerList3.Count && index < computerList2.Count;
          bool flag5 = index + 1 < computerList3.Count && index + 1 < computerList2.Count - 1;
          if (flag3)
          {
            computerList3[index - 1].links.Add(computerList1.IndexOf(computerList2[index]));
            computerList2[index - 1].links.Add(computerList1.IndexOf(computerList3[index]));
          }
          if (flag4)
          {
            computerList3[index].links.Add(computerList1.IndexOf(computerList2[index]));
            computerList2[index].links.Add(computerList1.IndexOf(computerList3[index]));
          }
          if (flag5)
          {
            computerList3[index + 1].links.Add(computerList1.IndexOf(computerList2[index]));
            computerList2[index + 1].links.Add(computerList1.IndexOf(computerList3[index]));
          }
        }
        computerList2.Clear();
        for (int index = 0; index < computerList3.Count; ++index)
          computerList2.Add(computerList3[index]);
        computerList3.Clear();
        compLocation.X += x;
        if (flag1)
        {
          --num4;
          num5 -= num2;
        }
        else
        {
          ++num4;
          num5 += num2;
        }
        if (num4 > num3)
        {
          --num4;
          flag1 = true;
          num5 += num2;
        }
        flag2 = false;
      }
      if (!os.multiplayer)
        computerList1.AddRange((IEnumerable<Computer>) this.generateGameNodes());
      if (Settings.isDemoMode)
        computerList1.AddRange((IEnumerable<Computer>) this.generateDemoNodes());
      return computerList1;
    }

    public List<Computer> generateSPNetwork(OS os)
    {
      List<Computer> computerList = new List<Computer>();
      Computer computer = new Computer(NameGenerator.generateName(), NetworkMap.generateRandomIP(), this.getRandomPosition(), 0, (byte) 2, os);
      computer.idName = "firstGeneratedNode";
      computer.files.root.searchForFolder("bin").files.Add(new FileEntry(Utils.readEntireFile("Content/files/config.txt"), "config.txt"));
      ThemeManager.setThemeOnComputer((object) computer, OSTheme.HackerGreen);
      computerList.Add(computer);
      if (!os.IsDLCConventionDemo)
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[4], "SecurityTracer.exe"));
      return computerList;
    }

    public List<Computer> generateGameNodes()
    {
      List<Computer> computerList = new List<Computer>();
      Computer computer1 = (Computer) ComputerLoader.loadComputer("Content/Missions/CoreServers/JMailServer.xml", false, false);
      computer1.location = new Vector2(0.7f, 0.2f);
      this.mailServer = computer1;
      computerList.Add(computer1);
      Computer c1 = new Computer("boatmail.com", "65.55.72.183", new Vector2(0.6f, 0.9f), 4, (byte) 3, this.os);
      c1.idName = "boatmail";
      c1.daemons.Add((Daemon) new BoatMail(c1, "Boatmail", this.os));
      c1.initDaemons();
      computerList.Add(c1);
      Computer c2 = (Computer) ComputerLoader.loadComputer("Content/Missions/CoreServers/InternationalAcademicDatabase.xml", false, false);
      AcademicDatabaseDaemon academicDatabaseDaemon = new AcademicDatabaseDaemon(c2, "Academic Database", this.os);
      c2.daemons.Add((Daemon) academicDatabaseDaemon);
      c2.initDaemons();
      this.academicDatabase = c2;
      computerList.Add(c2);
      Computer computer2 = (Computer) ComputerLoader.loadComputer("Content/Missions/CoreServers/ContractHubAssetsComp.xml", false, false);
      computerList.Add(computer2);
      Computer ch = (Computer) ComputerLoader.loadComputer("Content/Missions/CoreServers/ContractHubComp.xml", false, false);
      this.os.delayer.Post(ActionDelayer.NextTick(), (Action) (() =>
      {
        ch.daemons.Add((Daemon) new MissionHubServer(ch, "CSEC Contract Database", "CSEC", this.os));
        ch.initDaemons();
      }));
      computerList.Add(ch);
      computer2.location = ch.location + Corporation.getNearbyNodeOffset(ch.location, 1, 1, this, 0.0f, false);
      computerList.Add(new Computer("Cheater's Stash", "1337.1337.1337.1337", this.getRandomPosition(), 0, (byte) 2, this.os)
      {
        idName = "haxServer",
        files = {
          root = {
            files = {
              new FileEntry(PortExploits.crackExeData[PortExploits.portNums[0]], PortExploits.cracks[PortExploits.portNums[0]]),
              new FileEntry(PortExploits.crackExeData[PortExploits.portNums[1]], PortExploits.cracks[PortExploits.portNums[1]]),
              new FileEntry(PortExploits.crackExeData[PortExploits.portNums[2]], PortExploits.cracks[PortExploits.portNums[2]]),
              new FileEntry(PortExploits.crackExeData[PortExploits.portNums[3]], PortExploits.cracks[PortExploits.portNums[3]]),
              this.GetProgramForNum(1433),
              this.GetProgramForNum(104),
              this.GetProgramForNum(9),
              this.GetProgramForNum(13),
              this.GetProgramForNum(10)
            }
          }
        }
      });
      return computerList;
    }

    private FileEntry GetProgramForNum(int num)
    {
      return new FileEntry(PortExploits.crackExeData[num], PortExploits.cracks[num]);
    }

    public List<Computer> generateDemoNodes()
    {
      return new List<Computer>() { new Computer("AvCon Hatland Demo PC", "192.168.1.3", this.getRandomPosition(), 1, (byte) 2, this.os) { idName = "avcon1", externalCounterpart = new ExternalCounterpart("avcon1", ExternalCounterpart.getIPForServerName("avconServer")) } };
    }

    public void discoverNode(Computer c)
    {
      if (!this.visibleNodes.Contains(this.nodes.IndexOf(c)))
        this.visibleNodes.Add(this.nodes.IndexOf(c));
      c.highlightFlashTime = 1f;
      this.lastAddedNode = c;
    }

    public void discoverNode(string cName)
    {
      for (int index = 0; index < this.nodes.Count; ++index)
      {
        if (this.nodes[index].idName.Equals(cName))
        {
          this.discoverNode(this.nodes[index]);
          break;
        }
      }
    }

    public Vector2 getRandomPosition()
    {
      for (int index = 0; index < 50; ++index)
      {
        Vector2 pos = this.generatePos();
        if (!this.collides(pos, -1f))
          return pos;
      }
      return this.generatePos();
    }

    private Vector2 generatePos()
    {
      float nodeSize = (float) NetworkMap.NODE_SIZE;
      return new Vector2((float) Utils.random.NextDouble(), (float) Utils.random.NextDouble());
    }

    public bool collides(Vector2 location, float minSeperation = -1f)
    {
      if (this.nodes == null)
        return false;
      float num = 0.075f;
      if ((double) minSeperation > 0.0)
        num = minSeperation;
      for (int index = 0; index < this.nodes.Count; ++index)
      {
        if ((double) Vector2.Distance(location, this.nodes[index].location) <= (double) num)
          return true;
      }
      return false;
    }

    public void randomizeNetwork()
    {
      for (int index = 0; index < 10; ++index)
        this.nodes.Add(new Computer(NameGenerator.generateName(), NetworkMap.generateRandomIP(), new Vector2((float) Utils.random.NextDouble() * (float) this.bounds.Width, (float) Utils.random.NextDouble() * (float) this.bounds.Height), Utils.random.Next(0, 4), Utils.randomCompType(), this.os));
      int num1 = 2;
      for (int index1 = 0; index1 < this.nodes.Count; ++index1)
      {
        int num2 = num1 + Utils.random.Next(0, 2);
        for (int index2 = 0; index2 < num2; ++index2)
          this.nodes[index1].links.Add(Utils.random.Next(0, this.nodes.Count - 1));
      }
    }

    public void doGui(float t)
    {
      int num1 = -1;
      Color highlightColor = this.os.highlightColor;
      for (int nodeIndex = 0; nodeIndex < this.nodes.Count; ++nodeIndex)
      {
        Vector2 nodeDrawPos = this.GetNodeDrawPos(this.nodes[nodeIndex], nodeIndex);
        if (this.visibleNodes.Contains(nodeIndex) && !this.nodes[nodeIndex].disabled)
        {
          for (int index = 0; index < this.nodes[nodeIndex].links.Count; ++index)
          {
            if (this.visibleNodes.Contains(this.nodes[nodeIndex].links[index]) && !this.nodes[this.nodes[nodeIndex].links[index]].disabled)
              this.drawLine(this.GetNodeDrawPos(this.nodes[nodeIndex], nodeIndex), this.GetNodeDrawPos(this.nodes[this.nodes[nodeIndex].links[index]], this.nodes[nodeIndex].links[index]), new Vector2((float) this.bounds.X, (float) this.bounds.Y));
          }
          if ((double) this.pulseFade > 0.0)
          {
            Color color = this.os.highlightColor * (this.pulseFade * this.pulseFade);
            if (this.DimNonConnectedNodes)
              color = Utils.AddativeRed * 0.5f * (this.pulseFade * this.pulseFade);
            this.spriteBatch.Draw(this.circleOutline, new Vector2((float) (this.bounds.X + (int) nodeDrawPos.X + NetworkMap.NODE_SIZE / 2), (float) (this.bounds.Y + (int) nodeDrawPos.Y + NetworkMap.NODE_SIZE / 2)), new Rectangle?(), color, this.rotation, this.circleOrigin, new Vector2(NetworkMap.ADMIN_CIRCLE_SCALE * (float) (2.0 - 2.0 * (double) this.pulseFade)), SpriteEffects.None, 0.5f);
          }
          if (this.nodes[nodeIndex].idName == this.os.homeNodeID && !this.nodes[nodeIndex].disabled)
            this.spriteBatch.Draw(this.homeNodeCircle, new Vector2((float) (this.bounds.X + (int) nodeDrawPos.X + NetworkMap.NODE_SIZE / 2), (float) (this.bounds.Y + (int) nodeDrawPos.Y + NetworkMap.NODE_SIZE / 2)), new Rectangle?(), this.os.connectedComp == null || this.os.connectedComp != this.nodes[nodeIndex] ? (this.nodes[nodeIndex].Equals((object) this.os.thisComputer) ? this.os.thisComputerNode : this.os.highlightColor) : (this.nodes[nodeIndex].adminIP == this.os.thisComputer.ip ? Utils.AddativeWhite : Utils.AddativeWhite), -1f * this.rotation, this.circleOrigin, new Vector2(NetworkMap.ADMIN_CIRCLE_SCALE), SpriteEffects.None, 0.5f);
          if (this.nodes[nodeIndex] == this.lastAddedNode && !this.nodes[nodeIndex].disabled)
          {
            float num2 = this.nodes[nodeIndex].adminIP == this.os.thisComputer.ip ? 1f : 4f;
            this.spriteBatch.Draw(this.targetNodeCircle, new Vector2((float) (this.bounds.X + (int) nodeDrawPos.X + NetworkMap.NODE_SIZE / 2), (float) (this.bounds.Y + (int) nodeDrawPos.Y + NetworkMap.NODE_SIZE / 2)), new Rectangle?(), this.os.connectedComp == null || this.os.connectedComp != this.nodes[nodeIndex] ? (this.nodes[nodeIndex].Equals((object) this.os.thisComputer) ? this.os.thisComputerNode : (this.nodes[nodeIndex].adminIP == this.os.thisComputer.ip ? this.os.highlightColor : Utils.AddativeWhite)) : (this.nodes[nodeIndex].adminIP == this.os.thisComputer.ip ? this.os.highlightColor : Utils.AddativeWhite), -1f * this.rotation, this.circleOrigin, new Vector2(NetworkMap.ADMIN_CIRCLE_SCALE) + Vector2.One * ((float) Math.Sin((double) this.os.timer * 3.0) * num2) / (float) this.targetNodeCircle.Height, SpriteEffects.None, 0.5f);
          }
          if (this.nodes[nodeIndex].adminIP == this.os.thisComputer.ip && !this.nodes[nodeIndex].disabled)
            this.spriteBatch.Draw(this.nodes[nodeIndex].ip.Equals(this.os.thisComputer.ip) ? this.adminCircle : this.nodeGlow, new Vector2((float) (this.bounds.X + (int) nodeDrawPos.X + NetworkMap.NODE_SIZE / 2), (float) (this.bounds.Y + (int) nodeDrawPos.Y + NetworkMap.NODE_SIZE / 2)), new Rectangle?(), this.nodes[nodeIndex].Equals((object) this.os.thisComputer) ? this.os.thisComputerNode : this.os.highlightColor, this.rotation, this.circleOrigin, new Vector2(NetworkMap.ADMIN_CIRCLE_SCALE), SpriteEffects.None, 0.5f);
        }
      }
      lock (this.nodes)
      {
        for (int nodeIndex = 0; nodeIndex < this.nodes.Count; ++nodeIndex)
        {
          if (this.visibleNodes.Contains(nodeIndex) && !this.nodes[nodeIndex].disabled)
          {
            Color color;
            if (this.os.thisComputer.ip == this.nodes[nodeIndex].ip)
            {
              color = this.os.thisComputerNode;
            }
            else
            {
              color = this.os.connectedComp == null ? (!(this.nodes[nodeIndex].adminIP == this.os.thisComputer.ip) || !(this.nodes[nodeIndex].ip == this.os.opponentLocation) ? (!this.os.shellIPs.Contains(this.nodes[nodeIndex].ip) ? this.os.highlightColor : this.os.shellColor) : Color.DarkRed) : (!(this.os.connectedComp.ip == this.nodes[nodeIndex].ip) ? (!(this.nodes[nodeIndex].adminIP == this.os.thisComputer.ip) || !(this.nodes[nodeIndex].ip == this.os.opponentLocation) ? (!this.os.shellIPs.Contains(this.nodes[nodeIndex].ip) ? this.os.highlightColor : this.os.shellColor) : Color.DarkRed) : Color.White);
              if ((double) this.nodes[nodeIndex].highlightFlashTime > 0.0)
              {
                this.nodes[nodeIndex].highlightFlashTime -= t;
                color = Color.Lerp(color, Utils.AddativeWhite, Utils.QuadraticOutCurve(this.nodes[nodeIndex].highlightFlashTime));
              }
            }
            Vector2 nodeDrawPos = this.GetNodeDrawPos(this.nodes[nodeIndex], nodeIndex);
            if (this.DimNonConnectedNodes && this.os.connectedComp != null && this.os.connectedComp.ip != this.nodes[nodeIndex].ip)
              color *= 0.3f;
            if (Button.doButton(2000 + nodeIndex, this.bounds.X + (int) nodeDrawPos.X, this.bounds.Y + (int) nodeDrawPos.Y, NetworkMap.NODE_SIZE, NetworkMap.NODE_SIZE, "", new Color?(color), this.nodes[nodeIndex].adminIP == this.os.thisComputer.ip ? this.adminNodeCircle : this.nodeCircle) && this.os.inputEnabled)
            {
              bool flag = false;
              if (this.os.terminal.preventingExecution && this.os.terminal.executionPreventionIsInteruptable)
              {
                this.os.terminal.executeLine();
                flag = true;
              }
              int nodeindex = nodeIndex;
              Action action = (Action) (() => this.os.runCommand("connect " + this.nodes[nodeindex].ip));
              if (flag)
                this.os.delayer.Post(ActionDelayer.NextTick(), action);
              else
                action();
            }
            if (GuiData.hot == 2000 + nodeIndex)
              num1 = nodeIndex;
            if (this.nodes[nodeIndex].idName == this.os.homeAssetServerID && !this.nodes[nodeIndex].disabled)
              this.spriteBatch.Draw(this.assetServerNodeOverlay, new Vector2((float) (this.bounds.X + (int) nodeDrawPos.X + NetworkMap.NODE_SIZE / 2), (float) (this.bounds.Y + (int) nodeDrawPos.Y + NetworkMap.NODE_SIZE / 2)), new Rectangle?(), num1 == nodeIndex ? GuiData.Default_Lit_Backing_Color : (this.os.connectedComp == null || this.os.connectedComp != this.nodes[nodeIndex] ? (this.nodes[nodeIndex].Equals((object) this.os.thisComputer) ? this.os.thisComputerNode : this.os.highlightColor) : Color.Black), -0.5f * this.rotation, new Vector2((float) (this.assetServerNodeOverlay.Width / 2)), new Vector2(NetworkMap.ADMIN_CIRCLE_SCALE - 0.22f), SpriteEffects.None, 0.5f);
          }
        }
      }
      int nodeIndex1 = this.os.connectedComp == null ? 0 : this.nodes.IndexOf(this.os.connectedComp);
      if (this.os.connectedComp != null && !this.os.connectedComp.Equals((object) this.os.thisComputer) && !this.os.connectedComp.adminIP.Equals(this.os.thisComputer.ip))
      {
        GuiData.spriteBatch.Draw(this.nodeGlow, this.GetNodeDrawPos(this.os.connectedComp, nodeIndex1) - new Vector2((float) (this.nodeGlow.Width / 2), (float) (this.nodeGlow.Height / 2)) + new Vector2((float) this.bounds.X, (float) this.bounds.Y) + new Vector2((float) (NetworkMap.NODE_SIZE / 2)), this.os.connectedNodeHighlight);
        if (this.nodeEffect != null && this.os.connectedComp != null)
          this.nodeEffect.draw(this.spriteBatch, this.GetNodeDrawPos(this.os.connectedComp, nodeIndex1) + new Vector2((float) (NetworkMap.NODE_SIZE / 2)) + new Vector2((float) this.bounds.X, (float) this.bounds.Y));
      }
      else if (this.os.connectedComp != null && !this.os.connectedComp.Equals((object) this.os.thisComputer) && this.adminNodeEffect != null)
        this.adminNodeEffect.draw(this.spriteBatch, this.GetNodeDrawPos(this.os.connectedComp, nodeIndex1) + new Vector2((float) (NetworkMap.NODE_SIZE / 2)) + new Vector2((float) this.bounds.X, (float) this.bounds.Y));
      if (num1 != -1)
      {
        try
        {
          int nodeIndex2 = num1;
          Vector2 nodeDrawPos = this.GetNodeDrawPos(this.nodes[nodeIndex2], nodeIndex2);
          Vector2 ttpos = new Vector2((float) (this.bounds.X + (int) nodeDrawPos.X + NetworkMap.NODE_SIZE), (float) (this.bounds.Y + (int) nodeDrawPos.Y));
          string text = this.nodes[nodeIndex2].getTooltipString();
          Vector2 textSize = GuiData.tinyfont.MeasureString(text);
          this.os.postFXDrawActions += (Action) (() =>
          {
            GuiData.spriteBatch.Draw(Utils.white, new Rectangle((int) ttpos.X, (int) ttpos.Y, (int) textSize.X, (int) textSize.Y), this.os.netmapToolTipBackground);
            TextItem.doFontLabel(ttpos, text, GuiData.tinyfont, new Color?(this.os.netmapToolTipColor), float.MaxValue, float.MaxValue, false);
          });
        }
        catch (Exception ex)
        {
          DebugLog.add(ex.ToString());
        }
      }
      if (!Settings.debugDrawEnabled)
        return;
      for (int index = 0; index < Corporation.TestedPositions.Count; ++index)
      {
        Vector2 nodeDrawPosDebug = this.GetNodeDrawPosDebug(Corporation.TestedPositions[index]);
        Vector2 position = new Vector2((float) (this.bounds.X + (int) nodeDrawPosDebug.X + NetworkMap.NODE_SIZE), (float) (this.bounds.Y + (int) nodeDrawPosDebug.Y));
        GuiData.spriteBatch.Draw(Utils.white, position, new Rectangle?(), Utils.AddativeRed, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.8f);
      }
    }

    public Vector2 GetNodeDrawPosDebug(Vector2 nodeLocation)
    {
      int num1 = 3;
      nodeLocation = Utils.Clamp(nodeLocation, 0.0f, 1f);
      float num2 = (float) this.bounds.Width - (float) NetworkMap.NODE_SIZE * 1f;
      float num3 = (float) this.bounds.Height - (float) NetworkMap.NODE_SIZE * 1f;
      float num4 = num2 - (float) (2 * num1);
      float num5 = num3 - (float) (2 * num1);
      return new Vector2((float) ((double) nodeLocation.X * (double) num4 + (double) NetworkMap.NODE_SIZE / 4.0), (float) ((double) nodeLocation.Y * (double) num5 + (double) NetworkMap.NODE_SIZE / 4.0));
    }

    public Vector2 GetNodeDrawPos(Computer node)
    {
      return this.GetNodeDrawPos(node, this.nodes.IndexOf(node));
    }

    public Vector2 GetNodeDrawPos(Computer node, int nodeIndex)
    {
      int num1 = 3;
      Utils.Clamp(node.location, 0.0f, 1f);
      float num2 = (float) this.bounds.Width - (float) NetworkMap.NODE_SIZE * 1f;
      float num3 = (float) this.bounds.Height - (float) NetworkMap.NODE_SIZE * 1f;
      return NetmapSortingAlgorithms.GetNodePosition(this.SortingAlgorithm, num2 - (float) (2 * num1), num3 - (float) (2 * num1), node, nodeIndex, this.nodes.Count, this.visibleNodes.Count, this.os) + new Vector2((float) NetworkMap.NODE_SIZE / 4f);
    }

    public static string generateRandomIP()
    {
      return (Utils.random.Next(254) + 1).ToString() + "." + (object) (Utils.random.Next(254) + 1) + "." + (object) (Utils.random.Next(254) + 1) + "." + (object) (Utils.random.Next(254) + 1);
    }

    public void drawLine(Vector2 origin, Vector2 dest, Vector2 offset)
    {
      Vector2 vector2 = new Vector2((float) (NetworkMap.NODE_SIZE / 2));
      origin += vector2;
      dest += vector2;
      float y = Vector2.Distance(origin, dest);
      float rotation = (float) Math.Atan2((double) dest.Y - (double) origin.Y, (double) dest.X - (double) origin.X) + 4.712389f;
      this.spriteBatch.Draw(Utils.white, origin + offset, new Rectangle?(), this.os.outlineColor, rotation, Vector2.Zero, new Vector2(1f, y), SpriteEffects.None, 0.5f);
    }
  }
}
