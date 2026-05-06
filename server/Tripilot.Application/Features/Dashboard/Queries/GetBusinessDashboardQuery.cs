using MediatR;
using Tripilot.Application.DTOs.Dashboard;

namespace Tripilot.Application.Features.Dashboard.Queries;

public class GetBusinessDashboardQuery : IRequest<BusinessDashboardResponse>
{
    public Guid UserId { get; set; }
}

public class GetBusinessAnalyticsQuery : IRequest<BusinessAnalytics>
{
    public Guid UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
