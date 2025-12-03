using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public partial class CreateFile
{


    ////////////////////////////////////////////////////////////////////
    #region BinaryMessagePacker.cs

    readonly static string Header1 = @"using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using MessagePack;

namespace Data
{
    public partial class BinaryMessagePacker
    {
        
";
    readonly static string Tail1 = @"    }
}";
    public static void GenBinaryMessagePackerGen_Unpack(List<MessageCodeObj> list, string outputFile, StringBuilder sb)
    {
        sb.Append("        public static object UnpackBody(MessageCode messageCode, byte[] buffer, int offset, int count)").Append("\n");
        sb.Append("        {").Append("\n");

        sb.Append("            object obj = null;").Append("\n");
        sb.Append("            var readonlyMemory = new ReadOnlyMemory<byte>(buffer, offset, count);").Append("\n");

        sb.Append("            switch (messageCode)").Append("\n");
        sb.Append("            {").Append("\n");

        foreach (MessageCodeObj obj in list)
        {
            string[] extra = GenMessageCode.s_extras.Find(_ => _[0] == obj.name);
            if (extra != null)
            {
                sb.AppendFormat("                case MessageCode.{0}: obj = {1}; break;", extra[0], extra[1]).Append("\n");
            }
            else
            {
                if (!obj.commented)
                {
                    sb.AppendFormat("                case MessageCode.{0}: obj = MessagePackSerializer.Deserialize<{0}>(readonlyMemory); break;", obj.name).Append("\n");
                }
            }
        }
        sb.Append("                default:").Append("\n");
        sb.Append("                    throw new Exception();").Append("\n");
        sb.Append("                    //break;").Append("\n");
        sb.Append("            }").Append("\n");

        sb.Append("            return obj;").Append("\n");

        sb.Append("        }").Append("\n");

    }

    #endregion
    ////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////
    #region BinaryMessagePacker.cs

    public static void GenBinaryMessagePackerGen_Pack(List<MessageCodeObj> list, string outputFile, StringBuilder sb)
    {

        sb.Append("        public static byte[] PackBody(MessageCode messageCode, object obj)").Append("\n");
        sb.Append("        {").Append("\n");

        sb.Append("            byte[] bytes = null;").Append("\n");

        sb.Append("            switch (messageCode)").Append("\n");
        sb.Append("            {").Append("\n");

        foreach (MessageCodeObj obj in list)
        {
            string[] extra = GenMessageCode.s_extras.Find(_ => _[0] == obj.name);
            if (extra != null)
            {
                sb.AppendFormat("                case MessageCode.{0}: bytes = {1}; break;", extra[0], extra[2]).Append("\n");
            }
            else
            {
                if (!obj.commented)
                {
                    sb.AppendFormat("                case MessageCode.{0}: bytes = MessagePackSerializer.Serialize<{0}>(({0})obj); break;", obj.name).Append("\n");
                }
            }
        }

        sb.Append("                default:").Append("\n");
        sb.Append("                    throw new Exception();").Append("\n");
        sb.Append("                    //break;").Append("\n");
        sb.Append("            }").Append("\n");

        sb.Append("            return bytes;").Append("\n");

        sb.Append("        }").Append("\n");

    }

    #endregion
    ////////////////////////////////////////////////////////////////////

    public static void GenBinaryMessagePackerGen(List<MessageCodeObj> list, string outputFile)
    {
        var sb = new StringBuilder();
        sb.Append(Header1);

        GenBinaryMessagePackerGen_Pack(list, outputFile, sb);
        GenBinaryMessagePackerGen_Unpack(list, outputFile, sb);

        sb.Append(Tail1);

        File.WriteAllText(outputFile, sb.ToString());
    }
}
