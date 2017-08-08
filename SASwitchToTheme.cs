// Decompiled with JetBrains decompiler
// Type: Hacknet.SASwitchToTheme
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Extensions;
using System;
using System.Xml;

namespace Hacknet
{
  public class SASwitchToTheme : SerializableAction
  {
    public float FlickerInDuration = 2f;
    public string ThemePathOrName;
    public string DelayHost;
    public float Delay;

    public override void Trigger(object os_obj)
    {
      OS os = (OS) os_obj;
      if ((double) this.Delay <= 0.0)
      {
        OSTheme result = OSTheme.Custom;
        if (!Enum.TryParse<OSTheme>(this.ThemePathOrName, out result))
          result = OSTheme.Custom;
        try
        {
          os.EffectsUpdater.StartThemeSwitch(this.FlickerInDuration, result, (object) os, result == OSTheme.Custom ? this.ThemePathOrName : (string) null);
        }
        catch (Exception ex)
        {
          os.write(" ");
          os.write("---------------------- ");
          os.write("ERROR LOADING X-SERVER");
          os.write(ex.ToString());
          os.write(ex.Message);
          string filename = ExtensionLoader.ActiveExtensionInfo.FolderPath + "/xserver_error.txt";
          Utils.writeToFile("x-server load error for theme : " + this.ThemePathOrName + "\r\n." + Utils.GenerateReportFromException(ex), filename);
          os.write("---------------------- ");
          os.write("Full report saved to " + filename);
          os.write("---------------------- ");
          os.write(" ");
        }
      }
      else
      {
        Computer computer = Programs.getComputer(os, this.DelayHost);
        if (computer == null)
          throw new NullReferenceException("Computer " + (object) computer + " could not be found as DelayHost for Function");
        float delay = this.Delay;
        this.Delay = -1f;
        DelayableActionSystem.FindDelayableActionSystemOnComputer(computer).AddAction((SerializableAction) this, delay);
      }
    }

    public static SerializableAction DeserializeFromReader(XmlReader rdr)
    {
      SASwitchToTheme saSwitchToTheme = new SASwitchToTheme();
      if (rdr.MoveToAttribute("Delay"))
        saSwitchToTheme.Delay = rdr.ReadContentAsFloat();
      if (rdr.MoveToAttribute("ThemePathOrName"))
        saSwitchToTheme.ThemePathOrName = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("FlickerInDuration"))
        saSwitchToTheme.FlickerInDuration = rdr.ReadContentAsFloat();
      if (rdr.MoveToAttribute("DelayHost"))
        saSwitchToTheme.DelayHost = rdr.ReadContentAsString();
      return (SerializableAction) saSwitchToTheme;
    }
  }
}
