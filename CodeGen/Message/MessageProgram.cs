public class MessageProgram
{
    public static void MainX()
    {
        Script.CsvHelper helper = Script.CsvUtils.Parse(CodeGen.Program.ReadAllText("CodeGen/MessageConfig.csv"));
        var list = new List<MessageConfig>();
        while (helper.ReadRow())
        {
            var c = new MessageConfig();
            c.msgType = helper.ReadString("msgType");
            c.value = helper.ReadInt("value");
            c.msg = helper.ReadString("msg");
            c.res = helper.ReadString("res");
            list.Add(c);
        }

        Create_MsgType_cs.Create(list);
    }
}