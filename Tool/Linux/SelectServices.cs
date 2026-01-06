namespace Tool
{
    public partial class LinuxProgram
    {
        async Task<string> SelectServices(string? question, bool groupByProcess)
        {
            var tcs = new TaskCompletionSource<string>();

            if (question == null)
            {
                question = "Services (Split by ',')?";
            }

            List<string[]> runningServices = this.GetRunningServices();
            if (groupByProcess)
            {
                string[] options = runningServices.Select(array => string.Join(',', array)).ToArray();

                AskHelp.AskSelect(question, options)
                    .OnAnswers((List<KeyValuePair<int, string>> answers) =>
                {
                    List<string> targets = new List<string>();
                    for (int i = 0; i < runningServices.Count; i++)
                    {
                        if (answers.Exists(a => a.Key == i))
                        {
                            targets.AddRange(runningServices[i]);
                        }
                    }

                    tcs.SetResult(string.Join(",", targets));
                });
            }
            else
            {
                List<string> options = new List<string>();
                foreach (string[] array in runningServices)
                {
                    options.AddRange(array);
                }

                AskHelp.AskSelect(question, options.ToArray())
                    .OnAnswers((List<KeyValuePair<int, string>> answers) =>
                {
                    List<string> targets = new List<string>();
                    for (int i = 0; i < runningServices.Count; i++)
                    {
                        if (answers.Exists(a => a.Key == i))
                        {
                            targets.AddRange(runningServices[i]);
                        }
                    }

                    tcs.SetResult(string.Join(",", targets));
                });
            }

            return await tcs.Task;
        }
    }
}