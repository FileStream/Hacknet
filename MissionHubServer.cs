// Decompiled with JetBrains decompiler
// Type: Hacknet.MissionHubServer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Factions;
using Hacknet.Gui;
using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hacknet
{
  internal class MissionHubServer : AuthenticatingDaemon
  {
    public Color themeColor = Color.PaleTurquoise;
    public Color themeColorBackground = new Color(10, 15, 25, 200);
    public Color themeColorLine = new Color(20, 25, 45, 200);
    private string groupName = "UNKNOWN";
    private int contractRegistryNumber = 256;
    private Dictionary<string, ActiveMission> listingMissions = new Dictionary<string, ActiveMission>();
    public string MissionSourceFolderPath = "Content/Missions/MainHub/FirstSet/";
    public bool allowAbandon = true;
    private MissionHubServer.HubState state = MissionHubServer.HubState.Welcome;
    private string activeUserName = "UNKNOWN USER";
    private DateTime activeUserLoginTime = DateTime.Now;
    private float screenTransition = 0.0f;
    private int selectedElementIndex = 0;
    private int userListPageNumber = 0;
    private int missionListPageNumber = 0;
    private int missionListDisplayed = 1;
    private float timeSpentInLoading = 0.0f;
    public const string ROOT_FOLDERNAME = "ContractHub";
    public const string CONFIG_FILENAME = "settings.sys";
    public const string CRITICAL_FILE_FILENAME = "net64.sys";
    public const double BUTTON_TRANSITION_OFFSET = 40.0;
    public const float TRANSITION_TIME = 0.3f;
    public const double TRANSITION_ELEMENT_INCREASE = 0.1;
    private Folder root;
    private Folder missionsFolder;
    private Folder usersFolder;
    private Folder listingsFolder;
    private Folder listingArchivesFolder;
    private Texture2D decorationPanel;
    private Texture2D decorationPanelSide;
    private Texture2D lockIcon;
    private BarcodeEffect barcode;
    private ThinBarcode thinBarcodeTop;
    private ThinBarcode thinBarcodeBot;

    public MissionHubServer(Computer c, string serviceName, string group, OS _os)
      : base(c, serviceName, _os)
    {
      this.groupName = group;
      this.decorationPanel = TextureBank.load("Sprites/HubDecoration", this.os.content);
      this.decorationPanelSide = TextureBank.load("Sprites/HubDecorationSide", this.os.content);
      this.lockIcon = TextureBank.load("Lock", this.os.content);
    }

    public override void initFiles()
    {
      this.root = new Folder("ContractHub");
      this.missionsFolder = new Folder("Contracts");
      this.listingsFolder = new Folder("Listings");
      this.listingArchivesFolder = new Folder("Archives");
      this.missionsFolder.folders.Add(this.listingsFolder);
      this.missionsFolder.folders.Add(this.listingArchivesFolder);
      this.usersFolder = new Folder("Users");
      this.root.folders.Add(this.missionsFolder);
      this.root.folders.Add(this.usersFolder);
      this.root.files.Add(this.generateConfigFile());
      this.root.files.Add(new FileEntry(Computer.generateBinaryString(1024), "net64.sys"));
      this.populateUserList();
      this.os.delayer.Post(ActionDelayer.NextTick(), (Action) (() => this.loadInitialContracts()));
      this.comp.files.root.folders.Add(this.root);
    }

    private void populateUserList()
    {
      this.initializeUsers();
    }

    private void loadInitialContracts()
    {
      int num = 0;
      foreach (FileSystemInfo file in new DirectoryInfo(this.MissionSourceFolderPath).GetFiles("*.xml"))
      {
        string filename = this.MissionSourceFolderPath + file.Name;
        try
        {
          this.addMission((ActiveMission) ComputerLoader.readMission(filename), false, false, -1);
        }
        catch (Exception ex)
        {
          throw new FormatException("Error Loading Mission: " + filename, ex);
        }
        ++num;
      }
    }

    public void AddMissionToListings(string missionFilename, int desiredIndex = -1)
    {
      this.addMission((ActiveMission) ComputerLoader.readMission(missionFilename), true, false, desiredIndex);
    }

    public void RemoveMissionFromListings(string missionFilename)
    {
      List<ActiveMission> branchMissions = this.os.branchMissions;
      ActiveMission activeMission = (ActiveMission) ComputerLoader.readMission(Utils.GetFileLoadPrefix() + missionFilename);
      string key = (string) null;
      foreach (KeyValuePair<string, ActiveMission> listingMission in this.listingMissions)
      {
        if (listingMission.Value.reloadGoalsSourceFile == activeMission.reloadGoalsSourceFile)
        {
          key = listingMission.Key;
          break;
        }
      }
      if (key != null)
      {
        this.listingMissions.Remove(key);
        for (int index = 0; index < this.listingsFolder.files.Count; ++index)
        {
          if (this.listingsFolder.files[index].name.Contains("#" + key))
          {
            this.listingsFolder.files.RemoveAt(index);
            --index;
          }
        }
      }
      this.os.branchMissions = branchMissions;
    }

    private void addMission(ActiveMission mission, bool insertAtTop = false, bool preventRegistryNumberChange = false, int desiredInsertionIndex = -1)
    {
      if (insertAtTop && desiredInsertionIndex <= -1)
        desiredInsertionIndex = 0;
      this.contractRegistryNumber += (int) Utils.getRandomByte() + 1;
      this.listingMissions.Add(string.Concat((object) this.contractRegistryNumber), mission);
      FileEntry fileEntry = new FileEntry(MissionSerializer.generateMissionFile((object) mission, this.contractRegistryNumber, "CSEC", (string) null), "Contract#" + (object) this.contractRegistryNumber);
      if (desiredInsertionIndex < this.listingsFolder.files.Count && (insertAtTop || desiredInsertionIndex >= 0))
        this.listingsFolder.files.Insert(desiredInsertionIndex, fileEntry);
      else
        this.listingsFolder.files.Add(fileEntry);
    }

    private FileEntry generateConfigFile()
    {
      return new FileEntry("//Contract Hub Setup File\n" + "ThemeColor = " + Utils.convertColorToParseableString(this.themeColor) + "\n" + "ServiceName = " + this.name + "\n" + "GroupName = " + this.groupName + "\n" + "ContractListingIndexes = " + (object) this.contractRegistryNumber + "\n" + "LineColor = " + Utils.convertColorToParseableString(this.themeColorLine) + "\n" + "BackColor = " + Utils.convertColorToParseableString(this.themeColorBackground) + "\n" + "EnableABN = " + (object) this.allowAbandon + "\n" + "\n", "settings.sys");
    }

    private void loadFromConfigFileData(string config)
    {
      string[] strArray = config.Split(Utils.newlineDelim, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (!strArray[index].StartsWith("//"))
        {
          string line = strArray[index];
          if (line.StartsWith("ThemeColor"))
            this.themeColor = Utils.convertStringToColor(this.getDataFromConfigLine(line, "= "));
          else if (line.StartsWith("ServiceName"))
            this.name = this.getDataFromConfigLine(line, "= ");
          else if (line.StartsWith("GroupName"))
            this.groupName = this.getDataFromConfigLine(line, "= ");
          else if (line.StartsWith("ContractListingIndexes"))
          {
            try
            {
              this.contractRegistryNumber = Convert.ToInt32(this.getDataFromConfigLine(line, "= "));
            }
            catch (FormatException ex)
            {
              this.contractRegistryNumber = 0;
            }
            catch (OverflowException ex)
            {
              this.contractRegistryNumber = 0;
            }
          }
          if (line.StartsWith("LineColor"))
            this.themeColorLine = Utils.convertStringToColor(this.getDataFromConfigLine(line, "= "));
          if (line.StartsWith("BackColor"))
            this.themeColorBackground = Utils.convertStringToColor(this.getDataFromConfigLine(line, "= "));
          if (line.StartsWith("EnableABN"))
            this.allowAbandon = this.getDataFromConfigLine(line, "= ").ToLower() == "true";
        }
      }
    }

    private string getDataFromConfigLine(string line, string sentinel = "= ")
    {
      return line.Substring(line.IndexOf(sentinel) + 2);
    }

    public override void loadInit()
    {
      base.loadInit();
      this.root = this.comp.files.root.searchForFolder("ContractHub");
      this.missionsFolder = this.root.searchForFolder("Contracts");
      this.listingsFolder = this.missionsFolder.searchForFolder("Listings");
      this.listingArchivesFolder = this.missionsFolder.searchForFolder("Archives");
      this.usersFolder = this.root.searchForFolder("Users");
      FileEntry fileEntry = this.root.searchForFile("settings.sys");
      if (fileEntry != null)
        this.loadFromConfigFileData(fileEntry.data);
      this.loadListingMissionsFromFiles();
    }

    private void loadListingMissionsFromFiles()
    {
      for (int index = 0; index < this.listingsFolder.files.Count; ++index)
      {
        string data = this.listingsFolder.files[index].data;
        string stringForContractFile = this.getIDStringForContractFile(this.listingsFolder.files[index]);
        try
        {
          this.listingMissions.Add(stringForContractFile, (ActiveMission) MissionSerializer.restoreMissionFromFile(data, out this.contractRegistryNumber));
        }
        catch (FormatException ex)
        {
        }
      }
    }

    public override string getSaveString()
    {
      return "<MissionHubServer />";
    }

    private void initializeUsers()
    {
      List<int> intList = new List<int>();
      int num1 = 0;
      if (People.hubAgents == null)
        People.init();
      DateTime dateTime;
      for (int index = 0; index < People.hubAgents.Count; ++index)
      {
        num1 = 0;
        int num2;
        do
        {
          num2 = Utils.random.Next(9999);
        }
        while (intList.Contains(num2));
        string str1 = "USER: " + (object) num2 + "\n" + "Handle: " + People.hubAgents[index].handle + "\n";
        string str2 = "Date Joined : ";
        dateTime = DateTime.Now - TimeSpan.FromDays(Utils.random.NextDouble() * 200.0);
        string str3 = dateTime.ToString().Replace('/', '-').Replace(' ', '_');
        string str4 = "\n";
        this.usersFolder.files.Add(new FileEntry(str1 + str2 + str3 + str4 + "Status : " + this.generateUserState(People.hubAgents[index].handle) + "\n" + "Rank : " + this.generateUserRank(People.hubAgents[index].handle) + "\n", People.hubAgents[index].handle + "#" + (object) num2));
      }
      num1 = 0;
      int num3;
      do
      {
        num3 = Utils.random.Next(9999);
      }
      while (intList.Contains(num3));
      string str5 = "USER: " + (object) num3 + "\n" + "Handle: Bit\n";
      string str6 = "Date Joined : ";
      dateTime = DateTime.Now - TimeSpan.FromDays(411.0);
      string str7 = dateTime.ToString().Replace('/', '-').Replace(' ', '_');
      string str8 = "\n";
      this.usersFolder.files.Insert(0, new FileEntry(str5 + str6 + str7 + str8 + "Status : " + this.generateUserState("Bit") + "\n" + "Rank : " + this.generateUserRank("Bit") + "\n", "Bit#" + (object) num3));
    }

    public void addUser(UserDetail newUser)
    {
      int num = Utils.random.Next(9999);
      this.usersFolder.files.Add(new FileEntry("USER: " + (object) num + "\n" + "Handle: " + newUser.name + "\n" + "Date Joined : " + DateTime.Now.ToString().Replace('/', '-').Replace(' ', '_') + "\n" + "Status : Active\n" + "Rank : " + (object) 0 + "\n", newUser.name + "#" + (object) num));
    }

    private string generateUserRank(string username)
    {
      switch (username)
      {
        case "Bit":
          return "2116";
        default:
          return string.Concat((object) (int) (Utils.random.NextDouble() * 2500.0));
      }
    }

    private string generateUserState(string username)
    {
      switch (username)
      {
        case "Bit":
          return "UNKNOWN";
        default:
          return username.GetHashCode() < 1073741823 ? "Active" : "Passive";
      }
    }

    public int GetNumberOfAvaliableMissions()
    {
      return this.listingMissions.Count;
    }

    private void CheckForGameStateIssuesAndFix()
    {
      if (Settings.IsInExtensionMode)
        return;
      if (this.os.currentMission == null)
      {
        if (this.os.Flags.HasFlag("bitPathStarted") && !this.os.Flags.HasFlag("Victory"))
        {
          bool flag = false;
          foreach (KeyValuePair<string, ActiveMission> listingMission in this.listingMissions)
          {
            if (listingMission.Value.reloadGoalsSourceFile.Contains("BitPath/BitAdv_Recovery.xml"))
            {
              flag = true;
              break;
            }
          }
          if (!flag)
            this.AddMissionToListings("Content/Missions/BitPath/BitAdv_Recovery.xml", -1);
        }
        if (DLC1SessionUpgrader.HasDLC1Installed && !this.os.Flags.HasFlag("dlc_complete"))
        {
          bool flag = false;
          foreach (KeyValuePair<string, ActiveMission> listingMission in this.listingMissions)
          {
            if (listingMission.Value.reloadGoalsSourceFile.Contains("CSEC_DLCConnectorIntro.xml"))
            {
              flag = true;
              break;
            }
          }
          if (!flag)
            this.AddMissionToListings("Content/DLC/Missions/BaseGameConnectors/Missions/CSEC_DLCConnectorIntro.xml", -1);
        }
      }
      if (this.GetNumberOfAvaliableMissions() == 0 && this.os.currentMission == null && !this.os.Flags.HasFlag("bitPathStarted"))
      {
        foreach (KeyValuePair<string, Faction> faction in this.os.allFactions.factions)
        {
          if (faction.Value.idName == "hub")
          {
            HubFaction hubFaction = faction.Value as HubFaction;
            if (hubFaction != null)
              hubFaction.ForceStartBitMissions((object) this.os);
          }
        }
      }
      int num1 = 0;
      int num2 = 0;
      string str = (string) null;
      foreach (KeyValuePair<string, ActiveMission> listingMission in this.listingMissions)
      {
        bool flag = true;
        if (listingMission.Value.reloadGoalsSourceFile.Contains("DLC"))
          flag = false;
        if (listingMission.Value.postingAcceptFlagRequirements != null)
        {
          for (int index = 0; index < listingMission.Value.postingAcceptFlagRequirements.Length; ++index)
          {
            if (!this.os.Flags.HasFlag(listingMission.Value.postingAcceptFlagRequirements[index]))
            {
              flag = false;
              if (str == null)
                str = listingMission.Key;
            }
          }
        }
        if (flag)
          ++num1;
        else
          ++num2;
      }
      if (num1 <= 0 && num2 > 0 && (this.os.currentMission == null && !this.os.Flags.HasFlag("bitPathStarted")))
      {
        foreach (KeyValuePair<string, Faction> faction in this.os.allFactions.factions)
        {
          if (faction.Value.idName == "hub")
          {
            HubFaction hubFaction = faction.Value as HubFaction;
            if (hubFaction != null)
              hubFaction.ForceStartBitMissions((object) this.os);
          }
        }
      }
    }

    private string getIDStringForContractFile(FileEntry file)
    {
      int num1 = file.name.IndexOf('#');
      if (num1 <= -1)
        return "";
      int num2 = num1 + 1;
      return file.name.Substring(file.name.IndexOf('#') + 1);
    }

    public override void navigatedTo()
    {
      base.navigatedTo();
      this.screenTransition = 1f;
      this.state = MissionHubServer.HubState.Welcome;
      this.missionListPageNumber = 0;
      if (this.thinBarcodeTop != null)
        this.thinBarcodeTop.regenerate();
      if (this.thinBarcodeBot != null)
        this.thinBarcodeBot.regenerate();
      this.CheckForGameStateIssuesAndFix();
    }

    public override void loginGoBack()
    {
      base.loginGoBack();
      this.state = MissionHubServer.HubState.Welcome;
      this.screenTransition = 1f;
    }

    public override void userLoggedIn()
    {
      base.userLoggedIn();
      this.activeUserName = this.user.name;
      this.state = MissionHubServer.HubState.Menu;
      this.screenTransition = 1f;
      this.activeUserLoginTime = DateTime.Now;
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      this.updateScreenTransition();
      switch (this.state)
      {
        case MissionHubServer.HubState.Welcome:
          this.doBarcodeEffect(bounds, sb);
          this.drawWelcomeScreen(bounds, sb);
          break;
        case MissionHubServer.HubState.Menu:
          this.doMenuScreen(bounds, sb);
          this.doLoggedInScreenDetailing(bounds, sb);
          break;
        case MissionHubServer.HubState.Login:
          this.doBarcodeEffect(bounds, sb);
          this.doLoginDisplay(bounds, sb);
          break;
        case MissionHubServer.HubState.Listing:
          this.doListingScreen(bounds, sb);
          break;
        case MissionHubServer.HubState.ContractPreview:
          this.doLoggedInScreenDetailing(bounds, sb);
          this.doContractPreviewScreen(bounds, sb);
          break;
        case MissionHubServer.HubState.UserList:
          this.doLoggedInScreenDetailing(bounds, sb);
          this.doUserListScreen(bounds, sb);
          break;
        case MissionHubServer.HubState.CancelContract:
          this.doCancelContractScreen(bounds, sb);
          this.doLoggedInScreenDetailing(bounds, sb);
          break;
      }
    }

    private void updateScreenTransition()
    {
      this.screenTransition -= (float) (this.os.lastGameTime.ElapsedGameTime.TotalSeconds / 0.300000011920929);
      this.screenTransition = Math.Max(this.screenTransition, 0.0f);
    }

    private int getTransitionOffset(int position)
    {
      return (int) (Math.Pow(Math.Min((double) this.screenTransition + (double) position * 0.1, 1.0), 1.0) * 40.0 * (double) this.screenTransition);
    }

    private void doMenuScreen(Rectangle bounds, SpriteBatch sb)
    {
      Rectangle rectangle = new Rectangle(bounds.X + 10, bounds.Y + bounds.Height / 3 + 10, bounds.Width / 2, 40);
      if (Button.doButton(101010, rectangle.X + this.getTransitionOffset(0), rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("Contract Listing"), new Color?(this.themeColor)))
      {
        this.state = MissionHubServer.HubState.Listing;
        this.screenTransition = 1f;
      }
      rectangle.Y += rectangle.Height + 5;
      if (Button.doButton(101015, rectangle.X + this.getTransitionOffset(1), rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("User List"), new Color?(this.themeColor)))
      {
        this.state = MissionHubServer.HubState.UserList;
        this.screenTransition = 1f;
      }
      rectangle.Y += rectangle.Height + 5;
      if (this.allowAbandon)
      {
        if (Button.doButton(101017, rectangle.X + this.getTransitionOffset(1), rectangle.Y, rectangle.Width, rectangle.Height / 2, LocaleTerms.Loc("Abort Current Contract"), new Color?(this.os.currentMission == null ? Color.Black : this.themeColor)) && this.os.currentMission != null)
        {
          this.state = MissionHubServer.HubState.CancelContract;
          this.screenTransition = 1f;
        }
        rectangle.Y += rectangle.Height / 2 + 5;
      }
      if (Button.doButton(102015, rectangle.X + this.getTransitionOffset(3), rectangle.Y, rectangle.Width, rectangle.Height / 2, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
        this.os.display.command = "connect";
      rectangle.Y += rectangle.Height + 5;
    }

    private void doCancelContractScreen(Rectangle bounds, SpriteBatch sb)
    {
      Rectangle rectangle = new Rectangle(bounds.X + 10, bounds.Y + bounds.Height / 3 + 10, bounds.Width / 2, 40);
      TextItem.doFontLabel(new Vector2((float) rectangle.X, (float) rectangle.Y), LocaleTerms.Loc("Are you sure you with to abandon your current contract?") + "\n" + LocaleTerms.Loc("This cannot be reversed."), GuiData.font, new Color?(Color.White), (float) (bounds.Width - 30), (float) rectangle.Height, false);
      rectangle.Y += rectangle.Height + 4;
      if (Button.doButton(142011, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("Abandon Contract"), new Color?(Color.Red)) && this.os.currentMission != null)
      {
        this.os.currentMission = (ActiveMission) null;
        this.os.currentFaction.contractAbbandoned((object) this.os);
        this.screenTransition = 0.0f;
        this.state = MissionHubServer.HubState.Menu;
        this.CheckForGameStateIssuesAndFix();
      }
      rectangle.Y += rectangle.Height + 10;
      if (!Button.doButton(142015, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("Back"), new Color?(this.themeColor)))
        return;
      this.screenTransition = 1f;
      this.state = MissionHubServer.HubState.Menu;
    }

    private void doListingScreen(Rectangle bounds, SpriteBatch sb)
    {
      bool drawShadow = TextItem.DrawShadow;
      TextItem.DrawShadow = false;
      Rectangle rectangle1 = this.doListingScreenBackground(bounds, sb);
      Rectangle rectangle2 = new Rectangle(bounds.X + 10, rectangle1.Y, bounds.Width / 2, 30);
      int height = 36;
      for (int index = this.missionListPageNumber * this.missionListDisplayed; index < this.listingsFolder.files.Count && rectangle2.Y + height < rectangle1.Y + rectangle1.Height; ++index)
      {
        string stringForContractFile = this.getIDStringForContractFile(this.listingsFolder.files[index]);
        if (this.listingMissions.ContainsKey(stringForContractFile))
        {
          ActiveMission listingMission = this.listingMissions[stringForContractFile];
          if (listingMission == null)
          {
            Console.WriteLine("Mission: " + stringForContractFile + " is null! There is an error in one of your mission files preventing draw");
          }
          else
          {
            this.drawMissionEntry(new Rectangle(bounds.X + 1, rectangle2.Y, bounds.Width - 2, height), sb, listingMission, index);
            rectangle2.Y += height;
          }
        }
      }
      int num = 0;
      rectangle2 = new Rectangle(bounds.X + 10, rectangle1.Y, bounds.Width / 2, 30);
      while (rectangle2.Y + height < rectangle1.Y + rectangle1.Height)
      {
        rectangle2.Y += height;
        ++num;
      }
      this.missionListDisplayed = num;
      TextItem.DrawShadow = drawShadow;
    }

    private void drawMissionEntry(Rectangle bounds, SpriteBatch sb, ActiveMission mission, int index)
    {
      bool flag = false;
      if (mission.postingAcceptFlagRequirements != null)
      {
        for (int index1 = 0; index1 < mission.postingAcceptFlagRequirements.Length; ++index1)
        {
          if (!this.os.Flags.HasFlag(mission.postingAcceptFlagRequirements[index1]))
            flag = true;
        }
      }
      if (this.os.currentFaction != null && this.os.currentFaction.playerValue < mission.requiredRank)
        flag = true;
      int myID = index * 139284 + 984275 + index;
      bool outlineOnly = Button.outlineOnly;
      bool drawingOutline = Button.drawingOutline;
      Button.outlineOnly = true;
      Button.drawingOutline = false;
      if (GuiData.active == myID)
        sb.Draw(Utils.white, bounds, Color.Black);
      else if (GuiData.hot == myID)
      {
        sb.Draw(Utils.white, bounds, this.themeColor * 0.12f);
      }
      else
      {
        Color color = index % 2 == 0 ? this.themeColorLine : this.themeColorBackground;
        if (flag)
          color = Color.Lerp(color, Color.Gray, 0.25f);
        sb.Draw(Utils.white, bounds, color);
      }
      if (mission.postingTitle.StartsWith("#"))
        PatternDrawer.draw(bounds, 1f, Color.Transparent, Color.Black * 0.6f, sb);
      if (flag)
      {
        Rectangle destinationRectangle = bounds;
        destinationRectangle.Height -= 6;
        destinationRectangle.Y += 3;
        destinationRectangle.X += bounds.Width - bounds.Height - 6;
        destinationRectangle.Width = destinationRectangle.Height;
        sb.Draw(this.lockIcon, destinationRectangle, Color.White * 0.2f);
      }
      if (!flag && Button.doButton(myID, bounds.X, bounds.Y, bounds.Width, bounds.Height, "", new Color?(Color.Transparent)))
      {
        this.selectedElementIndex = index;
        this.state = MissionHubServer.HubState.ContractPreview;
        this.screenTransition = 1f;
      }
      string text1 = mission.postingTitle.Replace("#", "") ?? "";
      TextItem.doFontLabel(new Vector2((float) (bounds.X + 1 + this.getTransitionOffset(index)), (float) (bounds.Y + 3)), text1, GuiData.smallfont, new Color?(Color.White), (float) bounds.Width, float.MaxValue, false);
      string text2 = "Target: " + mission.target + " -- Client: " + mission.client + " -- Key: " + (object) mission.generationKeys;
      TextItem.doFontLabel(new Vector2((float) (bounds.X + 1), (float) (bounds.Y + bounds.Height - 16)), text2, GuiData.detailfont, new Color?(Color.White * 0.3f), (float) bounds.Width, 13f, false);
      bounds.Y += bounds.Height - 1;
      bounds.Height = 1;
      sb.Draw(Utils.white, bounds, this.themeColor * 0.2f);
      Button.outlineOnly = outlineOnly;
      Button.drawingOutline = drawingOutline;
    }

    private Rectangle doListingScreenBackground(Rectangle bounds, SpriteBatch sb)
    {
      Rectangle destinationRectangle1 = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2);
      sb.Draw(Utils.white, destinationRectangle1, this.themeColorBackground);
      this.doLoggedInScreenDetailing(bounds, sb);
      destinationRectangle1.Height = 5;
      destinationRectangle1.Width = 0;
      destinationRectangle1.Y += 12;
      if (this.thinBarcodeTop == null)
        this.thinBarcodeTop = new ThinBarcode(bounds.Width - 4, 5);
      if (this.thinBarcodeBot == null)
        this.thinBarcodeBot = new ThinBarcode(bounds.Width - 4, 5);
      this.thinBarcodeTop.Draw(sb, destinationRectangle1.X, destinationRectangle1.Y, this.themeColor);
      TextItem.doFontLabel(new Vector2((float) (bounds.X + 20), (float) (destinationRectangle1.Y + 10)), "MISSION LISTING", GuiData.titlefont, new Color?(this.themeColor), (float) (bounds.Width - 40), 35f, false);
      destinationRectangle1.Y += 45;
      destinationRectangle1.X = bounds.X + 1;
      this.thinBarcodeBot.Draw(sb, destinationRectangle1.X, destinationRectangle1.Y, this.themeColor);
      destinationRectangle1.Y += 10;
      destinationRectangle1.X = bounds.X + 1;
      destinationRectangle1.Width = this.decorationPanel.Width;
      destinationRectangle1.Height = this.decorationPanel.Height;
      sb.Draw(this.decorationPanel, destinationRectangle1, this.themeColor);
      destinationRectangle1.X += destinationRectangle1.Width;
      destinationRectangle1.Width = bounds.Width - 2 - destinationRectangle1.Width;
      sb.Draw(this.decorationPanelSide, destinationRectangle1, this.themeColor);
      Vector2 pos = new Vector2((float) (bounds.X + 6), (float) (destinationRectangle1.Y + this.decorationPanel.Height / 2 - 8));
      int height = (int) ((double) this.decorationPanel.Height * 0.300000011920929);
      float num1 = (float) (((double) this.decorationPanel.Height - (double) height) / 2.0);
      Rectangle destinationRectangle2 = new Rectangle(bounds.X + 1, (int) ((double) destinationRectangle1.Y + (double) num1), this.decorationPanel.Width, height);
      Rectangle rectangle = new Rectangle(0, (int) (((double) this.decorationPanel.Height - (double) height) / 2.0), this.decorationPanel.Width, height);
      bool outlineOnly = Button.outlineOnly;
      bool drawingOutline = Button.drawingOutline;
      Button.outlineOnly = true;
      Button.drawingOutline = false;
      int myID = 974748322;
      Color color = GuiData.active == myID ? Color.Black : (GuiData.hot == myID ? this.themeColorLine : this.themeColorBackground);
      sb.Draw(this.decorationPanel, destinationRectangle2, new Rectangle?(rectangle), color, 0.0f, Vector2.Zero, SpriteEffects.None, 0.6f);
      if (Button.doButton(myID, bounds.X + 1, destinationRectangle1.Y + this.decorationPanel.Height / 6 + 4, this.decorationPanel.Width - 30, (int) ((double) this.decorationPanel.Height - (double) (this.decorationPanel.Height / 6) * 3.20000004768372), LocaleTerms.Loc("Back"), new Color?(Color.Transparent)))
      {
        this.screenTransition = 1f;
        this.state = MissionHubServer.HubState.Menu;
      }
      Button.outlineOnly = outlineOnly;
      Button.drawingOutline = drawingOutline;
      pos.X += (float) (this.decorationPanel.Width - 10);
      pos.Y += 4f;
      string text1 = LocaleTerms.Loc(this.groupName + " secure contract listing panel : Verified Connection : Token last verified") + " " + DateTime.Now.ToString();
      TextItem.doFontLabel(pos, text1, GuiData.detailfont, new Color?(this.themeColor), (float) destinationRectangle1.Width - 10f, 14f, false);
      pos.Y += 12f;
      if (this.missionListPageNumber > 0 && Button.doButton(188278101, (int) pos.X, (int) pos.Y, 45, 20, "<", new Color?()))
      {
        --this.missionListPageNumber;
        this.screenTransition = 1f;
      }
      destinationRectangle1.X += 50;
      int num2 = this.listingsFolder.files.Count / this.missionListDisplayed + 1;
      string text2 = (this.missionListPageNumber + 1).ToString() + "/" + (object) num2;
      float num3 = (float) (50.0 - (double) GuiData.smallfont.MeasureString(text2).X / 2.0);
      sb.DrawString(GuiData.smallfont, text2, new Vector2((float) destinationRectangle1.X + num3, (float) ((int) pos.Y + 1)), Color.White);
      destinationRectangle1.X += 100;
      if (this.missionListPageNumber < num2 - 1 && Button.doButton(188278102, destinationRectangle1.X, (int) pos.Y, 45, 20, ">", new Color?()))
      {
        ++this.missionListPageNumber;
        this.screenTransition = 1f;
      }
      destinationRectangle1.Y += this.decorationPanel.Height + 4;
      destinationRectangle1.Width = bounds.Width - 2;
      destinationRectangle1.X = bounds.X + 1;
      destinationRectangle1.Height = 7;
      sb.Draw(Utils.white, destinationRectangle1, this.themeColor);
      destinationRectangle1.Y += destinationRectangle1.Height;
      return new Rectangle(bounds.X, destinationRectangle1.Y, bounds.Width, bounds.Height - bounds.Height / 12 - (destinationRectangle1.Y - bounds.Y));
    }

    private void doContractPreviewScreen(Rectangle bounds, SpriteBatch sb)
    {
      string stringForContractFile = this.getIDStringForContractFile(this.listingsFolder.files[this.selectedElementIndex]);
      if (!this.listingMissions.ContainsKey(stringForContractFile))
        return;
      ActiveMission listingMission = this.listingMissions[stringForContractFile];
      Vector2 vector2 = new Vector2((float) (bounds.X + 20), (float) (bounds.Y + 20));
      TextItem.doFontLabel(vector2 + new Vector2((float) this.getTransitionOffset(0), 0.0f), "CONTRACT:" + stringForContractFile, GuiData.titlefont, new Color?(), (float) (bounds.Width / 2), 40f, false);
      vector2.Y += 40f;
      TextItem.doFontLabel(vector2 + new Vector2((float) this.getTransitionOffset(1), 0.0f), listingMission.postingTitle.Replace("#", ""), GuiData.font, new Color?(), (float) (bounds.Width - 30), float.MaxValue, false);
      vector2.Y += 30f;
      string str = DisplayModule.cleanSplitForWidth(listingMission.postingBody, bounds.Width - 110);
      StringBuilder stringBuilder = new StringBuilder();
      int num1 = (int) ((double) (((float) bounds.Width - 20f) / GuiData.smallfont.MeasureString("-").X) / 2.0);
      for (int index = 1; index < num1 - 5; ++index)
        stringBuilder.Append("-");
      string text = str.Replace("###", stringBuilder.ToString());
      if (LocaleActivator.ActiveLocaleIsCJK())
      {
        text = Utils.SuperSmartTwimForWidth(listingMission.postingBody, bounds.Width - 110, GuiData.smallfont);
        vector2.Y += 20f;
      }
      TextItem.doFontLabel(vector2 + new Vector2((float) this.getTransitionOffset(2), 0.0f), text, GuiData.smallfont, new Color?(), (float) (bounds.Width - 20), float.MaxValue, false);
      int num2 = Math.Max(135, bounds.Height / 6);
      if (Button.doButton(2171618, bounds.X + 20 + this.getTransitionOffset(3), bounds.Y + bounds.Height - num2, bounds.Width / 5, 30, LocaleTerms.Loc("Back"), new Color?()))
      {
        this.state = MissionHubServer.HubState.Listing;
        this.screenTransition = 1f;
      }
      if (this.os.currentMission == null)
      {
        if (Button.doButton(2171615, bounds.X + 20 + this.getTransitionOffset(4), bounds.Y + bounds.Height - num2 - 40, bounds.Width / 5, 30, LocaleTerms.Loc("Accept"), new Color?(this.os.highlightColor)))
        {
          this.acceptMission(listingMission, this.selectedElementIndex, stringForContractFile);
          this.state = MissionHubServer.HubState.Listing;
          this.screenTransition = 1f;
        }
      }
      else
        TextItem.doFontLabelToSize(new Rectangle(bounds.X + 20 + this.getTransitionOffset(4), bounds.Y + bounds.Height - num2 - 40, bounds.Width / 2, 30), LocaleTerms.Loc("Abort current contract to accept new ones."), GuiData.smallfont, Color.White, false, false);
    }

    private void doUserListScreen(Rectangle bounds, SpriteBatch sb)
    {
      if (Button.doButton(101801, bounds.X + 2, bounds.Y + 20, bounds.Width / 4, 22, LocaleTerms.Loc("Back"), new Color?(this.themeColor)))
        this.state = MissionHubServer.HubState.Menu;
      Rectangle rectangle = new Rectangle(bounds.X + 30, bounds.Y + 50, bounds.Width / 2, 30);
      int num1 = 18 + rectangle.Height - 8;
      int num2 = (bounds.Height - 90) / num1;
      int num3 = num2 * this.userListPageNumber;
      int num4 = 0;
      for (int index1 = num2 * this.userListPageNumber; index1 < this.usersFolder.files.Count && index1 < num3 + num2; ++index1)
      {
        string[] strArray = this.usersFolder.files[index1].data.Split(Utils.newlineDelim);
        string str1;
        string str2 = str1 = "";
        string str3 = str1;
        string str4 = str1;
        string str5 = str1;
        for (int index2 = 0; index2 < strArray.Length; ++index2)
        {
          if (strArray[index2].StartsWith("USER"))
            str5 = this.getDataFromConfigLine(strArray[index2], ": ");
          if (strArray[index2].StartsWith("Rank"))
            str2 = this.getDataFromConfigLine(strArray[index2], ": ");
          if (strArray[index2].StartsWith("Handle"))
            str4 = this.getDataFromConfigLine(strArray[index2], ": ");
          if (strArray[index2].StartsWith("Date"))
            str3 = this.getDataFromConfigLine(strArray[index2], ": ");
        }
        Rectangle destinationRectangle = new Rectangle(rectangle.X + (bounds.Width - 60) - 10, rectangle.Y + 2, rectangle.Height + 1, bounds.Width - 60);
        GuiData.spriteBatch.Draw(Utils.gradient, destinationRectangle, new Rectangle?(), this.themeColorLine, 1.570796f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.8f);
        TextItem.doSmallLabel(new Vector2((float) rectangle.X, (float) rectangle.Y), "#" + str5 + " - \"" + str4 + "\" Rank:" + str2, new Color?());
        rectangle.Y += 18;
        TextItem.doFontLabel(new Vector2((float) rectangle.X, (float) rectangle.Y), string.Format(LocaleTerms.Loc("Joined {0}"), (object) str3), GuiData.detailfont, new Color?(), float.MaxValue, float.MaxValue, false);
        rectangle.Y += rectangle.Height - 10;
        num4 = index1;
      }
      rectangle.Y += 16;
      if (this.userListPageNumber > 0 && Button.doButton(101005, rectangle.X, rectangle.Y, rectangle.Width / 2 - 20, 15, LocaleTerms.Loc("Previous Page"), new Color?()))
        --this.userListPageNumber;
      TextItem.doTinyLabel(new Vector2((float) (rectangle.X + rectangle.Width / 2 - 8), (float) rectangle.Y), string.Concat((object) this.userListPageNumber), new Color?());
      if (this.usersFolder.files.Count <= num4 + 1 || !Button.doButton(101010, rectangle.X + rectangle.Width / 2 + 10, rectangle.Y, rectangle.Width / 2 - 10, 15, LocaleTerms.Loc("Next Page"), new Color?()))
        return;
      ++this.userListPageNumber;
    }

    private void acceptMission(ActiveMission mission, int index, string id)
    {
      this.os.currentMission = mission;
      ActiveMission activeMission = (ActiveMission) ComputerLoader.readMission(mission.reloadGoalsSourceFile);
      mission.sendEmail(this.os);
      mission.ActivateSuppressedStartFunctionIfPresent();
      FileEntry file = this.listingsFolder.files[index];
      this.listingsFolder.files.RemoveAt(index);
      this.listingArchivesFolder.files.Add(new FileEntry("Contract Archive:\nAccepted : " + DateTime.Now.ToString() + "\nUser : " + this.activeUserName + "\nActive Since : " + this.activeUserLoginTime.ToString() + "\n\n" + file.data, "Contract#" + id + "Archive"));
      this.listingMissions.Remove(id);
    }

    private void drawWelcomeScreen(Rectangle bounds, SpriteBatch sb)
    {
      Rectangle rectangle = new Rectangle(bounds.X + 10, bounds.Y + bounds.Height / 3 - 10, bounds.Width / 2, 30);
      string upper = string.Format("{0} Contract Hub", (object) this.groupName).ToUpper();
      TextItem.doFontLabel(new Vector2((float) rectangle.X, (float) rectangle.Y), upper, GuiData.titlefont, new Color?(), (float) bounds.Width / 0.6f, 50f, false);
      rectangle.Y += 50;
      if (Button.doButton(11005, rectangle.X + this.getTransitionOffset(0), rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("Login"), new Color?(this.themeColor)))
      {
        this.startLogin();
        this.state = MissionHubServer.HubState.Login;
      }
      rectangle.Y += rectangle.Height + 5;
      if (!Button.doButton(12010, rectangle.X + this.getTransitionOffset(1), rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("Exit"), new Color?(this.themeColor)))
        return;
      this.os.display.command = "connect";
    }

    private void doBarcodeEffect(Rectangle bounds, SpriteBatch sb)
    {
      if (this.barcode == null || this.barcode.maxWidth != bounds.Width - 2 || this.barcode.leftRightBias)
        this.barcode = new BarcodeEffect(bounds.Width - 2, true, false);
      this.barcode.Update((float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
      this.barcode.Draw(bounds.X + 1, bounds.Y + 5 * (bounds.Height / 6) - 1, bounds.Width - 2, bounds.Height / 6, sb, new Color?(this.themeColor));
      this.barcode.isInverted = false;
      this.barcode.Draw(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height / 6, sb, new Color?(this.themeColor));
      this.barcode.isInverted = true;
    }

    private void doLoggedInScreenDetailing(Rectangle bounds, SpriteBatch sb)
    {
      string text = LocaleTerms.Loc("Authenticated User:") + " " + this.activeUserName + " -- Token Active Since: " + this.activeUserLoginTime.ToString();
      TextItem.doFontLabel(new Vector2((float) (bounds.X + 1), (float) (bounds.Y + 1)), text, GuiData.detailfont, new Color?(), (float) (bounds.Width - 2), float.MaxValue, false);
      this.doBaseBarcodeEffect(bounds, sb);
    }

    private void doBaseBarcodeEffect(Rectangle bounds, SpriteBatch sb)
    {
      if (this.barcode == null || this.barcode.maxWidth != bounds.Width - 2 || !this.barcode.leftRightBias)
        this.barcode = new BarcodeEffect(bounds.Width - 2, true, true);
      this.barcode.Update((float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
      int y = bounds.Y + 11 * (bounds.Height / 12) - 1;
      this.barcode.Draw(bounds.X + 1, y, bounds.Width - 2, bounds.Height / 12, sb, new Color?(this.themeColor));
    }

    private enum HubState
    {
      Welcome,
      Menu,
      Login,
      Listing,
      ContractPreview,
      UserList,
      CancelContract,
    }
  }
}
