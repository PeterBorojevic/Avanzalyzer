namespace Core.Common.ConsoleTables.v1;

public record Col(string? Value, int Index, int? RowIndex = null)
{
    /// <summary>
    /// The value the column contains
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

    // TODO info about which row from top to bottom (or reverse)
}
