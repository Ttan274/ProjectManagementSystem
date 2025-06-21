using ProjectManagementSystem.Application.Abstractions.Repositories.Estimate;
using ProjectManagementSystem.Persistance.DbContext;

namespace ProjectManagementSystem.Persistance.Repositories.Estimate
{
    public class EstimateReadRepository : ReadRepository<Domain.Entities.Estimate>, IEstimateReadRepository
    {
        public EstimateReadRepository(AppDbContext context) : base(context)
        {
            
        }
    }
}
