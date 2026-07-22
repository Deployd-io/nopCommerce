using FluentAssertions;
using Nop.Core.Domain.Media;
using Nop.Services.Media;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Media;

[TestFixture]
public class PictureServiceFileDescriptorTests : ServiceTest
{
    private IPictureService _pictureService;
    private IThumbService _thumbService;
    private readonly List<string> _generatedThumbPaths = [];

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _pictureService = GetService<IPictureService>();
        _thumbService = GetService<IThumbService>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _pictureService → {_pictureService}", _pictureService);
		logger.LogDebug("SetUp(): _thumbService → {_thumbService}", _thumbService);
    }

    [OneTimeTearDown]
    public void TearDown()
    {
		logger.LogDebug(">>> Entering TearDown()");
        foreach (var path in _generatedThumbPaths)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch
            {
				logger.LogError("Exception in TearDown(): {Arg0}", ex.Message);
                // ignored — best-effort cleanup
            }
        }
		logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task GetDefaultPictureUrlAsync_DoesNotLeakFileDescriptors()
    {
		logger.LogDebug(">>> Entering GetDefaultPictureUrlAsync_DoesNotLeakFileDescriptors()");
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

		logger.LogDebug("GetDefaultPictureUrlAsync_DoesNotLeakFileDescriptors(): fdsBefore → {FdsBefore}", fdsBefore);
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
        var leakedLive = fdsAfterLoop - fdsBefore;
		logger.LogDebug("GetDefaultPictureUrlAsync_DoesNotLeakFileDescriptors(): fdsAfterLoop → {FdsAfterLoop}", fdsAfterLoop);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var fdsAfterGc = CountOpenFileDescriptors();
        var leakedAfterGc = fdsAfterGc - fdsBefore;
		logger.LogDebug("GetDefaultPictureUrlAsync_DoesNotLeakFileDescriptors(): fdsAfterGc → {FdsAfterGc}", fdsAfterGc);

        TestContext.Out.WriteLine($"[fd leak probe] live delta = {leakedLive}, after-GC delta = {leakedAfterGc}");

		logger.LogDebug("GetDefaultPictureUrlAsync_DoesNotLeakFileDescriptors(): delta → {Delta}", delta);
        leakedLive.Should().Be(0,
            $"GetDefaultPictureUrlAsync leaked {leakedLive} file descriptors across {testIterations} thumb generations " +
            $"(after forced GC: {leakedAfterGc}). Likely an IDisposable (SKCodec/SKBitmap) is not wrapped in `using`.");
			logger.LogDebug("<<< Exiting GetDefaultPictureUrlAsync_DoesNotLeakFileDescriptors()");
			logger.LogDebug(">>> Entering GetDefaultThumbPathAsync(targetSize={TargetSize})", targetSize);
			logger.LogDebug("<<< Exiting GetDefaultThumbPathAsync(targetSize={TargetSize})", targetSize);
    }

    private async Task<string> GetDefaultThumbPathAsync(int targetSize)
    {
		logger.LogDebug(">>> Entering GetDefaultThumbPathAsync(targetSize={TargetSize})", targetSize);
        var thumbFileName = $"default-image_{targetSize}.png";
		logger.LogDebug("GetDefaultThumbPathAsync(targetSize={TargetSize}): thumbFileName → {ThumbFileName}", targetSize, thumbFileName);
		logger.LogDebug("<<< Exiting GetDefaultThumbPathAsync(targetSize={TargetSize})", targetSize);
        return await _thumbService.GetThumbLocalPathByFileNameAsync(thumbFileName);
    }

    private static int CountOpenFileDescriptors()
    {
		logger.LogDebug(">>> Entering CountOpenFileDescriptors()");
        var fdDir = OperatingSystem.IsLinux() ? "/proc/self/fd" : "/dev/fd";
		logger.LogDebug("CountOpenFileDescriptors(): fdDir → {FdDir}", fdDir);
		logger.LogDebug("<<< Exiting CountOpenFileDescriptors()");
        return System.IO.Directory.EnumerateFileSystemEntries(fdDir).Count();
    }
}
