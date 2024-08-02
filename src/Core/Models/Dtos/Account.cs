using CsvHelper.Configuration.Attributes;

namespace Core.Models.Dtos;

public class Account
{
    [Name("Bank")]
    [Optional]
    public string Bank { get; set; }
    [Name("Kontonummer")]
    public string AccountNumber { get; set; }
    [Name("Kontotyp")]
    public string AccountType { get; set; }
    [Name("Totalvärde")]
    public decimal Balance { get; set; }
}
