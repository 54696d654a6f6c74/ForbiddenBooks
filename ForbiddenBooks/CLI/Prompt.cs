using System;

namespace ForbiddenBooks.CLI
{
    // Can repurose this class
    // so to use it with reflection:
    // Questions ask for property ane
    // then check if input is correct type
    // and sets value. Can bi linked to Utils
    public class Prompt
    {
        private string[] questions;
        private string[] answers;

        public string[] Answers { 
            get
            {
                foreach (string question in questions)
                    AskQuestion(question);
                return answers;
            }
        }

        public Prompt(params string[] questions)
        {
            this.questions = questions;
        }

        private string AskQuestion(string question)
        {
            while(true)
            {
                Console.Write(question + ": ");
                string answer = Console.ReadLine();

                if (VerifyAnswer(answer))
                    return answer;
                Console.WriteLine("Invalid answer.");
            }
        }

        // Can specify max lenght, etc
        private bool VerifyAnswer(string answer)
        {
            if (answer.Length < 15)
                return true;
            return false;
        }
    }
}
