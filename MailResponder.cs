// Decompiled with JetBrains decompiler
// Type: Hacknet.MailResponder
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet
{
  internal interface MailResponder
  {
    void mailSent(string mail, string userTo);

    void mailReceived(string mail, string userTo);
  }
}
