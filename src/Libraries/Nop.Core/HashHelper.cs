using System.Security.Cryptography;

namespace Nop.Core;

/// <summary>
/// Hash helper class
/// </summary>
public partial class HashHelper
{
    /// <summary>
    /// Create a data hash
    /// </summary>
    /// <param name="data">The data for calculating the hash</param>
    /// <param name="hashAlgorithm">Hash algorithm</param>
    /// <param name="trimByteCount">The number of bytes, which will be used in the hash algorithm; leave 0 to use all array</param>
    /// <returns>Data hash</returns>
    public static string CreateHash(byte[] data, string hashAlgorithm, int trimByteCount = 0)
    {
		logger.LogDebug(">>> Entering CreateHash(data={Data},hashAlgorithm={HashAlgorithm},trimByteCount={TrimByteCount})", data, hashAlgorithm, trimByteCount);
        ArgumentException.ThrowIfNullOrEmpty(hashAlgorithm);
            
        var algorithm = (HashAlgorithm)CryptoConfig.CreateFromName(hashAlgorithm) ?? throw new ArgumentException("Unrecognized hash name");

		logger.LogDebug("CreateHash(data={Data},hashAlgorithm={HashAlgorithm},trimByteCount={TrimByteCount}): algorithm → {Algorithm}", data, hashAlgorithm, trimByteCount, algorithm);
        if (trimByteCount > 0 && data.Length > trimByteCount)
        {
            var newData = new byte[trimByteCount];
            Array.Copy(data, newData, trimByteCount);

            return BitConverter.ToString(algorithm.ComputeHash(newData)).Replace("-", string.Empty);
        }

		logger.LogDebug("<<< Exiting CreateHash(data={Data},hashAlgorithm={HashAlgorithm},trimByteCount={TrimByteCount})", data, hashAlgorithm, trimByteCount);
        return BitConverter.ToString(algorithm.ComputeHash(data)).Replace("-", string.Empty);
    }
}
