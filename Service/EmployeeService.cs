using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service
{
    internal sealed class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public EmployeeService(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<EmployeeDto> employees, MetaData metaData)> GetEmployeesAsync(Guid companyId, EmployeeParameters empParams, bool trackChanges)
        {
            if(!empParams.ValidAgeRange)
                throw new MaxAgeRangeBadRequestException();

            await CheckIfCompanyExistsAsync(companyId, trackChanges);

            var employeesWithMetaData = await _repository.Employee.GetEmployeesAsync(companyId, empParams, trackChanges);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesWithMetaData);

            return (employees: employeesDto, metaData: employeesWithMetaData.MetaData);
            
        }

        public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await CheckIfCompanyExistsAsync(companyId, trackChanges);

            var employeeDb = await GetEmployeeForCompanyAndCheckIfItExistsAsync(companyId, id, trackChanges);

            var employee = _mapper.Map<EmployeeDto>(employeeDb);
            return employee;
        }

        public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId,
            EmployeeForCreationDto employeeForCreation, bool trackChanges)
        {
            await CheckIfCompanyExistsAsync(companyId, trackChanges);

            var employeeEntity = _mapper.Map<Employee>(employeeForCreation);

            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            await _repository.SaveAsync();

            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

            return employeeToReturn;
        }

        public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await CheckIfCompanyExistsAsync(companyId, trackChanges);

            var employeeDb = await GetEmployeeForCompanyAndCheckIfItExistsAsync(companyId, id, trackChanges);

            _repository.Employee.DeleteEmployee(employeeDb);
            await _repository.SaveAsync();
        }

        public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id,
            EmployeeForUpdateDto employeeForUpdate,
            bool compTrackChanges, bool empTrackChanges)
        {
            await CheckIfCompanyExistsAsync(companyId, compTrackChanges);

            var employeeDb = await GetEmployeeForCompanyAndCheckIfItExistsAsync(companyId, id, empTrackChanges);

            _mapper.Map(employeeForUpdate, employeeDb);
            await _repository.SaveAsync();
        }

        public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync
            (Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
        {
            await CheckIfCompanyExistsAsync(companyId, compTrackChanges);

            var employeDb = await GetEmployeeForCompanyAndCheckIfItExistsAsync(companyId, id, empTrackChanges);

            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeDb);

            return (employeeToPatch, employeDb);
        }

        public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
        {
            _mapper.Map(employeeToPatch, employeeEntity);
            await _repository.SaveAsync();
        }

        private async Task CheckIfCompanyExistsAsync(Guid companyId, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);
        }

        private async Task<Employee> GetEmployeeForCompanyAndCheckIfItExistsAsync(Guid companyId, Guid id, bool trackChanges)
        {
            var employeeDb = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
            if (employeeDb is null)
                throw new EmployeeNotFoundException(companyId);

            return employeeDb;
        }
    }
}
