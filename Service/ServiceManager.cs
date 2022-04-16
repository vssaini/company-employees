using AutoMapper;
using Contracts;
using Service.Contracts;

namespace Service
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<ICompanyService> _companyService;
        private readonly Lazy<IEmployeeService> _employeeService;

        public ServiceManager(IRepositoryManager repository, IMapper mapper, IEmployeeLinks employeeLinks)
        {
            _companyService = new Lazy<ICompanyService>(() => new CompanyService(repository, mapper));
            _employeeService = new Lazy<IEmployeeService>(() => new EmployeeService(repository, mapper, employeeLinks));
        }

        public ICompanyService CompanyService => _companyService.Value;

        public IEmployeeService EmployeeService => _employeeService.Value;
    }
}
