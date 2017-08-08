// Decompiled with JetBrains decompiler
// Type: Hacknet.SALaunchHackScript
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SALaunchHackScript : SerializableAction
  {
    public string Filepath;
    public string DelayHost;
    public float Delay;
    public string SourceComp;
    public string TargetComp;
    public bool RequireLogsOnSource;
    public bool RequireSourceIntact;

    public override void Trigger(object os_obj)
    {
      if ((double) this.Delay <= 0.0)
      {
        Computer computer1 = Programs.getComputer((OS) os_obj, this.SourceComp);
        Computer computer2 = Programs.getComputer((OS) os_obj, this.TargetComp);
        if (this.RequireLogsOnSource)
        {
          if (computer1 == null)
            throw new NullReferenceException("Launch Hacker Script Error: Source comp " + this.SourceComp + " does not exist");
          if (computer2 == null)
            throw new NullReferenceException("Launch Hacker Script Error: Target comp " + this.TargetComp + " does not exist");
          Folder folder1 = computer1.files.root.searchForFolder("log");
          bool flag1 = false;
          for (int index = 0; index < folder1.files.Count; ++index)
          {
            if (TrackerCompleteSequence.CompShouldStartTrackerFromLogs(os_obj, computer1, computer2.ip))
            {
              flag1 = true;
              break;
            }
          }
          if (!flag1)
            return;
          if (this.RequireSourceIntact)
          {
            Folder folder2 = computer1.files.root.searchForFolder("sys");
            bool flag2 = false;
            for (int index = 0; index < folder2.files.Count; ++index)
            {
              if (folder2.files[index].name == "netcfgx.dll" && folder2.files[index].data.Contains("1") && folder2.files[index].data.Contains("0"))
              {
                flag2 = true;
                break;
              }
            }
            if (!flag2)
              return;
          }
        }
        HackerScriptExecuter.runScript(this.Filepath, os_obj, this.SourceComp, this.TargetComp);
      }
      else
      {
        Computer computer = Programs.getComputer((OS) os_obj, this.DelayHost);
        if (computer == null)
          throw new NullReferenceException("Computer " + (object) computer + " could not be found as DelayHost for Function");
        float delay = this.Delay;
        this.Delay = -1f;
        DelayableActionSystem.FindDelayableActionSystemOnComputer(computer).AddAction((SerializableAction) this, delay);
      }
    }

    public static SerializableAction DeserializeFromReader(XmlReader rdr)
    {
      SALaunchHackScript launchHackScript = new SALaunchHackScript();
      if (rdr.MoveToAttribute("Filepath"))
        launchHackScript.Filepath = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("Delay"))
        launchHackScript.Delay = rdr.ReadContentAsFloat();
      if (rdr.MoveToAttribute("DelayHost"))
        launchHackScript.DelayHost = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("SourceComp"))
        launchHackScript.SourceComp = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("TargetComp"))
        launchHackScript.TargetComp = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("RequireLogsOnSource"))
        launchHackScript.RequireLogsOnSource = rdr.ReadContentAsString().ToLower() == "true";
      if (rdr.MoveToAttribute("RequireSourceIntact"))
        launchHackScript.RequireSourceIntact = rdr.ReadContentAsString().ToLower() == "true";
      if (string.IsNullOrWhiteSpace(launchHackScript.Filepath))
        throw new FormatException("Invalid Filepath");
      return (SerializableAction) launchHackScript;
    }
  }
}
