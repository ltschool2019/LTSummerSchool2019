using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LTRegistratorApi.Model
{
    public static class DtoConverter
    {
        /// <summary>
        /// List<Employee> to List<EmployeeDto>
        /// </summary>
        /// <param name="employees"></param>
        /// <returns></returns>
        public static List<EmployeeDto> ToEmployeeDto(List<Employee> employees)
        {
            var result = new List<EmployeeDto>();
            foreach (var employee in employees)
                result.Add(new EmployeeDto
                {
                    EmployeeId = employee.EmployeeId,
                    FirstName = employee.FirstName,
                    SecondName = employee.SecondName,
                    Mail = employee.Mail,
                    MaxRole = employee.MaxRole
                });
            return result;
        }
        /// <summary>
        /// List<Project> to List<ProjectDto>
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        public static List<ProjectDto> ToProjectDto(List<Project> projects)
        {
            var result = new List<ProjectDto>();
            foreach (var project in projects)
                result.Add(new ProjectDto { ProjectId = project.ProjectId, Name = project.Name });
            return result;
        }
    }
}
