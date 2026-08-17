using FluentAssertions;
using Nop.Core.Domain.Media;
using Nop.Services.Media;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Media;

[TestFixture]
public class PictureServiceFileDescriptorTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IPictureService _pictureService;
    private IThumbService _thumbService;
    private readonly List<string> _generatedThumbPaths = [];

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _pictureService = GetService<IPictureService>();
        _thumbService = GetService<IThumbService>();
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _thumbService → {_thumbService}", _thumbService);
		_logger.LogDebug("SetUp(): _pictureService → {_pictureService}", _pictureService);
    }

    [OneTimeTearDown]
    public void TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        foreach (var path in _generatedThumbPaths)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch
            {
				_logger.LogError("Exception in TearDown(): {Arg0}", ex.Message);
                // ignored — best-effort cleanup
            }
        }
		_logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task GetDefaultPictureUrlAsync_DoesNotLeakFileDescriptors()
    {
		_logger.LogDebug(">>> Entering GetDefaultPictureUrlAsync_DoesNotLeakFileDescriptors()");
        if (!OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS())
            Assert.Ignore("File descriptor counting requires /proc/self/fd (Linux) or /dev/fd (macOS).");

        const int warmupIterations = 10;
        const int testIterations = 100;
        const int baseSize = 990_000;

        for (var i = 0; i < warmupIterations; i++)
        {
            var size = baseSize + i;
            await _pictureService.GetDefaultPictureUrlAsync(size, PictureType.Entity);
            _generatedThumbPaths.Add(await GetDefaultThumbPathAsync(size));
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var fdsBefore = CountOpenFileDescriptors();
		_logger.LogDebug("GetDefaultPictureUrlAsync_DoesNotLeakFileDescriptors(): fdsBefore → {FdsBefore}", fdsBefore);

        for (var i = 0; i < testIterations; i++)
        {
            var size = baseSize + warmupIterations + i;
            await _pictureService.GetDefaultPictureUrlAsync(size, PictureType.Entity);
            _generatedThumbPaths.Add(await GetDefaultThumbPathAsync(size));
        }

        // Snapshot BEFORE forcing GC — finalizers would close the leaked fds and mask the bug.
        // Production symptom is exactly this "live" state: SKCodec instances sitting in older
        // generations holding open fds until Gen2 GC runs (hours, eventually hitting ulimit -n).
        var fdsAfterLoop = CountOpenFileDescriptors();
		_logger.LogDebug("GetDefaultPictureUrlAsync_DoesNotLeakFileDescriptors(): fdsAfterLoop → {FdsAfterLoop}", fdsAfterLoop);
        var leakedLive = fdsAfterLoop - fdsBefore;

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var fdsAfterGc = CountOpenFileDescriptors();
		_logger.LogDebug("GetDefaultPictureUrlAsync_DoesNotLeakFileDescriptors(): fdsAfterGc → {FdsAfterGc}", fdsAfterGc);
        var leakedAfterGc = fdsAfterGc - fdsBefore;

        TestContext.Out.WriteLine($"[fd leak probe] live delta = {leakedLive}, after-GC delta = {leakedAfterGc}");
		_logger.LogDebug("GetDefaultPictureUrlAsync_DoesNotLeakFileDescriptors(): delta → {Delta}", delta);

        leakedLive.Should().Be(0,
            $"GetDefaultPictureUrlAsync leaked {leakedLive} file descriptors across {testIterations} thumb generations " +
            $"(after forced GC: {leakedAfterGc}). Likely an IDisposable (SKCodec/SKBitmap) is not wrapped in `using`.");
			_logger.LogDebug("<<< Exiting GetDefaultPictureUrlAsync_DoesNotLeakFileDescriptors()");
			_logger.LogDebug(">>> Entering GetDefaultThumbPathAsync(targetSize={TargetSize})", targetSize);
			_logger.LogDebug("<<< Exiting GetDefaultThumbPathAsync(targetSize={TargetSize})", targetSize);
    }

    private async Task<string> GetDefaultThumbPathAsync(int targetSize)
    {
		_logger.LogDebug(">>> Entering GetDefaultThumbPathAsync(targetSize={TargetSize})", targetSize);
        var thumbFileName = $"default-image_{targetSize}.png";
		_logger.LogDebug("GetDefaultThumbPathAsync(targetSize={TargetSize}): thumbFileName → {ThumbFileName}", targetSize, thumbFileName);
		_logger.LogDebug("<<< Exiting GetDefaultThumbPathAsync(targetSize={TargetSize})", targetSize);
        return await _thumbService.GetThumbLocalPathByFileNameAsync(thumbFileName);
    }

    private static int CountOpenFileDescriptors()
    {
		_logger.LogDebug(">>> Entering CountOpenFileDescriptors()");
        var fdDir = OperatingSystem.IsLinux() ? "/proc/self/fd" : "/dev/fd";
		_logger.LogDebug("CountOpenFileDescriptors(): fdDir → {FdDir}", fdDir);
		_logger.LogDebug("<<< Exiting CountOpenFileDescriptors()");
        return System.IO.Directory.EnumerateFileSystemEntries(fdDir).Count();
    }
}
