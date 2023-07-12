using Domain.DTO.Loan;
using Domain.Entities;
using Mapster;

namespace Infrastructure.Maps
{
    public class LoanMapper
    {
        public static void ConfigureMappings()
        {
            TypeAdapterConfig<LoanRequestDTO, Loan>
                .NewConfig();

            TypeAdapterConfig<Loan, LoanDTO>
                .NewConfig();

            TypeAdapterConfig<LoanAccountantDTO, Loan>
                .NewConfig();
        }
    }
}
