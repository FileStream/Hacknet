// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.TextWriterTimed
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet.Effects
{
  public static class TextWriterTimed
  {
    public static int WriteTextToTerminal(string wholeText, object osObj, float timePerChar, float normalLettersDelayForNewline, float normalLetterDelayForWildcard, float elapsedTimeSoFar, int charsRenderedSoFar)
    {
      char ch = '%';
      OS os = (OS) osObj;
      int num1 = 0;
      float num2 = 0.0f;
      for (int index = 0; index < wholeText.Length; ++index)
      {
        num2 += (int) wholeText[index] == 10 ? normalLettersDelayForNewline * timePerChar : ((int) wholeText[index] == (int) ch ? normalLetterDelayForWildcard * timePerChar : timePerChar);
        if ((double) num2 < (double) elapsedTimeSoFar)
          ++num1;
        else
          break;
      }
      if (charsRenderedSoFar > num1)
        return num1;
      for (int index = charsRenderedSoFar; index < num1; ++index)
      {
        if ((int) wholeText[index] != (int) ch)
        {
          if ((int) wholeText[index] == 10)
            os.write(" ");
          else
            os.writeSingle(string.Concat((object) wholeText[index]));
        }
      }
      return num1;
    }
  }
}
