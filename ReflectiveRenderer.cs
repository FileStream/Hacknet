// Decompiled with JetBrains decompiler
// Type: Hacknet.ReflectiveRenderer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Hacknet
{
  public static class ReflectiveRenderer
  {
    public static Action<Vector2, Type, string> PreRenderForObject;

    public static int GetEntryLineHeight()
    {
      return (int) ((double) GuiData.ActiveFontConfig.tinyFontCharHeight * 2.0 + 2.0);
    }

    public static void RenderObject(object o, Rectangle bounds, SpriteBatch spriteBatch, ScrollableSectionedPanel panel, Color TitleColor)
    {
      List<ReflectiveRenderer.RenderableField> fields = ReflectiveRenderer.GetRenderablesFromType(o.GetType(), o, 0);
      int entryLineHeight = ReflectiveRenderer.GetEntryLineHeight();
      panel.PanelHeight = entryLineHeight;
      panel.NumberOfPanels = fields.Count;
      int pixelsPerIndentLevel = 20;
      panel.Draw((Action<int, Rectangle, SpriteBatch>) ((i, dest, sb) =>
      {
        int num = fields[i].IndentLevel * pixelsPerIndentLevel;
        dest.X += num;
        dest.Width -= num;
        if (fields[i].IsTitle)
        {
          TextItem.doFontLabelToSize(dest, fields[i].RenderedValue, GuiData.font, TitleColor, true, true);
          Rectangle destinationRectangle = new Rectangle(dest.X, dest.Y + dest.Height - 1, dest.Width, 1);
          sb.Draw(Utils.white, destinationRectangle, TitleColor);
        }
        else
        {
          string text1 = fields[i].VariableName + " :";
          Vector2 vector2_1 = GuiData.smallfont.MeasureString(text1);
          sb.DrawString(GuiData.smallfont, text1, new Vector2((float) dest.X, (float) dest.Y), Color.Gray);
          string text2 = fields[i].RenderedValue;
          Vector2 vector2_2 = GuiData.smallfont.MeasureString(text2);
          if ((double) vector2_2.X > (double) (dest.Width - 20) || (double) vector2_2.Y > (double) dest.Height)
          {
            text2.Replace("\n", " ");
            text2 = text2.Substring(0, Math.Min(text2.Length, (int) ((double) dest.Width / ((double) GuiData.ActiveFontConfig.tinyFontCharHeight + 2.0)))) + "...";
          }
          Vector2 position = new Vector2((float) ((double) dest.X + (double) vector2_1.X + 6.0), (float) dest.Y);
          if (ReflectiveRenderer.PreRenderForObject != null && sb.Name != "AltRTBatch")
            ReflectiveRenderer.PreRenderForObject(position, fields[i].t, fields[i].RenderedValue);
          sb.DrawString(GuiData.smallfont, text2, position, Color.White);
        }
      }), spriteBatch, bounds);
      ReflectiveRenderer.PreRenderForObject = (Action<Vector2, Type, string>) null;
    }

    private static List<ReflectiveRenderer.RenderableField> GetRenderablesFromType(Type type, object o, int indentLevel = 0)
    {
      List<ReflectiveRenderer.RenderableField> renderableFieldList = new List<ReflectiveRenderer.RenderableField>();
      if (ObjectSerializer.IsSimple(type))
        renderableFieldList.Add(new ReflectiveRenderer.RenderableField()
        {
          VariableName = "Data",
          RenderedValue = o.ToString(),
          IsTitle = false,
          IndentLevel = indentLevel,
          t = type
        });
      else if (ObjectSerializer.TypeInstanceOfInterface(type, typeof (ICollection)))
      {
        foreach (object o1 in (IEnumerable) (o as ICollection))
          renderableFieldList.AddRange((IEnumerable<ReflectiveRenderer.RenderableField>) ReflectiveRenderer.GetRenderablesFromType(o1.GetType(), o1, indentLevel + 1));
      }
      else
      {
        renderableFieldList.Add(new ReflectiveRenderer.RenderableField()
        {
          IsTitle = true,
          RenderedValue = ReflectiveRenderer.FilterTypeName(type.Name),
          IndentLevel = indentLevel
        });
        FieldInfo[] fields = type.GetFields();
        for (int index = 0; index < fields.Length; ++index)
        {
          if (ObjectSerializer.IsSimple(fields[index].FieldType))
            renderableFieldList.Add(new ReflectiveRenderer.RenderableField()
            {
              VariableName = fields[index].Name,
              RenderedValue = fields[index].GetValue(o).ToString(),
              IsTitle = false,
              IndentLevel = indentLevel,
              t = fields[index].FieldType
            });
          else
            renderableFieldList.AddRange((IEnumerable<ReflectiveRenderer.RenderableField>) ReflectiveRenderer.GetRenderablesFromType(fields[index].FieldType, fields[index].GetValue(o), indentLevel + 1));
        }
        PropertyInfo[] properties = type.GetProperties();
        for (int index = 0; index < properties.Length; ++index)
        {
          if (ObjectSerializer.IsSimple(properties[index].PropertyType))
            renderableFieldList.Add(new ReflectiveRenderer.RenderableField()
            {
              VariableName = properties[index].Name,
              RenderedValue = properties[index].GetValue(o, (object[]) null).ToString(),
              IsTitle = false,
              IndentLevel = indentLevel,
              t = properties[index].PropertyType
            });
          else
            renderableFieldList.AddRange((IEnumerable<ReflectiveRenderer.RenderableField>) ReflectiveRenderer.GetRenderablesFromType(properties[index].PropertyType, properties[index].GetValue(o, (object[]) null), indentLevel + 1));
        }
      }
      return renderableFieldList;
    }

    private static string FilterTypeName(string name)
    {
      return name.Replace("Hacknet.", "");
    }

    private struct RenderableField
    {
      public string VariableName;
      public string RenderedValue;
      public bool IsTitle;
      public Type t;
      public int IndentLevel;

      public override string ToString()
      {
        return (this.IsTitle ? "TITLE" : this.VariableName) + ": " + this.RenderedValue;
      }
    }
  }
}
