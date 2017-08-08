// Decompiled with JetBrains decompiler
// Type: Hacknet.Mission.SendEmailMission
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.Collections.Generic;

namespace Hacknet.Mission
{
  internal class SendEmailMission : MisisonGoal
  {
    private string mailSubject;
    private string mailRecipient;
    private MailServer server;

    public SendEmailMission(string mailServerID, string mailRecipient, string proposedEmailSubject, OS _os)
    {
      this.server = (MailServer) Programs.getComputer(_os, mailServerID).getDaemon(typeof (MailServer));
      this.mailSubject = proposedEmailSubject;
      this.mailRecipient = mailRecipient;
    }

    public override bool isComplete(List<string> additionalDetails = null)
    {
      if (this.server == null)
        return true;
      return this.server.MailWithSubjectExists(this.mailRecipient, this.mailSubject);
    }

    public override void reset()
    {
    }
  }
}
