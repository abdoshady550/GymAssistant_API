using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.User
{
    public sealed class ClientProfile : Entity
    {
        private readonly List<BodyMeasurement> _measurements = new();
        private readonly List<WorkoutSession> _workouts = new();
        private readonly List<UserExercise> _customExercises = new();
        private readonly List<TrainerTrainee> _trainees = new();
        private readonly List<TrainerTrainee> _trainers = new();
        private readonly List<PersonalRecord> _personalRecords = new();

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; } = default!;

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string FullName => $"{FirstName} {LastName}";
        public Gender Gender { get; private set; }
        public DateTime? BirthDate { get; private set; }
        public int? HeightCm { get; private set; }
        public UserRole Role { get; private set; }

        public IReadOnlyCollection<BodyMeasurement> Measurements => _measurements.AsReadOnly();
        public IReadOnlyCollection<WorkoutSession> Workouts => _workouts.AsReadOnly();
        public IReadOnlyCollection<UserExercise> CustomExercises => _customExercises.AsReadOnly();
        public IReadOnlyCollection<PersonalRecord> PersonalRecords => _personalRecords.AsReadOnly();

        // Navigation for Trainer / Trainee relations
        public IReadOnlyCollection<TrainerTrainee> Trainees => _trainees.AsReadOnly();
        public IReadOnlyCollection<TrainerTrainee> Trainers => _trainers.AsReadOnly();

        private ClientProfile() { }

        private ClientProfile(Guid id, string firstName, string lastName, Gender gender, UserRole role)
            : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
            Role = role;
            CreatedAtUtc = DateTimeOffset.UtcNow;
        }
        public void AddPersonalRecord(PersonalRecord record) => _personalRecords.Add(record); // New

        // Helper method to get current weight
        public decimal? CurrentWeight => _measurements
            .OrderByDescending(m => m.CreatedAtUtc)
            .FirstOrDefault()?.WeightKg;
        public static Result<ClientProfile> CreateProfile(Guid id, string firstName, string lastName, Gender gender, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                return UserErrors.FirstNameRequired;
            }
            if (string.IsNullOrWhiteSpace(lastName))
            {
                return UserErrors.LastNameRequired;
            }
            if (gender != Gender.Female && gender != Gender.Male)
            {
                return UserErrors.GenderInvalid;
            }
            if (role != UserRole.User && role != UserRole.Trainer)
            {
                return UserErrors.RoleInvalid;
            }
            return new ClientProfile(id, firstName, lastName, gender, role);


        }
        public Result<Updated> UpdateProfile(string firstName, string lastName, DateTime? birthDate, int? heightCm)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                return UserErrors.FirstNameRequired;
            }
            if (string.IsNullOrWhiteSpace(lastName))
            {
                return UserErrors.LastNameRequired;
            }
            if (birthDate is null)
            {
                return UserErrors.BirthDayRequired;
            }
            if (birthDate >= DateTime.UtcNow || birthDate > DateTime.UtcNow.AddYears(-10))
            {
                return UserErrors.BirthDayRequired;
            }

            if (heightCm is < 100 or > 250)
            {
                return UserErrors.HeightInvalid;
            }
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
            HeightCm = heightCm;
            return Result.Updated;

        }

        public void AddMeasurement(BodyMeasurement measurement)
        {
            if (measurement is null) throw new ArgumentNullException(nameof(measurement));
            _measurements.Add(measurement);
        }

        public void AddWorkout(WorkoutSession workout)
        {
            if (workout is null) throw new ArgumentNullException(nameof(workout));
            _workouts.Add(workout);
        }

        public void AddCustomExercise(UserExercise exercise)
        {
            if (exercise is null) throw new ArgumentNullException(nameof(exercise));
            _customExercises.Add(exercise);
        }

    }

    public enum Gender
    {
        Male = 1,
        Female = 2
    }

    public enum UserRole
    {
        User = 1,
        Trainer = 2
    }

}
