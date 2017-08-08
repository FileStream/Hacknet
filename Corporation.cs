// Decompiled with JetBrains decompiler
// Type: Hacknet.Corporation
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class Corporation
  {
    private static float COMPUTER_SEPERATION = 0.066f;
    private static float COMPUTER_SEPERATION_ADD_PER_CYCLE = 0.04f;
    private static float Y_ASPECT_RATIO_BIAS = 1.9f;
    public static List<Vector2> TestedPositions = new List<Vector2>();
    private OS os;
    public List<Computer> servers;
    private string name;
    private string postfix;
    private string ipSubstring;
    private string baseID;
    public Computer mainframe;
    public Computer mailServer;
    public Computer webServer;
    public Computer internalServices;
    public Computer fileServer;
    public Computer backupServer;
    private Vector2 basePosition;
    private int baseSecurityLevel;
    private int serverCount;
    private bool altRotation;

    public Corporation(OS _os)
    {
      this.os = _os;
      this.baseSecurityLevel = 3;
      this.serverCount = 5;
      this.baseID = "corp" + (object) GenerationStatics.CorportationsGenerated + "#";
      ++GenerationStatics.CorportationsGenerated;
      this.servers = new List<Computer>();
      this.altRotation = Utils.flipCoin();
      do
      {
        this.basePosition = this.os.netMap.getRandomPosition();
      }
      while (this.locationCollides(this.basePosition));
      this.generate();
    }

    private void generate()
    {
      this.generateName();
      this.generateServers();
    }

    private void generateName()
    {
      string[] companyName = NameGenerator.generateCompanyName();
      this.name = companyName[0];
      this.postfix = companyName[1];
      this.ipSubstring = NetworkMap.generateRandomIP();
      this.ipSubstring = this.ipSubstring.Substring(this.ipSubstring.Length - this.ipSubstring.LastIndexOf('.'));
      this.ipSubstring += ".";
    }

    private void generateServers()
    {
      this.generateMainframe();
      this.os.netMap.nodes.Add(this.servers[this.servers.Count - 1]);
      this.generateMailServer();
      this.os.netMap.nodes.Add(this.servers[this.servers.Count - 1]);
      this.generateWebServer();
      this.os.netMap.nodes.Add(this.servers[this.servers.Count - 1]);
      this.generateInternalServices();
      this.os.netMap.nodes.Add(this.servers[this.servers.Count - 1]);
      this.generateFileServer();
      this.os.netMap.nodes.Add(this.servers[this.servers.Count - 1]);
      this.generateBackupMachine();
      this.os.netMap.nodes.Add(this.servers[this.servers.Count - 1]);
      this.linkServers();
    }

    private void generateMainframe()
    {
      this.mainframe = new Computer(this.getFullName() + " Central Mainframe", this.getAddress(), this.getLocation(), this.baseSecurityLevel + 2, (byte) 3, this.os);
      this.mainframe.idName = this.baseID + "MF";
      this.servers.Add(this.mainframe);
    }

    private void generateMailServer()
    {
      this.mailServer = new Computer(this.name + " Mail Server", this.getAddress(), this.getLocation(), this.baseSecurityLevel, (byte) 3, this.os);
      this.mailServer.daemons.Add((Daemon) new MailServer(this.mailServer, this.name + " Mail", this.os));
      this.mailServer.initDaemons();
      this.mailServer.idName = this.baseID + "MS";
      this.servers.Add(this.mailServer);
    }

    private void generateWebServer()
    {
      this.webServer = new Computer(this.name + " Web Server", this.getAddress(), this.getLocation(), this.baseSecurityLevel, (byte) 3, this.os);
      WebServerDaemon webServerDaemon = new WebServerDaemon(this.webServer, this.name + " Web Server", this.os, "Content/Web/BaseImageWebPage.html");
      this.webServer.daemons.Add((Daemon) webServerDaemon);
      this.webServer.initDaemons();
      webServerDaemon.generateBaseCorporateSite(this.getFullName(), "Content/Web/BaseCorporatePage.html");
      this.webServer.idName = this.baseID + "WS";
      this.servers.Add(this.webServer);
    }

    private void generateInternalServices()
    {
      this.internalServices = new Computer(this.name + " Internal Services Machine", this.getAddress(), this.getLocation(), this.baseSecurityLevel - 1, (byte) 1, this.os);
      this.internalServices.idName = this.baseID + "IS";
      this.servers.Add(this.internalServices);
    }

    private void generateFileServer()
    {
      this.fileServer = new Computer(this.name + " File Server", this.getAddress(), this.getLocation(), this.baseSecurityLevel, (byte) 3, this.os);
      this.fileServer.daemons.Add((Daemon) new MissionListingServer(this.fileServer, "Mission Board", this.name, this.os, false, false));
      this.fileServer.initDaemons();
      this.fileServer.idName = this.baseID + "FS";
      this.servers.Add(this.fileServer);
    }

    private void generateBackupMachine()
    {
      this.backupServer = new Computer(this.getFullName() + " Backup Server", this.getAddress(), this.getLocation(), this.baseSecurityLevel - 1, (byte) 1, this.os);
      this.backupServer.idName = this.baseID + "BU";
      this.servers.Add(this.backupServer);
    }

    private void linkServers()
    {
      for (int index1 = 0; index1 < this.servers.Count; ++index1)
      {
        for (int index2 = 0; index2 < this.servers.Count; ++index2)
        {
          if (index2 != index1)
            this.servers[index1].links.Add(this.os.netMap.nodes.IndexOf(this.servers[index2]) + 1);
        }
      }
    }

    private void addServersToInternet()
    {
      for (int index = 0; index < this.servers.Count; ++index)
        this.os.netMap.nodes.Add(this.servers[index]);
    }

    private string getAddress()
    {
      string str;
      while (true)
      {
        str = this.ipSubstring + (object) (Utils.random.Next() % 253 + 1);
        bool flag = true;
        for (int index = 0; index < this.servers.Count; ++index)
        {
          if (this.servers[index].ip.Equals(str))
            flag = false;
        }
        if (flag)
          break;
      }
      return str;
    }

    private Vector2 getLocation()
    {
      return this.basePosition + Corporation.getNearbyNodeOffset(this.basePosition, this.servers.Count, this.serverCount, this.os.netMap, 0.0f, false);
    }

    public static Vector2 GetOffsetPositionFromCycle(int pos, int total, float ExtraDistance = 0.0f)
    {
      int num1 = Math.Max(0, pos / total - 1);
      int num2 = pos % total;
      float magnitude = (float) ((double) ExtraDistance + (double) Corporation.COMPUTER_SEPERATION + (double) num1 * (double) Corporation.COMPUTER_SEPERATION_ADD_PER_CYCLE);
      Vector2 cartesian = Utils.PolarToCartesian((float) ((double) num2 / (double) total * 6.28318548202515), magnitude);
      cartesian.Y *= Corporation.Y_ASPECT_RATIO_BIAS;
      return cartesian;
    }

    private static bool GeneratedPositionIsValid(Vector2 position, NetworkMap netMap, bool ignoreNetmap = false)
    {
      if ((double) position.X < 0.0 || (double) position.X > 1.0 || (double) position.Y < 0.0 || (double) position.Y > 1.0)
        return false;
      return ignoreNetmap || !netMap.collides(position, -1f);
    }

    public static Vector2 getNearbyNodeOffset(Vector2 basePos, int positionNumber, int total, NetworkMap map, float extraDistance = 0.0f, bool forceUseThisPosition = false)
    {
      int total1 = total;
      int num1 = positionNumber;
      if (total < 20)
      {
        int num2 = 30;
        float num3 = (float) positionNumber / (float) total;
        total1 = num2;
        num1 = (int) ((double) num3 * (double) num2);
      }
      int num4 = 300;
      for (int index = 0; index < num4; ++index)
      {
        Vector2 positionFromCycle = Corporation.GetOffsetPositionFromCycle(num1 + index, total1, extraDistance);
        if (Corporation.GeneratedPositionIsValid(positionFromCycle + basePos, map, forceUseThisPosition))
          return positionFromCycle;
        Corporation.TestedPositions.Add(positionFromCycle + basePos);
      }
      return map.getRandomPosition() - basePos;
    }

    public static Vector2 getNearbyNodeOffsetOld(Vector2 basePos, int positionNumber, int total, NetworkMap map, float ExtraSeperationDistance = 0.0f)
    {
      int num1 = 60;
      int num2 = 0;
      Vector2 vector2 = Vector2.Zero;
      Vector2 location;
      do
      {
        int num3 = positionNumber + num2;
        int num4 = total;
        while (num3 >= num4)
        {
          num3 -= num4;
          num4 += total;
        }
        if (num3 > 0)
          vector2 = Utils.PolarToCartesian((float) num3 / (float) num4 * 6.283185f, 1f);
        else
          vector2.X = 1f;
        vector2.Y *= Corporation.Y_ASPECT_RATIO_BIAS;
        float num5 = Corporation.COMPUTER_SEPERATION + ExtraSeperationDistance;
        vector2 = new Vector2(vector2.X * Corporation.COMPUTER_SEPERATION, vector2.Y * Corporation.COMPUTER_SEPERATION);
        location = basePos + vector2;
        ++num2;
        Corporation.TestedPositions.Add(vector2);
      }
      while (((double) location.X < 0.0 || (double) location.X > 1.0 || ((double) location.Y < 0.0 || (double) location.Y > 1.0) || map.collides(location, 0.075f)) && num2 < num1);
      if (num2 >= num1)
      {
        if ((double) ExtraSeperationDistance <= 0.0)
          return Corporation.getNearbyNodeOffsetOld(basePos, positionNumber, total, map, Corporation.COMPUTER_SEPERATION);
        if ((double) ExtraSeperationDistance <= (double) Corporation.COMPUTER_SEPERATION)
          return Corporation.getNearbyNodeOffsetOld(basePos, positionNumber, total, map, Corporation.COMPUTER_SEPERATION + Corporation.COMPUTER_SEPERATION);
        vector2 = map.getRandomPosition() - basePos;
      }
      return vector2;
    }

    private bool locationCollides(Vector2 loc)
    {
      for (int index = 0; index < this.servers.Count; ++index)
      {
        if ((double) Vector2.Distance(loc, this.servers[index].location) <= 0.0799999982118607)
          return true;
      }
      return (double) loc.X < 0.0 || (double) loc.X > 1.0 || ((double) loc.Y < 0.0 || (double) loc.Y > 1.0);
    }

    public string getName()
    {
      return this.name;
    }

    public string getFullName()
    {
      return this.name + this.postfix;
    }
  }
}
