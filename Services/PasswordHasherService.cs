using System.Security.Cryptography;

namespace StudentServiceRequestSystem.Services;

public class PasswordHasherService : IPasswordHasherService
{
    private const int SaltSize = 16; // 128 bit
    private const int KeySize = 32;  // 256 bit
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            Algorithm,
            KeySize
        );

        // Format: {salt (16 bytes)}{hash (32 bytes)}
        byte[] result = new byte[SaltSize + KeySize];
        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        Buffer.BlockCopy(hash, 0, result, SaltSize, KeySize);

        return Convert.ToBase64String(result);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword) || string.IsNullOrWhiteSpace(providedPassword))
        {
            return false;
        }

        try
        {
            byte[] decoded = Convert.FromBase64String(hashedPassword);
            if (decoded.Length != SaltSize + KeySize)
            {
                return false;
            }

            byte[] salt = new byte[SaltSize];
            byte[] expectedHash = new byte[KeySize];

            Buffer.BlockCopy(decoded, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(decoded, SaltSize, expectedHash, 0, KeySize);

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                providedPassword,
                salt,
                Iterations,
                Algorithm,
                KeySize
            );

            return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
        }
        catch
        {
            return false;
        }
    }
}
