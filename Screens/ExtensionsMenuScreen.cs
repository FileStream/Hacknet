// Decompiled with JetBrains decompiler
// Type: Hacknet.Screens.ExtensionsMenuScreen
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Extensions;
using Hacknet.Gui;
using Hacknet.Localization;
using Hacknet.Misc;
using Hacknet.PlatformAPI.Storage;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Hacknet.Screens
{
  public class ExtensionsMenuScreen
  {
    public List<ExtensionInfo> Extensions = new List<ExtensionInfo>();
    private bool HasLoaded = false;
    private bool HasStartedLoading = false;
    private ExtensionInfo ExtensionInfoToShow = (ExtensionInfo) null;
    private Texture2D DefaultModImage = (Texture2D) null;
    private SavefileLoginScreen SaveScreen = new SavefileLoginScreen();
    private SteamWorkshopPublishScreen workshopPublishScreen = (SteamWorkshopPublishScreen) null;
    private bool IsInPublishScreen = false;
    private string ReportOverride = (string) null;
    private ExtensionsMenuScreen.EMSState State = ExtensionsMenuScreen.EMSState.Normal;
    public string LoadErrors = "";
    public int ScrollStartIndex = 0;
    public const string ExtensionBaseFolder = "Extensions/";
    public Action ExitClicked;
    public Action<string, string> CreateNewAccountForExtension_UserAndPass;
    public Action<string, string> LoadAccountForExtension_FileAndUsername;

    public ExtensionsMenuScreen()
    {
      this.SaveScreen.StartNewGameForUsernameAndPass += (Action<string, string>) ((username, pass) =>
      {
        OS.WillLoadSave = false;
        if (this.CreateNewAccountForExtension_UserAndPass == null)
          return;
        this.CreateNewAccountForExtension_UserAndPass(username, pass);
      });
      this.SaveScreen.LoadGameForUserFileAndUsername += (Action<string, string>) ((filename, username) =>
      {
        OS.WillLoadSave = true;
        if (this.LoadAccountForExtension_FileAndUsername == null)
          return;
        this.LoadAccountForExtension_FileAndUsername(filename, username);
      });
      this.SaveScreen.RequestGoBack += (Action) (() => this.State = ExtensionsMenuScreen.EMSState.Normal);
      this.SaveScreen.DrawFromTop = true;
      if (!Settings.AllowExtensionPublish)
        return;
      this.workshopPublishScreen = new SteamWorkshopPublishScreen(Game1.getSingleton().Content);
      this.workshopPublishScreen.GoBack += (Action) (() => this.IsInPublishScreen = false);
    }

    private void LoadExtensions()
    {
      if (!Directory.Exists("Extensions/"))
        Directory.CreateDirectory("Extensions/");
      foreach (string directory in Directory.GetDirectories("Extensions/"))
        this.AddExtensionSafe(directory);
      this.DefaultModImage = Game1.getSingleton().Content.Load<Texture2D>("Sprites/Hammer");
      if (PlatformAPISettings.Running)
      {
        PublishedFileId_t[] pvecPublishedFileID = new PublishedFileId_t[200];
        uint subscribedItems = SteamUGC.GetSubscribedItems(pvecPublishedFileID, 200U);
        Console.WriteLine(subscribedItems);
        if (pvecPublishedFileID != null)
        {
          for (int index = 0; (long) index < (long) subscribedItems; ++index)
          {
            if (((EItemState) Enum.Parse(typeof (EItemState), string.Concat((object) SteamUGC.GetItemState(pvecPublishedFileID[index])))).HasFlag((Enum) EItemState.k_EItemStateInstalled))
            {
              ulong punSizeOnDisk = 0;
              string pchFolder = (string) null;
              uint cchFolderSize = 20000;
              uint punTimeStamp = 0;
              Console.WriteLine((SteamUGC.GetItemInstallInfo(pvecPublishedFileID[index], out punSizeOnDisk, out pchFolder, cchFolderSize, out punTimeStamp) ? "Installed" : "Uninstalled") + " Extension at: " + pchFolder);
              this.AddExtensionSafe(pchFolder);
            }
          }
        }
      }
      this.HasLoaded = true;
    }

    public void Reset()
    {
      this.State = ExtensionsMenuScreen.EMSState.Normal;
      this.ScrollStartIndex = 0;
    }

    private void AddExtensionSafe(string folderpath)
    {
      if (!ExtensionInfo.ExtensionExists(folderpath))
        return;
      try
      {
        this.Extensions.Add(ExtensionInfo.ReadExtensionInfo(folderpath));
      }
      catch (Exception ex)
      {
        Console.WriteLine(Utils.GenerateReportFromException(ex));
        ExtensionsMenuScreen extensionsMenuScreen = this;
        string str = extensionsMenuScreen.LoadErrors + LocaleTerms.Loc("Error loading ExtensionInfo for") + " " + folderpath + "\n" + LocaleTerms.Loc("Error details written to") + " " + folderpath.Replace("\\", "/") + "/error_report.txt\n\n";
        extensionsMenuScreen.LoadErrors = str;
        Utils.writeToFile("Error loading ExtensionInfo for extension in folder:\n" + folderpath + "\nError details: \n" + Utils.GenerateReportFromException(ex), folderpath + "/error_report.txt");
      }
    }

    private void EnsureHasLoaded()
    {
      if (this.HasStartedLoading)
        return;
      new Thread(new ThreadStart(this.LoadExtensions))
      {
        IsBackground = true
      }.Start();
      this.HasStartedLoading = true;
    }

    public void ShowError(string error)
    {
      this.ReportOverride = error;
      this.State = ExtensionsMenuScreen.EMSState.Normal;
    }

    private void ActivateExtensionPage(ExtensionInfo info)
    {
      LocaleActivator.ActivateLocale(info.Language, Game1.getSingleton().Content);
      this.ExtensionInfoToShow = info;
      this.ReportOverride = (string) null;
      this.SaveScreen.ProjectName = info.Name;
      Settings.IsInExtensionMode = true;
      ExtensionLoader.ActiveExtensionInfo = info;
      SaveFileManager.Init(true);
      this.SaveScreen.ResetForNewAccount();
    }

    private void ExitExtensionsScreen()
    {
      this.ExtensionInfoToShow = (ExtensionInfo) null;
      Settings.IsInExtensionMode = false;
      SaveFileManager.Init(true);
      if (this.ExitClicked == null)
        return;
      this.ExitClicked();
    }

    public void Draw(Rectangle dest, SpriteBatch sb, ScreenManager screenman)
    {
      this.EnsureHasLoaded();
      Rectangle rectangle = new Rectangle(dest.X, dest.Y, dest.Width, 40);
      Color color = Color.Lerp(Utils.AddativeWhite, Utils.AddativeRed, 0.3f + Utils.randm(0.22f));
      SpriteFont font = GuiData.font;
      Vector2 vector2_1 = new Vector2((float) dest.X, (float) dest.Y + 44f);
      string original = "E X T E N S I O N S";
      GuiData.spriteBatch.DrawString(font, Utils.FlipRandomChars(original, 0.0179999992251396), vector2_1, Utils.AddativeWhite * (0.17f + Utils.randm(0.04f)));
      GuiData.spriteBatch.DrawString(font, Utils.FlipRandomChars(original, 0.00400000018998981), vector2_1, color * (0.8f + Utils.randm(0.04f)));
      vector2_1.Y += 35f;
      Vector2 vector2_2;
      if (this.ExtensionInfoToShow == null)
      {
        vector2_2 = this.DrawExtensionList(vector2_1, dest, sb);
      }
      else
      {
        switch (this.State)
        {
          case ExtensionsMenuScreen.EMSState.GetUsername:
          case ExtensionsMenuScreen.EMSState.ShowAccounts:
            Rectangle dest1 = new Rectangle(dest.X, dest.Y + dest.Height / 4, dest.Width, (int) ((double) dest.Height * 0.800000011920929));
            vector2_2 = this.DrawExtensionCreateNewUserOrLoadScreen(vector2_1, dest1, sb, screenman, this.ExtensionInfoToShow);
            break;
          default:
            vector2_2 = this.DrawExtensionInfoDetail(vector2_1, dest, sb, screenman, this.ExtensionInfoToShow);
            break;
        }
      }
      if (!Button.doButton(391481, (int) vector2_2.X, (int) vector2_2.Y, 450, 25, LocaleTerms.Loc("Return to Main Menu"), new Color?(MainMenu.exitButtonColor)))
        return;
      this.ExitExtensionsScreen();
    }

    private Vector2 DrawExtensionCreateNewUserOrLoadScreen(Vector2 drawpos, Rectangle dest, SpriteBatch sb, ScreenManager screenMan, ExtensionInfo info)
    {
      this.SaveScreen.Draw(sb, dest);
      return drawpos;
    }

    private Vector2 DrawExtensionInfoDetail(Vector2 drawpos, Rectangle dest, SpriteBatch sb, ScreenManager screenMan, ExtensionInfo info)
    {
      sb.DrawString(GuiData.titlefont, info.Name.ToUpper(), drawpos, Utils.AddativeWhite * 0.66f);
      drawpos.Y += 80f;
      int height1 = sb.GraphicsDevice.Viewport.Height;
      int num1 = 256;
      if (height1 < 900)
        num1 = 120;
      Rectangle dest1 = new Rectangle((int) drawpos.X, (int) drawpos.Y, num1, num1);
      Texture2D texture = this.DefaultModImage;
      if (info.LogoImage != null)
        texture = info.LogoImage;
      FlickeringTextEffect.DrawFlickeringSprite(sb, dest1, texture, 2f, 0.5f, (object) null, Color.White);
      Vector2 position = drawpos + new Vector2((float) num1 + 40f, 20f);
      float num2 = (float) dest.Width - (drawpos.X - (float) dest.X);
      string text1 = Utils.SuperSmartTwimForWidth(info.Description, (int) num2, GuiData.smallfont);
      sb.DrawString(GuiData.smallfont, text1, position, Utils.AddativeWhite * 0.7f);
      drawpos.Y += (float) num1 + 10f;
      if (this.IsInPublishScreen)
      {
        Rectangle fullscreen = Utils.GetFullscreen();
        Rectangle dest2 = new Rectangle((int) drawpos.X, (int) drawpos.Y, fullscreen.Width - (int) drawpos.X * 2, fullscreen.Height - ((int) drawpos.Y + 50));
        return this.workshopPublishScreen.Draw(sb, dest2, info);
      }
      if (this.ReportOverride != null)
      {
        string text2 = Utils.SuperSmartTwimForWidth(this.ReportOverride, 800, GuiData.smallfont);
        sb.DrawString(GuiData.smallfont, text2, drawpos + new Vector2(460f, 0.0f), this.ReportOverride.Length > 250 ? Utils.AddativeRed : Utils.AddativeWhite);
      }
      int val1 = 40;
      int num3 = 5;
      int num4 = info.AllowSave ? 4 : 2;
      int num5 = height1 - (int) drawpos.Y - 55;
      int height2 = Math.Min(val1, (num5 - num4 * num3) / num4);
      if (Button.doButton(7900010, (int) drawpos.X, (int) drawpos.Y, 450, height2, string.Format(LocaleTerms.Loc("New {0} Account"), (object) info.Name), new Color?(MainMenu.buttonColor)))
      {
        this.State = ExtensionsMenuScreen.EMSState.GetUsername;
        this.SaveScreen.ResetForNewAccount();
      }
      drawpos.Y += (float) (height2 + num3);
      if (info.AllowSave)
      {
        bool flag = !string.IsNullOrWhiteSpace(SaveFileManager.LastLoggedInUser.FileUsername);
        if (Button.doButton(7900019, (int) drawpos.X, (int) drawpos.Y, 450, height2, flag ? "Continue Account : " + SaveFileManager.LastLoggedInUser.Username : " - No Accounts - ", new Color?(flag ? MainMenu.buttonColor : Color.Black)))
        {
          OS.WillLoadSave = true;
          if (this.LoadAccountForExtension_FileAndUsername != null)
            this.LoadAccountForExtension_FileAndUsername(SaveFileManager.LastLoggedInUser.FileUsername, SaveFileManager.LastLoggedInUser.Username);
        }
        drawpos.Y += (float) (height2 + num3);
        if (Button.doButton(7900020, (int) drawpos.X, (int) drawpos.Y, 450, height2, LocaleTerms.Loc("Login") + "...", new Color?(flag ? MainMenu.buttonColor : Color.Black)))
        {
          this.State = ExtensionsMenuScreen.EMSState.ShowAccounts;
          this.SaveScreen.ResetForLogin();
        }
        drawpos.Y += (float) (height2 + num3);
      }
      if (Button.doButton(7900030, (int) drawpos.X, (int) drawpos.Y, 450, height2, LocaleTerms.Loc("Run Verification Tests"), new Color?(MainMenu.buttonColor)))
      {
        int errorsAdded = 0;
        string str1 = ExtensionTests.TestExtensionForRuntime(screenMan, info.FolderPath, out errorsAdded);
        try
        {
          ExtensionInfo.VerifyExtensionInfo(info);
        }
        catch (Exception ex)
        {
          str1 = str1 + "\nExtension Metadata Error:\n" + Utils.GenerateReportFromException(ex);
          ++errorsAdded;
        }
        if (errorsAdded == 0)
        {
          this.ReportOverride = LocaleTerms.Loc("Testing...") + "\n" + LocaleTerms.Loc("All tests complete") + "\n0 " + LocaleTerms.Loc("Errors Found");
        }
        else
        {
          this.ReportOverride = LocaleTerms.Loc("Errors Found") + ". " + string.Format(LocaleTerms.Loc("Writing report to {0}"), (object) (info.FolderPath + "/report.txt\n").Replace("\\", "/")) + str1;
          string str2 = info.FolderPath + "/report.txt";
          if (File.Exists(str2))
            File.Delete(str2);
          Utils.writeToFile(this.ReportOverride, str2);
        }
        MusicManager.transitionToSong("Music/Ambient/AmbientDrone_Clipped");
        ExtensionLoader.ActiveExtensionInfo = this.ExtensionInfoToShow;
      }
      drawpos.Y += (float) (height2 + num3);
      if (Settings.AllowExtensionPublish && PlatformAPISettings.Running)
      {
        if (Button.doButton(7900031, (int) drawpos.X, (int) drawpos.Y, 450, height2, LocaleTerms.Loc("Steam Workshop Publishing"), new Color?(MainMenu.buttonColor)))
          this.IsInPublishScreen = true;
        drawpos.Y += (float) (height2 + num3);
      }
      if (Button.doButton(7900040, (int) drawpos.X, (int) drawpos.Y, 450, 25, LocaleTerms.Loc("Back to Extension List"), new Color?(MainMenu.exitButtonColor)))
        this.ExtensionInfoToShow = (ExtensionInfo) null;
      drawpos.Y += 30f;
      return drawpos;
    }

    private Vector2 DrawExtensionList(Vector2 drawpos, Rectangle dest, SpriteBatch sb)
    {
      if (this.HasLoaded)
      {
        Rectangle fullscreen = Utils.GetFullscreen();
        for (int scrollStartIndex = this.ScrollStartIndex; scrollStartIndex <= this.Extensions.Count; ++scrollStartIndex)
        {
          if ((double) drawpos.Y + 120.0 + 20.0 >= (double) fullscreen.Height || this.ScrollStartIndex > 0 && scrollStartIndex == this.Extensions.Count)
          {
            int height = 20;
            int num = (50 - height * 2) / 4;
            if (Button.doButton(790001 + scrollStartIndex, (int) drawpos.X, (int) drawpos.Y + num, 450, height, "   ^   ", new Color?(this.ScrollStartIndex > 0 ? MainMenu.buttonColor : Color.Black)) && this.ScrollStartIndex > 0)
              --this.ScrollStartIndex;
            bool flag = scrollStartIndex <= this.Extensions.Count - 1;
            if (Button.doButton(790101 + scrollStartIndex + 1, (int) drawpos.X, (int) drawpos.Y + height + num + num + 2, 450, height, "   v   ", new Color?(flag ? MainMenu.buttonColor : Color.Black)) && flag)
              ++this.ScrollStartIndex;
            drawpos.Y += 55f;
            break;
          }
          if (scrollStartIndex < this.Extensions.Count)
          {
            ExtensionInfo extension = this.Extensions[scrollStartIndex];
            if (Button.doButton(780001 + scrollStartIndex, (int) drawpos.X, (int) drawpos.Y, 450, 50, extension.Name, new Color?(Color.White)))
              this.ActivateExtensionPage(extension);
            drawpos.Y += 55f;
          }
        }
      }
      else
      {
        TextItem.doFontLabel(drawpos, LocaleTerms.Loc("Loading..."), GuiData.font, new Color?(Color.White), (float) dest.Width, 20f, false);
        drawpos.Y += 55f;
      }
      if (!string.IsNullOrWhiteSpace(this.LoadErrors))
        TextItem.doFontLabel(drawpos + new Vector2(0.0f, 30f), this.LoadErrors, GuiData.smallfont, new Color?(Color.Red), float.MaxValue, float.MaxValue, false);
      return drawpos;
    }

    private enum EMSState
    {
      Normal,
      GetUsername,
      ShowAccounts,
    }
  }
}
