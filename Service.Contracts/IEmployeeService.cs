using Shared.DataTransferObjects;

namespace Service.Contracts
{
    public interface IEmployeeService
    {
        public IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges);
        EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges);
    }
}
