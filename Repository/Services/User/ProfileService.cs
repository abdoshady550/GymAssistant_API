using GymAssistant_API.Data;
using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.User;
using GymAssistant_API.Req_Res.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;


namespace GymAssistant_API.Repository.Services.User
{
    public class ProfileService(ILogger<ProfileService> logger,
                                AppDbContext context,
                                UserManager<AppUser> userManager,
                                IWebHostEnvironment environment) : IProfile
    {
        private readonly ILogger<ProfileService> _logger = logger;
        private readonly AppDbContext _context = context;
        private readonly UserManager<AppUser> _user = userManager;
        private readonly IWebHostEnvironment _environment = environment;

        public async Task<Result<BodyMeasurement>> AddBodyMeasurementAsync(string userId,
                                                                           decimal? weightKg = default,
                                                                           decimal? weightGoal = default,
                                                                           decimal? bodyFatPercent = default,
                                                                           decimal? bodyFatGoal = default,
                                                                           decimal? muscleMassKg = default,
                                                                           decimal? muscleMassGoal = default,
                                                                           CancellationToken ct = default)
        {
            var user = await _context.ClientProfiles.FirstOrDefaultAsync(p => p.AppUserId == userId);
            if (user == null)
            {
                return Error.NotFound("User_NotFound", "User profile not found.");
            }
            var measurementResult = BodyMeasurement
                .Create(Guid.NewGuid(), userId, weightKg, weightGoal, bodyFatPercent, bodyFatGoal, muscleMassKg, muscleMassGoal);
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
                                                                      decimal? weightKg = default,
                                                                      decimal? weightGoal = default,
                                                                      decimal? bodyFatPercent = null,
                                                                      decimal? bodyFatGoal = default,
                                                                      decimal? muscleMassKg = null,
                                                                      decimal? muscleMassGoal = default
                                                                      , CancellationToken ct = default)
        {
            var measurement = await _context.BodyMeasurements.FirstOrDefaultAsync(p => p.Id == Id);

            if (measurement == null)
            {
                return Error.NotFound("Measurement_NotFound", "Body Measurement not found.");
            }

            measurement.Update(weightKg, weightGoal, bodyFatPercent, bodyFatGoal, muscleMassKg, muscleMassGoal);
            _context.BodyMeasurements.Update(measurement);
            await _context.SaveChangesAsync();

            return Result.Updated;
        }
        public async Task<Result<Deleted>> DeleteBodyMeasurementAsync(Guid Id,
                                                                     CancellationToken ct = default)
        {
            var measurement = await _context.BodyMeasurements.FirstOrDefaultAsync(p => p.Id == Id);

            if (measurement == null)
            {
                return Error.NotFound("Measurement_NotFound", "Body Measurement not found.");
            }
            _context.BodyMeasurements.Remove(measurement);
            await _context.SaveChangesAsync();

            return Result.Deleted;
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
        public async Task<Result<object>> GetMeasurementCardsAsync(string userId,
                                                                CancellationToken ct = default)
        {


            var measurements = await _context.BodyMeasurements
                .Where(m => m.UserId == userId)
                .OrderBy(m => m.CreatedAtUtc)
                .ToListAsync(ct);
            if (measurements == null || measurements.Count == 0)
            {
                return null;
            }
            var FirstWeight = measurements.FirstOrDefault(m => m.WeightKg.HasValue)?.WeightKg;
            var LastWeight = measurements.LastOrDefault(m => m.WeightKg.HasValue)?.WeightKg;
            var WeightGoal = measurements.LastOrDefault(m => m.WeightGoal.HasValue)?.WeightGoal;
            object weightCard;

            if (LastWeight > WeightGoal)
            {
                weightCard = new
                {
                    firstWeight = FirstWeight,
                    lastWeight = LastWeight,
                    weightGoal = WeightGoal,
                    weightLost = LastWeight - FirstWeight,
                };
            }
            else
            {
                weightCard = new
                {
                    firstWeight = FirstWeight,
                    lastWeight = LastWeight,
                    weightGoal = WeightGoal,
                    weightGained = FirstWeight - LastWeight
                };

            }

            var FirstBodyFat = measurements.FirstOrDefault(m => m.BodyFatPercent.HasValue)?.BodyFatPercent;
            var LastBodyFat = measurements.LastOrDefault(m => m.BodyFatPercent.HasValue)?.BodyFatPercent;
            var BodyFatGoal = measurements.LastOrDefault(m => m.BodyFatGoal.HasValue)?.BodyFatGoal;

            object bodyFatCard;

            if (LastWeight >= WeightGoal)
            {
                bodyFatCard = new
                {
                    firstBodyFat = FirstBodyFat,
                    lastBodyFat = LastBodyFat,
                    bodyFatGoal = BodyFatGoal,
                    bodyFatLost = LastBodyFat - FirstBodyFat
                };
            }
            else
            {
                bodyFatCard = new
                {
                    firstBodyFat = FirstBodyFat,
                    lastBodyFat = LastBodyFat,
                    bodyFatGoal = BodyFatGoal,
                    bodyFatGained = FirstBodyFat - LastBodyFat
                };
            }

            var FirstMuscleMass = measurements.FirstOrDefault(m => m.MuscleMassKg.HasValue)?.MuscleMassKg;
            var LastMuscleMass = measurements.LastOrDefault(m => m.MuscleMassKg.HasValue)?.MuscleMassKg;
            var MuscleMassGoal = measurements.LastOrDefault(m => m.MuscleMassGoal.HasValue)?.MuscleMassGoal;


            var muscleMassCard = new
            {
                firstMuscleMass = FirstMuscleMass,
                lastMuscleMass = LastMuscleMass,
                muscleMassGoal = MuscleMassGoal,
                muscleMassGained = LastMuscleMass - FirstMuscleMass
            };



            return new
            {
                WeightCard = weightCard,
                BodyFatCard = bodyFatCard,
                MuscleMassCard = muscleMassCard
            };

        }

        public async Task<Result<List<BodyMeasurement>>> GetMeasurementHistoryAsync(string userId,
                                                                                    int pageSize,
                                                                                    int pageNumber,
                                                                                    CancellationToken ct = default)
        {
            if (userId == null)
                return Error.Validation("UserId_Required", "UserId is required.");
            if (pageSize <= 0 || pageNumber <= 0)
                return Error.Validation("Pagination_Invalid", "PageSize and Page must be greater than zero.");

            return await _context.BodyMeasurements
         .Where(m => m.UserId == userId)
         .OrderByDescending(m => m.CreatedAtUtc)
         .Skip((pageNumber - 1) * pageSize)
         .Take(pageSize)
         .ToListAsync();
        }


        public async Task<Result<Updated>> UpdateProfileAsync(Guid Id,
                                                              string? firstName = default,
                                                              string? lastName = default,
                                                              IFormFile? imageFile = default,
                                                              Gender? gender = default,
                                                              string? phoneNumber = default,
                                                              DateTime? birthDate = default,
                                                              int? heightCm = default, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles.Include(P => P.Measurements)
                .FirstOrDefaultAsync(P => P.Id == Id, ct);
            if (profile == null)
            {
                _logger.LogError("User Profile not found for ProfileID {ProfileId}", Id);

                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }
            // 🖼️ حفظ الصورة في wwwroot (لو موجودة)
            string? imageUrl = profile.Image;
            if (imageFile != null && imageFile.Length > 0)
            {
                // 🗑️ امسح الصورة القديمة من wwwroot لو موجودة
                if (!string.IsNullOrEmpty(profile.Image))
                {
                    var oldImagePath = Path.Combine(_environment.WebRootPath, profile.Image.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "profile");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream, ct);
                }

                const string baseUrl = "https://gymassistantapi.runasp.net";
                imageUrl = $"{baseUrl}/images/profile/{uniqueFileName}";
            }
            profile.UpdateProfile(imageUrl, firstName, lastName, gender, phoneNumber, birthDate, heightCm);

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
                _logger.LogError("User Profile not found for ProfileID {ProfileId}", id);

                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }

            var profile = await _context.ClientProfiles
            .Include(p => p.AppUser)
            .Include(p => p.Measurements)
            .FirstOrDefaultAsync(p => p.Id == userProfile.Id);
            if (profile == null)
            {
                _logger.LogError("User Profile not found for ProfileID {ProfileId}", id);

                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }

            var lastMeasurement = profile.Measurements
              .OrderBy(m => m.CreatedAtUtc)
              .LastOrDefault();

            var response = new ProfileResponse
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Image = profile.Image,
                phoneNumber = profile.AppUser?.PhoneNumber,
                Gender = profile.Gender,
                Role = profile.Role,
                DateOfBirth = profile.BirthDate,
                HeightCm = profile.HeightCm,

                LastWeightKg = lastMeasurement?.WeightKg,
                LastWeightGoal = lastMeasurement?.WeightGoal,
                LastMuscleMassKgdecimal = lastMeasurement?.MuscleMassKg,
                LastMuscleMassGoal = lastMeasurement?.MuscleMassGoal,
                LastBodyFatPercent = lastMeasurement?.BodyFatPercent,
                LastBodyFatGoal = lastMeasurement?.BodyFatGoal
            };
            return response;
        }
    }
}
