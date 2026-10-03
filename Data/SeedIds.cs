namespace GymAssistant_API.Data;

public static class SeedIds
{
    // Fixed GUIDs to ensure consistency in relationships
    public static readonly Guid AdminUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid TrainerUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid Trainer2UserId = Guid.Parse("22222223-2223-2223-2222-222222222223");
    public static readonly Guid ClientUserId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid Client2UserId = Guid.Parse("44444444-4444-4444-4444-444444444444");

    public static readonly Guid AdminProfileId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid TrainerProfileId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public static readonly Guid Trainer2ProfileId = Guid.Parse("bbbbbbbc-bbbc-bbbc-bbbc-bbbbbbbbbbbc");
    public static readonly Guid ClientProfileId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    public static readonly Guid Client2ProfileId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

    public static readonly Guid ChestSectionId = Guid.Parse("10101010-1010-1010-1010-101010101010");
    public static readonly Guid BackSectionId = Guid.Parse("20202020-2020-2020-2020-202020202020");
    public static readonly Guid LegsSectionId = Guid.Parse("30303030-3030-3030-3030-303030303030");
    public static readonly Guid ShouldersSectionId = Guid.Parse("40404040-4040-4040-4040-404040404040");
    public static readonly Guid ArmsSectionId = Guid.Parse("50505050-5050-5050-5050-505050505050");

    public static readonly Guid BenchPressId = Guid.Parse("11111111-2222-3333-4444-555555555555");
    public static readonly Guid DeadliftId = Guid.Parse("22222222-3333-4444-5555-666666666666");
    public static readonly Guid SquatId = Guid.Parse("33333333-4444-5555-6666-777777777777");
    public static readonly Guid PullUpId = Guid.Parse("44444444-5555-6666-7777-888888888888");
    public static readonly Guid ShoulderPressId = Guid.Parse("55555555-6666-7777-8888-999999999999");
    public static readonly Guid BicepCurlId = Guid.Parse("66666666-7777-8888-9999-aaaaaaaaaaaa");

    public static readonly Guid WorkoutSession1Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
    public static readonly Guid WorkoutSession2Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
    public static readonly Guid WorkoutSession3Id = Guid.Parse("12345678-1234-1234-1234-123456789012");

    public static readonly Guid WorkoutExercise1Id = Guid.Parse("12121212-1212-1212-1212-121212121212");
    public static readonly Guid WorkoutExercise2Id = Guid.Parse("13131313-1313-1313-1313-131313131313");
    public static readonly Guid WorkoutExercise3Id = Guid.Parse("14141414-1414-1414-1414-141414141414");
    public static readonly Guid WorkoutExercise4Id = Guid.Parse("15151515-1515-1515-1515-151515151515");
    public static readonly Guid WorkoutExercise5Id = Guid.Parse("16161616-1616-1616-1616-161616161616");
}
