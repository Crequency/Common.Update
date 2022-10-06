using System.Text.Json;

namespace Common.Update.Manual
{
    public class StringCoupleNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name switch
        {
            "Item1" => "MD5",
            "Item2" => "SHA1",
            "Item3" => "Size",
            _ => name,
        };
    }
}
