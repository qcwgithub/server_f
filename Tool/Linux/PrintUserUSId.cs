namespace Tool
{
    public partial class LinuxProgram
    {
        async Task PrintUserUSId()
        {
            string answer = AskHelp.AskInput("userId:").OnAnswer();
        }
    }
}