// Decompiled with JetBrains decompiler
// Type: Hacknet.DLC.MarkovTextGenerator.Corpus
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hacknet.DLC.MarkovTextGenerator
{
  public class Corpus
  {
    public int LearningLength = 3;
    private Dictionary<string, Dictionary<string, int>> data = new Dictionary<string, Dictionary<string, int>>();
    private const string DICT_FILE_TYPE = ".csd";
    private const string BODY_FILE_TYPE = ".csb";
    private const string EMPTY_STRING_KEY_WILDCARD = "\tEMPTY\t";

    public List<PotentialEntry> GetPotentialEntries(string preceeding)
    {
      List<PotentialEntry> potentialEntryList = new List<PotentialEntry>();
      if (!this.data.ContainsKey(preceeding))
        return potentialEntryList;
      Dictionary<string, int> dictionary = this.data[preceeding];
      int num1 = 0;
      int num2 = 0;
      foreach (KeyValuePair<string, int> keyValuePair in dictionary)
      {
        potentialEntryList.Add(new PotentialEntry()
        {
          word = keyValuePair.Key,
          weighting = (double) keyValuePair.Value
        });
        if (keyValuePair.Value > num1)
          num1 = keyValuePair.Value;
        num2 += keyValuePair.Value;
      }
      for (int index = 0; index < potentialEntryList.Count; ++index)
        potentialEntryList[index].weighting = potentialEntryList[index].weighting / (double) num2;
      return potentialEntryList;
    }

    public void Serialize(string filename)
    {
      using (FileStream fileStream = File.OpenWrite(filename))
      {
        StreamWriter streamWriter = new StreamWriter((Stream) fileStream);
        foreach (KeyValuePair<string, Dictionary<string, int>> keyValuePair1 in this.data)
        {
          streamWriter.Write(keyValuePair1.Key + "##^@%##\r\n");
          try
          {
            foreach (KeyValuePair<string, int> keyValuePair2 in keyValuePair1.Value)
              streamWriter.Write("\t" + keyValuePair2.Key + "\t|\t" + (object) keyValuePair2.Value + "\r\n");
          }
          catch (Exception ex)
          {
          }
          streamWriter.Write("#*@*@*@*@*@#\r\n\r\n");
        }
        streamWriter.Flush();
        streamWriter.Close();
      }
    }

    public void LearnText(string input)
    {
      List<string> memory = new List<string>();
      for (int index = 0; index < this.LearningLength; ++index)
        memory.Add((string) null);
      foreach (string str1 in input.Split(Utils.WhitespaceDelim, StringSplitOptions.RemoveEmptyEntries))
      {
        string key = Corpus.ConvertMemoryToString(memory);
        Dictionary<string, int> dictionary1;
        if (this.data.ContainsKey(key))
        {
          dictionary1 = this.data[key];
        }
        else
        {
          dictionary1 = new Dictionary<string, int>();
          try
          {
            this.data.Add(key, dictionary1);
          }
          catch (OutOfMemoryException ex)
          {
            break;
          }
        }
        string str2 = str1;
        if (dictionary1.ContainsKey(str2))
        {
          Dictionary<string, int> dictionary2;
          string index;
          (dictionary2 = dictionary1)[index = str2] = dictionary2[index] + 1;
        }
        else
          dictionary1.Add(str2, 1);
        try
        {
          this.data[key] = dictionary1;
        }
        catch (OutOfMemoryException ex)
        {
          break;
        }
        if (Corpus.WordEndsWithSentenceEnder(str2))
        {
          for (int index = 0; index < this.LearningLength; ++index)
            memory[index] = (string) null;
        }
        else
        {
          memory.Add(str2);
          memory.RemoveAt(0);
        }
      }
    }

    private static bool WordEndsWithSentenceEnder(string word)
    {
      return ".!?;{}".Contains<char>(word[word.Length - 1]);
    }

    private static string ConvertMemoryToString(List<string> memory)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < memory.Count; ++index)
      {
        stringBuilder.Append(memory[index]);
        stringBuilder.Append(" ");
      }
      return stringBuilder.ToString().Trim();
    }

    public string GetAnalysisStringFromWordList(List<string> words)
    {
      List<string> memory = new List<string>();
      for (int index = 0; index < this.LearningLength; ++index)
        memory.Add(words[words.Count - this.LearningLength + index]);
      return Corpus.ConvertMemoryToString(memory);
    }

    public string GenerateSentence(Action<string> CompleteAction = null)
    {
      List<string> words = new List<string>();
      for (int index = 0; index < this.LearningLength; ++index)
        words.Add((string) null);
      int num1 = 0;
      while (num1 < 1000)
      {
        List<PotentialEntry> potentialEntries = this.GetPotentialEntries(this.GetAnalysisStringFromWordList(words));
        double num2 = Utils.random.NextDouble();
        double num3 = 0.0;
        for (int index = 0; index < potentialEntries.Count; ++index)
        {
          num3 += potentialEntries[index].weighting;
          if (num2 < num3)
          {
            words.Add(potentialEntries[index].word);
            break;
          }
        }
        ++num1;
        if (Corpus.WordEndsWithSentenceEnder(words[words.Count - 1]))
        {
          string str1 = "";
          for (int index = 0; index < words.Count; ++index)
            str1 = str1 + words[index] + " ";
          string str2 = str1.Trim();
          if (CompleteAction != null)
            CompleteAction(str2);
          return str2;
        }
      }
      if (CompleteAction != null)
        CompleteAction((string) null);
      return (string) null;
    }

    public void GenerateSentenceThreaded(Action<string> complete)
    {
      Task.Factory.StartNew<string>((Func<string>) (() => this.GenerateSentence(complete)));
    }
  }
}
