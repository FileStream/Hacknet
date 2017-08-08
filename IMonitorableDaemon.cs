// Decompiled with JetBrains decompiler
// Type: Hacknet.IMonitorableDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;

namespace Hacknet
{
  internal interface IMonitorableDaemon
  {
    void SubscribeToAlertActionFroNewMessage(Action<string, string> act);

    void UnSubscribeToAlertActionFroNewMessage(Action<string, string> act);

    bool ShouldDisplayNotifications();

    string GetName();

    void navigatedTo();
  }
}
