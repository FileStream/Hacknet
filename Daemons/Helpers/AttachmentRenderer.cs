// Decompiled with JetBrains decompiler
// Type: Hacknet.Daemons.Helpers.AttachmentRenderer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;

namespace Hacknet.Daemons.Helpers
{
  public static class AttachmentRenderer
  {
    private static string[] spaceDelim = new string[1]{ "#%#" };

    public static bool RenderAttachment(string data, object osObj, Vector2 dpos, int startingButtonIndex, SoundEffect buttonSound)
    {
      OS os = (OS) osObj;
      string[] strArray = data.Split(AttachmentRenderer.spaceDelim, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length < 1)
        return false;
      if (strArray[0].Equals("link"))
      {
        Vector2 labelSize = TextItem.doMeasuredTinyLabel(dpos, LocaleTerms.Loc("LINK") + " : " + strArray[1] + "@" + strArray[2], new Color?());
        Computer computer = Programs.getComputer(os, strArray[2]);
        if (!os.netMap.visibleNodes.Contains(os.netMap.nodes.IndexOf(computer)))
          AttachmentRenderer.DrawButtonGlow(dpos, labelSize, os);
        if (Button.doButton(800009 + startingButtonIndex, (int) ((double) dpos.X + (double) labelSize.X + 5.0), (int) dpos.Y, 20, 17, "+", new Color?()))
        {
          if (computer == null)
          {
            os.write("ERROR: Linked target not found");
          }
          else
          {
            computer.highlightFlashTime = 1f;
            os.netMap.discoverNode(computer);
            SFX.addCircle(Programs.getComputer(os, strArray[2]).getScreenSpacePosition(), Color.White, 32f);
            if (buttonSound != null && !Settings.soundDisabled)
              buttonSound.Play();
          }
        }
      }
      else if (strArray[0].Equals("account"))
      {
        Vector2 labelSize = TextItem.doMeasuredTinyLabel(dpos, LocaleTerms.Loc("ACCOUNT") + " : " + strArray[1] + " : User=" + strArray[3] + " Pass=" + strArray[4], new Color?());
        AttachmentRenderer.DrawButtonGlow(dpos, labelSize, os);
        if (Button.doButton(801009 + startingButtonIndex, (int) ((double) dpos.X + (double) labelSize.X + 5.0), (int) dpos.Y, 20, 17, "+", new Color?()))
        {
          Computer computer = Programs.getComputer(os, strArray[2]);
          computer.highlightFlashTime = 1f;
          os.netMap.discoverNode(computer);
          computer.highlightFlashTime = 1f;
          SFX.addCircle(computer.getScreenSpacePosition(), Color.White, 32f);
          for (int index = 0; index < computer.users.Count; ++index)
          {
            UserDetail user = computer.users[index];
            if (user.name.Equals(strArray[3]))
            {
              user.known = true;
              computer.users[index] = user;
              break;
            }
          }
          if (buttonSound != null && !Settings.soundDisabled)
            buttonSound.Play();
        }
      }
      else
      {
        if (!strArray[0].Equals("note"))
          return false;
        Vector2 labelSize = TextItem.doMeasuredTinyLabel(dpos, LocaleTerms.Loc("NOTE") + " : " + strArray[1], new Color?());
        string note = LocalizedFileLoader.SafeFilterString(strArray[2]);
        if (!NotesExe.NoteExists(note, os))
          AttachmentRenderer.DrawButtonGlow(dpos, labelSize, os);
        if (Button.doButton(800009 + startingButtonIndex, (int) ((double) dpos.X + (double) labelSize.X + 5.0), (int) dpos.Y, 20, 17, "+", new Color?()))
          NotesExe.AddNoteToOS(note, os, false);
      }
      return true;
    }

    private static void DrawButtonGlow(Vector2 dpos, Vector2 labelSize, OS os)
    {
      Rectangle rectangle = new Rectangle((int) ((double) dpos.X + (double) labelSize.X + 5.0), (int) dpos.Y, 20, 17);
      float num1 = Utils.QuadraticOutCurve((float) (1.0 - (double) os.timer % 1.0));
      float num2 = 8.5f;
      Rectangle destinationRectangle = Utils.InsetRectangle(rectangle, (int) (-1.0 * ((double) num2 * (1.0 - (double) num1))));
      GuiData.spriteBatch.Draw(Utils.white, destinationRectangle, Utils.AddativeWhite * num1 * 0.32f);
      GuiData.spriteBatch.Draw(Utils.white, rectangle, Color.Black * 0.7f);
    }
  }
}
