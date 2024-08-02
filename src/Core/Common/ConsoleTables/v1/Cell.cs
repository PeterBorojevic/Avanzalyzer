namespace Core.Common.ConsoleTables.v1;

public record Cell(string? Value, int Index, int? RowIndex = null)
{
    /// <summary>
    /// The value the cell contains
    /// </summary>
    public string Value { get; init; } = Value ?? string.Empty;

    /// <summary>
    /// The column index from left to right.
    /// </summary>
    public int Index { get; init; } = Index;

    /// <summary>
    /// The row number counted from top to bottom with starting index 0.
    /// </summary>
    public int? RowIndex { get; init; } = RowIndex;
}
