namespace Core.Models.Tables;

public class Row
{
    public Row(params Column[] args)
    {
        NumOfColumns = args.Length;
        _columns = args;
    }

    public int NumOfColumns { get; }
    private Column[] _columns;

    //public string Format()
    //{
    //    var text = "|";
    //    var count = 0;
    //    var max
    //    var values = _columns.Select(c => c.Value).ToArray();
    //    foreach (var column in _columns)
    //    {
    //        text += "{" + count 
    //    }

    //    return string.Format(text.ToString(), values);
    //}
}

public record Column(string Value, int FormatLength);