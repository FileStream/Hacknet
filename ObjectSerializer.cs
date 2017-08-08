// Decompiled with JetBrains decompiler
// Type: Hacknet.ObjectSerializer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Hacknet
{
  public static class ObjectSerializer
  {
    public static string SerializeObject(object o)
    {
      return ObjectSerializer.SerializeObject(o, false);
    }

    private static string SerializeObject(object o, bool preventOuterTag = false)
    {
      if (o == null)
        return "";
      Type type = o.GetType();
      string tagNameForType = ObjectSerializer.GetTagNameForType(type);
      StringBuilder stringBuilder = new StringBuilder();
      if (!preventOuterTag)
        stringBuilder.Append("<" + tagNameForType + ">");
      if (ObjectSerializer.TypeInstanceOfInterface(type, typeof (ICollection)))
      {
        stringBuilder.Append(ObjectSerializer.SerializeCollection((ICollection) o));
      }
      else
      {
        FieldInfo[] fields = type.GetFields();
        for (int index = 0; index < fields.Length; ++index)
        {
          string name = fields[index].Name;
          object o1 = fields[index].GetValue(o);
          if (o1 != null)
          {
            Type fieldType = fields[index].FieldType;
            string tagValue = !ObjectSerializer.TypeInstanceOfInterface(fieldType, typeof (IFormattable)) ? o1.ToString() : (o1 as IFormattable).ToString("", (IFormatProvider) CultureInfo.InvariantCulture);
            if (fieldType == typeof (Color))
              tagValue = Utils.convertColorToParseableString((Color) o1);
            if (!ObjectSerializer.IsSimple(o1.GetType()))
              tagValue = "\n" + ObjectSerializer.SerializeObject(o1, true);
            stringBuilder.Append(ObjectSerializer.GetSerializedStringForPrimative(name, tagValue));
          }
        }
      }
      if (!preventOuterTag)
        stringBuilder.Append("\n</" + tagNameForType + ">");
      else
        stringBuilder.Append("\n");
      return stringBuilder.ToString();
    }

    public static string GetTagNameForType(Type t)
    {
      string str = t.Name;
      if (str.StartsWith("Hacknet."))
        str = str.Substring("Hacknet.".Length);
      return str.Replace("`", "_");
    }

    public static bool TypeInstanceOfInterface(Type objType, Type interfaceType)
    {
      return interfaceType.IsAssignableFrom(objType);
    }

    private static string GetSerializedStringForPrimative(string tagName, string tagValue)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\n\t<{0}>{1}</{0}>", new object[2]
      {
        (object) tagName,
        (object) tagValue
      });
    }

    private static string SerializeCollection(ICollection o)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("\n");
      foreach (object o1 in (IEnumerable) o)
      {
        string str = !ObjectSerializer.IsSimple(o1.GetType()) ? ObjectSerializer.SerializeObject(o1) : ObjectSerializer.GetSerializedStringForPrimative(ObjectSerializer.GetTagNameForType(o1.GetType()), o1.ToString());
        stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\n\t{0}", new object[1]
        {
          (object) str
        }));
      }
      return stringBuilder.ToString();
    }

    public static bool IsSimple(Type type)
    {
      if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>))
        return ObjectSerializer.IsSimple(type.GetGenericArguments()[0]);
      return type.IsPrimitive || type.IsEnum || (type.Equals(typeof (string)) || type.Equals(typeof (Decimal))) || (type.Equals(typeof (string)) || type.Equals(typeof (DateTime))) || type.Equals(typeof (TimeSpan));
    }

    public static object DeserializeObject(Stream s, Type t)
    {
      using (XmlReader rdr = XmlReader.Create(s))
        return ObjectSerializer.DeserializeObject(rdr, t);
    }

    public static object DeserializeObject(XmlReader rdr, Type t)
    {
      if (ObjectSerializer.IsSimple(t))
      {
        XmlNamespaceManager namespaceManager = new XmlNamespaceManager((XmlNameTable) new NameTable());
        return rdr.ReadElementContentAs(t, (IXmlNamespaceResolver) namespaceManager);
      }
      if (ObjectSerializer.TypeInstanceOfInterface(t, typeof (ICollection)))
        return ObjectSerializer.DeserializeCollection(rdr, t, "list");
      return ObjectSerializer.DeserializeXMLObject(rdr, t, (string) null);
    }

    private static object DeserializeXMLObject(XmlReader rdr, Type t, string overrideExpectedEndTag = null)
    {
      object objectOfType = ObjectSerializer.CreateObjectOfType(t);
      FieldInfo[] fields = t.GetFields();
      XmlNamespaceManager namespaceManager = new XmlNamespaceManager((XmlNameTable) new NameTable());
      string str = ObjectSerializer.GetTagNameForType(t);
      if (overrideExpectedEndTag != null)
        str = overrideExpectedEndTag;
      while (!rdr.EOF)
      {
        if (!string.IsNullOrWhiteSpace(rdr.Name) && rdr.IsStartElement())
        {
          for (int index = 0; index < fields.Length; ++index)
          {
            if (fields[index].Name == rdr.Name)
            {
              int content = (int) rdr.MoveToContent();
              object obj = (object) null;
              if (fields[index].FieldType == typeof (Color))
                obj = (object) Utils.convertStringToColor(rdr.ReadElementContentAsString());
              else if (ObjectSerializer.TypeInstanceOfInterface(fields[index].FieldType, typeof (ICollection)))
                obj = ObjectSerializer.DeserializeCollection(rdr, fields[index].FieldType, fields[index].Name);
              else if (!ObjectSerializer.IsSimple(fields[index].FieldType))
              {
                obj = ObjectSerializer.DeserializeXMLObject(rdr, fields[index].FieldType, fields[index].Name);
              }
              else
              {
                try
                {
                  obj = ObjectSerializer.ReadElementContentWithType(rdr, fields[index].FieldType, (IXmlNamespaceResolver) namespaceManager);
                }
                catch (FormatException ex)
                {
                }
              }
              fields[index].SetValue(objectOfType, obj);
              break;
            }
          }
        }
        rdr.Read();
        if (!(rdr.Name == str) || rdr.IsStartElement())
        {
          if (rdr.EOF)
            throw new FormatException();
        }
        else
          break;
      }
      return objectOfType;
    }

    private static object ReadElementContentWithType(XmlReader reader, Type type, IXmlNamespaceResolver resolver)
    {
      if (type == typeof (DateTime))
        return (object) DateTime.Parse(reader.ReadElementContentAsString(), (IFormatProvider) CultureInfo.InvariantCulture);
      if (type == typeof (bool))
        return (object) (reader.ReadElementContentAsString().ToLower() == "true");
      if (type == typeof (TimeSpan))
        return (object) TimeSpan.Parse(reader.ReadElementContentAsString(), (IFormatProvider) CultureInfo.InvariantCulture);
      if (!type.IsEnum)
        return reader.ReadElementContentAs(type, resolver);
      string str = reader.ReadElementContentAsString();
      return Enum.Parse(type, str);
    }

    private static object CreateObjectOfType(Type targetType)
    {
      if (Type.GetTypeCode(targetType) == TypeCode.String)
        return (object) string.Empty;
      Type[] types = new Type[0];
      ConstructorInfo constructor = targetType.GetConstructor(types);
      object obj;
      if (constructor == (ConstructorInfo) null)
      {
        if (!targetType.BaseType.UnderlyingSystemType.FullName.Contains("Enum"))
          throw new ArgumentException("Unable to instantiate type: " + targetType.AssemblyQualifiedName + " - Constructor not found");
        obj = Activator.CreateInstance(targetType);
      }
      else
        obj = constructor.Invoke((object[]) null);
      if (obj == null)
        throw new ArgumentException("Unable to instantiate type: " + targetType.AssemblyQualifiedName + " - Unknown Error");
      return obj;
    }

    private static object DeserializeCollection(XmlReader rdr, Type t, string tagName = "list")
    {
      IList objectOfType = ObjectSerializer.CreateObjectOfType(t) as IList;
      if (objectOfType == null)
        throw new NotSupportedException();
      bool flag1 = rdr.Name.ToLower().StartsWith(tagName.ToLower()) && rdr.IsStartElement();
      while (!flag1)
      {
        rdr.Read();
        flag1 = rdr.Name.ToLower().StartsWith(tagName.ToLower()) && rdr.IsStartElement();
        if (rdr.EOF)
          throw new FormatException();
      }
      bool flag2 = false;
      do
      {
        rdr.Read();
        if (rdr.IsStartElement())
        {
          Type typeForName = ObjectSerializer.GetTypeForName(rdr.Name);
          object obj = ObjectSerializer.DeserializeObject(rdr, typeForName);
          objectOfType.Add(obj);
        }
        if (rdr.Name.ToLower().StartsWith(tagName.ToLower()) && !rdr.IsStartElement())
          flag2 = true;
        if (rdr.EOF)
          throw new FormatException();
      }
      while (!flag2);
      return (object) objectOfType;
    }

    public static Type GetTypeForName(string name)
    {
      if (string.IsNullOrWhiteSpace(name) || name.ToLower() == "none")
        return (Type) null;
      Type type = Type.GetType(name, false, true);
      if (type == (Type) null)
        type = Type.GetType("Hacknet." + name, false, true);
      if (type == (Type) null)
        type = Type.GetType("System." + name, false, true);
      return type;
    }

    public static object DeepCopy(object input)
    {
      return ObjectSerializer.DeserializeObject(Utils.GenerateStreamFromString(ObjectSerializer.SerializeObject(input)), input.GetType());
    }

    public static object GetValueFromObject(object o, string FieldName)
    {
      Type type = o.GetType();
      PropertyInfo[] properties = type.GetProperties();
      for (int index = 0; index < properties.Length; ++index)
      {
        if (properties[index].Name.ToLower() == FieldName.ToLower())
          return properties[index].GetValue(o, (object[]) null);
      }
      FieldInfo[] fields = type.GetFields();
      for (int index = 0; index < fields.Length; ++index)
      {
        if (fields[index].Name.ToLower() == FieldName.ToLower())
          return fields[index].GetValue(o);
      }
      return (object) null;
    }
  }
}
