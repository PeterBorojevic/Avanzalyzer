using CsvHelper.Configuration.Attributes;

namespace Core.Models.Dtos.Csv;
/* TODO
* This class is used by the infrastructure layer as well by application.Could we separate infrastructure models and internal ones?
*/
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
