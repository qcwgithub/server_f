public class Create_MsgType_cs
{
    public static void Create(List<MessageConfig> configs)
    {
        var f = new FileFormatter();
        f.AddTab(2);
        
        for (int i = 0; i < configs.Count; i++)
        {
            var config = configs[i];
            f.TabPushF("{0} = {1},\n", config.msgType, config.value);
        }

        XInfoProgram.ReplaceFile("Data/Common/MsgType.cs", new Mark[]
        {
            new Mark { startMark = "#region auto", text = f.GetString() },
        });
    }
}