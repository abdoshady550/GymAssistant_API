using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Identity;
using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.User
{
    public sealed class BodyMeasurement : Entity
    {

        public string UserId { get; private set; }

        public decimal? WeightKg { get; private set; }
        public decimal? WeightGoal { get; private set; }

        public decimal? BodyFatPercent { get; private set; }
        public decimal? BodyFatGoal { get; private set; }

        public decimal? MuscleMassKg { get; private set; }
        public decimal? MuscleMassGoal { get; private set; }



        public Guid ClientProfileId { get; private set; }
        public ClientProfile User { get; private set; } = default!;

        private BodyMeasurement() { }

        private BodyMeasurement(Guid id,
                                string userId,
                                decimal? weightKg, decimal? weightGoal,
                                decimal? bodyFatPercent = null, decimal? bodyFatGoal = default,
                                decimal? muscleMassKg = null, decimal? muscleMassGoal = default)
       : base(id)
        {
            UserId = userId;
            WeightKg = weightKg;
            WeightGoal = weightGoal;
            BodyFatPercent = bodyFatPercent;
            BodyFatGoal = bodyFatGoal;
            MuscleMassKg = muscleMassKg;
            MuscleMassGoal = muscleMassGoal;
            CreatedAtUtc = DateTimeOffset.UtcNow;

        }
        public static Result<BodyMeasurement> Create(Guid id, string userId,
                                                     decimal? weightKg,
                                                     decimal? weightGoal,
                                                     decimal? bodyFatPercent = null,
                                                     decimal? bodyFatGoal = default,
                                                     decimal? muscleMassKg = null,
                                                     decimal? muscleMassGoal = default
                                                     )
        {

            if (userId == null)
            {
                return UserErrors.IdRequired;
            }
            if (weightKg.HasValue && weightKg < 20 || weightKg > 400)
            {
                return UserErrors.WeightKgInvalid;
            }
            if (weightGoal.HasValue && weightGoal < 20 || weightGoal > 400)
            {
                return UserErrors.WeightKgInvalid;
            }
            return new BodyMeasurement(id, userId, weightKg, weightGoal, bodyFatPercent, bodyFatGoal, muscleMassKg, muscleMassGoal);

        }
        public Result<Updated> Update(decimal? weightKg,
                                      decimal? weightGoal,
                                      decimal? bodyFatPercent = default,
                                      decimal? bodyFatGoal = default,
                                      decimal? muscleMassKg = default,
                                      decimal? muscleMassGoal = default)
        {
            if (weightKg < 20 || weightKg > 400)
            {
                return UserErrors.WeightKgInvalid;
            }
            if (weightGoal < 20 || weightGoal > 400)
            {
                return UserErrors.WeightKgInvalid;
            }
            if (bodyFatPercent < 0 || bodyFatPercent > 100)
            {
                return UserErrors.BodyFatPercentInvalid;
            }
            if (bodyFatGoal < 0 || bodyFatGoal > 100)
            {
                return UserErrors.BodyFatPercentInvalid;
            }
            if (muscleMassKg < 10 || muscleMassKg > 200)
            {
                return UserErrors.MuscleMassKgInvalid;
            }
            if (muscleMassGoal < 10 || muscleMassGoal > 200)
            {
                return UserErrors.MuscleMassKgInvalid;
            }
            if (weightKg.HasValue)
            {
                WeightKg = weightKg.Value;
            }
            if (weightGoal.HasValue)
            {
                WeightGoal = weightGoal.Value;
            }
            if (bodyFatPercent.HasValue)
            {
                BodyFatPercent = bodyFatPercent;
            }
            if (bodyFatGoal.HasValue)
            {
                BodyFatGoal = bodyFatGoal;
            }
            if (muscleMassKg.HasValue)
            {
                MuscleMassKg = muscleMassKg;

            }
            if (muscleMassGoal.HasValue)
            {
                MuscleMassGoal = muscleMassGoal;
            }


            return Result.Updated;

        }
    }
}
