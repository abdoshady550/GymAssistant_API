using GymAssistant_API.Data;
using Microsoft.Extensions.Localization;

namespace GymAssistant_API.Resources;

public interface ISeedDataLocalizer
{
    string SectionName(Guid id, string fallback);
    string? SectionDescription(Guid id, string? fallback);
    string ExerciseName(Guid id, string fallback);
    string? ExerciseDescription(Guid id, string? fallback);
    string? ExerciseInstructions(Guid id, string? fallback);
    string? ExerciseEquipment(Guid id, string? fallback);
}

public class SeedDataLocalizer(IStringLocalizer<SharedResources> localizer) : ISeedDataLocalizer
{
    private readonly IStringLocalizer<SharedResources> _localizer = localizer;

    public string SectionName(Guid id, string fallback)
    {
        string? key = id switch
        {
            var _ when id == SeedIds.ChestSectionId => LocalizationKeys.Data.Section.ChestName,
            var _ when id == SeedIds.BackSectionId => LocalizationKeys.Data.Section.BackName,
            var _ when id == SeedIds.LegsSectionId => LocalizationKeys.Data.Section.LegsName,
            var _ when id == SeedIds.ShouldersSectionId => LocalizationKeys.Data.Section.ShouldersName,
            var _ when id == SeedIds.ArmsSectionId => LocalizationKeys.Data.Section.ArmsName,
            _ => null
        };

        if (key == null) return fallback;
        var localized = _localizer[key];
        return localized.ResourceNotFound ? fallback : localized.Value;
    }

    public string? SectionDescription(Guid id, string? fallback)
    {
        string? key = id switch
        {
            var _ when id == SeedIds.ChestSectionId => LocalizationKeys.Data.Section.ChestDesc,
            var _ when id == SeedIds.BackSectionId => LocalizationKeys.Data.Section.BackDesc,
            var _ when id == SeedIds.LegsSectionId => LocalizationKeys.Data.Section.LegsDesc,
            var _ when id == SeedIds.ShouldersSectionId => LocalizationKeys.Data.Section.ShouldersDesc,
            var _ when id == SeedIds.ArmsSectionId => LocalizationKeys.Data.Section.ArmsDesc,
            _ => null
        };

        if (key == null) return fallback;
        var localized = _localizer[key];
        return localized.ResourceNotFound ? fallback : localized.Value;
    }

    public string ExerciseName(Guid id, string fallback)
    {
        string? key = id switch
        {
            var _ when id == SeedIds.BenchPressId => LocalizationKeys.Data.Exercise.BenchPressName,
            var _ when id == SeedIds.DeadliftId => LocalizationKeys.Data.Exercise.DeadliftName,
            var _ when id == SeedIds.SquatId => LocalizationKeys.Data.Exercise.SquatName,
            var _ when id == SeedIds.PullUpId => LocalizationKeys.Data.Exercise.PullUpName,
            var _ when id == SeedIds.ShoulderPressId => LocalizationKeys.Data.Exercise.ShoulderPressName,
            var _ when id == SeedIds.BicepCurlId => LocalizationKeys.Data.Exercise.BicepCurlName,
            _ => null
        };

        if (key == null) return fallback;
        var localized = _localizer[key];
        return localized.ResourceNotFound ? fallback : localized.Value;
    }

    public string? ExerciseDescription(Guid id, string? fallback)
    {
        string? key = id switch
        {
            var _ when id == SeedIds.BenchPressId => LocalizationKeys.Data.Exercise.BenchPressDesc,
            var _ when id == SeedIds.DeadliftId => LocalizationKeys.Data.Exercise.DeadliftDesc,
            var _ when id == SeedIds.SquatId => LocalizationKeys.Data.Exercise.SquatDesc,
            var _ when id == SeedIds.PullUpId => LocalizationKeys.Data.Exercise.PullUpDesc,
            var _ when id == SeedIds.ShoulderPressId => LocalizationKeys.Data.Exercise.ShoulderPressDesc,
            var _ when id == SeedIds.BicepCurlId => LocalizationKeys.Data.Exercise.BicepCurlDesc,
            _ => null
        };

        if (key == null) return fallback;
        var localized = _localizer[key];
        return localized.ResourceNotFound ? fallback : localized.Value;
    }

    public string? ExerciseInstructions(Guid id, string? fallback)
    {
        string? key = id switch
        {
            var _ when id == SeedIds.BenchPressId => LocalizationKeys.Data.Exercise.BenchPressInst,
            var _ when id == SeedIds.DeadliftId => LocalizationKeys.Data.Exercise.DeadliftInst,
            var _ when id == SeedIds.SquatId => LocalizationKeys.Data.Exercise.SquatInst,
            var _ when id == SeedIds.PullUpId => LocalizationKeys.Data.Exercise.PullUpInst,
            var _ when id == SeedIds.ShoulderPressId => LocalizationKeys.Data.Exercise.ShoulderPressInst,
            var _ when id == SeedIds.BicepCurlId => LocalizationKeys.Data.Exercise.BicepCurlInst,
            _ => null
        };

        if (key == null) return fallback;
        var localized = _localizer[key];
        return localized.ResourceNotFound ? fallback : localized.Value;
    }

    public string? ExerciseEquipment(Guid id, string? fallback)
    {
        string? key = id switch
        {
            var _ when id == SeedIds.BenchPressId => LocalizationKeys.Data.Exercise.BenchPressEquip,
            var _ when id == SeedIds.DeadliftId => LocalizationKeys.Data.Exercise.DeadliftEquip,
            var _ when id == SeedIds.SquatId => LocalizationKeys.Data.Exercise.SquatEquip,
            var _ when id == SeedIds.PullUpId => LocalizationKeys.Data.Exercise.PullUpEquip,
            var _ when id == SeedIds.ShoulderPressId => LocalizationKeys.Data.Exercise.ShoulderPressEquip,
            var _ when id == SeedIds.BicepCurlId => LocalizationKeys.Data.Exercise.BicepCurlEquip,
            _ => null
        };

        if (key == null) return fallback;
        var localized = _localizer[key];
        return localized.ResourceNotFound ? fallback : localized.Value;
    }
}
