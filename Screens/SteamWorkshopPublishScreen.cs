// Decompiled with JetBrains decompiler
// Type: Hacknet.Screens.SteamWorkshopPublishScreen
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Extensions;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hacknet.Screens
{
  public class SteamWorkshopPublishScreen
  {
    private bool isInUpload = false;
    private bool showLoadingSpinner = false;
    public bool HasInitializedSteamCallbacks = false;
    private string currentStatusMessage = "";
    private string currentTitleMessage = "";
    private string currentBodyMessage = "";
    private float spinnerRot = 0.0f;
    private AppId_t hacknetAppID = new AppId_t(365450U);
    public Action GoBack;
    private Texture2D spinnerTex;
    private ExtensionInfo ActiveInfo;
    protected CallResult<CreateItemResult_t> m_CreateItemResult;
    protected CallResult<Steamworks.SubmitItemUpdateResult_t> SubmitItemUpdateResult_t;
    protected CallResult<NumberOfCurrentPlayers_t> m_NumberOfCurrentPlayers;
    private UGCUpdateHandle_t updateHandle;
    private DateTime transferStarted;

    public SteamWorkshopPublishScreen(ContentManager content)
    {
      this.spinnerTex = content.Load<Texture2D>("Sprites/Spinner");
    }

    private void SubmitUpdateResult(Steamworks.SubmitItemUpdateResult_t callback, bool bIOFailure)
    {
      Console.WriteLine("Upload complete!");
      if (callback.m_eResult == EResult.k_EResultOK)
      {
        this.currentTitleMessage = "Update Complete!";
        this.currentBodyMessage = "Upload completed successfully";
      }
      else
      {
        this.currentTitleMessage = "Update Failed";
        this.currentBodyMessage = "Error code: " + (object) callback.m_eResult;
      }
      this.isInUpload = false;
      this.showLoadingSpinner = false;
    }

    private void CreateItemResult(CreateItemResult_t callback, bool bIOFailure)
    {
      switch (callback.m_eResult)
      {
        case EResult.k_EResultTimeout:
          this.currentTitleMessage = "Error: Timeout";
          this.currentBodyMessage = "Current user is not currently logged into steam";
          break;
        case EResult.k_EResultNotLoggedOn:
          this.currentTitleMessage = "Error: Not logged on";
          this.currentBodyMessage = "The user creating the item is currently banned in the community";
          break;
        case EResult.k_EResultInsufficientPrivilege:
          this.currentTitleMessage = "Error: Insufficient Privilege";
          this.currentBodyMessage = "The user creating the item is currently banned in the community";
          break;
        default:
          ulong publishedFileId = callback.m_nPublishedFileId.m_PublishedFileId;
          this.showLoadingSpinner = false;
          this.currentTitleMessage = "Extension successfully created!";
          this.currentBodyMessage = "Extension Publish ID: " + (object) callback.m_nPublishedFileId;
          this.ActiveInfo.WorkshopPublishID = string.Concat((object) callback.m_nPublishedFileId.m_PublishedFileId);
          if (callback.m_bUserNeedsToAcceptWorkshopLegalAgreement)
            SteamFriends.ActivateGameOverlayToWebPage("steam://url/CommunityFilePage/" + (object) callback.m_nPublishedFileId);
          string str = this.ActiveInfo.FolderPath + "/ExtensionInfo.xml";
          string contents = Utils.readEntireFile(str).Replace("<WorkshopPublishID>NONE</WorkshopPublishID>", "<WorkshopPublishID>" + (object) publishedFileId + "</WorkshopPublishID>");
          File.WriteAllText(str, contents);
          break;
      }
    }

    private void PerformUpdate(ExtensionInfo info)
    {
      this.updateHandle = SteamUGC.StartItemUpdate(this.hacknetAppID, new PublishedFileId_t((ulong) Convert.ToInt64(info.WorkshopPublishID)));
      this.isInUpload = true;
      bool flag1 = ((true & SteamUGC.SetItemTitle(this.updateHandle, info.Name) & SteamUGC.SetItemDescription(this.updateHandle, info.WorkshopDescription) ? 1 : 0) & (SteamUGC.SetItemTags(this.updateHandle, (IList<string>) info.WorkshopTags.Split(new string[3]{ " ,", ", ", "," }, StringSplitOptions.RemoveEmptyEntries)) ? 1 : 0)) != 0;
      string str = (Path.Combine(Directory.GetCurrentDirectory(), info.FolderPath) + "/" + info.WorkshopPreviewImagePath).Replace("\\", "/");
      if (File.Exists(str))
      {
        if (new FileInfo(str).Length < 1000000L)
          flag1 &= SteamUGC.SetItemPreview(this.updateHandle, str);
        else
          this.currentStatusMessage = "Workshop Preview Image too large! Max size 1mb. Ignoring...";
      }
      ERemoteStoragePublishedFileVisibility eVisibility = (int) info.WorkshopVisibility <= 0 ? ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic : ((int) info.WorkshopVisibility == 1 ? ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityFriendsOnly : ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate);
      bool flag2 = flag1 & SteamUGC.SetItemVisibility(this.updateHandle, eVisibility);
      string pszContentFolder = Path.Combine(Directory.GetCurrentDirectory(), info.FolderPath).Replace("\\", "/");
      Console.WriteLine("Content Path: " + pszContentFolder);
      if (!(flag2 & SteamUGC.SetItemContent(this.updateHandle, pszContentFolder)))
        Console.WriteLine("Error!");
      string path = info.FolderPath + "/Changelog.txt";
      string pchChangeNote = "";
      if (File.Exists(path))
        pchChangeNote = File.ReadAllText(path);
      this.SubmitItemUpdateResult_t.Set(SteamUGC.SubmitItemUpdate(this.updateHandle, pchChangeNote), (CallResult<Steamworks.SubmitItemUpdateResult_t>.APIDispatchDelegate) null);
      this.transferStarted = DateTime.Now;
      this.showLoadingSpinner = true;
      this.isInUpload = true;
      this.currentBodyMessage = "Uploading to Steam Workshop...";
      this.currentTitleMessage = "Upload in progress";
    }

    private void OnNumberOfCurrentPlayers(NumberOfCurrentPlayers_t pCallback, bool bIOFailure)
    {
      SteamWorkshopPublishScreen workshopPublishScreen = this;
      string str = workshopPublishScreen.currentBodyMessage + "\nPlayer callback: " + (object) pCallback.m_cPlayers;
      workshopPublishScreen.currentBodyMessage = str;
    }

    private void InitSteamCallbacks()
    {
      this.m_CreateItemResult = CallResult<CreateItemResult_t>.Create(new CallResult<CreateItemResult_t>.APIDispatchDelegate(this.CreateItemResult));
      this.m_NumberOfCurrentPlayers = CallResult<NumberOfCurrentPlayers_t>.Create(new CallResult<NumberOfCurrentPlayers_t>.APIDispatchDelegate(this.OnNumberOfCurrentPlayers));
      this.SubmitItemUpdateResult_t = CallResult<Steamworks.SubmitItemUpdateResult_t>.Create(new CallResult<Steamworks.SubmitItemUpdateResult_t>.APIDispatchDelegate(this.SubmitUpdateResult));
      this.HasInitializedSteamCallbacks = true;
    }

    public void Update()
    {
      SteamAPI.RunCallbacks();
    }

    public Vector2 Draw(SpriteBatch sb, Rectangle dest, ExtensionInfo info)
    {
      this.ActiveInfo = info;
      if (!this.HasInitializedSteamCallbacks)
        this.InitSteamCallbacks();
      this.Update();
      Vector2 vector2 = new Vector2((float) dest.X, (float) dest.Y);
      bool flag1 = info.WorkshopPublishID != "NONE";
      this.currentStatusMessage = flag1 ? "Ready to push Updates" : "Ready to create in steam";
      if (!flag1 && string.IsNullOrWhiteSpace(this.currentBodyMessage))
        this.currentBodyMessage = "By submitting this item, you agree to the workshop terms of service\nhttp://steamcommunity.com/sharedfiles/workshoplegalagreement";
      Vector2 pos1 = new Vector2(vector2.X + (float) (dest.Width / 2), vector2.Y);
      TextItem.doFontLabel(pos1, this.currentStatusMessage, GuiData.font, new Color?(Color.Gray), (float) dest.Width / 2f, 30f, false);
      pos1.Y += 30f;
      TextItem.doFontLabel(pos1, this.currentTitleMessage, GuiData.font, new Color?(Color.White), (float) dest.Width / 2f, 30f, false);
      pos1.Y += 30f;
      Vector2 pos2 = pos1;
      if (this.showLoadingSpinner)
      {
        pos1.X += 16f;
        this.spinnerRot += 0.1f;
        Rectangle destinationRectangle = new Rectangle((int) pos1.X, (int) pos1.Y + 20, 40, 40);
        sb.Draw(this.spinnerTex, destinationRectangle, new Rectangle?(), Color.White, this.spinnerRot, this.spinnerTex.GetCentreOrigin(), SpriteEffects.None, 0.5f);
        pos2.X += 45f;
      }
      if (this.isInUpload)
      {
        Rectangle rectangle = new Rectangle((int) pos2.X, (int) pos2.Y + 6, dest.Width / 2, 20);
        ulong punBytesProcessed = 0;
        ulong punBytesTotal = 1;
        int itemUpdateProgress = (int) SteamUGC.GetItemUpdateProgress(this.updateHandle, out punBytesProcessed, out punBytesTotal);
        double val1 = (double) punBytesProcessed / (double) punBytesTotal;
        if ((long) punBytesTotal == 0L)
          val1 = 0.0;
        sb.Draw(Utils.white, Utils.InsetRectangle(rectangle, -1), Utils.AddativeWhite * 0.7f);
        sb.Draw(Utils.white, rectangle, Utils.VeryDarkGray);
        rectangle.Width = (int) ((double) rectangle.Width * val1);
        sb.Draw(Utils.white, rectangle, Color.LightBlue);
        pos2.Y += 31f;
        if (punBytesTotal > 0UL)
        {
          string format = "{0}% - {1}mb of {2}mb Transfered";
          string str1 = (val1 * 100.0).ToString("00.00");
          ulong num = punBytesProcessed / 1000000UL;
          string str2 = num.ToString("0.00");
          num = punBytesTotal / 1000000UL;
          string str3 = num.ToString("0.00");
          string text1 = string.Format(format, (object) str1, (object) str2, (object) str3);
          Utils.DrawStringMonospace(sb, text1, GuiData.smallfont, pos2, Color.White, 9f);
          pos2.Y += 20f;
          TimeSpan time = DateTime.Now - this.transferStarted;
          string text2 = string.Format("ETA: {0} // Elapsed : {1}", (object) this.getTimespanDisplayString(TimeSpan.FromSeconds(time.TotalSeconds / Math.Max(val1, 0.01))), (object) this.getTimespanDisplayString(time));
          Utils.DrawStringMonospace(sb, text2, GuiData.smallfont, pos2, Color.White, 9f);
          pos2.Y += 25f;
        }
      }
      TextItem.doFontLabel(pos2, this.currentBodyMessage, GuiData.smallfont, new Color?(Color.White * 0.8f), (float) dest.Width / 2f, 30f, false);
      int height = 40;
      int width = 450;
      bool flag2 = !this.showLoadingSpinner;
      if (!this.isInUpload)
      {
        if (Button.doButton(371711001, (int) vector2.X, (int) vector2.Y, width, height, flag1 ? " - Item Created -" : "Create Entry in Steam Workshop", new Color?(flag1 ? Color.LightBlue : Color.Gray)) && !flag1 && flag2)
          this.CreateExtensionInSteam(info);
        vector2.Y += (float) (height + 4);
        if (Button.doButton(371711003, (int) vector2.X, (int) vector2.Y, width, height, "Upload to Steam Workshop", new Color?(flag1 ? Color.LightBlue : Color.Gray)) && flag2)
          this.PerformUpdate(info);
        vector2.Y += (float) (height + 4);
        if (Button.doButton(371711005, (int) vector2.X, (int) vector2.Y + 10, width, height - 10, "Back to Extension Menu", new Color?(Color.Black)) && flag2 && this.GoBack != null)
          this.GoBack();
        vector2.Y += (float) (height + 4);
      }
      return vector2;
    }

    private string getTimespanDisplayString(TimeSpan time)
    {
      double num1 = time.TotalSeconds / 60.0;
      double num2 = num1 - num1 % 1.0;
      double num3 = time.TotalSeconds % 60.0;
      return (num2 >= 1.0 ? num2.ToString("0") + "m " : "") + num3.ToString("00") + "s";
    }

    private void CreateExtensionInSteam(ExtensionInfo info)
    {
      this.m_CreateItemResult.Set(SteamUGC.CreateItem(this.hacknetAppID, EWorkshopFileType.k_EWorkshopFileTypeFirst), (CallResult<CreateItemResult_t>.APIDispatchDelegate) null);
      this.showLoadingSpinner = true;
      this.currentTitleMessage = "Creating Extension with Steam...";
    }
  }
}
