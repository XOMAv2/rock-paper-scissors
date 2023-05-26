using System;
using System.Collections.Immutable;
using System.Linq;

namespace RockPaperScissors;

public class GameRules
{
    public enum GameResult
    {
        Lose,
        Win,
        Draw,
    }

    public readonly ImmutableArray<string> Moves;

    /// <summary>
    /// A dictionary containing the result of the game for the first player's
    /// move specified in the first indexer, against the second player's move
    /// specified in the second indexer.
    /// </summary>
    public readonly ImmutableDictionary<string, ImmutableDictionary<string, GameResult>> RulesTable;

    public GameRules(string[] moves)
    {
        if (moves.Length % 2 != 1)
        {
            throw new ArgumentException(
                "The number of moves in the game must be odd.", nameof(moves));
        }

        if (moves.Length != moves.Distinct().Count())
        {
            throw new ArgumentException(
                "All moves should be unique.", nameof(moves));
        }

        int halfLength = moves.Length / 2;
        var rulesTableBuilder = ImmutableDictionary.CreateBuilder<string, ImmutableDictionary<string, GameResult>>();

        for (int i = 0; i < moves.Length; i++)
        {
            var rules = ImmutableDictionary.CreateBuilder<string, GameResult>();

            rules.Add(moves[i], GameResult.Draw);

            for (int j = i + 1; j < i + halfLength + 1; j++)
            {
                int moveIndex = j % moves.Length;
                rules.Add(moves[moveIndex], GameResult.Lose);
            }

            for (int j = i - halfLength; j < i; j++)
            {
                int moveIndex = (j + moves.Length) % moves.Length;
                rules.Add(moves[moveIndex], GameResult.Win);
            }

            rulesTableBuilder.Add(moves[i], rules.ToImmutable());
        }

        RulesTable = rulesTableBuilder.ToImmutable();
        Moves = ImmutableArray.Create(moves);
    }

    /// <summary>
    /// Does the first player's move lose to the second player's move?
    /// </summary>
    public bool DoseLose(string firstPlayerMove, string secondPlayerMove) =>
        RulesTable[firstPlayerMove][secondPlayerMove] == GameResult.Lose;

    /// <summary>
    /// Does the first player's move win against the second player's move?
    /// </summary>
    public bool DoseWin(string firstPlayerMove, string secondPlayerMove) =>
        RulesTable[firstPlayerMove][secondPlayerMove] == GameResult.Win;

    /// <summary>
    /// Is there a draw between the first player's move and the second player's move?
    /// </summary>
    public bool IsDraw(string firstPlayerMove, string secondPlayerMove) =>
        RulesTable[firstPlayerMove][secondPlayerMove] == GameResult.Draw;

    public override string ToString()
    {
        var title = Moves.Prepend(@"1st Player \ 2nd Player");
        TablePrinter printer = new TablePrinter(title);

        foreach (var firstPlayerMove in Moves)
        {
            var row = Moves
                .Select(secondPlayerMove => (object)RulesTable[firstPlayerMove][secondPlayerMove])
                .Prepend(firstPlayerMove);
            printer = printer.AddRow(row);
        }

        return printer.ToString();
    }
}
