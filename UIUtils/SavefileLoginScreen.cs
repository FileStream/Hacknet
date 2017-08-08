// Decompiled with JetBrains decompiler
// Type: Hacknet.UIUtils.SavefileLoginScreen
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Hacknet.PlatformAPI.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hacknet.UIUtils
{
  public class SavefileLoginScreen
  {
    private static Color CancelColor = new Color(125, 82, 82);
    private string terminalString = "";
    private List<string> History = new List<string>();
    private string currentPrompt = "USERNAME :";
    private int promptIndex = 0;
    public string ProjectName = "Hacknet";
    private List<string> PromptSequence = new List<string>();
    private List<string> Answers = new List<string>();
    private bool IsReady = false;
    private bool IsNewAccountMode = true;
    private bool InPasswordMode = false;
    private bool CanReturnEnter = false;
    private string userPathCache = (string) null;
    public bool DrawFromTop = false;
    private bool HasOverlayScreen = false;
    private bool PreventAdvancing = false;
    public Action<string, string> StartNewGameForUsernameAndPass;
    public Action<string, string> LoadGameForUserFileAndUsername;
    public Action RequestGoBack;

    public void WriteToHistory(string message)
    {
      this.History.Add(message);
    }

    public void ClearTextBox()
    {
      GuiData.getFilteredKeys();
      this.terminalString = "";
      TextBox.cursorPosition = 0;
    }

    public void ResetForNewAccount()
    {
      this.promptIndex = 0;
      this.IsReady = false;
      this.PromptSequence.Clear();
      this.PromptSequence.Add(LocaleTerms.Loc("USERNAME") + " :");
      this.PromptSequence.Add(LocaleTerms.Loc("PASSWORD") + " :");
      this.PromptSequence.Add(LocaleTerms.Loc("CONFIRM PASS") + " :");
      this.History.Clear();
      this.History.Add("-- " + LocaleTerms.Loc("New " + this.ProjectName + " User Registration") + " --");
      this.currentPrompt = this.PromptSequence[this.promptIndex];
      this.IsNewAccountMode = true;
      this.terminalString = "";
      TextBox.cursorPosition = 0;
    }

    public void ResetForLogin()
    {
      this.promptIndex = 0;
      this.IsReady = false;
      this.PromptSequence.Clear();
      this.PromptSequence.Add(LocaleTerms.Loc("USERNAME") + " :");
      this.PromptSequence.Add(LocaleTerms.Loc("PASSWORD") + " :");
      this.History.Clear();
      this.History.Add("-- Login --");
      this.currentPrompt = this.PromptSequence[this.promptIndex];
      this.IsNewAccountMode = false;
      this.terminalString = "";
      TextBox.cursorPosition = 0;
    }

    private void Advance(string answer)
    {
      ++this.promptIndex;
      this.Answers.Add(answer);
      if (this.IsNewAccountMode)
      {
        if (this.promptIndex == 1)
        {
          if (string.IsNullOrWhiteSpace(this.Answers[0]))
          {
            this.History.Add(" -- " + LocaleTerms.Loc("Username cannot be blank. Try Again") + " -- ");
            this.promptIndex = 0;
            this.Answers.Clear();
          }
          else if (Utils.StringContainsInvalidFilenameChars(this.Answers[0]))
          {
            this.History.Add(" -- " + LocaleTerms.Loc("Username contains invalid characters. Try Again") + " -- ");
            this.promptIndex = 0;
            this.Answers.Clear();
          }
          else if (!SaveFileManager.CanCreateAccountForName(this.Answers[0]))
          {
            this.History.Add(" -- " + LocaleTerms.Loc("Username already in use. Try Again") + " -- ");
            this.promptIndex = 0;
            this.Answers.Clear();
          }
        }
        if (this.promptIndex == 3)
        {
          if (string.IsNullOrWhiteSpace(answer))
          {
            this.History.Add(" -- " + LocaleTerms.Loc("Password Cannot be Blank! Try Again") + " -- ");
            this.promptIndex = 1;
            string answer1 = this.Answers[0];
            this.Answers.Clear();
            this.Answers.Add(answer1);
          }
          else if (this.Answers[1] != answer)
          {
            this.History.Add(" -- " + LocaleTerms.Loc("Password Mismatch! Try Again") + " -- ");
            this.promptIndex = 1;
            string answer1 = this.Answers[0];
            this.Answers.Clear();
            this.Answers.Add(answer1);
          }
        }
        this.InPasswordMode = this.promptIndex == 1 || this.promptIndex == 2;
      }
      else
        this.InPasswordMode = this.promptIndex == 1;
      if (this.promptIndex >= this.PromptSequence.Count)
      {
        if (this.IsNewAccountMode)
        {
          this.History.Add(" -- " + LocaleTerms.Loc("Details Confirmed") + " -- ");
          this.History.Add(LocaleTerms.Loc("WARNING") + " : " + LocaleTerms.Loc("Once created, a session's language cannot be changed"));
          this.currentPrompt = LocaleTerms.Loc("READY - PRESS ENTER TO CONFIRM");
          this.IsReady = true;
        }
        else
        {
          string filePathForLogin = SaveFileManager.GetFilePathForLogin(this.Answers[0], this.Answers[1]);
          this.userPathCache = filePathForLogin;
          if (filePathForLogin == null)
          {
            this.promptIndex = 0;
            this.currentPrompt = this.PromptSequence[this.promptIndex];
            this.History.Add(" -- " + LocaleTerms.Loc("Invalid Login Details") + " -- ");
          }
          else
          {
            this.IsReady = true;
            this.currentPrompt = LocaleTerms.Loc("READY - PRESS ENTER TO CONFIRM");
          }
        }
      }
      else
        this.currentPrompt = this.PromptSequence[this.promptIndex];
    }

    public void Draw(SpriteBatch sb, Rectangle dest)
    {
      int width = 300;
      Rectangle rectangle1 = dest;
      Rectangle rectangle2 = new Rectangle(dest.X, dest.Y, dest.Width - width, dest.Height);
      if (!this.IsNewAccountMode)
        dest = rectangle2;
      SpriteFont smallfont = GuiData.smallfont;
      int num1 = (int) ((double) smallfont.MeasureString(this.currentPrompt).X + 4.0);
      int num2 = this.DrawFromTop ? dest.Y : dest.Y + dest.Height - 18;
      GuiData.spriteBatch.DrawString(smallfont, this.currentPrompt, new Vector2((float) dest.X, (float) num2), Color.White);
      if (!this.IsReady)
      {
        TextBox.MaskingText = this.InPasswordMode;
        this.terminalString = TextBox.doTerminalTextField(16392802, dest.X + num1, num2 - 2, dest.Width, 20, 1, this.terminalString, GuiData.UISmallfont);
      }
      if (!this.IsNewAccountMode)
      {
        Vector2 pos = new Vector2((float) (rectangle1.X + rectangle2.Width), (float) rectangle1.Y);
        if (SaveFileManager.Accounts.Count > 0)
        {
          TextItem.doFontLabel(pos, LocaleTerms.Loc("LOCAL ACCOUNTS") + " ::", GuiData.font, new Color?(Color.Gray), (float) width, 22f, false);
          pos.Y += 22f;
        }
        if (!this.HasOverlayScreen)
        {
          for (int index1 = 0; index1 < SaveFileManager.Accounts.Count; ++index1)
          {
            if (Button.doButton(2870300 + index1 + index1 * 12, (int) pos.X, (int) pos.Y, width, 18, SaveFileManager.Accounts[index1].Username, new Color?(Color.Black)))
            {
              this.Answers = new List<string>((IEnumerable<string>) new string[2]
              {
                SaveFileManager.Accounts[index1].Username,
                SaveFileManager.Accounts[index1].Password
              });
              this.promptIndex = 2;
              TextBox.BoxWasActivated = true;
              this.IsReady = true;
              break;
            }
            int index = index1;
            if (Button.doButton(7070300 + index1 + index1 * 12, (int) pos.X + width + 4, (int) pos.Y, 21, 18, "X", new Color?(Color.DarkRed)))
            {
              MessageBoxScreen messageBoxScreen = new MessageBoxScreen(Utils.SuperSmartTwimForWidth(string.Format(LocaleTerms.Loc("Are you sure you wish to delete account {0}?"), (object) ("\"" + SaveFileManager.Accounts[index1].Username + "\"")), 400, GuiData.font));
              messageBoxScreen.OverrideAcceptedText = LocaleTerms.Loc("Delete Account");
              messageBoxScreen.OverrideCancelText = LocaleTerms.Loc("Cancel");
              messageBoxScreen.AcceptedClicked += (Action) (() =>
              {
                this.HasOverlayScreen = false;
                SaveFileManager.DeleteUser(SaveFileManager.Accounts[index].Username);
              });
              messageBoxScreen.CancelClicked += (Action) (() => this.HasOverlayScreen = false);
              Game1.getSingleton().sman.AddScreen((GameScreen) messageBoxScreen);
              this.HasOverlayScreen = true;
            }
            pos.Y += 22f;
          }
        }
      }
      if (!this.HasOverlayScreen && TextBox.BoxWasActivated)
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(this.currentPrompt);
        stringBuilder.Append(" ");
        string str1 = this.terminalString;
        if (this.InPasswordMode)
        {
          string str2 = "";
          for (int index = 0; index < str1.Length; ++index)
            str2 += "*";
          str1 = str2;
        }
        stringBuilder.Append(str1);
        this.History.Add(stringBuilder.ToString());
        this.Advance(this.terminalString);
        this.terminalString = "";
        TextBox.cursorPosition = 0;
        TextBox.BoxWasActivated = false;
      }
      int y1 = this.DrawFromTop ? dest.Y + 24 : dest.Y + dest.Height + 12;
      GuiData.spriteBatch.Draw(Utils.white, new Rectangle(dest.X, y1, dest.Width / 2, 1), Utils.SlightlyDarkGray);
      int y2 = y1 + 10;
      int num3 = y2 - 60;
      if (this.IsReady)
      {
        if (!GuiData.getKeyboadState().IsKeyDown(Keys.Enter))
          this.CanReturnEnter = true;
        if ((!this.HasOverlayScreen && (!this.IsNewAccountMode || Button.doButton(16392804, dest.X, y2, dest.Width / 3, 28, LocaleTerms.Loc("CONFIRM"), new Color?(Color.White))) || this.CanReturnEnter && Utils.keyPressed(GuiData.lastInput, Keys.Enter, new PlayerIndex?())) && !this.PreventAdvancing)
        {
          if (this.IsNewAccountMode)
          {
            if (this.Answers.Count < 3)
            {
              this.ResetForNewAccount();
            }
            else
            {
              string answer1 = this.Answers[0];
              string answer2 = this.Answers[1];
              TextBox.MaskingText = false;
              if (this.StartNewGameForUsernameAndPass != null)
                this.StartNewGameForUsernameAndPass(answer1, answer2);
            }
          }
          else
          {
            TextBox.MaskingText = false;
            if (this.LoadGameForUserFileAndUsername != null)
              this.LoadGameForUserFileAndUsername(this.userPathCache, this.Answers[0]);
            this.History.Clear();
            this.currentPrompt = "";
          }
          this.PreventAdvancing = true;
        }
        y2 += 36;
      }
      if (!this.HasOverlayScreen && Button.doButton(16392806, dest.X, this.DrawFromTop ? num3 : y2, dest.Width / 3, 22, LocaleTerms.Loc("CANCEL"), new Color?(SavefileLoginScreen.CancelColor)) && this.RequestGoBack != null)
      {
        this.InPasswordMode = false;
        TextBox.MaskingText = false;
        this.RequestGoBack();
      }
      float num4 = GuiData.ActiveFontConfig.tinyFontCharHeight + 8f;
      Vector2 position = new Vector2((float) dest.X, this.DrawFromTop ? (float) y2 : (float) (dest.Y + dest.Height - 20) - num4);
      for (int index = this.History.Count - 1; index >= 0; --index)
      {
        sb.DrawString(GuiData.UISmallfont, this.History[index], position, Color.White);
        position.Y -= num4 * (this.DrawFromTop ? -1f : 1f);
      }
    }
  }
}
