using System;
using System.Security.Cryptography;
using System.Text;

namespace RockPaperScissors;

public class HmacWorks
{
    private readonly byte[] keyBytes;

    public readonly string Key;

    public HmacWorks()
    {
        using (var hmacsha256 = new HMACSHA256())
        {
            keyBytes = hmacsha256.Key;
            Key = Convert.ToBase64String(keyBytes);
        }
    }

    public HmacWorks(string key)
    {
        keyBytes = Convert.FromBase64String(key);
        Key = key;
    }

    public HmacWorks(byte[] key)
    {
        keyBytes = key;
        Key = Convert.ToBase64String(key);
    }

    public string GetHmacSignature(string text)
    {
        using (var hmacsha256 = new HMACSHA256(keyBytes))
        {
            byte[] hash = hmacsha256.ComputeHash(Encoding.Default.GetBytes(text));
            return Convert.ToBase64String(hash);
        }
    }
}
