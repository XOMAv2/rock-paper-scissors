using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RockPaperScissors;

class Program
{
    private static string GetMainMenu(IEnumerable<string> moves)
    {
        StringBuilder menuBuilder = new StringBuilder();
        menuBuilder.AppendLine("Available moves:");
        _ = moves.Select((m, i) => menuBuilder.AppendLine($"{i + 1} - {m}")).ToArray();
        menuBuilder.AppendLine("0 - exit");
        menuBuilder.AppendLine("? - help");
        menuBuilder.Append("Enter your move: ");

        return menuBuilder.ToString();
    }

    private static void RunGame(GameRules rules, HmacWorks hmac)
    {
        string mainMenu = GetMainMenu(rules.Moves);
        var choiseToMove = new Dictionary<string, string>(
            rules.Moves.Select((m, i) => new KeyValuePair<string, string>($"{i + 1}", m)));

        string computersMove = rules.Moves[new Random().Next(rules.Moves.Length)];
        string computersMoveSignature = hmac.GetHmacSignature(computersMove);

        bool shouldGameContinue = true;

        while (shouldGameContinue)
        {
            Console.WriteLine();
            Console.WriteLine($"HMAC: {computersMoveSignature}");
            Console.Write(mainMenu);
            string? choise = Console.ReadLine();

            if (choise is null)
            {
                continue;
            }
            else if (choiseToMove.TryGetValue(choise, out var usersMove))
            {
                Console.WriteLine($"Your move: {usersMove}");
                Console.WriteLine($"Computer's move: {computersMove}");
                string gameResult = rules.RulesTable[usersMove][computersMove] switch
                {
                    GameRules.GameResult.Win => "You win!",
                    GameRules.GameResult.Lose => "You lose(",
                    GameRules.GameResult.Draw => "It's a draw!",
                    _ => throw new NotImplementedException(),
                };
                Console.WriteLine(gameResult);
                Console.WriteLine($"HMAC key: {hmac.Key}");
                shouldGameContinue = false;
            }
            else if (choise == "0")
            {
                shouldGameContinue = false;
            }
            else if (choise == "?")
            {
                Console.WriteLine(rules);
            }
            else
            {
                continue;
            }
        }
    }

    static void Main(string[] args)
    {
        try
        {
            var rules = new GameRules(args);
            var hmac = new HmacWorks();
            RunGame(rules, hmac);
        }
        catch (ArgumentException e)
        when (e.Message.Contains("The number of moves in the game must be odd."))
        {
            Console.WriteLine(e.Message);
        }
        catch (ArgumentException e)
        when (e.Message.Contains("All moves should be unique."))
        {
            Console.WriteLine(e.Message);
        }
    }
}
