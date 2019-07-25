using System.Collections.Generic;
using System.Linq;

namespace LTRegistratorApi.Model
{
    /// <summary>
    /// Class with methods performing transformations with dto
    /// </summary>
    public static class DtoConverter
    {
        /// <summary>
        /// List<ProjectEmployee> to List<Project>
        /// </summary>
        /// <returns>List<Project>, which contains basic information about the List<ProjectEmployee></returns>
        public static List<Project> ToProject(List<ProjectEmployee> pEmployees)
        {
            var result = new List<Project>();
            foreach (var pe in pEmployees)
                result.Add(new Project { ProjectId = pe.ProjectId, Name = pe.Project.Name });

            return result;
        }

        /// <summary>
        /// List<Project> to List<ProjectDto>
        /// </summary>
        /// <returns>List<ProjectDto>, which contains basic information about the List<Project></returns>
        public static List<ProjectDto> ToProjectDto(List<Project> projects)
        {
            var result = new List<ProjectDto>();
            foreach (var project in projects)
                result.Add(new ProjectDto { ProjectId = project.ProjectId, Name = project.Name });

            return result;
        }

        /// <summary>
        /// List<Employee> to List<EmployeeDto>
        /// </summary>
        /// <returns>List<EmployeeDto>, which contains basic information about the List<Employee></returns>
        public static List<EmployeeDto> ToEmployeeDto(List<Employee> employees)
        {
            var result = new List<EmployeeDto>();
            foreach (var employee in employees)
            {
                result.Add(new EmployeeDto
                {
                    EmployeeId = employee.EmployeeId,
                    FirstName = employee.FirstName,
                    SecondName = employee.SecondName,
                    Mail = employee.Mail,
                    MaxRole = employee.MaxRole,
                    Projects = ToProjectDto(ToProject(employee.ProjectEmployee.ToList()))
                });
            }

            return result;
        }

        /// <summary>
        /// Employee to EmployeeDto
        /// </summary>
        /// <returns>EmployeeDto, which contains basic information about the Employee</returns>
        public static EmployeeDto ToEmployeeDto(Employee employee) 
            => ToEmployeeDto(new List<Employee> { employee })[0];
    }
}
