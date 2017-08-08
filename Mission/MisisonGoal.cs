// Decompiled with JetBrains decompiler
// Type: Hacknet.Mission.MisisonGoal
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.Collections.Generic;

namespace Hacknet.Mission
{
  public class MisisonGoal
  {
    public virtual bool isComplete(List<string> additionalDetails = null)
    {
      return true;
    }

    public virtual void reset()
    {
    }

    public virtual string TestCompletable()
    {
      return "";
    }
  }
}
