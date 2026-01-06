using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class AskSelectById
{
    public class Option
    {
        public int id;
        public string text;
    }

    string question;
    List<Option> options;
    public AskSelectById(string question, List<Option> options)
    {
        this.question = question;
        this.options = options;
    }

    public void OnAnswer(Action<Option> action)
    {
        for (int i = 0; i < this.options.Count; i++)
        {
            Console.WriteLine("{0}", this.options[i].text);
        }
        Console.Write(this.question);

        string answer = Console.ReadLine();

        if (answer.Length == 0)
        {
            // 重新问
            OnAnswer(action);
            return;
        }

        if (answer == "-1")
        {
            Process.GetCurrentProcess().Kill();
            return;
        }

        Option selectOption = null;

        if (!int.TryParse(answer, out int selectId) || (selectOption = this.options.Find(_ => _.id == selectId)) == null)
        {
            // 重新问
            OnAnswer(action);
        }
        else
        {
            Console.WriteLine();
            action(selectOption);
        }
    }

    public void OnAnswers(Action<List<Option>> action)
    {
        for (int i = 0; i < this.options.Count; i++)
        {
            Console.WriteLine("{0}", this.options[i].text);
        }
        Console.Write(this.question);

        string answers = Console.ReadLine();

        if (answers.Length == 0)
        {
            // 重新问
            OnAnswers(action);
            return;
        }

        if (answers == "-1")
        {
            Process.GetCurrentProcess().Kill();
            return;
        }

        string[] array = answers.Split(',');

        var list = new List<Option>();
        foreach (string answer in array)
        {
            Option selectOption = null;
            if (!int.TryParse(answers, out int selectId) || (selectOption = this.options.Find(_ => _.id == selectId)) == null)
            {
                // 重新问
                OnAnswers(action);
                return;
            }
            else
            {
                list.Add(selectOption);
            }
        }

        Console.WriteLine();
        action(list);
    }
}