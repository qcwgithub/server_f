using Data;
namespace Tool
{
    public partial class LinuxProgram
    {
        List<ServiceConfig> SelectServices(string? question, bool groupByProcess)
        {
            var tcs = new TaskCompletionSource<string>();

            if (question == null)
            {
                question = "Services (Split by ',')?";
            }

            List<string[]> runningServices = this.GetRunningServices();
            List<string> targets = new List<string>();

            if (groupByProcess)
            {
                string[] options = runningServices.Select(array => string.Join(',', array)).ToArray();

                List<KeyValuePair<int, string>> answers = AskHelp.AskSelect(question, options)
                    .OnAnswers2();

                for (int i = 0; i < runningServices.Count; i++)
                {
                    if (answers.Exists(a => a.Key == i))
                    {
                        targets.AddRange(runningServices[i]);
                    }
                }
            }
            else
            {
                List<string> options = new List<string>();
                foreach (string[] array in runningServices)
                {
                    options.AddRange(array);
                }

                List<KeyValuePair<int, string>> answers = AskHelp.AskSelect(question, options.ToArray())
                    .OnAnswers2();

                for (int i = 0; i < runningServices.Count; i++)
                {
                    if (answers.Exists(a => a.Key == i))
                    {
                        targets.AddRange(runningServices[i]);
                    }
                }
            }

            return this.FindServiceConfigs(targets);
        }
    }
}