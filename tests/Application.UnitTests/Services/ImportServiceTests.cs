using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Heracles.Application.Interfaces;
using Heracles.Application.Resources;
using Heracles.Application.Services.Import;
using Heracles.Application.TrackAggregate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Heracles.Application.UnitTests.Services
{
    public class ImportServiceTests
    {
        private Mock<IFormFile> _mockFormFile;
        private Mock<IGpxService> _mockGpxService;
        private Mock<ILogger<ImportService>> _mockLogger;
        private Mock<ITrackRepository> _mockTrackRepository;
        private Mock<IExistingTracks> _mockExistingTracks;
        private ImportService _sut;
        private string _goodFilename = "Filename.gpx";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _mockLogger = new Mock<ILogger<ImportService>>();
            _mockFormFile = new Mock<IFormFile>();
            _mockGpxService = new Mock<IGpxService>();
            _mockTrackRepository = new Mock<ITrackRepository>();
        }

        [SetUp]
        public void BeforeEachTest()
        {
            _mockExistingTracks = new Mock<IExistingTracks>();
            _mockFormFile.Setup(x => x.Name).Returns(_goodFilename);
            _mockGpxService.Setup(x => x.LoadContentsOfGpxFile(It.IsAny<IFormFile>()))
                .Returns(GetNewTrack());
            _sut = new ImportService(_mockGpxService.Object, _mockTrackRepository.Object, _mockLogger.Object);
        }

        [Test]
        public void ImportTracksFromGpxFiles_NullFilesCollection_ThrowsNullArgumentException()
        {
            _sut.Invoking(x=>x.ImportTracksFromGpxFilesAsync(null)).Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task ImportTracksFromGpxFiles_EmptyFilesCollection_ReturnsEmptyImportedFilesCollection()
        {
            var result = await _sut.ImportTracksFromGpxFilesAsync(new FormFileCollection());

            result.ImportedFiles.Count.Should().Be(0);
            result.FailedFiles.Count.Should().Be(0);
        }

        [Test]
        public async Task ImportTracksFromGpxFiles_FileNotGpxExtension_ReturnsOneFailedImport()
        {
            const string badFilename = "Filename.xxx"; 
            _mockFormFile.Setup(x => x.Name).Returns(badFilename);

            var result = await _sut.ImportTracksFromGpxFilesAsync(new FormFileCollection { _mockFormFile.Object });

            result.ImportedFiles.Count.Should().Be(0);
            result.FailedFiles.Count.Should().Be(1);
            result.FailedFiles[0].Filename.Should().Be(badFilename);
            result.FailedFiles[0].Reason.Should().Be(ImportServiceStrings.IncorrectFileExtension);
        }

        [Test]
        public async Task ImportTracksFromGpxFiles_GetTrackThrowsException_ReturnsOneFailedImport()
        {
            const string exceptionMessage = "Something went wrong";
            _mockGpxService.Setup(x => x.LoadContentsOfGpxFile(It.IsAny<IFormFile>()))
                .Throws(new Exception(exceptionMessage));
            
            var result = await _sut.ImportTracksFromGpxFilesAsync(new FormFileCollection { _mockFormFile.Object });

            result.ImportedFiles.Count.Should().Be(0);
            result.FailedFiles.Count.Should().Be(1);
            result.FailedFiles[0].Filename.Should().Be(_goodFilename);
            result.FailedFiles[0].Reason.Should().Be(exceptionMessage);
        }

        [Test]
        public async Task ImportTracksFromGpxFiles_ExceptionCreatingTrack_ReturnsOneFailedImport()
        {
            var reasonMessage = ImportServiceStrings.FileCouldNotBeProcessed;
            _mockGpxService.Setup(x => x.LoadContentsOfGpxFile(It.IsAny<IFormFile>()))
                .Returns((Track)null);

            var result = await _sut.ImportTracksFromGpxFilesAsync(new FormFileCollection { _mockFormFile.Object });

            result.ImportedFiles.Count.Should().Be(0);
            result.FailedFiles.Count.Should().Be(1);
            result.FailedFiles[0].Filename.Should().Be(_goodFilename);
            result.FailedFiles[0].Reason.Should().Be(reasonMessage);
        }

        [Test]
        public async Task ImportTracksFromGpxFiles_ImportedTrackAlreadyExists_ReturnsOneFailedImport()
        {
            var reasonMessage = ImportServiceStrings.DuplicateTrackRecord;
            _mockExistingTracks.Setup(x => x.TrackExists(It.IsAny<string>())).Returns(true);

            var result = await _sut.ImportTracksFromGpxFilesAsync(new FormFileCollection { _mockFormFile.Object }, _mockExistingTracks.Object);

            result.ImportedFiles.Count.Should().Be(0);
            result.FailedFiles.Count.Should().Be(1);
            result.FailedFiles[0].Filename.Should().Be(_goodFilename);
            result.FailedFiles[0].Reason.Should().Be(reasonMessage);
        }

        [Test]
        public async Task ImportTracksFromGpxFiles_NoTrackSegments_FailedFileWithMessage()
        {
            var reasonMessage = ImportServiceStrings.NoTrackSegmentsFound;
            _mockGpxService.Setup(x => x.LoadContentsOfGpxFile(It.IsAny<IFormFile>()))
                .Returns(new Track { Name = "NewTrack" });
            _mockExistingTracks.Setup(x => x.TrackExists(It.IsAny<string>())).Returns(false);
            _mockExistingTracks.Setup(x => x.AddTrack(It.IsAny<string>()));

            var result = await _sut.ImportTracksFromGpxFilesAsync(new FormFileCollection { _mockFormFile.Object }, _mockExistingTracks.Object);

            _mockExistingTracks.Verify(x => x.AddTrack(It.IsAny<string>()), Times.Never);
            result.ImportedFiles.Count.Should().Be(0);
            result.FailedFiles.Count.Should().Be(1);
            result.FailedFiles[0].Filename.Should().Be(_goodFilename);
            result.FailedFiles[0].Reason.Should().Be(reasonMessage);
        }

        [Test]
        public async Task ImportTracksFromGpxFiles_NoTrackPoints_FailedFileWithMessage()
        {
            var newTrack = new Track
            {
                Name = "NewTrack",
                TrackSegments = new List<TrackSegment>
                {
                    new()
                }
            };

            var reasonMessage = ImportServiceStrings.NoTrackPointsFound;
            _mockGpxService.Setup(x => x.LoadContentsOfGpxFile(It.IsAny<IFormFile>()))
                .Returns(newTrack);
            _mockExistingTracks.Setup(x => x.TrackExists(It.IsAny<string>())).Returns(false);
            _mockExistingTracks.Setup(x => x.AddTrack(It.IsAny<string>()));

            var result = await _sut.ImportTracksFromGpxFilesAsync(new FormFileCollection { _mockFormFile.Object }, _mockExistingTracks.Object);

            _mockExistingTracks.Verify(x => x.AddTrack(It.IsAny<string>()), Times.Never);
            result.ImportedFiles.Count.Should().Be(0);
            result.FailedFiles.Count.Should().Be(1);
            result.FailedFiles[0].Filename.Should().Be(_goodFilename);
            result.FailedFiles[0].Reason.Should().Be(reasonMessage);
        }

        [Test]
        public async Task ImportTracksFromGpxFiles_DuplicateTrack_ReturnsFailedFileWithDuplicateMessage()
        {
            var newTrack = GetNewTrack();
            var existingTracks = await ExistingTracks.CreateAsync(_mockTrackRepository.Object);
            existingTracks.AddTrack(newTrack.Name);

            var result = await _sut.ImportTracksFromGpxFilesAsync(new FormFileCollection { _mockFormFile.Object }, existingTracks);

            result.ImportedFiles.Count.Should().Be(0);
            result.FailedFiles.Count.Should().Be(1);
            result.FailedFiles[0].Filename.Should().Be(_goodFilename);
            result.FailedFiles[0].Reason.Should().Be(ImportServiceStrings.DuplicateTrackRecord);
        }

        [Test]
        public async Task ImportTracksFromGpxFiles_TrackImported_TrackNameAddedToImportedTracksList()
        {
            _mockExistingTracks.Setup(x => x.TrackExists(It.IsAny<string>())).Returns(false);
            _mockExistingTracks.Setup(x => x.AddTrack(It.IsAny<string>()));

            var result = await _sut.ImportTracksFromGpxFilesAsync(new FormFileCollection { _mockFormFile.Object }, _mockExistingTracks.Object);

            _mockExistingTracks.Verify(x => x.AddTrack(It.IsAny<string>()), Times.Once);
            result.ImportedFiles.Count.Should().Be(1);
            result.FailedFiles.Count.Should().Be(0);
            result.ImportedFiles[0].Filename.Should().Be(_goodFilename);
            result.ImportedFiles[0].Reason.Should().Be("Success");
        }

        [Test]
        public async Task ImportTracksFromGpxFiles_FileGpxExtension_ReturnsOneSuccessImport()
        {
            //Happy path
            _mockExistingTracks.Setup(x => x.TrackExists(It.IsAny<string>())).Returns(false);

            var result = await _sut.ImportTracksFromGpxFilesAsync(new FormFileCollection { _mockFormFile.Object }, _mockExistingTracks.Object);

            result.ImportedFiles.Count.Should().Be(1);
            result.FailedFiles.Count.Should().Be(0);
            result.ImportedFiles[0].Filename.Should().Be(_goodFilename);
            result.ImportedFiles[0].Reason.Should().Be("Success");
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
}
