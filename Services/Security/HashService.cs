using System.Security.Cryptography;

namespace LoginApp.Services.Security;

public sealed class HashService
{
    public string GetHash(string password)
    {
        var salt = new byte[32];
        RandomNumberGenerator.Create().GetBytes(salt);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 20000, HashAlgorithmName.SHA256);


        byte[] hash = pbkdf2.GetBytes(32);

        byte[] hashBytes = new byte[64];

        Array.Copy(salt, 0, hashBytes, 0, 32);
        Array.Copy(hash, 0, hashBytes, 32, 32);

        var result = Convert.ToBase64String(hashBytes);
        return result;
    }

    public bool VerifyHash(string password, string paswordHash)
    {
        try
        {
            byte[] hashBytes = Convert.FromBase64String(paswordHash);
            byte[] salt = new byte[32];
            Array.Copy(hashBytes, 0, salt, 0, 32);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 20000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);

            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 32] != hash[i])
                {
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
        return true;
    }
}