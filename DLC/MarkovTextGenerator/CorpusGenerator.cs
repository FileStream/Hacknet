// Decompiled with JetBrains decompiler
// Type: Hacknet.DLC.MarkovTextGenerator.CorpusGenerator
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hacknet.DLC.MarkovTextGenerator
{
  public static class CorpusGenerator
  {
    public static Corpus GenerateCorpusFromFolder(string name, int maxFilesToRead = -1, Action<float, string> percentCompleteUpdated = null, Action<Corpus> Complete = null)
    {
      try
      {
        Corpus corpus = new Corpus();
        DirectoryInfo directoryInfo = new DirectoryInfo(name + "/");
        string[] extensions = new string[2]{ ".txt", ".cs" };
        FileInfo[] array = ((IEnumerable<FileInfo>) directoryInfo.GetFiles()).Where<FileInfo>((Func<FileInfo, bool>) (f => ((IEnumerable<string>) extensions).Contains<string>(f.Extension.ToLower()))).ToArray<FileInfo>();
        for (int index = 0; index < array.Length && (maxFilesToRead == -1 || index < maxFilesToRead); ++index)
        {
          float num = (float) index / (float) array.Length;
          num *= 100f;
          if (maxFilesToRead != -1)
            num = (float) index / (float) maxFilesToRead;
          string input = File.ReadAllText(name + "/" + array[index].Name);
          if (percentCompleteUpdated != null)
            percentCompleteUpdated(num, num.ToString("00.00") + "% | Reading " + array[index].Name + "...");
          corpus.LearnText(input);
        }
        if (Complete != null)
          Complete(corpus);
        return corpus;
      }
      catch (Exception ex)
      {
        if (Complete != null)
          Complete((Corpus) null);
        return (Corpus) null;
      }
    }

    public static void GenerateCorpusFromFolderThreaded(string foldername, Action<Corpus> Complete, Action<float, string> percentCompleteUpdated)
    {
      Task.Factory.StartNew<Corpus>((Func<Corpus>) (() => CorpusGenerator.GenerateCorpusFromFolder(foldername, -1, percentCompleteUpdated, Complete)));
    }
  }
}
