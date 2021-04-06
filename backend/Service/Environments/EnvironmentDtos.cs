using System;
using Environment = Repository.Models.Environment;

namespace Service.Environments
{
    public record UpsertEnvironmentRequest(Guid? Id, string Name, string Description, string ScheduledStartup, int ScheduledUptime);

    public record GetEnvironmentRequest(Guid Id);

    public record GetEnvironmentResponse(EnvironmentDto Environment);

    public record ListEnvironmentsRequest;

    public record ListEnvironmentsResponse
    {
        public EnvironmentDto[] Environments { get; init; } = Array.Empty<EnvironmentDto>();
    }

    public record EnvironmentDto(Guid Id, string Name, string Description, string ScheduledStartup, int ScheduledUptime)
    {
        internal static EnvironmentDto FromEntity(Environment entity) => 
            entity == null ? null : 
            new EnvironmentDto(
                entity.Id, 
                entity.Name, 
                entity.Description, 
                entity.ScheduledStartup, 
                entity.ScheduledUptime
            );
    }
}
