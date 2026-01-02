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
            c.queue = helper.ReadString("queue");
            c.external = helper.ReadString("external") == "1";
            c.arg_serviceId = helper.ReadString("arg_serviceId") == "1";


            list.Add(c);
        }

        Create_MsgType.Create(list);
        Create_MessageConfigData.Create(list);
        Create_ServiceProxy.Create(list);
    }
}