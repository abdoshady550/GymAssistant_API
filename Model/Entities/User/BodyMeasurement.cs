using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Identity;
using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.User
{
    public sealed class BodyMeasurement : Entity
    {

        public string UserId { get; private set; }
        public decimal WeightKg { get; private set; }
        public decimal? BodyFatPercent { get; private set; }
        public decimal? MuscleMassKg { get; private set; }

        public Guid ClientProfileId { get; private set; }
        public ClientProfile User { get; private set; } = default!;

        private BodyMeasurement() { }

        private BodyMeasurement(Guid id, string userId, decimal weightKg, decimal? bodyFatPercent = null, decimal? muscleMassKg = null)
       : base(id)
        {
            UserId = userId;
            WeightKg = weightKg;
            BodyFatPercent = bodyFatPercent;
            MuscleMassKg = muscleMassKg;
            CreatedAtUtc = DateTimeOffset.UtcNow;

        }
        public static Result<BodyMeasurement> Create(Guid id, string userId,
                                                     decimal weightKg,
                                                     decimal? bodyFatPercent = null,
                                                     decimal? muscleMassKg = null)
        {
            if (userId == null)
            {
                return UserErrors.IdRequired;
            }
            if (weightKg < 20 || weightKg > 400)
            {
                return UserErrors.WeightKgInvalid;
            }
            return new BodyMeasurement(id, userId, weightKg, bodyFatPercent, muscleMassKg);

        }
        public Result<Updated> Update(decimal weightKg,
                                      decimal? bodyFatPercent = null,
                                      decimal? muscleMassKg = null)
        {
            if (weightKg < 20 || weightKg > 400)
            {
                return UserErrors.WeightKgInvalid;
            }
            if (bodyFatPercent < 0 || bodyFatPercent > 100)
            {
                return UserErrors.BodyFatPercentInvalid;
            }
            if (muscleMassKg < 10 || muscleMassKg > 200)
            {
                return UserErrors.MuscleMassKgInvalid;
            }
            WeightKg = weightKg;
            BodyFatPercent = bodyFatPercent;
            MuscleMassKg = muscleMassKg;


            return Result.Updated;

        }
    }
}
