using FluentAssertions;
using GymAssistant.TestCommon.Fixtures;
using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Repository.Services.Exercise;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;

namespace GymAssistant.UnitTests.Mapping;

public class PredefinedMappingTests
{
    private readonly Mock<IWebHostEnvironment> _envMock = new();

    [Fact]
    public async Task GetClientSectionsAsync_MapsEntitiesToDtosAccurately()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var sectionResult = Section.Create(Guid.NewGuid(), "Chest", "الصدر", "Chest exercises", "تمارين الصدر", "/images/chest.png");
        context.Sections.Add(sectionResult.Value);
        await context.SaveChangesAsync();

        var service = new PredefinedDataService(context, _envMock.Object);

        // Act
        var result = await service.GetClientSectionsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        var mappedDto = result.Value.First(s => s.Id == sectionResult.Value.Id);
        mappedDto.Name.Should().Be("Chest");
        mappedDto.Description.Should().Be("Chest exercises");
        mappedDto.ImageUrl.Should().Be("/images/chest.png");
    }

    [Fact]
    public async Task GetClientExercisesAsync_MapsAndFiltersCorrectly()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var sectionId = Guid.NewGuid();
        var sectionResult = Section.Create(sectionId, "Chest", "الصدر");
        context.Sections.Add(sectionResult.Value);

        var exResult = Exercise.Create(
            id: Guid.NewGuid(),
            sectionId: sectionId,
            nameEn: "Bench Press",
            difficultyLevel: DifficultyLevel.Intermediate,
            defaultSets: 3,
            defaultReps: 10,
            imageUrl: "/images/bench.png"
        );
        context.Exercises.Add(exResult.Value);
        await context.SaveChangesAsync();

        var service = new PredefinedDataService(context, _envMock.Object);

        // Act
        var result = await service.GetClientExercisesAsync(sectionId: sectionId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        var mapped = result.Value[0];
        mapped.Name.Should().Be("Bench Press");
        mapped.DifficultyLevel.Should().Be(DifficultyLevel.Intermediate);
        mapped.DefaultSets.Should().Be(3);
        mapped.DefaultReps.Should().Be(10);
    }
}
