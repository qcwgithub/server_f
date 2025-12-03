using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

public class MessageCodeObj
{
    public bool commented;
    public string name;
    public int value;

    public static MessageCodeObj Parse(string s)
    {
        var self = new MessageCodeObj();
        if (s.StartsWith("//"))
        {
            self.commented = true;
            s = s.Substring(2);
        }

        int i = s.IndexOf('=');
        self.name = s.Substring(0, i).Trim();

        string value_str = s.Substring(i + 1, s.Length - i - 1).Trim();
        self.value = int.Parse(value_str);
        return self;
    }

    public static MessageCodeObj Create(bool commented, string str, int value)
    {
        var self = new MessageCodeObj();
        self.commented = commented;
        self.name = str;
        self.value = value;
        return self;
    }
}

public class GenMessageCode
{
    public static List<string[]> s_extras = new List<string[]>
    {
        new string[]
        {
            "Null", "null", "new byte[0]"
        },
        // new string[]
        // {
        //     "MsgType", "MessagePackSerializer.Deserialize<MsgType>(readonlyMemory)", "MessagePackSerializer.Serialize<MsgType>((MsgType)obj)"
        // }
    };

    readonly static string Header = @"namespace Data
{
    public enum MessageCode
    {
        #region AUTO
";

    readonly static string Tail = @"        #endregion
    }
}";
    // public static void SaveInt(string file)
    // {
    //     string content = File.ReadAllText(file);
    //     int i = content.IndexOf("#region AUTO");
    //     i += "#region AUTO".Length;

    //     int j = content.IndexOf("#endregion", i);
    //     string inner = content.Substring(i, j - i - 1);
    //     List<string> list = inner.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();

    //     foreach (string[] extra in GenMessageCode.s_extras)
    //     {
    //         if (!list.Contains(extra[0]))
    //         {
    //             list.Insert(0, extra[0]);
    //         }
    //     }

    //     var sb = new StringBuilder();
    //     sb.Append(Header);

    //     // foreach (var array in s_extras)
    //     // {
    //     //     sb.Append("        " + array[0]).Append(",\n");
    //     // }

    //     int value = 0;
    //     for (int k = 0; k < list.Count; k++)
    //     {
    //         if (list[k] == "DirtyElementType")
    //         {
    //             value = 10000;
    //         }

    //         sb.AppendFormat("        {0} = {1},\n", list[k], value);
    //         value++;
    //     }

    //     sb.Append(Tail);

    //     File.WriteAllText(file, sb.ToString());
    // }

    // 把当前所有的 MessageCode 读出来
    // file = Assets/Scripts/Gen/MessageCode.cs
    public static List<MessageCodeObj> Read(string file)
    {
        string content = File.ReadAllText(file);
        int i = content.IndexOf("#region AUTO");
        i += "#region AUTO".Length;

        int j = content.IndexOf("#endregion", i);
        string inner = content.Substring(i, j - i - 1);
        List<MessageCodeObj> list = inner
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(x => MessageCodeObj.Parse(x))
            .ToList();
        return list;
    }

    public static void Gen(List<MessageCodeObj> list, string outputFile)
    {
        var sb = new StringBuilder();
        sb.Append(Header);

        foreach (MessageCodeObj obj in list)
        {
            if (!obj.commented)
            {
                sb.AppendFormat("        {0} = {1},\n", obj.name, obj.value);
            }
            else
            {
                sb.AppendFormat("        // {0} = {1},\n", obj.name, obj.value);
            }
        }

        sb.Append(Tail);

        File.WriteAllText(outputFile, sb.ToString());
    }
}