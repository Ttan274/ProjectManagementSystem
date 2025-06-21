using ProjectManagementSystem.Application.Abstractions.Repositories.Estimate;
using ProjectManagementSystem.Persistance.DbContext;

namespace ProjectManagementSystem.Persistance.Repositories.Estimate
{
    public class EstimateWriteRepository : WriteRepository<Domain.Entities.Estimate>, IEstimateWriteRepository
    {
        public EstimateWriteRepository(AppDbContext context) : base(context)
        {
                
        }
    }
}
