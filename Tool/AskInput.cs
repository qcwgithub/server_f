using System;

public class AskInput
{
    string question;
    public AskInput(string question)
    {
        this.question = question;
    }

    public void OnAnswer(Action<string> action)
    {
        Console.Write(this.question);

        string answer = Console.ReadLine();
        action(answer);
    }
}