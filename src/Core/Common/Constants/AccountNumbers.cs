namespace Core.Common.Constants;

public class AccountNumbers
{
    public static string Get(string accountName)
    {
        return accountName switch
        {
            "Swishproxy💸" => "",
            "Fondue au fonder 🫕💼" => "4119988",
            "Långthörm säjving🚀🚀" => "2615122",
            "The Trust Fund🐋" => "4266071",
            "Pension" => "1618352",
            _ => string.Empty
        };
    }
}
