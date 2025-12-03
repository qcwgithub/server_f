using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public partial class CreateFile
{
    readonly static string Header2 = @"using System;
using System.Collections;
using System.Collections.Generic;
using Data;

namespace Data
{
    public partial class JsonMessagePacker
    {  
";
    readonly static string Tail2 = @"    }
}";
    public static void GenJsonMessagePackerGen_Unpack(List<MessageCodeObj> list, string outputFile, StringBuilder sb)
    {
        sb.Append("        public static object ParseMessage(MessageCode messageCode, string json)").Append("\n");
        sb.Append("        {").Append("\n");

        sb.Append("            object msg = null;").Append("\n");

        sb.Append("            switch (messageCode)").Append("\n");
        sb.Append("            {").Append("\n");

        foreach (MessageCodeObj obj in list)
        {
            string[] extra = GenMessageCode.s_extras.Find(_ => _[0] == obj.name);
            if (extra != null)
            {
                sb.AppendFormat("                case MessageCode.{0}: msg = {1}; break;", extra[0], extra[1]).Append("\n");
            }
            else
            {
                if (!obj.commented)
                {
                    sb.AppendFormat("                case MessageCode.{0}: msg = JsonUtils.parse<{0}>(json); break;", obj.name).Append("\n");
                }
            }
        }
        sb.Append("                default:").Append("\n");
        sb.Append("                    throw new Exception();").Append("\n");
        sb.Append("                    //break;").Append("\n");
        sb.Append("            }").Append("\n");

        sb.Append("            return msg;").Append("\n");

        sb.Append("        }").Append("\n");
    }

    public static void GenJsonMessagePackerGen(List<MessageCodeObj> list, string outputFile)
    {
        var sb = new StringBuilder();
        sb.Append(Header2);

        GenJsonMessagePackerGen_Unpack(list, outputFile, sb);

        sb.Append(Tail2);

        File.WriteAllText(outputFile, sb.ToString());
    }
}
