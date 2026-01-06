using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class AskSelect
{
    string question;
    List<string> options;
    int defaultIndex;
    public AskSelect(string question, List<string> options)
    {
        this.question = question;
        this.options = options;
        this.defaultIndex = -1;
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i].EndsWith("*"))
            {
                this.defaultIndex = i;
                options[i] = options[i].Substring(0, options[i].Length - 1);
            }
        }
    }

    public int OnAnswer()
    {
        for (int i = 0; i < this.options.Count; i++)
        {
            Console.WriteLine("{0}) {1}", i + 1, this.options[i] + (i == this.defaultIndex ? "*" : ""));
        }
        Console.Write(this.question);

        string answer = Console.ReadLine();

        if (answer.Length == 0 && this.defaultIndex >= 0)
        {
            Console.WriteLine();
            return this.defaultIndex;
        }

        int select;
        if (!int.TryParse(answer, out select) || select - 1 < 0 || select - 1 >= this.options.Count)
        {
            // 重新问
            return OnAnswer();
        }
        else
        {
            Console.WriteLine();
            return select - 1;
        }
    }

    public void OnAnswers(Action<List<int>> action)
    {
        for (int i = 0; i < this.options.Count; i++)
        {
            Console.WriteLine("{0}) {1}", i + 1, this.options[i] + (i == this.defaultIndex ? "*" : ""));
        }
        Console.Write(this.question);

        string answer = Console.ReadLine();
        if (answer.Length == 0)
        {
            // 重新问
            OnAnswers(action);
            return;
        }

        List<string> answers_string = answer.Split(',').ToList();
        if (answers_string.Exists(answer =>
        {
            if (!int.TryParse(answer, out int select) || select - 1 < 0 || select - 1 >= this.options.Count)
            {
                return true;
            }
            return false;
        }))
        {
            // 重新问
            OnAnswers(action);
            return;
        }

        List<int> answers = answers_string.Select(answer => int.Parse(answer) - 1).ToList();
        Console.WriteLine();
        action(answers);
    }

    public (int, string) OnAnswer2()
    {
        int index = this.OnAnswer();
        return (index, this.options[index]);
    }

    public void OnAnswers(Action<List<KeyValuePair<int, string>>> action)
    {
        this.OnAnswers(indexes => action(indexes.Select(i => new KeyValuePair<int, string>(i, this.options[i])).ToList()));
    }
}