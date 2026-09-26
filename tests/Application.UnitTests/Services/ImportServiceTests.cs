using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Heracles.Application.Data;
using Heracles.Application.Import;
using Heracles.Application.Import.Progress;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Heracles.Application.UnitTests.Services;

[TestFixture]
public class ImportServiceTests
{
    private const long MaximumCombinedSize = 25L * 1024 * 1024;
    private const string GoodFilename = "Filename.gpx";

    private Mock<IGpxService> _mockGpxService = null!;
    private Mock<ILogger<ImportService>> _mockLogger = null!;
    private Mock<ITrackRepository> _mockTrackRepository = null!;

    private ImportService _sut = null!;

    [SetUp]
    public void BeforeEachTest()
    {
        _mockLogger = new Mock<ILogger<ImportService>>();
        _mockGpxService = new Mock<IGpxService>();
        _mockTrackRepository = new Mock<ITrackRepository>();

        _mockTrackRepository
            .Setup(x => x.GetExistingTracksAsync())
            .ReturnsAsync(new List<string>());

        _mockGpxService
            .Setup(x => x.LoadContentsOfGpxFileAsync(
                It.IsAny<IBrowserFile>(),
                It.IsAny<long>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => GetNewTrack());

        _mockTrackRepository
            .Setup(x => x.SaveImportedFilesAsync(
                It.IsAny<ImportFilesResult>(),
                It.IsAny<TrackImportProgress>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _sut = new ImportService(
            _mockGpxService.Object,
            _mockTrackRepository.Object,
            _mockLogger.Object);
    }

    [Test]
    public async Task ImportTracksFromGpxFiles_NullFilesCollection_ThrowsArgumentNullException()
    {
        Func<Task> action = () =>
            _sut.ImportTracksFromGpxFilesAsync(
                null!,
                MaximumCombinedSize);

        await action.Should()
            .ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task ImportTracksFromGpxFiles_EmptyFilesCollection_ThrowsArgumentException()
    {
        Func<Task> action = () =>
            _sut.ImportTracksFromGpxFilesAsync(
                Array.Empty<IBrowserFile>(),
                MaximumCombinedSize);

        await action.Should()
            .ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task ImportTracksFromGpxFiles_InvalidMaximumSize_ThrowsArgumentOutOfRangeException()
    {
        var files = CreateFiles(GoodFilename);

        Func<Task> action = () =>
            _sut.ImportTracksFromGpxFilesAsync(files, 0);

        await action.Should()
            .ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task ImportTracksFromGpxFiles_CombinedSizeExceedsLimit_ThrowsArgumentException()
    {
        var files = new List<IBrowserFile>
        {
            CreateFile("First.gpx", 15L * 1024 * 1024),
            CreateFile("Second.gpx", 15L * 1024 * 1024)
        };

        Func<Task> action = () =>
            _sut.ImportTracksFromGpxFilesAsync(
                files,
                MaximumCombinedSize);

        await action.Should()
            .ThrowAsync<ArgumentException>();

        VerifyPersistenceNeverCalled();
    }

    [Test]
    public async Task ImportTracksFromGpxFiles_FileNotGpxExtension_ReturnsOneFailedImport()
    {
        const string filename = "Filename.xxx";

        var result = await ImportAsync(
            CreateFiles(filename));

        result.ImportedFiles.Should().BeEmpty();
        result.FailedFiles.Should().ContainSingle();

        result.FailedFiles[0].Filename.Should().Be(filename);

        result.FailedFiles[0].Reason.Should()
            .Be(ImportServiceStrings.IncorrectFileExtension);

        _mockGpxService.Verify(
            x => x.LoadContentsOfGpxFileAsync(
                It.IsAny<IBrowserFile>(),
                It.IsAny<long>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        VerifyPersistenceNeverCalled();
    }

    [Test]
    public async Task ImportTracksFromGpxFiles_UppercaseGpxExtension_ImportsSuccessfully()
    {
        var result = await ImportAsync(
            CreateFiles("Filename.GPX"));

        result.ImportedFiles.Should().ContainSingle();
        result.FailedFiles.Should().BeEmpty();

        VerifyPersistenceCalledOnce();
    }

    [Test]
    public async Task ImportTracksFromGpxFiles_GetTrackThrowsException_ReturnsStandardFailureMessage()
    {
        _mockGpxService
            .Setup(x => x.LoadContentsOfGpxFileAsync(
                It.IsAny<IBrowserFile>(),
                It.IsAny<long>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong"));

        var result = await ImportAsync(
            CreateFiles(GoodFilename));

        result.ImportedFiles.Should().BeEmpty();
        result.FailedFiles.Should().ContainSingle();

        result.FailedFiles[0].Filename.Should()
            .Be(GoodFilename);

        result.FailedFiles[0].Reason.Should()
            .Be(ImportServiceStrings.FileCouldNotBeProcessed);

        VerifyPersistenceNeverCalled();
    }

    [Test]
    public async Task ImportTracksFromGpxFiles_NullTrack_ReturnsOneFailedImport()
    {
        _mockGpxService
            .Setup(x => x.LoadContentsOfGpxFileAsync(
                It.IsAny<IBrowserFile>(),
                It.IsAny<long>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Track)null!);

        var result = await ImportAsync(
            CreateFiles(GoodFilename));

        result.ImportedFiles.Should().BeEmpty();
        result.FailedFiles.Should().ContainSingle();

        result.FailedFiles[0].Reason.Should()
            .Be(ImportServiceStrings.NoTrackFound);

        VerifyPersistenceNeverCalled();
    }

    [Test]
    public async Task ImportTracksFromGpxFiles_ImportedTrackAlreadyExists_ReturnsDuplicateFailure()
    {
        _mockTrackRepository
            .Setup(x => x.GetExistingTracksAsync())
            .ReturnsAsync(new List<string> { "NewTrack" });

        var result = await ImportAsync(
            CreateFiles(GoodFilename));

        result.ImportedFiles.Should().BeEmpty();
        result.FailedFiles.Should().ContainSingle();

        result.FailedFiles[0].Reason.Should()
            .Be(ImportServiceStrings.DuplicateTrackRecord);

        VerifyPersistenceNeverCalled();
    }

    [Test]
    public async Task ImportTracksFromGpxFiles_NoTrackSegments_ReturnsValidationFailure()
    {
        _mockGpxService
            .Setup(x => x.LoadContentsOfGpxFileAsync(
                It.IsAny<IBrowserFile>(),
                It.IsAny<long>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Track
            {
                Name = "NewTrack"
            });

        var result = await ImportAsync(
            CreateFiles(GoodFilename));

        result.ImportedFiles.Should().BeEmpty();
        result.FailedFiles.Should().ContainSingle();

        result.FailedFiles[0].Reason.Should()
            .Be(ImportServiceStrings.NoTrackSegmentsFound);

        VerifyPersistenceNeverCalled();
    }

    [Test]
    public async Task ImportTracksFromGpxFiles_NoTrackPoints_ReturnsValidationFailure()
    {
        _mockGpxService
            .Setup(x => x.LoadContentsOfGpxFileAsync(
                It.IsAny<IBrowserFile>(),
                It.IsAny<long>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Track
            {
                Name = "NewTrack",
                TrackSegments = new List<TrackSegment>
                {
                    new()
                }
            });

        var result = await ImportAsync(
            CreateFiles(GoodFilename));

        result.ImportedFiles.Should().BeEmpty();
        result.FailedFiles.Should().ContainSingle();

        result.FailedFiles[0].Reason.Should()
            .Be(ImportServiceStrings.NoTrackPointsFound);

        VerifyPersistenceNeverCalled();
    }

    [Test]
    public async Task ImportTracksFromGpxFiles_ValidFile_ReturnsSuccessfulImport()
    {
        var result = await ImportAsync(
            CreateFiles(GoodFilename));

        result.ImportedFiles.Should().ContainSingle();
        result.FailedFiles.Should().BeEmpty();

        result.ImportedFiles[0].Filename.Should()
            .Be(GoodFilename);

        result.ImportedFiles[0].Reason.Should()
            .Be(ImportServiceStrings.ImportSuccess);

        result.Tracks.Should().ContainSingle();
        result.TrackSegments.Should().ContainSingle();
        result.TrackPoints.Should().ContainSingle();

        VerifyPersistenceCalledOnce();
    }

    [Test]
    public async Task ImportTracksFromGpxFiles_DuplicateWithinSameBatch_ImportsOnlyFirstFile()
    {
        var files = CreateFiles(
            "First.gpx",
            "Second.gpx");

        var result = await ImportAsync(files);

        result.ImportedFiles.Should().ContainSingle();
        result.FailedFiles.Should().ContainSingle();

        result.ImportedFiles[0].Filename.Should()
            .Be("First.gpx");

        result.FailedFiles[0].Filename.Should()
            .Be("Second.gpx");

        result.FailedFiles[0].Reason.Should()
            .Be(ImportServiceStrings.DuplicateTrackRecord);

        result.Tracks.Should().ContainSingle();

        VerifyPersistenceCalledOnce();
    }

    [Test]
    public async Task ImportTracksFromGpxFiles_MixedValidAndInvalidFiles_PersistsValidFiles()
    {
        var files = CreateFiles(
            "Valid.gpx",
            "Invalid.txt");

        var result = await ImportAsync(files);

        result.ImportedFiles.Should().ContainSingle();
        result.FailedFiles.Should().ContainSingle();

        result.ImportedFiles[0].Filename.Should()
            .Be("Valid.gpx");

        result.FailedFiles[0].Filename.Should()
            .Be("Invalid.txt");

        VerifyPersistenceCalledOnce();
    }

    [Test]
    public async Task ImportTracksFromGpxFiles_AllFilesFail_DoesNotPersist()
    {
        var files = CreateFiles(
            "First.txt",
            "Second.txt");

        var result = await ImportAsync(files);

        result.ImportedFiles.Should().BeEmpty();
        result.FailedFiles.Should().HaveCount(2);

        VerifyPersistenceNeverCalled();
    }

    [Test]
    public async Task ImportTracksFromGpxFiles_ReportsCompletedProgress()
    {
        var progressValues = new List<decimal>();

        var result =
            await _sut.ImportTracksFromGpxFilesAsync(
                CreateFiles(GoodFilename),
                MaximumCombinedSize,
                progressValues.Add);

        result.ImportedFiles.Should().ContainSingle();

        progressValues.Should().NotBeEmpty();
        progressValues.Last().Should().Be(1M);

        progressValues.Should()
            .OnlyContain(value => value >= 0M && value <= 1M);
    }

    [Test]
    public async Task ImportTracksFromGpxFiles_PersistenceFailure_PropagatesException()
    {
        _mockTrackRepository
            .Setup(x => x.SaveImportedFilesAsync(
                It.IsAny<ImportFilesResult>(),
                It.IsAny<TrackImportProgress>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Database persistence failed"));

        Func<Task> action = () =>
            ImportAsync(CreateFiles(GoodFilename));

        await action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Database persistence failed");
    }

    [Test]
    public async Task ImportTracksFromGpxFiles_CancelledBeforeProcessing_ThrowsOperationCanceledException()
    {
        using var cancellationTokenSource =
            new CancellationTokenSource();

        cancellationTokenSource.Cancel();

        Func<Task> action = () =>
            _sut.ImportTracksFromGpxFilesAsync(
                CreateFiles(GoodFilename),
                MaximumCombinedSize,
                cancellationToken:
                    cancellationTokenSource.Token);

        await action.Should()
            .ThrowAsync<OperationCanceledException>();

        VerifyPersistenceNeverCalled();
    }

    [Test]
    public async Task ImportTracksFromGpxFiles_CancelledDuringProcessing_DoesNotPersist()
    {
        using var cancellationTokenSource =
            new CancellationTokenSource();

        _mockGpxService
            .Setup(x => x.LoadContentsOfGpxFileAsync(
                It.IsAny<IBrowserFile>(),
                It.IsAny<long>(),
                It.IsAny<CancellationToken>()))
            .Returns(
                (IBrowserFile file,
                 long maxAllowedSize,
                 CancellationToken cancellationToken) =>
                {
                    cancellationTokenSource.Cancel();

                    cancellationToken
                        .ThrowIfCancellationRequested();

                    return Task.FromResult(GetNewTrack());
                });

        Func<Task> action = () =>
            _sut.ImportTracksFromGpxFilesAsync(
                CreateFiles(GoodFilename),
                MaximumCombinedSize,
                cancellationToken:
                    cancellationTokenSource.Token);

        await action.Should()
            .ThrowAsync<OperationCanceledException>();

        VerifyPersistenceNeverCalled();
    }

    private Task<ImportFilesResult> ImportAsync(
        IReadOnlyList<IBrowserFile> files)
    {
        return _sut.ImportTracksFromGpxFilesAsync(
            files,
            MaximumCombinedSize);
    }

    private static IReadOnlyList<IBrowserFile> CreateFiles(
        params string[] filenames)
    {
        return filenames
            .Select(filename => CreateFile(filename))
            .ToList();
    }

    private static IBrowserFile CreateFile(
        string filename,
        long size = 1024)
    {
        var mock = new Mock<IBrowserFile>();

        mock.Setup(x => x.Name)
            .Returns(filename);

        mock.Setup(x => x.Size)
            .Returns(size);

        return mock.Object;
    }

    private void VerifyPersistenceCalledOnce()
    {
        _mockTrackRepository.Verify(
            x => x.SaveImportedFilesAsync(
                It.IsAny<ImportFilesResult>(),
                It.IsAny<TrackImportProgress>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private void VerifyPersistenceNeverCalled()
    {
        _mockTrackRepository.Verify(
            x => x.SaveImportedFilesAsync(
                It.IsAny<ImportFilesResult>(),
                It.IsAny<TrackImportProgress>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Track GetNewTrack()
    {
        return new Track
        {
            Name = "NewTrack",
            TrackSegments = new List<TrackSegment>
            {
                new()
                {
                    TrackPoints = new List<TrackPoint>
                    {
                        new()
                    }
                }
            }
        };
    }
}