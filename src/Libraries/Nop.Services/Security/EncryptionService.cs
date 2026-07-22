using System.Security.Cryptography;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security;

/// <summary>
/// Encryption service
/// </summary>
public partial class EncryptionService : IEncryptionService
{
    #region Fields

    protected readonly SecuritySettings _securitySettings;

    #endregion

    #region Ctor

    public EncryptionService(SecuritySettings securitySettings)
    {
		logger.LogDebug(">>> Entering EncryptionService(securitySettings={SecuritySettings})", securitySettings);
        _securitySettings = securitySettings;
		logger.LogDebug("<<< Exiting EncryptionService(securitySettings={SecuritySettings})", securitySettings);
		logger.LogDebug("EncryptionService(securitySettings={SecuritySettings}): _securitySettings → {_securitySettings}", securitySettings, _securitySettings);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Encrypt text
    /// </summary>
    /// <param name="data">Text to encrypt</param>
    /// <param name="provider">Encryption algorithm</param>
    /// <returns>Encrypted data</returns>
    protected static byte[] EncryptTextToMemory(string data, SymmetricAlgorithm provider)
    {
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, provider.CreateEncryptor(), CryptoStreamMode.Write))
        {
            var toEncrypt = Encoding.Unicode.GetBytes(data);
            cs.Write(toEncrypt, 0, toEncrypt.Length);
            cs.FlushFinalBlock();
        }

        return ms.ToArray();
    }

    /// <summary>
    /// Decrypt text
    /// </summary>
    /// <param name="data">Encrypted data</param>
    /// <param name="provider">Encryption algorithm</param>
    /// <returns>Decrypted text</returns>
    protected static string DecryptTextFromMemory(byte[] data, SymmetricAlgorithm provider)
    {
        using var ms = new MemoryStream(data);
        using var cs = new CryptoStream(ms, provider.CreateDecryptor(), CryptoStreamMode.Read);
        using var sr = new StreamReader(cs, Encoding.Unicode);
            
        return sr.ReadToEnd();
    }

    /// <summary>
    /// Gets encryption algorithm
    /// </summary>
    /// <param name="encryptionKey">Encryption key</param>
    /// <returns>Encryption algorithm</returns>
    protected virtual SymmetricAlgorithm GetEncryptionAlgorithm(string encryptionKey)
    {
		logger.LogDebug(">>> Entering GetEncryptionAlgorithm(encryptionKey={EncryptionKey})", encryptionKey);
        ArgumentException.ThrowIfNullOrEmpty(encryptionKey);

        SymmetricAlgorithm provider = _securitySettings.UseAesEncryptionAlgorithm ? Aes.Create() : TripleDES.Create();

		logger.LogDebug("GetEncryptionAlgorithm(encryptionKey={EncryptionKey}): provider → {Provider}", encryptionKey, provider);
        var vectorBlockSize = provider.BlockSize / 8;

        provider.Key = Encoding.ASCII.GetBytes(encryptionKey[0..16]);
        provider.IV = Encoding.ASCII.GetBytes(encryptionKey[^vectorBlockSize..]);

		logger.LogDebug("<<< Exiting GetEncryptionAlgorithm(encryptionKey={EncryptionKey})", encryptionKey);
        return provider;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create salt key
    /// </summary>
    /// <param name="size">Key size</param>
    /// <returns>Salt key</returns>
    public virtual string CreateSaltKey(int size)
    {
		logger.LogDebug(">>> Entering CreateSaltKey(size={Size})", size);
        //generate a cryptographic random number
        using var provider = RandomNumberGenerator.Create();
        var buff = new byte[size];
		logger.LogDebug("CreateSaltKey(size={Size}): provider → {Provider}", size, provider);
        provider.GetBytes(buff);

        // Return a Base64 string representation of the random number
		logger.LogDebug("<<< Exiting CreateSaltKey(size={Size})", size);
        return Convert.ToBase64String(buff);
    }

    /// <summary>
    /// Create a password hash
    /// </summary>
    /// <param name="password">Password</param>
    /// <param name="saltkey">Salk key</param>
    /// <param name="passwordFormat">Password format (hash algorithm)</param>
    /// <returns>Password hash</returns>
    public virtual string CreatePasswordHash(string password, string saltkey, string passwordFormat)
    {
		logger.LogDebug(">>> Entering CreatePasswordHash(password={Password},saltkey={Saltkey},passwordFormat={PasswordFormat})", password, saltkey, passwordFormat);
		logger.LogDebug("<<< Exiting CreatePasswordHash(password={Password},saltkey={Saltkey},passwordFormat={PasswordFormat})", password, saltkey, passwordFormat);
        return HashHelper.CreateHash(Encoding.UTF8.GetBytes(string.Concat(password, saltkey)), passwordFormat);
    }

    /// <summary>
    /// Encrypt text
    /// </summary>
    /// <param name="plainText">Text to encrypt</param>
    /// <param name="encryptionPrivateKey">Encryption private key</param>
    /// <returns>Encrypted text</returns>
    public virtual string EncryptText(string plainText, string encryptionPrivateKey = "")
    {
		logger.LogDebug(">>> Entering EncryptText(plainText={PlainText},encryptionPrivateKey={EncryptionPrivateKey})", plainText, encryptionPrivateKey);
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        if (string.IsNullOrEmpty(encryptionPrivateKey))
            encryptionPrivateKey = _securitySettings.EncryptionKey;

        using var provider = GetEncryptionAlgorithm(encryptionPrivateKey);
        var encryptedBinary = EncryptTextToMemory(plainText, provider);
		logger.LogDebug("EncryptText(plainText={PlainText},encryptionPrivateKey={EncryptionPrivateKey}): provider → {Provider}", plainText, encryptionPrivateKey, provider);
		logger.LogDebug("EncryptText(plainText={PlainText},encryptionPrivateKey={EncryptionPrivateKey}): encryptedBinary → {EncryptedBinary}", plainText, encryptionPrivateKey, encryptedBinary);

		logger.LogDebug("<<< Exiting EncryptText(plainText={PlainText},encryptionPrivateKey={EncryptionPrivateKey})", plainText, encryptionPrivateKey);
        return Convert.ToBase64String(encryptedBinary);
    }

    /// <summary>
    /// Decrypt text
    /// </summary>
    /// <param name="cipherText">Text to decrypt</param>
    /// <param name="encryptionPrivateKey">Encryption private key</param>
    /// <returns>Decrypted text</returns>
    public virtual string DecryptText(string cipherText, string encryptionPrivateKey = "")
    {
		logger.LogDebug(">>> Entering DecryptText(cipherText={CipherText},encryptionPrivateKey={EncryptionPrivateKey})", cipherText, encryptionPrivateKey);
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        if (string.IsNullOrEmpty(encryptionPrivateKey))
            encryptionPrivateKey = _securitySettings.EncryptionKey;

        using var provider = GetEncryptionAlgorithm(encryptionPrivateKey);

		logger.LogDebug("DecryptText(cipherText={CipherText},encryptionPrivateKey={EncryptionPrivateKey}): provider → {Provider}", cipherText, encryptionPrivateKey, provider);
        var buffer = Convert.FromBase64String(cipherText);
		logger.LogDebug("DecryptText(cipherText={CipherText},encryptionPrivateKey={EncryptionPrivateKey}): buffer → {Buffer}", cipherText, encryptionPrivateKey, buffer);
		logger.LogDebug("<<< Exiting DecryptText(cipherText={CipherText},encryptionPrivateKey={EncryptionPrivateKey})", cipherText, encryptionPrivateKey);
        return DecryptTextFromMemory(buffer, provider);
    }

    #endregion
}
