namespace Script
{
    public partial class CommandService
    {
        protected override Task StopBusinesses()
        {
            return Task.CompletedTask;
        }
    }
}