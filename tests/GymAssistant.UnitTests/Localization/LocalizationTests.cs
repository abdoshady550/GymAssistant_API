using System.Collections;
using System.Globalization;
using System.Resources;
using FluentAssertions;
using GymAssistant.TestCommon.Security;
using GymAssistant_API.Extensions;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Resources;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;

namespace GymAssistant.UnitTests.Localization;

public class LocalizationTests
{
    [Fact]
    public void ResxFiles_ShouldContainMatchingKeysBetweenEnglishAndArabic()
    {
        var enManager = new ResourceManager(typeof(SharedResources));
        var enResourceSet = enManager.GetResourceSet(CultureInfo.InvariantCulture, true, true);
        var arResourceSet = enManager.GetResourceSet(new CultureInfo("ar"), true, true);

        enResourceSet.Should().NotBeNull();
        arResourceSet.Should().NotBeNull();

        var enKeys = enResourceSet!.Cast<DictionaryEntry>().Select(e => e.Key.ToString()!).ToHashSet();
        var arKeys = arResourceSet!.Cast<DictionaryEntry>().Select(e => e.Key.ToString()!).ToHashSet();

        enKeys.Should().NotBeEmpty();
        arKeys.Should().NotBeEmpty();

        var missingInArabic = enKeys.Except(arKeys).ToList();
        missingInArabic.Should().BeEmpty("all English resource keys must be present in Arabic resources");
    }

    [Fact]
    public void LocalizeExtension_WhenErrorHasArgs_FormatsCorrectly()
    {
        var localizerMock = new Mock<IStringLocalizer>();
        var error = Error.Validation("TEST_CODE", "Hello {0}");
        error = Error.Validation("TEST_CODE", "Default desc", "World");

        localizerMock
            .Setup(l => l["TEST_CODE", It.IsAny<object[]>()])
            .Returns(new LocalizedString("TEST_CODE", "Hello World"));

        var result = localizerMock.Object.Localize(error);

        result.Should().Be("Hello World");
    }

    [Fact]
    public void LocalizeExtension_WhenKeyNotFound_ReturnsFallbackDescription()
    {
        var localizerMock = new Mock<IStringLocalizer>();
        var error = Error.NotFound("UNKNOWN_CODE", "Fallback Description");

        localizerMock
            .Setup(l => l["UNKNOWN_CODE"])
            .Returns(new LocalizedString("UNKNOWN_CODE", "UNKNOWN_CODE", resourceNotFound: true));

        var result = localizerMock.Object.Localize(error);

        result.Should().Be("Fallback Description");
    }

    [Fact]
    public void ProductionSafetyGuard_ThrowsWhenGivenDangerousConnectionString()
    {
        var dangerousConn = "Server=db27400.public.databaseasp.net; Database=db27400;";
        var act = () => ProductionProtectionGuard.AssertSafeConnectionString(dangerousConn);

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*CRITICAL SAFETY GUARD VIOLATION*");
    }

    [Fact]
    public void ProductionSafetyGuard_SucceedsForSafeTestConnectionString()
    {
        var safeConn = "Server=(localdb)\\mssqllocaldb;Database=GymAssistant_Test_123;Trusted_Connection=True;";
        var act = () => ProductionProtectionGuard.AssertSafeConnectionString(safeConn);

        act.Should().NotThrow();
    }
}
