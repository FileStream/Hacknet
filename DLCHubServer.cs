// Decompiled with JetBrains decompiler
// Type: Hacknet.DLCHubServer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Daemons.Helpers;
using Hacknet.Effects;
using Hacknet.Gui;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hacknet
{
  internal class DLCHubServer : Daemon, IMonitorableDaemon
  {
    public List<string> Agents = new List<string>();
    public Color themeColor = Color.MediumVioletRed;
    private string groupName = "Bibliotheca";
    public bool AddsFactionPointForMissionCompleteion = true;
    public bool AutoClearMissionsOnSingleComplete = true;
    public bool AllowContractAbbandon = false;
    private DLCHubServer.DHSState State = DLCHubServer.DHSState.Welcome;
    private DLCHubServer.ClaimableMission SelectedMission = (DLCHubServer.ClaimableMission) null;
    private bool isAddingTextResponse = false;
    private List<string> MissionTextResponses = new List<string>();
    private string inProgressTextResponse = (string) null;
    private float BaseWelcomeFadeoutTime = 4f;
    private float WelcomeFadeoutTimerLeft = 4f;
    private Dictionary<string, Color> HighlightedWords = new Dictionary<string, Color>();
    internal List<DLCHubServer.ClaimableMission> ActiveMissions = new List<DLCHubServer.ClaimableMission>(3);
    private int UIButtonOffset = 0;
    private bool ShouldShowMissionIncompleteMessage = false;
    private bool AbandonMissionShowConfirmation = false;
    private float timeSpentInLoading = 0.0f;
    private bool HasStartedWoosh = false;
    public const string ROOT_FOLDERNAME = "HomeBase";
    public const string MISSION_FOLDERNAME = "contracts";
    public const string ARCHIVE_FOLDERNAME = "archive";
    public const string ACTIONS_FOLDERNAME = "runtime";
    public const string CONFIG_FILENAME = "dhs_config.sys";
    public DelayableActionSystem DelayedActions;
    private Folder rootFolder;
    private Folder missionFolder;
    private Folder archivesFolder;
    private Folder actionsFolder;
    public IRCSystem IRCSystem;
    private SoundEffect ButtonPressSound;
    private SoundEffect WooshBuildup;
    private SoundEffect USIntro;
    private Texture2D MissionAvaliableIcon;
    private Texture2D MissionTakenIcon;
    private Texture2D MissionPlayersIcon;
    private Texture2D LoadingSpinner;
    private ScrollableTextRegion ScrollableTextPanel;
    private HexGridBackground HexBackground;

    public DLCHubServer(Computer c, string serviceName, string group, OS _os)
      : base(c, serviceName, _os)
    {
      this.groupName = group;
      this.MissionAvaliableIcon = this.os.content.Load<Texture2D>("DLC/Icons/focus_icon");
      this.MissionTakenIcon = this.os.content.Load<Texture2D>("DLC/Icons/cross_icon");
      this.MissionPlayersIcon = this.os.content.Load<Texture2D>("DLC/Icons/focus_icon");
      this.LoadingSpinner = this.os.content.Load<Texture2D>("Sprites/Spinner");
      this.ScrollableTextPanel = new ScrollableTextRegion(GuiData.spriteBatch.GraphicsDevice);
      this.HexBackground = new HexGridBackground(this.os.content);
      this.ButtonPressSound = this.os.content.Load<SoundEffect>("SFX/Bip");
      this.WooshBuildup = this.os.content.Load<SoundEffect>("DLC/SFX/Kilmer_Woosh");
      this.USIntro = this.os.content.Load<SoundEffect>("DLC/SFX/UserspaceIntro");
      this.InitDefaults();
    }

    public override void initFiles()
    {
      this.initFilesystem();
    }

    private void InitDefaults()
    {
      this.Agents.Add("Channel");
      this.HighlightedWords.Add("Channel", new Color(240, 234, 81));
    }

    public void initFilesystem()
    {
      this.rootFolder = new Folder("HomeBase");
      this.missionFolder = new Folder("contracts");
      this.rootFolder.folders.Add(this.missionFolder);
      this.archivesFolder = new Folder("archive");
      this.rootFolder.folders.Add(this.archivesFolder);
      this.actionsFolder = new Folder("runtime");
      this.rootFolder.folders.Add(this.actionsFolder);
      this.DelayedActions = new DelayableActionSystem(this.actionsFolder, (object) this.os);
      this.rootFolder.files.Add(new FileEntry(Computer.generateBinaryString(512), "runtime.dll"));
      this.ReGenerateConfigFile();
      this.IRCSystem = new IRCSystem(this.rootFolder);
      this.IRCSystem.AttachmentPressedSound = this.ButtonPressSound;
      this.InitTests();
      this.comp.files.root.folders.Add(this.rootFolder);
    }

    private void InitTests()
    {
    }

    public override void navigatedTo()
    {
      base.navigatedTo();
      this.ReadActiveMissions();
      this.State = this.os.Flags.HasFlag("DLC_Player_IRC_Authenticated") ? DLCHubServer.DHSState.Home : DLCHubServer.DHSState.Welcome;
      this.ShouldShowMissionIncompleteMessage = false;
      if (!(this.comp.idName == "dhs"))
        return;
      if (this.ActiveMissions.Count == 0 && !this.os.Flags.HasFlag("dlc_complete"))
        MissionFunctions.runCommand(1, "addRankSilent");
      Computer computer = Programs.getComputer(this.os, "dhsDrop");
      if (computer != null)
      {
        Folder folder = computer.files.root.searchForFolder("bin");
        bool flag = false;
        for (int index = 0; index < folder.files.Count; ++index)
        {
          if (folder.files[index].data == PortExploits.crackExeData[13])
            flag = true;
        }
        if (!flag)
          folder.files.Add(new FileEntry(PortExploits.crackExeData[13], PortExploits.cracks[13]));
      }
    }

    public void SubscribeToAlertActionFroNewMessage(Action<string, string> act)
    {
      this.IRCSystem.LogAdded += act;
    }

    public void UnSubscribeToAlertActionFroNewMessage(Action<string, string> act)
    {
      this.IRCSystem.LogAdded -= act;
    }

    public string GetName()
    {
      return this.name;
    }

    private void ReGenerateConfigFile()
    {
      FileEntry fileEntry = this.rootFolder.searchForFile("dhs_config.sys");
      if (fileEntry != null)
        this.rootFolder.files.Remove(fileEntry);
      this.rootFolder.files.Add(this.generateConfigFile());
    }

    private FileEntry generateConfigFile()
    {
      return new FileEntry("##DHS_CONFIG\n" + "ThemeColor = " + Utils.convertColorToParseableString(this.themeColor) + "\n" + "ServiceName = " + this.name + "\n" + "GroupName = " + this.groupName + "\n" + "Agents = " + Utils.SerializeListToCSV(this.Agents) + "\n" + "FactionReferral = " + (object) this.AddsFactionPointForMissionCompleteion + "\n" + "AutoClearMissions = " + (object) this.AutoClearMissionsOnSingleComplete + "\n" + "HighlightedWords = " + this.GetSerializedWordHighlightList() + "\n" + "AllowContractAbbandon = " + (object) this.AllowContractAbbandon + "\n" + "\n", "dhs_config.sys");
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
          else if (line.StartsWith("Agents"))
            this.Agents = new List<string>((IEnumerable<string>) this.getDataFromConfigLine(line, "= ").Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries));
          else if (line.StartsWith("FactionReferral"))
            this.AddsFactionPointForMissionCompleteion = this.getDataFromConfigLine(line, "= ").ToLower() == "true";
          else if (line.StartsWith("AutoClearMissions"))
            this.AutoClearMissionsOnSingleComplete = this.getDataFromConfigLine(line, "= ").ToLower() == "true";
          else if (line.StartsWith("HighlightedWords"))
            this.DeserializeWordHighlightList(this.getDataFromConfigLine(line, "= "));
          else if (line.StartsWith("AllowContractAbbandon"))
            this.AllowContractAbbandon = this.getDataFromConfigLine(line, "= ").ToLower() == "true";
        }
      }
    }

    private string getDataFromConfigLine(string line, string sentinel = "= ")
    {
      return line.Substring(line.IndexOf(sentinel) + 2);
    }

    private string GetSerializedWordHighlightList()
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<string, Color> highlightedWord in this.HighlightedWords)
      {
        stringBuilder.Append(highlightedWord.Key + "=" + Utils.convertColorToParseableString(highlightedWord.Value));
        stringBuilder.Append("/");
      }
      return stringBuilder.ToString();
    }

    private void DeserializeWordHighlightList(string input)
    {
      this.HighlightedWords = new Dictionary<string, Color>();
      string str1 = input;
      char[] separator1 = new char[1]{ '/' };
      int num1 = 1;
      foreach (string str2 in str1.Split(separator1, (StringSplitOptions) num1))
      {
        char[] separator2 = new char[1]{ '=' };
        int num2 = 1;
        string[] strArray = str2.Split(separator2, (StringSplitOptions) num2);
        this.HighlightedWords.Add(strArray[0], Utils.convertStringToColor(strArray[1]));
      }
    }

    public override void loadInit()
    {
      base.loadInit();
      this.rootFolder = this.comp.files.root.searchForFolder("HomeBase");
      this.missionFolder = this.rootFolder.searchForFolder("contracts");
      this.archivesFolder = this.rootFolder.searchForFolder("archive");
      this.actionsFolder = this.rootFolder.searchForFolder("runtime");
      this.DelayedActions = new DelayableActionSystem(this.actionsFolder, (object) this.os);
      FileEntry fileEntry = this.rootFolder.searchForFile("dhs_config.sys");
      if (fileEntry != null)
        this.loadFromConfigFileData(fileEntry.data);
      this.IRCSystem = new IRCSystem(this.rootFolder);
      this.IRCSystem.AttachmentPressedSound = this.ButtonPressSound;
    }

    public override string getSaveString()
    {
      return "<DHSDaemon />";
    }

    public void AddAgent(string AgentName, string agentPassword, Color color)
    {
      AgentName = AgentName.Replace(" ", "_").Replace("/", "").Replace("=", "_");
      if (AgentName.Contains<char>(' ') || AgentName.Contains<char>('/') || AgentName.Contains<char>('=') || string.IsNullOrWhiteSpace(AgentName))
        throw new InvalidOperationException("Invalid Agent Name \"" + AgentName + "\"!");
      if (this.Agents.Contains(AgentName))
      {
        this.HighlightedWords[AgentName] = color;
      }
      else
      {
        this.Agents.Add(AgentName);
        this.HighlightedWords.Add(AgentName, color);
      }
      if (this.rootFolder != null)
        this.ReGenerateConfigFile();
      bool flag = false;
      for (int index = 0; index < this.comp.users.Count; ++index)
        flag |= this.comp.users[index].name == AgentName;
      if (flag)
        return;
      this.comp.users.Add(new UserDetail(AgentName, agentPassword, (byte) 3));
    }

    public void AddMission(string missionPath, string AgentClaimName = null, bool startsComplete = false)
    {
      List<ActiveMission> branchMissions = this.os.branchMissions;
      ActiveMission mission = (ActiveMission) ComputerLoader.readMission(missionPath);
      this.os.branchMissions = branchMissions;
      this.AddMission(mission, AgentClaimName, startsComplete);
    }

    public void AddMission(ActiveMission mission, string AgentClaimName = null, bool startsComplete = false)
    {
      this.ActiveMissions.Add(new DLCHubServer.ClaimableMission()
      {
        Mission = mission,
        IsComplete = startsComplete,
        AgentClaim = AgentClaimName
      });
      this.ReSerializeActiveMissions();
    }

    public void RemoveMission(string missionPath)
    {
      for (int index = 0; index < this.ActiveMissions.Count; ++index)
      {
        if (this.ActiveMissions[index].Mission.reloadGoalsSourceFile == LocalizedFileLoader.GetLocalizedFilepath(missionPath))
        {
          this.ActiveMissions.RemoveAt(index);
          --index;
        }
      }
      this.ReSerializeActiveMissions();
    }

    private void ReadActiveMissions()
    {
      this.ActiveMissions.Clear();
      for (int index = 0; index < this.missionFolder.files.Count; ++index)
      {
        if (this.missionFolder.files[index].name.EndsWith(".ctc"))
        {
          int contractRegistryNumber = 0;
          string Tag = (string) null;
          try
          {
            ActiveMission activeMission = (ActiveMission) MissionSerializer.restoreMissionFromFile(this.missionFolder.files[index].data, out contractRegistryNumber, out Tag);
            DLCHubServer.ClaimableMission claimableMission = new DLCHubServer.ClaimableMission() { AgentClaim = Tag, Mission = activeMission, IsComplete = contractRegistryNumber >= 1 };
            this.ActiveMissions.Add(claimableMission);
            if (this.SelectedMission == null && claimableMission.AgentClaim == this.os.defaultUser.name)
              this.SelectedMission = claimableMission;
          }
          catch (Exception ex)
          {
          }
        }
      }
    }

    private void ReSerializeActiveMissions()
    {
      this.missionFolder.files.Clear();
      for (int index = 0; index < this.ActiveMissions.Count; ++index)
        this.missionFolder.files.Add(new FileEntry(MissionSerializer.generateMissionFile((object) this.ActiveMissions[index].Mission, this.ActiveMissions[index].IsComplete ? 1 : 0, this.groupName, this.ActiveMissions[index].AgentClaim), this.GetFilenameForMission(this.ActiveMissions[index].Mission)));
    }

    private string GetFilenameForMission(ActiveMission mission)
    {
      if (mission.client == null || mission.client == "UNKNOWN")
        mission.client = string.Concat((object) Utils.getRandomByte());
      return Utils.GetNonRepeatingFilename("Contact_" + mission.client, ".ctc", this.missionFolder);
    }

    private bool PlayerHasClaimedMission()
    {
      for (int index = 0; index < this.ActiveMissions.Count; ++index)
      {
        if (this.ActiveMissions[index].AgentClaim == this.os.defaultUser.name)
          return true;
      }
      return false;
    }

    private void PlayerAcceptMission(DLCHubServer.ClaimableMission mission)
    {
      mission.AgentClaim = this.os.defaultUser.name;
      this.os.currentMission = mission.Mission;
      mission.Mission.ActivateSuppressedStartFunctionIfPresent();
      ActiveMission activeMission = (ActiveMission) ComputerLoader.readMission(mission.Mission.reloadGoalsSourceFile);
      this.IRCSystem.AddLog("Channel", string.Format(LocaleTerms.Loc("CONTRACT CLAIMED: @{0} claimed contract \"{1}\""), (object) this.os.defaultUser.name, (object) mission.Mission.postingTitle), (string) null);
      this.ReSerializeActiveMissions();
    }

    private void PlayerAbandonedMission(DLCHubServer.ClaimableMission mission)
    {
    }

    private bool PlayerAttemptCompleteMission(DLCHubServer.ClaimableMission mission, bool ForceComplete = false)
    {
      ForceComplete = ForceComplete && Settings.forceCompleteEnabled;
      List<DLCHubServer.ClaimableMission> missionsToArchive = new List<DLCHubServer.ClaimableMission>();
      for (int index = 0; index < this.ActiveMissions.Count; ++index)
        missionsToArchive.Add(this.ActiveMissions[index]);
      this.ActiveMissions.Clear();
      this.missionFolder.files.Clear();
      if (ForceComplete || this.os.currentMission.isComplete(this.MissionTextResponses))
      {
        ActiveMission currentMission = this.os.currentMission;
        this.os.currentMission = (ActiveMission) null;
        if (currentMission.endFunctionName != null)
          MissionFunctions.runCommand(currentMission.endFunctionValue, currentMission.endFunctionName);
        if (this.AddsFactionPointForMissionCompleteion)
          MissionFunctions.runCommand(1, "addRankSilent");
        this.IRCSystem.AddLog("Channel", string.Format(LocaleTerms.Loc("CONTRACT COMPLETE: @{0} completed contract \"{1}\""), (object) this.os.defaultUser.name, (object) mission.Mission.postingTitle), (string) null);
        if (this.MissionTextResponses.Count > 0)
        {
          this.IRCSystem.AddLog("Channel", LocaleTerms.Loc("Additional details provided:"), (string) null);
          for (int index = 0; index < this.MissionTextResponses.Count; ++index)
            this.IRCSystem.AddLog("Channel", "[" + this.MissionTextResponses[index] + "]", (string) null);
        }
        this.MissionTextResponses.Clear();
        mission.IsComplete = true;
        if (this.AutoClearMissionsOnSingleComplete)
          this.CompleteAndArchiveMissionSet(missionsToArchive);
        this.os.saveGame();
        return true;
      }
      this.ActiveMissions = missionsToArchive;
      this.ReSerializeActiveMissions();
      this.MissionTextResponses.Clear();
      return false;
    }

    private void CompleteAndArchiveMissionSet(List<DLCHubServer.ClaimableMission> missionsToArchive)
    {
      for (int index = 0; index < missionsToArchive.Count; ++index)
      {
        DLCHubServer.ClaimableMission claimableMission = missionsToArchive[index];
        claimableMission.IsComplete = true;
        this.archivesFolder.files.Add(new FileEntry(MissionSerializer.generateMissionFile((object) claimableMission.Mission, claimableMission.IsComplete ? 1 : 0, this.groupName, claimableMission.AgentClaim), this.GetFilenameForMission(claimableMission.Mission)));
      }
    }

    public void ClearAllActiveMissions()
    {
      this.CompleteAndArchiveMissionSet(this.ActiveMissions);
      this.ActiveMissions.Clear();
    }

    private void Update()
    {
      this.os.delayer.Post(ActionDelayer.Wait(this.os.lastGameTime.ElapsedGameTime.TotalSeconds * 1.999), (Action) (() =>
      {
        if (!(this.os.display.command != this.name))
          return;
        this.IRCSystem.LeftView();
      }));
    }

    public bool ShouldDisplayNotifications()
    {
      return this.os.Flags.HasFlag("DLC_Player_IRC_Authenticated") || (double) this.WelcomeFadeoutTimerLeft <= 0.0;
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      this.UIButtonOffset = 0;
      this.Update();
      Rectangle rectangle1 = Utils.InsetRectangle(bounds, 2);
      Rectangle rectangle2 = new Rectangle(rectangle1.X, rectangle1.Y + 10, rectangle1.Width, rectangle1.Height - 10);
      this.HexBackground.Update((float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
      sb.Draw(Utils.white, rectangle2, Color.Black * 0.2f);
      this.HexBackground.Draw(rectangle2, sb, Color.Black * 0.1f, this.themeColor * 0.02f, HexGridBackground.ColoringAlgorithm.CorrectedSinWash, 0.0f);
      int height = 30;
      Rectangle bounds1 = new Rectangle(rectangle1.X, rectangle1.Y + 2 + height, rectangle1.Width, rectangle1.Height - height - 2);
      this.DrawOptionsPanel(new Rectangle(rectangle1.X, rectangle1.Y, rectangle1.Width, height), sb);
      switch (this.State)
      {
        case DLCHubServer.DHSState.Login:
          break;
        case DLCHubServer.DHSState.Home:
          this.DrawHomeScreen(bounds1, sb);
          if (this.os.Flags.HasFlag("DLC_Player_IRC_Authenticated"))
            break;
          this.DoLoadingPlayerInScreen(bounds, sb);
          this.WelcomeFadeoutTimerLeft -= (float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
          break;
        case DLCHubServer.DHSState.ArchiveList:
          break;
        case DLCHubServer.DHSState.MissionSelectView:
          this.DrawMissionSelectView(bounds1, sb);
          break;
        case DLCHubServer.DHSState.ContractDetailView:
          this.DrawMissionDetailsPanel(bounds1, sb, this.SelectedMission);
          break;
        default:
          if (!this.os.Flags.HasFlag("DLC_Player_IRC_Authenticated"))
          {
            this.DoLoadingPlayerInScreen(bounds, sb);
            break;
          }
          this.State = DLCHubServer.DHSState.Home;
          break;
      }
    }

    private void DoLoadingPlayerInScreen(Rectangle bounds, SpriteBatch sb)
    {
      this.timeSpentInLoading += (float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
      float num1 = 10f;
      float num2 = 7f;
      float num3 = this.WelcomeFadeoutTimerLeft / this.BaseWelcomeFadeoutTime;
      Rectangle rectangle1 = Utils.InsetRectangle(bounds, 2);
      Rectangle rectangle2 = new Rectangle(rectangle1.X, rectangle1.Y + 30, rectangle1.Width, rectangle1.Height - 30);
      sb.Draw(Utils.white, rectangle2, Color.Black * num3);
      this.HexBackground.Draw(rectangle2, sb, Color.Black * 0.1f * num3, this.themeColor * 0.02f * num3, HexGridBackground.ColoringAlgorithm.CorrectedSinWash, 0.0f);
      if (!this.HasStartedWoosh && (double) this.timeSpentInLoading >= 5.0)
      {
        this.WooshBuildup.Play(0.7f, 0.0f, 0.0f);
        MusicManager.loadAsCurrentSong("DLC\\Music\\Userspacelike");
        this.HasStartedWoosh = true;
      }
      if ((double) this.timeSpentInLoading >= (double) num1)
      {
        if ((double) this.WelcomeFadeoutTimerLeft <= 0.0)
          this.os.Flags.AddFlag("DLC_Player_IRC_Authenticated");
        this.State = DLCHubServer.DHSState.Home;
      }
      if (MusicManager.isPlaying && (double) this.timeSpentInLoading < (double) num2 && MusicManager.currentSongName == "DLC\\Music\\Userspacelike")
        MusicManager.stop();
      Rectangle destinationRectangle = new Rectangle(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2, this.LoadingSpinner.Width, this.LoadingSpinner.Height);
      bool flag = (double) this.timeSpentInLoading > (double) num2;
      float amount = flag ? Utils.QuadraticOutCurve(1f - Math.Min(this.timeSpentInLoading - num2, 1f)) : 0.0f;
      sb.Draw(flag ? this.MissionAvaliableIcon : this.LoadingSpinner, destinationRectangle, new Rectangle?(), Color.Lerp(Utils.makeColorAddative(this.themeColor) * num3, Utils.AddativeWhite, amount), flag ? 0.0f : this.os.timer * 4f, flag ? this.MissionAvaliableIcon.GetCentreOrigin() : this.LoadingSpinner.GetCentreOrigin(), SpriteEffects.None, 0.4f);
      Rectangle dest1 = new Rectangle(bounds.X + 10, destinationRectangle.Y + this.LoadingSpinner.Height / 2 + 4, bounds.Width - 20, 26);
      if ((double) this.timeSpentInLoading > 1.0)
      {
        TextItem.doCenteredFontLabel(dest1, "Accessing Whitelist...", GuiData.smallfont, Utils.AddativeWhite * 0.5f * num3, false);
        dest1.Y += dest1.Height;
      }
      if ((double) this.timeSpentInLoading > 3.0)
      {
        TextItem.doCenteredFontLabel(dest1, "Authenticating...", GuiData.smallfont, Utils.AddativeWhite * 0.5f * num3, false);
        dest1.Y += dest1.Height;
      }
      if ((double) this.timeSpentInLoading > 5.0)
      {
        TextItem.doCenteredFontLabel(dest1, "Registering Connection...", GuiData.smallfont, Utils.AddativeWhite * 0.5f * num3, false);
        dest1.Y += dest1.Height;
      }
      if ((double) this.timeSpentInLoading > 5.5)
      {
        TextItem.doCenteredFontLabel(dest1, "Unlocking...", GuiData.smallfont, Utils.AddativeWhite * 0.5f * num3, false);
        dest1.Y += dest1.Height;
      }
      if ((double) this.timeSpentInLoading <= (double) num2)
        return;
      if (!MusicManager.isPlaying)
      {
        MusicManager.playSongImmediatley("DLC\\Music\\Userspacelike");
        this.USIntro.Play(0.4f, 0.0f, 0.0f);
      }
      float num4 = Utils.QuadraticOutCurve(Math.Min(1f, (float) (((double) this.timeSpentInLoading - (double) num2) / 2.0)));
      string text = " :: " + string.Format(LocaleTerms.Loc("Welcome {0}"), (object) this.os.SaveUserAccountName) + " :: ";
      Vector2 vector2 = GuiData.smallfont.MeasureString(text);
      Rectangle dest2 = new Rectangle(dest1.X, dest1.Y, dest1.Width / 2 - (int) ((double) vector2.X / 2.0), dest1.Height);
      int width = dest2.Width;
      dest2.Width = (int) ((double) dest2.Width * (double) num4);
      dest2.X += width - dest2.Width;
      Rectangle dest3 = new Rectangle(dest1.X + width + (int) vector2.X, dest1.Y, dest2.Width, dest1.Height);
      PatternDrawer.draw(dest2, 1f, Color.Transparent, this.themeColor * num3, sb);
      PatternDrawer.draw(dest3, 1f, Color.Transparent, this.themeColor * num3, sb);
      TextItem.doCenteredFontLabel(dest1, text, GuiData.smallfont, this.themeColor * num3, false);
      dest1.Y += dest1.Height;
    }

    private void DrawOptionsPanel(Rectangle bounds, SpriteBatch sb)
    {
      int width1 = Math.Min(200, bounds.Width / 3);
      int height = bounds.Height - 8;
      bool flag1 = this.PlayerHasClaimedMission();
      int x1 = bounds.X + 2;
      int y = bounds.Y + bounds.Height / 2 - height / 2;
      bool flag2 = this.os.Flags.HasFlag("DLC_Player_IRC_Authenticated") || (double) this.WelcomeFadeoutTimerLeft <= 0.0;
      int num;
      switch (this.State)
      {
        case DLCHubServer.DHSState.Welcome:
        case DLCHubServer.DHSState.Login:
        case DLCHubServer.DHSState.Home:
          if (Button.doButton(393001, x1, y, width1, height, LocaleTerms.Loc("Live Projects"), new Color?(flag1 ? Color.Gray : this.themeColor)) && flag2)
            this.State = DLCHubServer.DHSState.MissionSelectView;
          int x2 = x1 + (width1 + 8);
          if (flag1 && Button.doButton(393003, x2, y, width1, height, LocaleTerms.Loc("View Active Project"), new Color?(this.themeColor)) && flag2)
            this.State = DLCHubServer.DHSState.ContractDetailView;
          num = x2 + (width1 + 8);
          break;
        case DLCHubServer.DHSState.ArchiveList:
        case DLCHubServer.DHSState.MissionSelectView:
        case DLCHubServer.DHSState.ContractDetailView:
          if (Button.doButton(393001, x1, y, width1, height, LocaleTerms.Loc("Chat"), new Color?(flag1 ? Color.Gray : this.themeColor)) && flag2)
            this.State = DLCHubServer.DHSState.Home;
          num = x1 + (width1 + 8);
          break;
      }
      int width2 = 100;
      if (Button.doButton(393909, bounds.X + bounds.Width - width2, y, width2, height, LocaleTerms.Loc("Close"), new Color?(this.os.lockedColor)) && flag2)
        this.os.display.command = "connect";
      Rectangle destinationRectangle = bounds;
      destinationRectangle.Y += destinationRectangle.Height;
      destinationRectangle.Height = 1;
      sb.Draw(Utils.white, destinationRectangle, this.themeColor);
    }

    private void DrawHomeScreen(Rectangle bounds, SpriteBatch sb)
    {
      sb.Draw(Utils.gradient, bounds, Color.Black * 0.8f);
      bounds = Utils.InsetRectangle(bounds, 2);
      bounds.Height -= 6;
      this.IRCSystem.Draw(bounds, sb, false, LocaleTerms.Loc("Unknown"), this.HighlightedWords);
    }

    private void DrawMissionSelectView(Rectangle bounds, SpriteBatch sb)
    {
      int num = 2;
      int height1 = this.ActiveMissions.Count > 0 ? bounds.Height / this.ActiveMissions.Count - num : 0;
      if (this.ActiveMissions.Count > 0)
      {
        Rectangle bounds1 = new Rectangle(bounds.X, bounds.Y, bounds.Width, height1);
        for (int index = 0; index < this.ActiveMissions.Count; ++index)
        {
          this.ScrollableTextPanel.SetScrollbarUIIndexOffset(index + 100);
          this.DrawMissionPanel(bounds1, sb, this.ActiveMissions[index]);
          bounds1.Y += num + height1;
        }
      }
      else
      {
        int height2 = 60;
        Rectangle rectangle = new Rectangle(bounds.X, bounds.Y + bounds.Height / 2 - height2 / 2, bounds.Width, height2);
        sb.Draw(Utils.white, rectangle, Color.Black * 0.9f);
        TextItem.doFontLabelToSize(Utils.InsetRectangle(rectangle, 12), LocaleTerms.Loc("No Projects Available"), GuiData.font, Color.White, true, false);
      }
    }

    private void DrawMissionPanel(Rectangle bounds, SpriteBatch sb, DLCHubServer.ClaimableMission mission)
    {
      bool flag1 = false;
      bool flag2 = this.PlayerHasClaimedMission();
      Texture2D texture = this.MissionAvaliableIcon;
      Color color1 = Color.White;
      Color color2 = Color.White;
      if (mission.AgentClaim != null)
      {
        if (mission.AgentClaim == this.os.defaultUser.name)
        {
          texture = this.MissionPlayersIcon;
          flag1 = true;
        }
        else
        {
          texture = this.MissionTakenIcon;
          color1 = Color.Gray * 0.8f;
          color2 = Color.Black;
        }
      }
      else
      {
        color1 = Color.Lerp(Utils.AddativeWhite, Utils.makeColorAddative(this.themeColor), 0.5f);
        color2 = this.themeColor;
      }
      bool flag3 = flag1 || !flag2 && mission.AgentClaim == null;
      RenderedRectangle.doRectangleOutline(bounds.X, bounds.Y, bounds.Width, bounds.Height, 1, new Color?(flag3 ? this.themeColor : Color.Gray));
      Rectangle destinationRectangle1 = Utils.InsetRectangle(bounds, 1);
      sb.Draw(Utils.gradient, destinationRectangle1, color2 * 0.2f);
      int num1 = 4;
      int num2 = Math.Min(bounds.Width / 6, bounds.Height - 2 * num1);
      Rectangle rectangle1 = new Rectangle(bounds.X + num1, bounds.Y + num1, num2, num2);
      rectangle1 = Utils.InsetRectangle(rectangle1, 2);
      Rectangle rectangle2 = new Rectangle(rectangle1.X + rectangle1.Width + num1, bounds.Y + num1, bounds.Width - rectangle1.Width - num1 * 6, 40);
      Rectangle destinationRectangle2 = new Rectangle(rectangle2.X, rectangle2.Y, rectangle2.Width + num1 * 3, rectangle2.Height);
      sb.Draw(Utils.white, destinationRectangle2, Utils.VeryDarkGray * 0.7f);
      TextItem.doFontLabel(new Vector2((float) (rectangle2.X + 2 * num1), (float) rectangle2.Y), mission.Mission.postingTitle, GuiData.font, new Color?(flag3 ? this.themeColor : Color.Gray), (float) rectangle2.Width, (float) rectangle2.Height, true);
      int width = bounds.Width / 5 + num1;
      if (width < 150)
        width = 150;
      Rectangle dest1 = new Rectangle(rectangle2.X + 10, rectangle2.Y + rectangle2.Height + num1, bounds.Width / 5 * 4 - (10 + width - num1), bounds.Height - (rectangle2.Height + num1 * 3));
      if (flag1 || mission.AgentClaim == null)
      {
        if (!flag1 && flag2)
          dest1.Width = bounds.Width - (num1 * 3 + num2);
        string text = Utils.SuperSmartTwimForWidth(mission.Mission.postingBody, dest1.Width - num1, GuiData.tinyfont);
        ScrollBar.AlwaysDrawUnderBar = true;
        this.ScrollableTextPanel.UpdateScroll(mission.UITextScrollDown);
        this.ScrollableTextPanel.Draw(dest1, text, sb, flag1 || mission.AgentClaim == null ? Color.White : Color.LightGray * 0.6f);
        mission.UITextScrollDown = this.ScrollableTextPanel.GetScrollDown();
        ScrollBar.AlwaysDrawUnderBar = false;
      }
      else
      {
        dest1.X = bounds.X;
        dest1.Width = bounds.Width;
        Rectangle rectangle3 = new Rectangle(dest1.X, dest1.Y + (bounds.Height > 200 ? 20 : 0), dest1.Width, 30);
        string text1 = "- " + LocaleTerms.Loc("CLAIMED") + " : ";
        string text2 = text1 + mission.AgentClaim + " - ";
        Vector2 vector2_1 = GuiData.font.MeasureString(text2);
        Vector2 vector2_2 = GuiData.font.MeasureString(text1);
        Vector2 vector2_3 = GuiData.font.MeasureString(mission.AgentClaim);
        float num3 = Math.Min((float) rectangle3.Width / vector2_1.X, (float) rectangle3.Height / vector2_1.Y);
        float x1 = (float) (rectangle3.X + rectangle3.Width / 2) - (float) ((double) vector2_1.X * (double) num3 / 2.0);
        float y = (float) (rectangle3.Y + rectangle3.Height / 2) - (float) ((double) vector2_1.Y * (double) num3 / 2.0);
        sb.DrawString(GuiData.font, text1, new Vector2(x1, y), Color.LightGray, 0.0f, Vector2.Zero, new Vector2(num3), SpriteEffects.None, 0.4f);
        float x2 = x1 + vector2_2.X * num3;
        sb.DrawString(GuiData.font, mission.AgentClaim, new Vector2(x2, y), Color.Lerp(this.themeColor, Color.Gray, 0.55f), 0.0f, Vector2.Zero, new Vector2(num3), SpriteEffects.None, 0.4f);
        float x3 = x2 + vector2_3.X * num3;
        sb.DrawString(GuiData.font, " -", new Vector2(x3, y), Color.LightGray, 0.0f, Vector2.Zero, new Vector2(num3), SpriteEffects.None, 0.4f);
        rectangle3.Y += rectangle3.Height;
        Rectangle destinationRectangle3 = new Rectangle(bounds.X + num1 + num2, rectangle3.Y + 6, bounds.Width - num1 * 2 - num2, (int) ((double) dest1.Height * 0.6));
        sb.Draw(Utils.white, destinationRectangle3, Color.Black * 0.65f);
        Rectangle dest2 = new Rectangle(destinationRectangle3.X + num1 + 10, destinationRectangle3.Y, destinationRectangle3.Width - num2 - 6, destinationRectangle3.Height);
        string text3 = Utils.SuperSmartTwimForWidth(mission.Mission.postingBody, dest2.Width - num1, GuiData.tinyfont);
        bool offsetToTopLeft = false;
        if (text3.StartsWith("-- "))
        {
          text3 = "\n                " + text3;
          offsetToTopLeft = true;
        }
        TextItem.doFontLabelToSize(dest2, text3, GuiData.tinyfont, Color.DarkGray * 0.5f, true, offsetToTopLeft);
        rectangle3.Y += rectangle3.Height + 5;
        rectangle3.Height = 25;
      }
      sb.Draw(texture, rectangle1, color1);
      int height = Math.Min((bounds.Height - rectangle2.Height - num1 * 6) / 3, 22);
      Vector2 vector2 = Utils.ClipVec2ForTextRendering(new Vector2((float) (dest1.X + dest1.Width + num1), (float) dest1.Y));
      if (mission.AgentClaim == null && !flag2)
      {
        if (Button.doButton(391001 + this.UIButtonOffset, (int) vector2.X, (int) vector2.Y, width, height, LocaleTerms.Loc("Claim Contract"), new Color?(this.themeColor)))
        {
          this.PlayerAcceptMission(mission);
          this.SelectedMission = mission;
          this.State = DLCHubServer.DHSState.ContractDetailView;
        }
        vector2.Y += (float) (height + num1);
        ++this.UIButtonOffset;
      }
      if (!flag1)
        return;
      if (Button.doButton(391002 + this.UIButtonOffset, (int) vector2.X, (int) vector2.Y, width, height, width > 140 ? LocaleTerms.Loc("View Details") : LocaleTerms.Loc("Details"), new Color?(Color.White)))
      {
        this.State = DLCHubServer.DHSState.ContractDetailView;
        this.ScrollableTextPanel.UpdateScroll(0.0f);
        this.SelectedMission = mission;
        this.AbandonMissionShowConfirmation = false;
        this.ShouldShowMissionIncompleteMessage = false;
      }
      vector2.Y += (float) (height + num1);
      ++this.UIButtonOffset;
      if (Button.doButton(391003 + this.UIButtonOffset, (int) vector2.X, (int) vector2.Y, width, height, LocaleTerms.Loc("Complete"), new Color?(this.themeColor)) && !this.PlayerAttemptCompleteMission(mission, false))
      {
        this.State = DLCHubServer.DHSState.ContractDetailView;
        this.ShouldShowMissionIncompleteMessage = true;
      }
      vector2.Y += (float) (height + num1);
      ++this.UIButtonOffset;
    }

    private void DrawMissionDetailsPanel(Rectangle bounds, SpriteBatch sb, DLCHubServer.ClaimableMission mission)
    {
      bool flag1 = false;
      bool flag2 = this.PlayerHasClaimedMission();
      Texture2D texture = this.MissionAvaliableIcon;
      if (mission.AgentClaim != null)
      {
        if (mission.AgentClaim == this.os.defaultUser.name)
        {
          texture = this.MissionPlayersIcon;
          flag1 = true;
        }
        else
          texture = this.MissionTakenIcon;
      }
      bool flag3 = flag1 || !flag2 && mission.AgentClaim == null;
      int num1 = 4;
      int num2 = Math.Min(bounds.Width / 6, bounds.Height - 2 * num1);
      Rectangle destinationRectangle1 = new Rectangle(bounds.X + num1, bounds.Y + num1, num2, num2);
      Rectangle destinationRectangle2 = new Rectangle(destinationRectangle1.X + destinationRectangle1.Width + num1, bounds.Y + num1, bounds.Width - destinationRectangle1.Width - num1 * 3, 40);
      sb.Draw(Utils.white, destinationRectangle2, Utils.VeryDarkGray * 0.8f);
      TextItem.doFontLabel(new Vector2((float) (destinationRectangle2.X + 2 * num1), (float) destinationRectangle2.Y), mission.Mission.email.subject, GuiData.font, new Color?(flag3 ? this.themeColor : Color.Gray), (float) destinationRectangle2.Width, (float) destinationRectangle2.Height, true);
      sb.Draw(texture, destinationRectangle1, Color.White);
      int height = 30;
      Rectangle dest1 = new Rectangle(destinationRectangle1.X + destinationRectangle1.Width + num1, destinationRectangle2.Y + destinationRectangle2.Height + num1 * 2, bounds.Width - (destinationRectangle1.X - bounds.X + destinationRectangle1.Width + num1 * 2), height);
      this.DrawHeaderLine(dest1, LocaleTerms.Loc("Briefing"), sb);
      Rectangle dest2 = dest1;
      dest2.Y += dest1.Height + num1;
      string text = Utils.SuperSmartTwimForWidth(mission.Mission.email.body, dest2.Width, GuiData.tinyfont);
      Vector2 vector2 = GuiData.tinyfont.MeasureString(text);
      int num3 = this.MissionTextResponses.Count * 24 + height * 4 + num1 * 4 + mission.Mission.email.attachments.Count * 30 + 10;
      int val1_1 = bounds.Height - (num3 + (dest2.Y - bounds.Y)) - num1 * 2;
      int val1_2 = 70;
      dest2.Height = Math.Max(val1_2, Math.Min(val1_1, Math.Min((int) vector2.Y + 2 * num1, bounds.Height / 3 * 2)));
      this.ScrollableTextPanel.Draw(dest2, text, sb);
      dest1.Y = dest2.Y + dest2.Height + num1;
      this.DrawHeaderLine(dest1, LocaleTerms.Loc("Attachments"), sb);
      Vector2 dpos = new Vector2((float) dest1.X, (float) (dest1.Y + dest1.Height + num1));
      if (mission.Mission.email.attachments.Count == 0)
      {
        Rectangle dest3 = dest1;
        dest3.Y += dest3.Height;
        TextItem.doFontLabelToSize(dest3, "- " + LocaleTerms.Loc("Empty") + " -", GuiData.smallfont, Color.LightGray, true, false);
        dpos.Y += 26f;
      }
      else
      {
        for (int index = 0; index < mission.Mission.email.attachments.Count; ++index)
        {
          if (AttachmentRenderer.RenderAttachment(mission.Mission.email.attachments[index], (object) this.os, dpos, this.UIButtonOffset++, this.ButtonPressSound))
            dpos.Y += 22f;
        }
      }
      dpos.Y += (float) num1;
      dest1.Y = (int) dpos.Y;
      if (mission.IsComplete)
        return;
      this.DrawHeaderLine(dest1, LocaleTerms.Loc("Tools"), sb);
      if (this.ShouldShowMissionIncompleteMessage)
      {
        Rectangle dest3 = dest1;
        dest3.Height -= 4;
        PatternDrawer.draw(dest3, this.os.timer * 0.0008f, this.os.lockedColor * 0.3f, this.os.brightLockedColor * 0.7f, sb, PatternDrawer.thinStripe);
        TextItem.doRightAlignedBackingLabelScaled(dest3, LocaleTerms.Loc("Mission Incomplete"), GuiData.font, Color.Transparent, Color.White);
      }
      Rectangle rectangle = dest1;
      rectangle.Y += dest1.Height + num1;
      int width = (rectangle.Width - num1 * 2) / 2;
      if (Button.doButton(639013 + this.UIButtonOffset++, rectangle.X, rectangle.Y, width, rectangle.Height, LocaleTerms.Loc("Complete"), new Color?(this.themeColor)))
      {
        if (!this.PlayerAttemptCompleteMission(mission, false))
        {
          this.ShouldShowMissionIncompleteMessage = true;
        }
        else
        {
          this.ShouldShowMissionIncompleteMessage = false;
          this.State = DLCHubServer.DHSState.Home;
        }
      }
      if (Button.doButton(639414 + this.UIButtonOffset++, rectangle.X + width + num1, rectangle.Y, width, rectangle.Height, this.AbandonMissionShowConfirmation ? LocaleTerms.Loc("CONFIRM?") : LocaleTerms.Loc("Abandon"), new Color?(this.AllowContractAbbandon ? (this.AbandonMissionShowConfirmation ? this.os.brightLockedColor : this.os.lockedColor) : Color.Gray)) && this.AllowContractAbbandon)
      {
        if (!this.AbandonMissionShowConfirmation)
          this.AbandonMissionShowConfirmation = true;
        else
          this.PlayerAbandonedMission(mission);
      }
      if (Settings.forceCompleteEnabled && Button.doButton(649017 + this.UIButtonOffset++, rectangle.X + width + num1, rectangle.Y + rectangle.Height + 2, width, rectangle.Height, LocaleTerms.Loc("Force Complete"), new Color?(this.themeColor)))
      {
        this.PlayerAttemptCompleteMission(mission, true);
        this.State = DLCHubServer.DHSState.Home;
      }
      Rectangle bounds1 = rectangle;
      bounds1.Y += rectangle.Height + num1;
      bounds1.Height = bounds.Y + bounds.Height - bounds1.Y - num1;
      this.DrawAdditionalDetailsSection(bounds1, sb);
    }

    private void DrawAdditionalDetailsSection(Rectangle bounds, SpriteBatch sb)
    {
      int num1 = 24;
      Vector2 pos = new Vector2((float) bounds.X, (float) bounds.Y);
      TextItem.doFontLabel(pos, LocaleTerms.Loc("Additional Details") + " :", GuiData.smallfont, new Color?(), Math.Max(200f, (float) bounds.Width - (float) (((double) pos.X - (double) bounds.Width) * 2.0)), float.MaxValue, false);
      pos.Y += (float) num1;
      for (int index = 0; index < this.MissionTextResponses.Count; ++index)
      {
        TextItem.doFontLabel(pos + new Vector2(25f, 0.0f), this.MissionTextResponses[index], GuiData.tinyfont, new Color?(), (float) ((double) bounds.Width - ((double) pos.X - (double) bounds.X) * 2.0 - 20.0), float.MaxValue, false);
        float num2 = Math.Min(GuiData.tinyfont.MeasureString(this.MissionTextResponses[index]).X, (float) ((double) bounds.Width - ((double) pos.X - (double) bounds.X) * 2.0 - 20.0));
        if (Button.doButton(80000 + index * 100, (int) ((double) pos.X + (double) num2 + 30.0), (int) pos.Y, 20, 20, "-", new Color?()))
          this.MissionTextResponses.RemoveAt(index);
        pos.Y += (float) num1;
      }
      if (this.isAddingTextResponse)
      {
        string data = (string) null;
        bool getStringCommand = Programs.parseStringFromGetStringCommand(this.os, out data);
        if (data == null)
          data = "";
        pos.Y += 5f;
        GuiData.spriteBatch.Draw(Utils.white, new Rectangle(bounds.X + 1, (int) pos.Y, bounds.Width - 2 - bounds.Width / 9, 40), this.os.indentBackgroundColor);
        pos.Y += 10f;
        TextItem.doFontLabel(pos + new Vector2(25f, 0.0f), data, GuiData.tinyfont, new Color?(), float.MaxValue, float.MaxValue, false);
        Vector2 vector2 = GuiData.tinyfont.MeasureString(data);
        vector2.Y = 0.0f;
        if ((double) this.os.timer % 1.0 <= 0.5)
          GuiData.spriteBatch.Draw(Utils.white, new Rectangle((int) ((double) pos.X + (double) vector2.X + 2.0) + 25, (int) pos.Y, 4, 20), Color.White);
        int num2 = bounds.Width - 1 - bounds.Width / 10;
        if (getStringCommand || Button.doButton(8000094, bounds.X + num2 - 4, (int) pos.Y - 10, bounds.Width / 9 - 3, 40, LocaleTerms.Loc("Add"), new Color?(this.os.highlightColor)))
        {
          if (!getStringCommand)
            this.os.terminal.executeLine();
          this.isAddingTextResponse = false;
          if (!string.IsNullOrWhiteSpace(data))
            this.MissionTextResponses.Add(data);
          this.inProgressTextResponse = (string) null;
        }
        else
          this.inProgressTextResponse = data;
      }
      else if (Button.doButton(8000098, (int) ((double) pos.X + 4.0), (int) pos.Y, 140, 24, LocaleTerms.Loc("Add Detail"), new Color?()))
      {
        this.isAddingTextResponse = true;
        this.os.execute("getString Detail");
        this.os.terminal.executionPreventionIsInteruptable = true;
      }
    }

    private void DrawHeaderLine(Rectangle dest, string header, SpriteBatch sb)
    {
      Vector2 vector2 = GuiData.font.MeasureString(header);
      Vector2 scale = new Vector2(1f, Math.Min((float) dest.Height, vector2.Y) / vector2.Y);
      if ((double) vector2.X > (double) dest.Width)
        scale.X = (float) (1.0 / ((double) vector2.X / (double) dest.Width));
      scale = new Vector2(Math.Min(scale.X, scale.Y), Math.Min(scale.X, scale.Y));
      Vector2 position = new Vector2((float) dest.X, (float) (dest.Y + dest.Height) - vector2.Y * scale.Y);
      sb.DrawString(GuiData.font, header, position, this.themeColor, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.4f);
      dest.Y += dest.Height - 1;
      dest.Height = 1;
      sb.Draw(Utils.white, dest, this.themeColor);
    }

    private enum DHSState
    {
      Welcome,
      Login,
      Home,
      ArchiveList,
      MissionSelectView,
      ContractDetailView,
    }

    internal class ClaimableMission
    {
      public string AgentClaim;
      public bool IsComplete;
      public ActiveMission Mission;
      public float UITextScrollDown;
    }
  }
}
