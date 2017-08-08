// Decompiled with JetBrains decompiler
// Type: Hacknet.Firewall
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Hacknet
{
  public class Firewall
  {
    private int solutionLength = 6;
    public bool solved = false;
    private int complexity = 1;
    private int analysisPasses = 0;
    private float additionalDelay = 0.0f;
    private const int MIN_SOLUTION_LENGTH = 6;
    private const int OUTPUT_LINE_WIDTH = 20;
    private const int CHARS_SOLVED_PER_PASS = 3;
    private const string SOLVED_CHAR = "0";
    private string solution;

    public Firewall()
    {
      this.generateRandomSolution();
    }

    public Firewall(int complexity)
    {
      this.complexity = complexity;
      this.solutionLength = 6 + complexity;
      this.generateRandomSolution();
    }

    public Firewall(int complexity, string solution)
    {
      this.complexity = complexity;
      this.solution = solution;
      this.solutionLength = solution.Length;
    }

    public Firewall(int complexity, string solution, float additionalTime)
    {
      this.complexity = complexity;
      this.solution = solution;
      this.additionalDelay = additionalTime;
      this.solutionLength = solution.Length;
    }

    private void generateRandomSolution()
    {
      StringBuilder stringBuilder = new StringBuilder(this.solutionLength);
      for (int index = 0; index < this.solutionLength; ++index)
        stringBuilder.Append(Utils.getRandomChar());
      this.solution = stringBuilder.ToString().ToUpperInvariant();
    }

    public static Firewall load(XmlReader reader)
    {
      while (reader.Name != "firewall")
        reader.Read();
      int complexity = 0;
      string solution = (string) null;
      float additionalTime = 0.0f;
      if (reader.MoveToAttribute("complexity"))
        complexity = reader.ReadContentAsInt();
      if (reader.MoveToAttribute("solution"))
        solution = reader.ReadContentAsString();
      if (reader.MoveToAttribute("additionalDelay"))
        additionalTime = reader.ReadContentAsFloat();
      return new Firewall(complexity, solution, additionalTime);
    }

    public string getSaveString()
    {
      return "<firewall complexity=\"" + (object) this.complexity + "\" solution=\"" + this.solution + "\" additionalDelay=\"" + this.additionalDelay.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "\" />";
    }

    public void resetSolutionProgress()
    {
      this.analysisPasses = 0;
    }

    public bool attemptSolve(string attempt, object os)
    {
      if (attempt.Length != this.solution.Length)
      {
        string str = attempt.Length < this.solution.Length ? LocaleTerms.Loc("Too few characters") : LocaleTerms.Loc("Too many characters");
        ((OS) os).write(LocaleTerms.Loc("Solution Incorrect Length") + " - " + str);
      }
      else if (attempt.ToLower().Equals(this.solution.ToLower()))
      {
        this.solved = true;
        return true;
      }
      return false;
    }

    public void writeAnalyzePass(object os_object, object target_object)
    {
      Computer target = (Computer) target_object;
      OS os = (OS) os_object;
      if (target.firewallAnalysisInProgress)
      {
        os.write("-" + LocaleTerms.Loc("Analysis already in Progress") + "-");
      }
      else
      {
        os.delayer.PostAnimation(this.generateOutputPass(this.analysisPasses, os, target));
        ++this.analysisPasses;
      }
    }

    private IEnumerator<ActionDelayer.Condition> generateOutputPass(int pass, OS os, Computer target)
    {
      target.firewallAnalysisInProgress = true;
      os.write(string.Format(LocaleTerms.Loc("Firewall Analysis Pass {0}"), (object) this.analysisPasses) + "\n");
      yield return ActionDelayer.Wait(0.03);
      os.write("--------------------");
      yield return ActionDelayer.Wait(0.03);
      string preceedeString = "     ";
      double secondsDelayPerLine = 0.08 + 0.06 * (double) pass + (double) this.additionalDelay;
      for (int i = 0; i < this.solutionLength; ++i)
      {
        os.write(preceedeString + this.generateOutputLine(i));
        yield return ActionDelayer.Wait(secondsDelayPerLine);
      }
      os.write("--------------------\n");
      target.firewallAnalysisInProgress = false;
    }

    private string generateOutputLine(int location)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < 20; ++index)
        stringBuilder.Append("0");
      int num = 20 - 3 * this.analysisPasses;
      for (int index = 0; index < num; ++index)
        stringBuilder[index] = string.Concat((object) Utils.getRandomChar()).ToLower()[0];
      int index1 = Utils.random.Next(stringBuilder.Length);
      if (location < this.solution.Length)
        stringBuilder[index1] = this.solution[location];
      int index2 = 0;
      while (index2 < stringBuilder.Length)
      {
        stringBuilder.Insert(index2, " ");
        index2 += 2;
      }
      return stringBuilder.ToString();
    }

    public override bool Equals(object obj)
    {
      Firewall firewall = obj as Firewall;
      if (firewall != null)
        return (double) firewall.additionalDelay == (double) this.additionalDelay && firewall.complexity == this.complexity && firewall.solution == this.solution;
      return false;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public override string ToString()
    {
      return "Firewall: solution\"" + this.solution + "\" - time:" + (object) this.additionalDelay + " - complexity:" + (object) this.complexity;
    }
  }
}
