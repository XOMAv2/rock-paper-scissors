using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace RockPaperScissors;

public class TablePrinter
{
    private readonly ImmutableArray<string> title;
    private readonly ImmutableList<ImmutableArray<string>> rows;
    private readonly ImmutableArray<int> lengths;

    private ImmutableArray<string> ToStringArray(IEnumerable<object> cells) =>
        ImmutableArray.CreateRange(cells.Select(c => $"{c}"));

    public TablePrinter(IEnumerable<object> title, IEnumerable<IEnumerable<object>> rows)
    {
        this.title = ToStringArray(title);

        IEnumerable<ImmutableArray<string>> collOfStringCells = rows.Select((r, i) =>
        {
            var row = ToStringArray(r);

            if (row.Length != this.title.Length)
            {
                throw new ArgumentException(
                    $"The number of cells in the row with index {i} is "
                    + $"{row.Length}, which does not correspond to the "
                    + $"number of cells in the title being {this.title.Length}.",
                    nameof(rows));
            }

            return row;
        });

        this.rows = ImmutableList.CreateRange(collOfStringCells);

        var columnToMaxLength = (string titleCell, int titleIndex) =>
        {
            return collOfStringCells
            .Select(stringCells => stringCells[titleIndex].Length)
            .Append(titleCell.Length)
            .Max();
        };

        lengths = ImmutableArray.CreateRange(this.title.Select(columnToMaxLength));
    }

    public TablePrinter(IEnumerable<object> title) : this(title, new List<List<object>>())
    { }

    public TablePrinter AddRow(IEnumerable<object> row) =>
        new TablePrinter(title, rows.Select(cell => (IEnumerable<object>)cell).Append(row));

    private string GetHorizontalDelimiter() =>
        $"+-{string.Join("-+-", lengths.Select(l => new string('-', l)))}-+";

    private string GetRow(IEnumerable<string> row) =>
        $"| {string.Join(" | ", row.Select((r, i) => r.PadRight(lengths[i])))} |";

    public override string ToString()
    {
        StringBuilder tableBuilder = new StringBuilder();
        string delimiter = GetHorizontalDelimiter();

        tableBuilder.AppendLine(delimiter);
        tableBuilder.AppendLine(GetRow(title));
        tableBuilder.AppendLine(delimiter);

        foreach (var row in rows)
        {
            tableBuilder.AppendLine(GetRow(row));
        }

        tableBuilder.Append(delimiter);

        return tableBuilder.ToString();
    }
}
