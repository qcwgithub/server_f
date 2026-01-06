using System;

public class AskInput
{
    string question;
    public AskInput(string question)
    {
        this.question = question;
    }

    public string OnAnswer()
    {
        Console.Write(this.question);

        string answer = Console.ReadLine();
        return answer;
    }
}