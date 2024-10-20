using Core.Consul;
using Core.Domain.Cache.Interfaces;
using Core.Tracing;
using Lesson.Application.Interfaces.Services;
using Lesson.Application.Queries.GetMyLessons.Responses;
using Lesson.Domain.Interfaces.Repositories;
using MediatR;
using Shared.Interfaces;

namespace Lesson.Application.Queries.GetMyLessons;

public class GetMyLessonsQueryHandler(
    ILessonRepository lessonRepository,
    IUserClaimsService contextService,
    IMapperService mapperService,
    ActivityTracing activityTracing,
    ConsulService consulService,
    IDistributedCache cache
) : IRequestHandler<GetMyLessonsQuery, GetMyLessonsQueryResponse>
{
    public async Task<GetMyLessonsQueryResponse> Handle(GetMyLessonsQuery request, CancellationToken cancellationToken)
    {

        var identityServiceUrl = await consulService.GetServiceUrl(Core.Domain.Consul.Services.IdentityService);
        
        return await activityTracing.ExecuteWithTracingAsync<GetMyLessonsQueryResponse>(
            nameof(GetMyLessonsQueryHandler),
            async () =>
            {
                GetMyLessonsQueryResponse response = new();

                var lessonsDbResult = await lessonRepository.GetMyLessons(userId: contextService.UserContext.UserId);

                var lessons =
                    mapperService.Map<List<Domain.Entities.Lesson>, List<Queries.GetMyLessons.Responses.Lesson>>(
                        lessonsDbResult.ToList());

                response.Lessons = lessons;

                return response;
            });
    }
}