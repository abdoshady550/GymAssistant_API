using GymAssistant_API.Data;
using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.User.Trainer;
using GymAssistant_API.Req_Res.Response.Trainer;
using Microsoft.EntityFrameworkCore;

namespace GymAssistant_API.Repository.Services.User.Trainer
{
    public class TrainerRequestService : ITrainerRequestService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TrainerRequestService> _logger;

        public TrainerRequestService(AppDbContext context, ILogger<TrainerRequestService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<TrainerRequestResponse>> SendRequestAsync(
            string trainerId,
            string traineeId,
            string? message = null,
            CancellationToken ct = default)
        {
            // Verify trainer profile exists and has trainer role
            var trainerProfile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == trainerId && p.Role == UserRole.Trainer, ct);

            if (trainerProfile == null)
            {
                return Error.Validation("Trainer_NotFound", "Trainer profile not found or user is not a trainer.");
            }

            // Verify trainee exists
            var traineeProfile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == traineeId && p.Role == UserRole.User, ct);

            if (traineeProfile == null)
            {
                return Error.NotFound("Trainee_NotFound", "Trainee not found.");
            }

            // Check if relationship already exists
            var existingRelation = await _context.TrainerTrainees
                .AnyAsync(tt => tt.TrainerId == trainerProfile.Id && tt.TraineeId == traineeProfile.Id, ct);

            if (existingRelation)
            {
                return TrainerRequestErrors.RelationshipAlreadyExists;
            }

            // Check if pending request already exists
            var existingUserRequest = await _context.UserRequests
                   .AnyAsync(tr => tr.TrainerId == trainerProfile.Id &&
                                  tr.TraineeId == traineeProfile.Id &&
                                  tr.Status == RequestStatus.Pending, ct);
            var existingTrainerRequest = await _context.TrainerRequests
                .AnyAsync(tr => tr.TrainerId == trainerProfile.Id &&
                               tr.TraineeId == traineeProfile.Id &&
                               tr.Status == RequestStatus.Pending, ct);

            if (existingUserRequest || existingTrainerRequest)
            {
                return TrainerRequestErrors.RequestAlreadyExists;
            }
            // Create the request
            var requestResult = TrainerRequest.Create(Guid.NewGuid(), trainerProfile.Id, traineeProfile.Id, message);

            if (requestResult.IsError)
            {
                return requestResult.Errors;
            }

            var request = requestResult.Value;
            _context.TrainerRequests.Add(request);
            await _context.SaveChangesAsync(ct);

            // Load navigation properties for response
            await _context.Entry(request)
                .Reference(r => r.Trainer)
                .LoadAsync(ct);
            await _context.Entry(request)
                .Reference(r => r.Trainee)
                .LoadAsync(ct);

            _logger.LogInformation("Trainer {TrainerId} sent request to trainee {TraineeId}",
                trainerProfile.Id, traineeProfile.Id);

            return TrainerRequestResponse.FromEntity(request);
        }

        public async Task<Result<TrainerRequestListResponse>> GetSentRequestsAsync(
            string trainerId,
            int pageSize,
            int pageNumber,
            CancellationToken ct = default)
        {
            var trainerProfile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == trainerId && p.Role == UserRole.Trainer, ct);

            if (trainerProfile == null)
            {
                return Error.Validation("Trainer_NotFound", "Trainer profile not found or user is not a trainer.");
            }

            var query = _context.TrainerRequests
                .Where(tr => tr.TrainerId == trainerProfile.Id)
                .Include(tr => tr.Trainer)
                .Include(tr => tr.Trainee);

            var totalCount = await query.CountAsync(ct);
            var pendingCount = await query.CountAsync(tr => tr.Status == RequestStatus.Pending, ct);

            var requests = await query
                .OrderByDescending(tr => tr.CreatedAtUtc)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new TrainerRequestListResponse
            {
                TotalCount = totalCount,
                PendingCount = pendingCount,
                Requests = requests.Select(TrainerRequestResponse.FromEntity).ToList()
            };
        }

        public async Task<Result<Deleted>> CancelRequestAsync(
            string trainerId,
            Guid requestId,
            CancellationToken ct = default)
        {
            var trainerProfile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == trainerId && p.Role == UserRole.Trainer, ct);

            if (trainerProfile == null)
            {
                return Error.Validation("Trainer_NotFound", "Trainer profile not found or user is not a trainer.");
            }

            var request = await _context.TrainerRequests
                .FirstOrDefaultAsync(tr => tr.Id == requestId && tr.TrainerId == trainerProfile.Id, ct);

            if (request == null)
            {
                return TrainerRequestErrors.RequestNotFound;
            }

            var cancelResult = request.Cancel();
            if (cancelResult.IsError)
            {
                return cancelResult.Errors;
            }

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Trainer {TrainerId} cancelled request {RequestId}",
                trainerProfile.Id, requestId);

            return Result.Deleted;
        }

        public async Task<Result<TrainerRequestListResponse>> GetReceivedRequestsAsync(string trainerId,
                                                                                       int pageSize,
                                                                                       int pageNumber,
                                                                                       CancellationToken ct = default)
        {
            var trainerProfile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == trainerId, ct);

            if (trainerProfile == null)
            {
                return Error.NotFound("Trainer_NotFound", "Trainer profile not found.");
            }

            var query = _context.UserRequests
                .Where(tr => tr.TrainerId == trainerProfile.Id)
                .Include(tr => tr.Trainer)
                .Include(tr => tr.Trainee);

            var totalCount = await query.CountAsync(ct);
            var pendingCount = await query.CountAsync(tr => tr.Status == RequestStatus.Pending, ct);

            var requests = await query
                .OrderByDescending(tr => tr.CreatedAtUtc)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new TrainerRequestListResponse
            {
                TotalCount = totalCount,
                PendingCount = pendingCount,
                Requests = requests.Select(TrainerRequestResponse.FromEntity).ToList()
            };
        }

        public async Task<Result<TrainerRequestResponse>> AcceptRequestAsync(
            string trainerId,
            Guid requestId,
            CancellationToken ct = default)
        {
            var trainerProfile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == trainerId, ct);

            if (trainerProfile == null)
            {
                return Error.NotFound("Trainer_NotFound", "Trainer profile not found.");
            }

            var request = await _context.UserRequests
                .Include(tr => tr.Trainer)
                .Include(tr => tr.Trainee)
                .FirstOrDefaultAsync(tr => tr.Id == requestId && tr.TrainerId == trainerProfile.Id, ct);

            if (request == null)
            {
                return TrainerRequestErrors.RequestNotFound;
            }

            var acceptResult = request.Accept();
            if (acceptResult.IsError)
            {
                return acceptResult.Errors;
            }

            // Create trainer-trainee relationship
            var relationResult = TrainerTrainee.Create(Guid.NewGuid(), request.TrainerId, request.TraineeId);
            if (relationResult.IsError)
            {
                return relationResult.Errors;
            }

            _context.TrainerTrainees.Add(relationResult.Value);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Trainer {TrainerId} accepted request {RequestId} from trainee {TraineeId}",
                trainerProfile.Id, requestId, request.TraineeId);

            return TrainerRequestResponse.FromEntity(request);
        }

        public async Task<Result<TrainerRequestResponse>> RejectRequestAsync(
            string trainerId,
            Guid requestId,
            CancellationToken ct = default)
        {
            var trainerProfile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == trainerId, ct);

            if (trainerProfile == null)
            {
                return Error.NotFound("Trainee_NotFound", "User profile not found.");
            }

            var request = await _context.UserRequests
                .Include(tr => tr.Trainer)
                .Include(tr => tr.Trainee)
                .FirstOrDefaultAsync(tr => tr.Id == requestId && tr.TrainerId == trainerProfile.Id, ct);

            if (request == null)
            {
                return TrainerRequestErrors.RequestNotFound;
            }

            var rejectResult = request.Reject();
            if (rejectResult.IsError)
            {
                return rejectResult.Errors;
            }

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Trainer {TrainerId} rejected request {RequestId} from trainer {TraineeId}",
                trainerProfile.Id, requestId, request.TraineeId);

            return TrainerRequestResponse.FromEntity(request);
        }

        public async Task<Result<TrainerRequestResponse>> GetRequestByIdAsync(
            string userId,
            Guid requestId,
            CancellationToken ct = default)
        {
            var userProfile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (userProfile == null)
            {
                return Error.NotFound("User_NotFound", "User profile not found.");
            }

            var request = await _context.TrainerRequests
                .Include(tr => tr.Trainer)
                .Include(tr => tr.Trainee)
                .FirstOrDefaultAsync(tr => tr.Id == requestId &&
                    (tr.TrainerId == userProfile.Id), ct);

            if (request == null)
            {
                return TrainerRequestErrors.RequestNotFound;
            }

            return TrainerRequestResponse.FromEntity(request);
        }
        public async Task<Result<List<UserDto>>> GetAllUserAsync(
          string? searchTerm,
          int pageSize,
          int pageNumber,
          CancellationToken ct = default)
        {
            if (pageSize <= 0 || pageNumber <= 0)
                return Error.Validation("Invalid_Pagination", "Invalid pagination parameters.");

            var query = _context.ClientProfiles
                .AsNoTracking()
                .Include(u => u.AppUser)
                .Where(u => u.Role == UserRole.User);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(u =>
                (u.FirstName.ToLower().Contains(search)) ||
                (u.LastName.ToLower().Contains(search)) ||
                (u.AppUser != null && u.AppUser.Email != null &&
                u.AppUser.Email.ToLower().Contains(search)) ||
                (u.AppUser != null && u.AppUser.PhoneNumber != null &&
                u.AppUser.PhoneNumber.ToLower().Contains(search)));
            }
            var users = await query
           .OrderBy(u => u.FirstName)
           .Skip((pageNumber - 1) * pageSize)
           .Take(pageSize)
           .Select(u => new UserDto(
               u.AppUserId,
               u.FullName,
               u.AppUser.Email,
               u.Gender,
               u.AppUser.PhoneNumber
           ))
           .ToListAsync(ct);
            return users;
        }
        public async Task<Result<List<UserDto>>> GetAllTrainerAsync(
         string? searchTerm,
         int pageSize,
         int pageNumber,
         CancellationToken ct = default)
        {
            if (pageSize <= 0 || pageNumber <= 0)
                return Error.Validation("Invalid_Pagination", "Invalid pagination parameters.");

            var query = _context.ClientProfiles
                .AsNoTracking()
                .Include(u => u.AppUser)
                .Where(u => u.Role == UserRole.Trainer);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(u =>
                (u.FirstName.ToLower().Contains(search)) ||
                (u.LastName.ToLower().Contains(search)) ||
                (u.AppUser != null && u.AppUser.Email != null &&
                u.AppUser.Email.ToLower().Contains(search)) ||
                (u.AppUser != null && u.AppUser.PhoneNumber != null &&
                u.AppUser.PhoneNumber.ToLower().Contains(search)));
            }
            var users = await query
             .OrderBy(u => u.FirstName)
             .Skip((pageNumber - 1) * pageSize)
             .Take(pageSize)
             .Select(u => new UserDto(
                 u.AppUserId,
                 u.FullName,
                 u.AppUser.Email,
                 u.Gender,
                 u.AppUser.PhoneNumber
             ))
             .ToListAsync(ct);
            return users;
        }
    }
}
