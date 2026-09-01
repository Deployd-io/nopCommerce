using System.Security.Cryptography;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Security;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Security;

/// <summary>
/// Encryption service
/// </summary>
public partial class EncryptionService : IEncryptionService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly SecuritySettings _securitySettings;

    #endregion

    #region Ctor

    public EncryptionService(SecuritySettings securitySettings)
    {
		_logger.LogDebug(">>> Entering EncryptionService(securitySettings={SecuritySettings})", securitySettings);
        _securitySettings = securitySettings;
		_logger.LogDebug("EncryptionService(securitySettings={SecuritySettings}): _securitySettings → {_securitySettings}", securitySettings, _securitySettings);
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
		_logger.LogDebug(">>> Entering EncryptTextToMemory(data={Data},provider={Provider})", data, provider);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, provider.CreateEncryptor(), CryptoStreamMode.Write))
        {
		_logger.LogDebug("EncryptTextToMemory(data={Data},provider={Provider}): ms → {Ms}", data, provider, ms);
            var toEncrypt = Encoding.Unicode.GetBytes(data);
            cs.Write(toEncrypt, 0, toEncrypt.Length);
            cs.FlushFinalBlock();
        }

		_logger.LogDebug("<<< Exiting EncryptTextToMemory(data={Data},provider={Provider})", data, provider);
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
		_logger.LogDebug(">>> Entering DecryptTextFromMemory(data={Data},provider={Provider})", data, provider);
        using var ms = new MemoryStream(data);
        using var cs = new CryptoStream(ms, provider.CreateDecryptor(), CryptoStreamMode.Read);
		_logger.LogDebug("DecryptTextFromMemory(data={Data},provider={Provider}): cs → {Cs}", data, provider, cs);
        using var sr = new StreamReader(cs, Encoding.Unicode);
			_logger.LogDebug("DecryptTextFromMemory(data={Data},provider={Provider}): sr → {Sr}", data, provider, sr);
			_logger.LogDebug("DecryptTextFromMemory(data={Data},provider={Provider}): ms → {Ms}", data, provider, ms);
            
		_logger.LogDebug("<<< Exiting DecryptTextFromMemory(data={Data},provider={Provider})", data, provider);
        return sr.ReadToEnd();
    }

    /// <summary>
    /// Gets encryption algorithm
    /// </summary>
    /// <param name="encryptionKey">Encryption key</param>
    /// <returns>Encryption algorithm</returns>
    protected virtual SymmetricAlgorithm GetEncryptionAlgorithm(string encryptionKey)
    {
		_logger.LogDebug(">>> Entering GetEncryptionAlgorithm(encryptionKey=<redacted>)");
        ArgumentException.ThrowIfNullOrEmpty(encryptionKey);

        SymmetricAlgorithm provider = _securitySettings.UseAesEncryptionAlgorithm ? Aes.Create() : TripleDES.Create();
		_logger.LogDebug("GetEncryptionAlgorithm(encryptionKey=<redacted>): provider → {Provider}", provider);

        var vectorBlockSize = provider.BlockSize / 8;

        provider.Key = Encoding.ASCII.GetBytes(encryptionKey[0..16]);
        provider.IV = Encoding.ASCII.GetBytes(encryptionKey[^vectorBlockSize..]);

		_logger.LogDebug("<<< Exiting GetEncryptionAlgorithm(encryptionKey=<redacted>)");
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
		_logger.LogDebug(">>> Entering CreateSaltKey(size={Size})", size);
        //generate a cryptographic random number
        using var provider = RandomNumberGenerator.Create();
		_logger.LogDebug("CreateSaltKey(size={Size}): provider → {Provider}", size, provider);
        var buff = new byte[size];
        provider.GetBytes(buff);

        // Return a Base64 string representation of the random number
		_logger.LogDebug("<<< Exiting CreateSaltKey(size={Size})", size);
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
		_logger.LogDebug(">>> Entering CreatePasswordHash(password=<redacted>,saltkey=<redacted>,passwordFormat=<redacted>)");
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
		_logger.LogDebug(">>> Entering EncryptText(plainText={PlainText},encryptionPrivateKey=<redacted>)", plainText);
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        if (string.IsNullOrEmpty(encryptionPrivateKey))
            encryptionPrivateKey = _securitySettings.EncryptionKey;

        using var provider = GetEncryptionAlgorithm(encryptionPrivateKey);
		_logger.LogDebug("EncryptText(plainText={PlainText},encryptionPrivateKey=<redacted>): provider → {Provider}", plainText, provider);
        var encryptedBinary = EncryptTextToMemory(plainText, provider);
		_logger.LogDebug("EncryptText(plainText={PlainText},encryptionPrivateKey=<redacted>): encryptedBinary → {EncryptedBinary}", plainText, encryptedBinary);

		_logger.LogDebug("<<< Exiting EncryptText(plainText={PlainText},encryptionPrivateKey=<redacted>)", plainText);
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
		_logger.LogDebug(">>> Entering DecryptText(cipherText={CipherText},encryptionPrivateKey=<redacted>)", cipherText);
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        if (string.IsNullOrEmpty(encryptionPrivateKey))
            encryptionPrivateKey = _securitySettings.EncryptionKey;

        using var provider = GetEncryptionAlgorithm(encryptionPrivateKey);
		_logger.LogDebug("DecryptText(cipherText={CipherText},encryptionPrivateKey=<redacted>): provider → {Provider}", cipherText, provider);

        var buffer = Convert.FromBase64String(cipherText);
		_logger.LogDebug("DecryptText(cipherText={CipherText},encryptionPrivateKey=<redacted>): buffer → {Buffer}", cipherText, buffer);
		_logger.LogDebug("<<< Exiting DecryptText(cipherText={CipherText},encryptionPrivateKey=<redacted>)", cipherText);
        return DecryptTextFromMemory(buffer, provider);
    }

    #endregion
}