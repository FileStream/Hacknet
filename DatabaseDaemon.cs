// Decompiled with JetBrains decompiler
// Type: Hacknet.DatabaseDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Gui;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class DatabaseDaemon : Daemon
  {
    private bool FilenameIsPersonName = false;
    private FileEntry ActiveFile = (FileEntry) null;
    private string errorMessage = "None";
    private object DeserializedFile = (object) null;
    public List<object> Dataset = (List<object>) null;
    private bool HadThemeColorApplied = true;
    private bool HasSpecialCaseDraw = false;
    private Dictionary<object, Texture2D> WildcardAssets = new Dictionary<object, Texture2D>();
    private string passwordResetHelperString = (string) null;
    public string adminResetEmailHostID = (string) null;
    public string adminResetPassEmailAccount = (string) null;
    private bool AdminEmailResetStarted = false;
    private bool AdminEmailResetHasHappened = false;
    private string passwordResetMessage = "";
    private MovingBarsEffect sideEffect;
    private DatabaseDaemon.DatabasePermissions Permissions;
    private DatabaseDaemon.DatabaseState State;
    private Folder DatasetFolder;
    private Type DataType;
    private string DataTypeIdentifier;
    private string Foldername;
    private Color ThemeColor;
    private Vector2 BlockStartTopLeft;
    private float blockSize;
    private int blocksWide;
    private int blocksHigh;
    private Texture2D PlaceholderSprite;
    private Texture2D Triangle;
    private ScrollableSectionedPanel ScrollPanel;
    private ScrollableTextRegion TextRegion;

    public DatabaseDaemon(Computer c, OS os, string name, DatabaseDaemon.DatabasePermissions permissions, string DataTypeIdentifier, string Foldername = null, Color? ThemeColor = null)
      : base(c, name, os)
    {
      this.Init(c, os, name, permissions, DataTypeIdentifier, Foldername == null ? "Database" : Foldername, !ThemeColor.HasValue ? os.highlightColor : ThemeColor.Value);
      this.HadThemeColorApplied = ThemeColor.HasValue;
    }

    public DatabaseDaemon(Computer c, OS os, string name, string permissions, string DataTypeIdentifier, string Foldername = null, Color? ThemeColor = null)
      : base(c, name, os)
    {
      this.Init(c, os, name, DatabaseDaemon.GetDatabasePermissionsFromString(permissions), DataTypeIdentifier, Foldername == null ? "Database" : Foldername, !ThemeColor.HasValue ? os.highlightColor : ThemeColor.Value);
      this.HadThemeColorApplied = ThemeColor.HasValue;
    }

    public static DatabaseDaemon.DatabasePermissions GetDatabasePermissionsFromString(string data)
    {
      DatabaseDaemon.DatabasePermissions databasePermissions = DatabaseDaemon.DatabasePermissions.Public;
      if (data.ToLower() == "private" || data.ToLower().StartsWith("admin"))
        databasePermissions = DatabaseDaemon.DatabasePermissions.AdminOnly;
      return databasePermissions;
    }

    private void Init(Computer c, OS os, string name, DatabaseDaemon.DatabasePermissions permissions, string DataTypeIdentifier, string Foldername, Color themeColor)
    {
      this.sideEffect = new MovingBarsEffect();
      this.Permissions = permissions;
      this.State = DatabaseDaemon.DatabaseState.Welcome;
      this.DataTypeIdentifier = DataTypeIdentifier;
      this.DataType = ObjectSerializer.GetTypeForName(DataTypeIdentifier);
      this.ScrollPanel = new ScrollableSectionedPanel(26, GuiData.spriteBatch.GraphicsDevice);
      this.TextRegion = new ScrollableTextRegion(GuiData.spriteBatch.GraphicsDevice);
      this.Foldername = Foldername == null ? "Database" : Foldername;
      this.ThemeColor = themeColor;
      if (DataTypeIdentifier.EndsWith("NeopalsAccount"))
      {
        this.WildcardAssets.Add((object) Neopal.PetType.Blundo, os.content.Load<Texture2D>("DLC/Sprites/Neopals/Blundo"));
        this.WildcardAssets.Add((object) Neopal.PetType.Chisha, os.content.Load<Texture2D>("DLC/Sprites/Neopals/Chisha"));
        this.WildcardAssets.Add((object) Neopal.PetType.Jubdub, os.content.Load<Texture2D>("DLC/Sprites/Neopals/Jubdub"));
        this.WildcardAssets.Add((object) Neopal.PetType.Kachici, os.content.Load<Texture2D>("DLC/Sprites/Neopals/Kachici"));
        this.WildcardAssets.Add((object) Neopal.PetType.Kyrill, os.content.Load<Texture2D>("DLC/Sprites/Neopals/Kyrill"));
        this.WildcardAssets.Add((object) Neopal.PetType.Myncl, os.content.Load<Texture2D>("DLC/Sprites/Neopals/Myncl"));
        this.WildcardAssets.Add((object) Neopal.PetType.Pageri, os.content.Load<Texture2D>("DLC/Sprites/Neopals/Pageri"));
        this.WildcardAssets.Add((object) Neopal.PetType.Psybunny, os.content.Load<Texture2D>("DLC/Sprites/Neopals/Psybunny"));
        this.WildcardAssets.Add((object) Neopal.PetType.Scorchum, os.content.Load<Texture2D>("DLC/Sprites/Neopals/Scorchum"));
        this.WildcardAssets.Add((object) Neopal.PetType.Unisam, os.content.Load<Texture2D>("DLC/Sprites/Neopals/Unisam"));
      }
      this.PlaceholderSprite = os.content.Load<Texture2D>("Sprites/Chip");
      this.Triangle = os.content.Load<Texture2D>("DLC/Sprites/Triangle");
    }

    public override void initFiles()
    {
      base.initFiles();
      this.DatasetFolder = this.comp.files.root.searchForFolder(this.Foldername);
      if (this.DatasetFolder == null)
      {
        this.DatasetFolder = new Folder(this.Foldername);
        this.comp.files.root.folders.Add(this.DatasetFolder);
      }
      this.InitDataset();
    }

    public override void loadInit()
    {
      base.loadInit();
      this.DatasetFolder = this.comp.files.root.searchForFolder(this.Foldername);
      this.DataType = ObjectSerializer.GetTypeForName(this.DataTypeIdentifier);
      switch (this.DataTypeIdentifier)
      {
        case "NeopalsAccount":
          this.HasSpecialCaseDraw = true;
          break;
      }
    }

    private void InitDataset()
    {
      if (this.Dataset != null)
      {
        this.FilenameIsPersonName = this.DataType == typeof (Person);
        for (int index = 0; index < this.Dataset.Count; ++index)
        {
          string dataEntry = this.CleanXMLForFile(ObjectSerializer.SerializeObject(this.Dataset[index]));
          string filenameForObject = this.GetFilenameForObject(this.Dataset[index]);
          try
          {
          }
          catch (Exception ex)
          {
            Console.WriteLine("Deserialization error for Generic Object: " + (object) ex);
          }
          this.DatasetFolder.files.Add(new FileEntry(dataEntry, filenameForObject));
        }
      }
      else if (this.DataTypeIdentifier.EndsWith("VehicleInfo"))
      {
        this.FilenameIsPersonName = true;
        this.DataType = People.all[0].vehicles.GetType();
        for (int index = 0; index < People.all.Count; ++index)
        {
          if (People.all[index].vehicles.Count != 0)
          {
            string dataEntry = this.CleanXMLForFile(ObjectSerializer.SerializeObject((object) People.all[index].vehicles));
            List<VehicleRegistration> vehicles = People.all[index].vehicles;
            try
            {
              string filenameForPersonName = this.GetFilenameForPersonName(People.all[index].firstName, People.all[index].lastName);
              this.DatasetFolder.files.Add(new FileEntry(dataEntry, filenameForPersonName));
            }
            catch (Exception ex)
            {
              Console.WriteLine("Deserialization error for Vehicle: " + (object) ex);
            }
          }
        }
      }
      else if (this.DataTypeIdentifier.EndsWith("NeopalsAccount"))
      {
        this.FilenameIsPersonName = false;
        this.HasSpecialCaseDraw = true;
        this.DataType = typeof (NeopalsAccount);
        if (People.all.Count == 0)
          return;
        for (int index = 0; index < People.all.Count; ++index)
        {
          if (People.all[index].NeopalsAccount != null)
          {
            string dataEntry = this.CleanXMLForFile(ObjectSerializer.SerializeObject((object) People.all[index].NeopalsAccount));
            NeopalsAccount neopalsAccount = People.all[index].NeopalsAccount;
            try
            {
              string filenameForObject = this.GetFilenameForObject((object) neopalsAccount);
              this.DatasetFolder.files.Add(new FileEntry(dataEntry, filenameForObject));
            }
            catch (Exception ex)
            {
              Console.WriteLine("Deserialization error for NeopalsAccount: " + (object) ex);
            }
          }
        }
      }
      else
      {
        if (!this.DataTypeIdentifier.EndsWith("Person"))
          return;
        this.FilenameIsPersonName = true;
        this.DataType = People.all[0].GetType();
        for (int index = 0; index < People.all.Count; ++index)
        {
          if (People.all[index].vehicles.Count != 0)
          {
            string dataEntry = this.CleanXMLForFile(ObjectSerializer.SerializeObject((object) People.all[index]));
            Person person = People.all[index];
            try
            {
              string filenameForPersonName = this.GetFilenameForPersonName(People.all[index].firstName, People.all[index].lastName);
              this.DatasetFolder.files.Add(new FileEntry(dataEntry, filenameForPersonName));
            }
            catch (Exception ex)
            {
              Console.WriteLine("Deserialization error for People: " + (object) ex);
            }
          }
        }
      }
    }

    public override void navigatedTo()
    {
      base.navigatedTo();
      this.State = DatabaseDaemon.DatabaseState.Welcome;
      this.AdminEmailResetStarted = false;
    }

    public object GetObjectForRecordName(string recordName)
    {
      for (int index = 0; index < this.DatasetFolder.files.Count; ++index)
      {
        if (this.DatasetFolder.files[index].name.ToLower().Contains(recordName.ToLower()))
          return ObjectSerializer.DeserializeObject(Utils.GenerateStreamFromString(this.DeCleanXMLForFile(this.DatasetFolder.files[index].data)), this.DataType);
      }
      return (object) null;
    }

    private void ResetAccessPassword()
    {
      this.AdminEmailResetHasHappened = true;
      try
      {
        MailServer daemon = (MailServer) Programs.getComputer(this.os, this.adminResetEmailHostID).getDaemon(typeof (MailServer));
        string userTo = this.adminResetPassEmailAccount;
        if (this.adminResetPassEmailAccount.Contains("@"))
          userTo = this.adminResetPassEmailAccount.Substring(0, this.adminResetPassEmailAccount.IndexOf("@"));
        string str = ((int) Utils.getRandomLetter()).ToString() + (object) Utils.getRandomLetter() + (object) Utils.getRandomNumberChar() + (object) Utils.getRandomLetter() + (object) Utils.getRandomNumberChar();
        this.comp.adminPass = str;
        this.comp.adminIP = this.comp.ip;
        for (int index = 0; index < this.comp.users.Count; ++index)
        {
          if (this.comp.users[index].name.ToLower() == "admin")
          {
            UserDetail user = this.comp.users[index];
            user.pass = str;
            this.comp.users[index] = user;
          }
        }
        string body = string.Format(Utils.readEntireFile("Content/DLC/Docs/DatabasePasswordResetEmail.txt"), (object) this.name, (object) str);
        daemon.addMail(MailServer.generateEmail(string.Format("{0} Password Reset", (object) this.name), body, "AdminBot"), userTo);
        this.passwordResetMessage = "Password Reset Complete";
      }
      catch (Exception ex)
      {
        this.passwordResetMessage = "CRITICAL RESET ERROR\n" + ex.ToString();
      }
    }

    private string CleanXMLForFile(string data)
    {
      return data.Replace("<", "[").Replace(">", "]").Replace("`", "_").Replace("\t", "    ");
    }

    private string DeCleanXMLForFile(string data)
    {
      return data.Replace("[", "<").Replace("]", ">");
    }

    private string GetFilenameForPersonName(string firstname, string lastname)
    {
      return Utils.GetNonRepeatingFilename((lastname + "_" + firstname).ToLower(), ".rec", this.DatasetFolder);
    }

    private string GetFilenameForObject(object obj)
    {
      return Utils.GetNonRepeatingFilename(obj.ToString().Replace(" ", "_").ToLower(), ".rec", this.DatasetFolder);
    }

    public override string getSaveString()
    {
      string str1 = this.HadThemeColorApplied ? " Color=\"" + Utils.convertColorToParseableString(this.ThemeColor) + "\"" : "";
      string str2 = "";
      if (!string.IsNullOrWhiteSpace(this.adminResetPassEmailAccount))
        str2 = string.Format(" AdminEmailAccount=\"{0}\" AdminEmailHostID=\"{1}\" ", (object) this.adminResetPassEmailAccount, (object) this.adminResetEmailHostID);
      return "<DatabaseDaemon Name=\"" + this.name + "\" Permissions=\"" + this.Permissions.ToString() + "\" DataType=\"" + this.DataTypeIdentifier + "\" Foldername=\"" + this.Foldername + "\"" + str1 + str2 + " />";
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      Rectangle dest = Utils.InsetRectangle(bounds, 2);
      this.DrawBackground(dest, sb, 6);
      switch (this.State)
      {
        case DatabaseDaemon.DatabaseState.Welcome:
          this.DrawWelcome(dest, sb);
          break;
        case DatabaseDaemon.DatabaseState.Browse:
          this.DrawBrowse(dest, sb);
          break;
        case DatabaseDaemon.DatabaseState.EntryDisplay:
          this.DrawEntry(dest, sb);
          break;
        case DatabaseDaemon.DatabaseState.Error:
          this.DrawError(dest, sb);
          break;
      }
    }

    private void DrawWelcome(Rectangle dest, SpriteBatch sb)
    {
      this.TextRegion.UpdateScroll(0.0f);
      Rectangle drawRectForRow1 = this.GetDrawRectForRow(0, 12);
      bool flag = this.Permissions == DatabaseDaemon.DatabasePermissions.Public || this.comp.adminIP == this.os.thisComputer.ip;
      TextItem.doCenteredFontLabel(drawRectForRow1, this.name, GuiData.font, Color.White, true);
      PatternDrawer.draw(new Rectangle(dest.X + 1, (int) this.BlockStartTopLeft.Y, dest.Width - 2, (int) ((double) this.blockSize * 2.0)), 0.5f, Color.Transparent, (flag ? this.ThemeColor : Utils.AddativeRed) * 0.2f, sb, flag ? PatternDrawer.thinStripe : PatternDrawer.errorTile);
      TextItem.doCenteredFontLabel(this.GetDrawRectForRow(1, 12), flag ? LocaleTerms.Loc("Access Granted") : LocaleTerms.Loc("Administrator Login Required"), GuiData.font, flag ? this.ThemeColor * 0.8f : Utils.AddativeRed * 0.8f, true);
      Rectangle drawRectForRow2 = this.GetDrawRectForRow(3, 14);
      if (flag && Button.doButton(73616101, (int) this.BlockStartTopLeft.X, drawRectForRow2.Y, dest.Width / 2, drawRectForRow2.Height, LocaleTerms.Loc("Browse Records"), new Color?(this.ThemeColor)))
        this.State = DatabaseDaemon.DatabaseState.Browse;
      drawRectForRow2.Y += drawRectForRow2.Height + 10;
      drawRectForRow2.Height = drawRectForRow2.Height / 2;
      if (Button.doButton(73616102, (int) this.BlockStartTopLeft.X, drawRectForRow2.Y, dest.Width / 2, drawRectForRow2.Height, LocaleTerms.Loc("Login"), new Color?(this.ThemeColor)))
        this.os.execute("login");
      drawRectForRow2.Y += drawRectForRow2.Height + 6;
      Color color = Color.Lerp(Color.Black, this.ThemeColor, 0.5f);
      if (!string.IsNullOrWhiteSpace(this.adminResetPassEmailAccount) && Button.doButton(73616106, (int) this.BlockStartTopLeft.X, drawRectForRow2.Y, dest.Width / 2, drawRectForRow2.Height, this.AdminEmailResetHasHappened ? LocaleTerms.Loc("Password Reset In Cooldown") : (this.AdminEmailResetStarted ? LocaleTerms.Loc("Cancel") : LocaleTerms.Loc("Reset Access Password")), new Color?(this.AdminEmailResetStarted ? (this.AdminEmailResetHasHappened ? Color.Gray : Color.DarkRed) : color)))
        this.AdminEmailResetStarted = !this.AdminEmailResetStarted;
      if (this.AdminEmailResetStarted)
      {
        Rectangle rectangle = this.GetDrawRectForRow(3, 10);
        rectangle.X += dest.Width / 2 + 10;
        rectangle.Width -= dest.Width / 2;
        rectangle.Height = Math.Max(300, (int) ((double) this.blockSize * 3.0 - 30.0));
        rectangle.Width += 30;
        if (rectangle.Height == 300)
          rectangle.Y -= (int) this.blockSize;
        Color backingColor = Color.Gray * 0.3f;
        sb.Draw(Utils.white, rectangle, Color.Black * 0.8f);
        rectangle = Utils.InsetRectangle(rectangle, 2);
        PatternDrawer.draw(rectangle, 0.5f, backingColor, backingColor * 0.4f, sb, PatternDrawer.thinStripe);
        int num = 20;
        Rectangle destinationRectangle = new Rectangle(rectangle.X - num - 2, drawRectForRow2.Y + (drawRectForRow2.Height - num) / 2, num, num);
        sb.Draw(this.Triangle, destinationRectangle, Color.Black * 0.8f);
        Rectangle dest1 = new Rectangle(rectangle.X + 6, rectangle.Y, rectangle.Width - 12, rectangle.Height / 6);
        TextItem.doCenteredFontLabel(dest1, LocaleTerms.Loc("Reset Admin Password"), GuiData.font, Color.LightGray, true);
        Rectangle dest2 = new Rectangle(dest1.X, dest1.Y + dest1.Height + 4, dest1.Width + 9, rectangle.Height / 6 * 4);
        if (this.passwordResetHelperString == null)
          this.passwordResetHelperString = Utils.readEntireFile("Content/DLC/Docs/PasswordResetText.txt");
        string data = this.passwordResetHelperString.Replace("[PASSWORD]", this.adminResetPassEmailAccount);
        if (this.AdminEmailResetHasHappened)
          data = this.passwordResetMessage;
        string text = Utils.SuperSmartTwimForWidth(data, dest2.Width, GuiData.UITinyfont);
        TextItem.doFontLabelToSize(dest2, text, GuiData.tinyfont, Color.White, true, true);
        if (!this.AdminEmailResetHasHappened && Button.doButton(73606111, rectangle.X + 4, rectangle.Y + rectangle.Height - 28, rectangle.Width - 8, 26, LocaleTerms.Loc("Reset Password"), new Color?(Color.Red)))
          this.ResetAccessPassword();
      }
      drawRectForRow2.Y += drawRectForRow2.Height + 6;
      if (!Button.doButton(73616129, (int) this.BlockStartTopLeft.X, drawRectForRow2.Y, dest.Width / 2, drawRectForRow2.Height, LocaleTerms.Loc("Exit"), new Color?(Color.Black)))
        return;
      this.os.display.command = "connect";
    }

    private void DrawError(Rectangle dest, SpriteBatch sb)
    {
      Rectangle drawRectForRow1 = this.GetDrawRectForRow(0, 12);
      bool flag = this.Permissions == DatabaseDaemon.DatabasePermissions.Public || this.comp.adminIP == this.os.thisComputer.ip;
      TextItem.doFontLabelToSize(drawRectForRow1, this.name, GuiData.font, Color.White, false, true);
      PatternDrawer.draw(new Rectangle(dest.X + 1, (int) this.BlockStartTopLeft.Y, dest.Width - 2, (int) ((double) this.blockSize * 2.0)), 0.5f, Color.Transparent, Utils.AddativeRed * 0.2f, sb, flag ? PatternDrawer.thinStripe : PatternDrawer.errorTile);
      TextItem.doFontLabelToSize(this.GetDrawRectForRow(1, 12), LocaleTerms.Loc("ERROR"), GuiData.font, Utils.AddativeRed * 0.8f, true, true);
      Rectangle drawRectForRow2 = this.GetDrawRectForRow(2, 14);
      if (Button.doButton(73616101, (int) this.BlockStartTopLeft.X, drawRectForRow2.Y, dest.Width / 2, drawRectForRow2.Height, LocaleTerms.Loc("Back"), new Color?(this.ThemeColor)))
        this.State = DatabaseDaemon.DatabaseState.Welcome;
      Rectangle rectangle = this.GetDrawRectForRow(3, 14);
      rectangle.Height = (int) (((double) this.blocksHigh - 4.0) * (double) this.blockSize);
      rectangle = Utils.InsetRectangle(rectangle, 2);
      string text = Utils.SuperSmartTwimForWidth(this.errorMessage, rectangle.Width, GuiData.smallfont);
      this.TextRegion.Draw(rectangle, text, sb, Utils.AddativeRed);
    }

    private void DrawBrowse(Rectangle dest, SpriteBatch spriteBatch)
    {
      Rectangle drawRectForRow1 = this.GetDrawRectForRow(0, 12);
      bool flag = this.Permissions == DatabaseDaemon.DatabasePermissions.Public || this.comp.adminIP == this.os.thisComputer.ip;
      TextItem.doFontLabelToSize(drawRectForRow1, string.Format(LocaleTerms.Loc("{0} : Records"), (object) this.name), GuiData.font, Color.White, true, true);
      Rectangle drawRectForRow2 = this.GetDrawRectForRow(0, 0);
      PatternDrawer.draw(drawRectForRow2, 0.5f, Color.Transparent, this.ThemeColor * 0.2f, spriteBatch, PatternDrawer.thinStripe);
      Rectangle rectangle = new Rectangle(drawRectForRow2.X + 4, drawRectForRow2.Y + drawRectForRow2.Height / 2 + 12, drawRectForRow2.Width / 3, drawRectForRow2.Height / 2 - 15);
      if (Button.doButton(71118000, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("Back"), new Color?(Color.Gray)))
        this.State = DatabaseDaemon.DatabaseState.Welcome;
      if (this.DataType == (Type) null || string.IsNullOrWhiteSpace(this.DataTypeIdentifier) || this.DataTypeIdentifier.ToLower() == "none")
      {
        Rectangle drawRectForRow3 = this.GetDrawRectForRow(1, 2);
        drawRectForRow3.Height += (int) ((double) this.blockSize * (double) (this.blocksHigh - 3));
        PatternDrawer.draw(drawRectForRow3, 0.4f, Color.Black * 0.3f, Utils.makeColorAddative(this.ThemeColor) * 0.25f, spriteBatch, PatternDrawer.binaryTile);
        TextItem.doCenteredFontLabel(this.GetDrawRectForRow(3, 8), " - " + LocaleTerms.Loc("API Access Enabled") + " - ", GuiData.smallfont, Color.White, false);
      }
      else
      {
        Rectangle ListDest = this.GetDrawRectForRow(1, 4);
        ListDest.Height += (int) ((double) this.blockSize * ((double) this.blocksHigh - 2.5));
        this.ScrollPanel.NumberOfPanels = this.DatasetFolder.files.Count;
        this.ScrollPanel.Draw((Action<int, Rectangle, SpriteBatch>) ((i, bounds, sb) =>
        {
          FileEntry file = this.DatasetFolder.files[i];
          string text = "REC#" + i.ToString("000") + " : " + this.GetAnnounceNameForFileName(file.name);
          int width = Math.Max(160, bounds.Width / 3);
          if (Button.doButton(71118100 + i, bounds.X, bounds.Y + 2, width, bounds.Height - 4, string.Format(LocaleTerms.Loc("View Record #{0}"), (object) i.ToString("000")), new Color?(this.ThemeColor)))
          {
            this.ActiveFile = this.DatasetFolder.files[i];
            this.State = DatabaseDaemon.DatabaseState.EntryDisplay;
            this.DeserializedFile = (object) null;
          }
          Vector2 vector2 = GuiData.smallfont.MeasureString(text);
          Rectangle destinationRectangle = new Rectangle(bounds.X + bounds.Width / 3 + 6, bounds.Y + bounds.Height / 2 - 1, ListDest.Width - (width + (int) vector2.X + 26), 1);
          sb.Draw(Utils.white, destinationRectangle, Color.White * 0.1f);
          TextItem.doFontLabel(new Vector2((float) (destinationRectangle.X + destinationRectangle.Width) + 6f, (float) bounds.Y + 2f), text, GuiData.smallfont, new Color?(Color.White), (float) bounds.Width, (float) bounds.Height, false);
        }), spriteBatch, ListDest);
      }
    }

    private void DrawEntry(Rectangle dest, SpriteBatch spriteBatch)
    {
      if (this.DeserializedFile == null)
      {
        try
        {
          this.DeserializedFile = ObjectSerializer.DeserializeObject(Utils.GenerateStreamFromString(this.DeCleanXMLForFile(this.ActiveFile.data)), this.DataType);
        }
        catch (Exception ex)
        {
          this.State = DatabaseDaemon.DatabaseState.Error;
          this.errorMessage = LocaleTerms.Loc("Error De-serializing Entry") + " : " + this.ActiveFile.name + "\n" + Utils.GenerateReportFromException(ex).Replace("Hacknet", "Database");
          return;
        }
      }
      Rectangle drawRectForRow = this.GetDrawRectForRow(0, 0);
      drawRectForRow.Height = drawRectForRow.Height / 2;
      TextItem.doFontLabelToSize(drawRectForRow, string.Format(LocaleTerms.Loc("Entry for {0}"), (object) this.GetAnnounceNameForFileName(this.ActiveFile.name)), GuiData.font, Color.White, true, true);
      drawRectForRow.Y += drawRectForRow.Height;
      if (Button.doButton(7301991, drawRectForRow.X, drawRectForRow.Y, drawRectForRow.Width / 2, drawRectForRow.Height, LocaleTerms.Loc("Back"), new Color?(Color.Gray)))
      {
        this.State = DatabaseDaemon.DatabaseState.Browse;
        this.ActiveFile = (FileEntry) null;
        this.DeserializedFile = (object) null;
      }
      else
      {
        Rectangle bounds = new Rectangle((int) this.BlockStartTopLeft.X, (int) this.BlockStartTopLeft.Y + (int) this.blockSize, (int) ((double) this.blockSize * (double) (this.blocksWide - 1)), (int) ((double) this.blockSize * (double) (this.blocksHigh - 2)));
        if (this.HasSpecialCaseDraw)
          ReflectiveRenderer.PreRenderForObject += (Action<Vector2, Type, string>) ((pos, targetType, targetValue) => this.DrawSpecialCase(pos, bounds, targetType, targetValue, spriteBatch));
        ReflectiveRenderer.RenderObject(this.DeserializedFile, bounds, spriteBatch, this.ScrollPanel, this.ThemeColor);
        if (this.HasSpecialCaseDraw)
          ReflectiveRenderer.PreRenderForObject = (Action<Vector2, Type, string>) null;
      }
    }

    private string GetAnnounceNameForFileName(string filename)
    {
      filename = filename.Replace(".rec", "");
      if (!this.FilenameIsPersonName)
        return filename;
      string[] strArray = filename.Split(new char[1]{ '_' }, StringSplitOptions.None);
      return strArray[1] + " " + strArray[0];
    }

    private Rectangle GetDrawRectForRow(int row, int inset)
    {
      return Utils.InsetRectangle(new Rectangle((int) this.BlockStartTopLeft.X, (int) ((double) this.BlockStartTopLeft.Y + (double) this.blockSize * (double) row), (int) ((double) this.blockSize * (double) (this.blocksWide - 1)), (int) this.blockSize), inset);
    }

    private void DrawBackground(Rectangle dest, SpriteBatch sb, int desiredNumOfBlocks)
    {
      Color color = Color.Gray * 0.4f;
      int num1 = dest.Width / (desiredNumOfBlocks + 2);
      int width = 9;
      float num2 = 1f;
      int num3 = dest.X + num1 / 2;
      int num4 = dest.Y + num1 / 2;
      this.BlockStartTopLeft = new Vector2((float) num3, (float) num4);
      this.blocksWide = this.blocksHigh = 0;
      this.blockSize = (float) num1;
      do
      {
        this.blocksWide = 0;
        do
        {
          Rectangle destinationRectangle1 = new Rectangle(Math.Max(dest.X, num3 - width / 2), num4 - (int) ((double) num2 / 2.0 + 0.5), width, (int) num2);
          sb.Draw(Utils.white, destinationRectangle1, color);
          Rectangle destinationRectangle2 = new Rectangle(num3 - (int) ((double) num2 / 2.0), Math.Max(dest.Y, num4 - (int) ((double) width / 2.0 + 0.5)), (int) num2, width / 2 - (int) ((double) num2 / 2.0));
          sb.Draw(Utils.white, destinationRectangle2, color);
          Rectangle destinationRectangle3 = new Rectangle(num3 - (int) ((double) num2 / 2.0), num4 + (int) ((double) num2 / 2.0), (int) num2, width / 2 - (int) ((double) num2 / 2.0));
          sb.Draw(Utils.white, destinationRectangle3, color);
          num3 += num1;
          ++this.blocksWide;
        }
        while (num3 <= dest.X + dest.Width - num1 / 2);
        num3 = dest.X + num1 / 2;
        num4 += num1;
        ++this.blocksHigh;
      }
      while (num4 <= dest.Y + dest.Height - num1 / 2);
    }

    public void DrawSpecialCase(Vector2 currentPos, Rectangle totalArea, Type objType, string drawnValue, SpriteBatch sb)
    {
      if (!(sb.Name != "AltRTBatch") || !(objType == typeof (Neopal.PetType)))
        return;
      int num1 = Math.Min(ReflectiveRenderer.GetEntryLineHeight() * 6, totalArea.Width / 4);
      Texture2D sprite = this.PlaceholderSprite;
      Neopal.PetType petType = (Neopal.PetType) Enum.Parse(objType, drawnValue);
      if (this.WildcardAssets.ContainsKey((object) petType))
        sprite = this.WildcardAssets[(object) petType];
      Rectangle petDrawPos = new Rectangle(totalArea.X + totalArea.Width - num1 - 10, (int) currentPos.Y + 4, num1, num1);
      double num2 = (double) sprite.Height / (double) petDrawPos.Height;
      int val2 = totalArea.Y + totalArea.Height - petDrawPos.Y;
      int height = Math.Min(sprite.Height, (int) (num2 * (double) val2));
      Rectangle? clip = new Rectangle?(new Rectangle(0, 0, sprite.Width, height));
      petDrawPos.Height = Math.Min(petDrawPos.Height, val2);
      this.os.postFXDrawActions += (Action) (() => sb.Draw(sprite, petDrawPos, clip, Color.White));
    }

    public enum DatabasePermissions
    {
      AdminOnly,
      Public,
    }

    private enum DatabaseState
    {
      Welcome,
      Search,
      Browse,
      Loading,
      EntryDisplay,
      Error,
    }
  }
}
