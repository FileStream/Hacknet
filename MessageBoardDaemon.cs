// Decompiled with JetBrains decompiler
// Type: Hacknet.MessageBoardDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class MessageBoardDaemon : AuthenticatingDaemon, IMonitorableDaemon
  {
    private static Color UsernameColor = new Color(17, 119, 67);
    private static Color ImplicationColor = new Color(56, 184, 131);
    private string boardsListingString = "[a/b/c/d/e/f/g/gif/h/hr/k/m/o/p/r/s/t/u/v/vg/vr/w/wg][i/ic][r9k][s4s][cm/hm/lgbt/y][3/adv/an/asp/cgl/ck/co/diy/fa/fit/gd/hc/int/jp/lit/mlp/mu/n/out/po/pol/sci/soc/sp/tg/toy/trv/tv/vp/wsg/x][rs]";
    public string BoardName = "/el/ - " + LocaleTerms.Loc("Digital Security");
    public List<string> ThreadsToAdd = new List<string>();
    private MessageBoardDaemon.MessageBoardState state = MessageBoardDaemon.MessageBoardState.Board;
    private Vector2 ThreadScrollPosition = Vector2.Zero;
    private int CurrentThreadHeight = 100;
    private const int THREAD_PREVIEW_HEIGHT = 415;
    private static Dictionary<MessageBoardPostImage, Texture2D> Images;
    private ScrollableSectionedPanel threadsPanel;
    private MessageBoardThread viewingThread;
    private Folder rootFolder;
    private Folder threadsFolder;
    public Action<string, string> MessageAdded;

    public MessageBoardDaemon(Computer c, OS os)
      : base(c, LocaleTerms.Loc("Message Board"), os)
    {
      if (MessageBoardDaemon.Images == null)
      {
        MessageBoardDaemon.Images = new Dictionary<MessageBoardPostImage, Texture2D>();
        MessageBoardDaemon.Images.Add(MessageBoardPostImage.Academic, os.content.Load<Texture2D>("Sprites/Academic_Logo"));
        MessageBoardDaemon.Images.Add(MessageBoardPostImage.Sun, os.content.Load<Texture2D>("Sprites/Sun"));
        MessageBoardDaemon.Images.Add(MessageBoardPostImage.Snake, os.content.Load<Texture2D>("Sprites/Snake"));
        MessageBoardDaemon.Images.Add(MessageBoardPostImage.Circle, os.content.Load<Texture2D>("CircleOutline"));
        MessageBoardDaemon.Images.Add(MessageBoardPostImage.Duck, os.content.Load<Texture2D>("Sprites/Duck"));
        MessageBoardDaemon.Images.Add(MessageBoardPostImage.Page, os.content.Load<Texture2D>("Sprites/Page"));
        MessageBoardDaemon.Images.Add(MessageBoardPostImage.Speech, os.content.Load<Texture2D>("Sprites/SpeechBubble"));
        MessageBoardDaemon.Images.Add(MessageBoardPostImage.Mod, os.content.Load<Texture2D>("Sprites/Hammer"));
        MessageBoardDaemon.Images.Add(MessageBoardPostImage.Chip, os.content.Load<Texture2D>("Sprites/Chip"));
      }
      this.threadsPanel = new ScrollableSectionedPanel(415, GuiData.spriteBatch.GraphicsDevice);
    }

    public override void initFiles()
    {
      base.initFiles();
      this.rootFolder = new Folder("ImageBoard");
      this.threadsFolder = new Folder("Threads");
      this.rootFolder.folders.Add(this.threadsFolder);
      this.comp.files.root.folders.Add(this.rootFolder);
      for (int index = 0; index < this.ThreadsToAdd.Count; ++index)
        this.AddThread(this.ThreadsToAdd[index]);
      this.ThreadsToAdd.Clear();
    }

    public override void loadInit()
    {
      base.loadInit();
      this.rootFolder = this.comp.files.root.searchForFolder("ImageBoard");
      this.threadsFolder = this.rootFolder.searchForFolder("Threads");
    }

    public void SubscribeToAlertActionFroNewMessage(Action<string, string> act)
    {
      this.MessageAdded += act;
    }

    public void UnSubscribeToAlertActionFroNewMessage(Action<string, string> act)
    {
      this.MessageAdded -= act;
    }

    public bool ShouldDisplayNotifications()
    {
      return true;
    }

    public string GetName()
    {
      return this.name;
    }

    public override string getSaveString()
    {
      return "<MessageBoard name=\"" + this.name + "\" boardName=\"" + this.BoardName + "\"/>";
    }

    public void AddThread(string threadData)
    {
      if (this.threadsFolder == null)
      {
        this.ThreadsToAdd.Add(threadData);
      }
      else
      {
        threadData = ComputerLoader.filter(threadData);
        string str;
        do
        {
          str = Utils.getRandomByte().ToString("000") + Utils.getRandomByte().ToString("000") + Utils.getRandomByte().ToString("000") + ".tm";
        }
        while (this.threadsFolder.searchForFile(str) != null);
        if (this.MessageAdded != null)
          this.MessageAdded("anon", threadData);
        this.threadsFolder.files.Add(new FileEntry(threadData, str));
      }
    }

    public MessageBoardThread ParseThread(string threadData)
    {
      try
      {
        string[] strArray = threadData.Split(new string[3]{ "------------------------------------------\r\n", "------------------------------------------\n", "------------------------------------------" }, StringSplitOptions.None);
        string str1 = strArray[0].Replace("\n", "").Replace("\r", "");
        MessageBoardThread messageBoardThread = new MessageBoardThread() { id = str1, posts = new List<MessageBoardPost>() };
        for (int index = 1; index < strArray.Length; ++index)
        {
          if (strArray[index].Length > 1)
          {
            MessageBoardPost messageBoardPost = new MessageBoardPost();
            string str2 = strArray[index];
            if (strArray[index].StartsWith("#"))
            {
              string str3 = strArray[index].Substring(1, strArray[index].IndexOf('\n'));
              try
              {
                MessageBoardPostImage messageBoardPostImage = (MessageBoardPostImage) Enum.Parse(typeof (MessageBoardPostImage), str3);
                messageBoardPost.img = messageBoardPostImage;
                str2 = strArray[index].Substring(strArray[index].IndexOf('\n') + 1);
              }
              catch (ArgumentException ex)
              {
                messageBoardPost.img = MessageBoardPostImage.None;
              }
            }
            else
              messageBoardPost.img = MessageBoardPostImage.None;
            messageBoardPost.text = str2;
            messageBoardThread.posts.Add(messageBoardPost);
          }
        }
        return messageBoardThread;
      }
      catch (Exception ex)
      {
        MessageBoardThread messageBoardThread = new MessageBoardThread();
        messageBoardThread.id = "error";
        // ISSUE: explicit reference operation
        // ISSUE: explicit reference operation
        (^@messageBoardThread).posts = new List<MessageBoardPost>((IEnumerable<MessageBoardPost>) new MessageBoardPost[1]
        {
          new MessageBoardPost()
          {
            img = MessageBoardPostImage.None,
            text = "-- Error Parsing Thread --"
          }
        });
        return messageBoardThread;
      }
    }

    public void ViewThread(MessageBoardThread thread, int width, int margin, int ImageSize, int headerOffset)
    {
      this.CurrentThreadHeight = 20;
      for (int index = 0; index < thread.posts.Count; ++index)
        this.CurrentThreadHeight += this.MeasurePost(thread.posts[index], width, margin, ImageSize, headerOffset) + margin * 2;
      this.state = MessageBoardDaemon.MessageBoardState.Thread;
      this.viewingThread = thread;
      this.ThreadScrollPosition = Vector2.Zero;
    }

    public override void navigatedTo()
    {
      base.navigatedTo();
      this.state = MessageBoardDaemon.MessageBoardState.Board;
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      int height = 60;
      Rectangle dest = new Rectangle(bounds.X, bounds.Y, bounds.Width, height);
      this.DrawHeader(sb, dest);
      Rectangle rectangle = bounds;
      rectangle.Y += height;
      rectangle.Height -= height + 1;
      switch (this.state)
      {
        case MessageBoardDaemon.MessageBoardState.Thread:
          this.DrawFullThreadView(sb, this.viewingThread, rectangle);
          break;
        default:
          this.threadsPanel.NumberOfPanels = this.threadsFolder.files.Count;
          this.threadsPanel.Draw((Action<int, Rectangle, SpriteBatch>) ((index, drawArea, sBatch) => this.DrawThread(this.ParseThread(this.threadsFolder.files[index].data), sBatch, drawArea, true)), sb, rectangle);
          break;
      }
    }

    private void DrawHeader(SpriteBatch sb, Rectangle dest)
    {
      int num1 = 4;
      int width = 200;
      sb.Draw(Utils.white, new Rectangle(dest.X + num1, dest.Y + 4, dest.Width - num1 * 2, 1), Color.White * 0.5f);
      Vector2 position = new Vector2((float) (dest.X + num1), (float) (dest.Y + 5));
      int index = 0;
      int num2 = 7;
      while ((double) position.X + (double) (num2 * 2) < (double) (dest.X + dest.Width) && index < this.boardsListingString.Length)
      {
        sb.DrawString(GuiData.detailfont, string.Concat((object) this.boardsListingString[index]), position, Color.White * 0.6f);
        ++index;
        position.X += (float) num2;
      }
      sb.DrawString(GuiData.detailfont, "]", position, Color.White * 0.8f);
      try
      {
        if (this.BoardName == null)
          this.BoardName = this.name;
        TextItem.doFontLabel(new Vector2((float) (dest.X + num1), (float) (dest.Y + 22)), this.BoardName, GuiData.font, new Color?(this.os.highlightColor), (float) (dest.Width - (width + 6 + 22)), (float) dest.Height, false);
      }
      catch (Exception ex)
      {
      }
      if (this.state != MessageBoardDaemon.MessageBoardState.Board && Button.doButton(1931655802, dest.X + dest.Width - width - 6, dest.Y + dest.Height / 2 - 4, width, 24, LocaleTerms.Loc("Back to Board"), new Color?(this.os.highlightColor)))
        this.state = MessageBoardDaemon.MessageBoardState.Board;
      sb.Draw(Utils.white, new Rectangle(dest.X + num1, dest.Y + dest.Height - 2, dest.Width - num1 * 2, 1), Color.White * 0.5f);
    }

    private void DrawFullThreadView(SpriteBatch sb, MessageBoardThread thread, Rectangle dest)
    {
      Rectangle rectangle = dest;
      rectangle.Height = this.CurrentThreadHeight + 50;
      bool flag = this.CurrentThreadHeight > dest.Height;
      if (flag)
      {
        ScrollablePanel.beginPanel(1931655001, rectangle, this.ThreadScrollPosition);
        rectangle.X = rectangle.Y = 0;
      }
      this.DrawThread(thread, GuiData.spriteBatch, rectangle, false);
      if (!flag)
        return;
      float maxScroll = (float) Math.Max(dest.Height, this.CurrentThreadHeight - dest.Height);
      this.ThreadScrollPosition = ScrollablePanel.endPanel(1931655001, this.ThreadScrollPosition, dest, maxScroll, false);
    }

    private void DrawThread(MessageBoardThread thread, SpriteBatch sb, Rectangle bounds, bool isPreview = false)
    {
      int num1 = 4;
      int margin = 8;
      int num2 = 450;
      int height = 36;
      int num3 = 20;
      int ImageSize = 80;
      int ThreadFooterSize = 30;
      SpriteFont tinyfont = GuiData.tinyfont;
      Rectangle dest = new Rectangle(bounds.X + margin, bounds.Y + margin, num2, height);
      int y = bounds.Y;
      List<MessageBoardPost> messageBoardPostList = thread.posts;
      if (isPreview)
        messageBoardPostList = this.GetLastPostsToFitHeight(thread, 415 - ThreadFooterSize, bounds.Width, margin, ImageSize, num3, ThreadFooterSize, int.MaxValue);
      for (int index = 0; index < messageBoardPostList.Count; ++index)
      {
        MessageBoardPost messageBoardPost = messageBoardPostList[index];
        int width = bounds.Width - 4 * margin;
        if (messageBoardPost.img != MessageBoardPostImage.None)
          width -= ImageSize;
        string text1 = Utils.SmartTwimForWidth(messageBoardPost.text, width, tinyfont);
        Vector2 vector2 = tinyfont.MeasureString(text1);
        vector2.Y *= 1.3f;
        dest.Y = y;
        int val2 = (int) vector2.X + margin * 4;
        if (messageBoardPost.img != MessageBoardPostImage.None)
          val2 += ImageSize + margin;
        dest.Width = Math.Max(num2, val2);
        dest.Height = (int) vector2.Y + 2 * margin;
        dest.Width = Math.Max(dest.Width, ImageSize + 2 * margin);
        if (messageBoardPost.img != MessageBoardPostImage.None)
          dest.Height = Math.Max(dest.Height, ImageSize + 2 * margin);
        dest.Height += num3 + margin * 2;
        this.DrawPost(text1, messageBoardPost.img, dest, margin, ImageSize, num3, sb, tinyfont);
        y += dest.Height + num1;
        if (index == 0 && isPreview)
        {
          MessageBoardThread thread1 = thread;
          string text2 = "[+] " + string.Format(LocaleTerms.Loc("{0} posts and image replies omitted"), (object) (thread.posts.Count - messageBoardPostList.Count));
          string text3 = LocaleTerms.Loc("Click here to view.");
          TextItem.doFontLabel(new Vector2((float) dest.X, (float) y), text2, GuiData.tinyfont, new Color?(this.os.lightGray), 288f, 18f, true);
          if (Button.doButton(17839000 + thread.id.GetHashCode(), dest.X + 290, y - 2, bounds.Width - 316, 17, text3, new Color?()))
          {
            Console.WriteLine("clicked " + (object) index);
            this.ViewThread(thread1, bounds.Width, margin, ImageSize, num3);
          }
          y += 16 + num1;
        }
      }
      sb.Draw(Utils.white, new Rectangle(bounds.X + margin, bounds.Y + bounds.Height - 6, bounds.Width - margin * 2, 1), Color.White * 0.5f);
    }

    private int MeasurePost(MessageBoardPost post, int width, int margin, int ImageSize, int postHeaderOffset)
    {
      SpriteFont tinyfont = GuiData.tinyfont;
      int num = postHeaderOffset;
      int width1 = width - 4 * margin;
      if (post.img != MessageBoardPostImage.None)
        width1 -= ImageSize;
      string text = Utils.SmartTwimForWidth(post.text, width1, tinyfont);
      Vector2 vector2 = tinyfont.MeasureString(text);
      vector2.Y *= 1.3f;
      return Math.Max(num + (int) vector2.Y + 2 * margin, ImageSize + 2 * margin);
    }

    private void DrawPost(string text, MessageBoardPostImage img, Rectangle dest, int margin, int ImageSize, int postheaderOffset, SpriteBatch sb, SpriteFont font)
    {
      sb.Draw(Utils.white, dest, this.os.highlightColor * 0.2f);
      Vector2 vector2_1 = new Vector2((float) (dest.X + margin), (float) (dest.Y + margin));
      sb.Draw(Utils.white, new Rectangle((int) vector2_1.X, (int) vector2_1.Y, postheaderOffset - 5, postheaderOffset - 5), this.os.indentBackgroundColor);
      sb.DrawString(GuiData.smallfont, LocaleTerms.Loc("Anonymous"), vector2_1 + new Vector2((float) postheaderOffset, -2f), MessageBoardDaemon.UsernameColor);
      Vector2 vector2_2 = GuiData.smallfont.MeasureString(LocaleTerms.Loc("Anonymous"));
      sb.DrawString(GuiData.detailfont, "01/01/1970(Thu)00:00 UTC+0:0", vector2_1 + new Vector2(Math.Max(112f - (float) postheaderOffset, vector2_2.X + 4f) + (float) postheaderOffset, 3f), Utils.SlightlyDarkGray);
      vector2_1.Y += (float) postheaderOffset;
      if (img != MessageBoardPostImage.None && MessageBoardDaemon.Images.ContainsKey(img))
      {
        Rectangle destinationRectangle = new Rectangle(dest.X + margin, dest.Y + margin + postheaderOffset, ImageSize, ImageSize);
        sb.Draw(MessageBoardDaemon.Images[img], destinationRectangle, Color.White);
        vector2_1.X += (float) (ImageSize + margin + margin);
      }
      string[] strArray = text.Split(Utils.newlineDelim);
      float y = font.MeasureString(strArray[0]).Y;
      for (int index = 0; index < strArray.Length; ++index)
        sb.DrawString(font, strArray[index], vector2_1 + new Vector2(0.0f, (float) index * (y + 2f)), strArray[index].StartsWith(">") ? MessageBoardDaemon.ImplicationColor : Color.White);
    }

    private List<MessageBoardPost> GetLastPostsToFitHeight(MessageBoardThread thread, int height, int width, int margin, int ImageSize, int PostHeaderOffset, int ThreadFooterSize, int maxOPSize = 2147483647)
    {
      List<MessageBoardPost> messageBoardPostList = new List<MessageBoardPost>();
      int num1 = ThreadFooterSize + margin * 4;
      int val2 = this.MeasurePost(thread.posts[0], width, margin, ImageSize, PostHeaderOffset);
      int num2 = Math.Min(maxOPSize, val2);
      messageBoardPostList.Add(thread.posts[0]);
      int num3 = num1 + num2;
      for (int index = thread.posts.Count - 1; index >= 0; --index)
      {
        int num4 = this.MeasurePost(thread.posts[index], width, margin, ImageSize, PostHeaderOffset);
        if (num3 + num4 < height)
        {
          messageBoardPostList.Insert(1, thread.posts[index]);
          num3 += num4;
        }
        else
          break;
      }
      return messageBoardPostList;
    }

    private enum MessageBoardState
    {
      Thread,
      Board,
    }
  }
}
