using GymAssistant_API.Data;
using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.User;
using GymAssistant_API.Req_Res.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace GymAssistant_API.Repository.Services.User
{
    public class ProfileService(ILogger<ProfileService> logger,
                                AppDbContext context,
                                UserManager<AppUser> userManager) : IProfile
    {
        private readonly ILogger<ProfileService> _logger = logger;
        private readonly AppDbContext _context = context;
        private readonly UserManager<AppUser> _user = userManager;



        public async Task<Result<BodyMeasurement>> AddBodyMeasurementAsync(string userId,
                                                                           decimal weightKg,
                                                                           decimal? bodyFatPercent = null,
                                                                           decimal? muscleMassKg = null, CancellationToken ct = default)
        {
            var user = await _context.ClientProfiles.FirstOrDefaultAsync(p => p.AppUserId == userId);
            if (user == null)
            {
                return Error.NotFound("User_NotFound", "User profile not found.");
            }
            var measurementResult = BodyMeasurement
                .Create(Guid.NewGuid(), userId, weightKg, bodyFatPercent, muscleMassKg);
            if (measurementResult.IsError)
            {
                return measurementResult.Errors;
            }
            var measurement = measurementResult.Value;

            user.AddMeasurement(measurement);
            await _context.BodyMeasurements.AddAsync(measurement);
            await _context.SaveChangesAsync();
            return measurement;
        }

        public async Task<Result<Updated>> UpdateBodyMeasurementAsync(Guid Id,
                                                                      decimal weightKg,
                                                                      decimal? bodyFatPercent = null,
                                                                      decimal? muscleMassKg = null, CancellationToken ct = default)
        {
            var measurement = await _context.BodyMeasurements.FirstOrDefaultAsync(p => p.Id == Id);
            if (measurement == null)
            {
                return Error.NotFound("Measurement_NotFound", "Body Measurement not found.");
            }

            measurement.Update(weightKg, bodyFatPercent, muscleMassKg);
            _context.BodyMeasurements.Update(measurement);
            await _context.SaveChangesAsync();

            return Result.Updated;
        }

        public async Task<Result<ClientProfile>> CreateProfileAsync(string userId,
                                                                    string firstName,
                                                                    string lastName,
                                                                    Gender gender, CancellationToken ct = default)
        {
            var user = await _user.FindByIdAsync(userId);
            if (user == null)
            {
                return Error.NotFound("User_NotFound", "User not found.");
            }

            var userRole = await _user.GetRolesAsync(user);

            _logger.LogInformation("User roles for {UserId}: {Roles}", userId, string.Join(", ", userRole));

            if (userRole == null || userRole.Count == 0)
            {
                return Error.Conflict("User_HasNoRole", "User has no assigned role.");
            }

            // Assuming UserRole is an enum and role names match enum names
            if (!Enum.TryParse<UserRole>(userRole.First(), out var userRoleEnum))
            {
                return Error.Conflict("Invalid_Role", $"Role '{userRole.First()}' is not valid.");
            }


            var existingProfile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId);

            if (existingProfile != null)
            {
                return Error.Conflict("Profile_Exists", "User profile already exists.");
            }

            var profileResult = ClientProfile
                .CreateProfile(Guid.NewGuid(), userId, firstName, lastName, gender, userRoleEnum);

            if (profileResult.IsError)
            {
                return profileResult.Errors;
            }
            _context.ClientProfiles.Add(profileResult.Value);
            await _context.SaveChangesAsync();

            return profileResult;
        }

        public async Task<Result<object>> GetMeasurementChartsAsync(string userId,
                                                                    int days,
                                                                    CancellationToken ct = default)
        {
            var fromDate = DateTimeOffset.UtcNow.AddDays(-days);

            var measurements = await _context.BodyMeasurements
                .Where(m => m.UserId == userId && m.CreatedAtUtc >= fromDate)
                .OrderBy(m => m.CreatedAtUtc)
                .Select(m => new
                {
                    Date = m.CreatedAtUtc.Date,
                    Weight = m.WeightKg,
                    BodyFat = m.BodyFatPercent,
                    MuscleMass = m.MuscleMassKg
                })
                .ToListAsync();

            return new
            {
                WeightChart = measurements.Select(m => new { m.Date, Value = m.Weight }),
                BodyFatChart = measurements.Where(m => m.BodyFat.HasValue).Select(m => new { m.Date, Value = m.BodyFat }),
                MuscleMassChart = measurements.Where(m => m.MuscleMass.HasValue).Select(m => new { m.Date, Value = m.MuscleMass })
            };
        }

        public async Task<Result<List<BodyMeasurement>>> GetMeasurementHistoryAsync(string userId,
                                                                                    int pageSize,
                                                                                    int pageNumber,
                                                                                    CancellationToken ct = default)
        {
            return await _context.BodyMeasurements
         .Where(m => m.UserId == userId)
         .OrderByDescending(m => m.CreatedAtUtc)
         .Skip((pageNumber - 1) * pageSize)
         .Take(pageSize)
         .ToListAsync();
        }


        public async Task<Result<Updated>> UpdateProfileAsync(Guid Id,
                                                              string firstName,
                                                              string lastName,
                                                              Gender gender,
                                                              DateTime? birthDate,
                                                              int? heightCm, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles.FindAsync(Id);
            if (profile == null)
            {
                _logger.LogError("User Profile not found for ProfileID {ProfileId}", Id);

                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }
            profile.UpdateProfile(firstName, lastName, gender, birthDate, heightCm);

            _context.ClientProfiles.Update(profile);
            await _context.SaveChangesAsync();

            return Result.Updated;

        }
        public async Task<Result<ProfileResponse>> GetProfileAsync(string id, CancellationToken ct = default)
        {
            var userProfile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == id);
            if (userProfile == null)
            {
                _logger.LogError("User Profile not found for UserID {UserId}", id);
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }
            var profile = await _context.ClientProfiles
            .Include(p => p.Measurements)
            .FirstOrDefaultAsync(p => p.Id == userProfile.Id);

            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }

            var latestMeasurement = profile.Measurements
                .OrderByDescending(m => m.CreatedAtUtc)
                .FirstOrDefault();
            var response = new ProfileResponse
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Gender = profile.Gender,
                Role = profile.Role,
                DateOfBirth = profile.BirthDate,
                HeightCm = profile.HeightCm,
                WeightKg = latestMeasurement.WeightKg,
                MuscleMassKgdecimal = latestMeasurement.MuscleMassKg,
                BodyFatPercent = latestMeasurement.BodyFatPercent
            };
            return response;
        }
    }
}
