// Decompiled with JetBrains decompiler
// Type: Hacknet.MemoryDumpInjector
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.IO;
using System.Xml;

namespace Hacknet
{
  public static class MemoryDumpInjector
  {
    public static void InjectMemory(string memoryFilepath, object computer)
    {
      using (FileStream fileStream = File.OpenRead(memoryFilepath))
      {
        MemoryContents memoryContents = MemoryContents.Deserialize(XmlReader.Create((Stream) fileStream));
        ((Computer) computer).Memory = memoryContents;
      }
    }
  }
}
