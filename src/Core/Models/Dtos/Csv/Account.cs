using CsvHelper.Configuration.Attributes;

namespace Core.Models.Dtos.Csv;

public class Account
{
    [Name("Bank")]
    [Optional]
    public string Bank { get; set; }

    [Name("Kontonamn")]
    [Optional]
    public string AccountName { get; set; }

    [Name("Kontonummer")]
    public string AccountNumber { get; set; }
    [Name("Kontotyp")]
    public string AccountType { get; set; }
    [Name("Totalvärde")]
    public decimal Balance { get; set; }
}
