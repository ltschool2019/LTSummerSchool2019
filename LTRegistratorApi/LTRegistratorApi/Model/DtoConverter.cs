using System.Collections.Generic;

namespace LTRegistratorApi.Model
{
    public static class DtoConverter
    {
        // List<Project> to List<ProjectDto>
        public static List<ProjectDto> ToProjectDto(List<Project> projects)
        {
            var result = new List<ProjectDto>();
            foreach (var project in projects)
                result.Add(new ProjectDto { ProjectId = project.ProjectId, Name = project.Name });

            return result;
        }

        // List<Employee> to List<EmployeeDto>
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

        // List<Department> to List<DepartmentDto>
        public static List<DepartmentDto> ToDepartmentDto(List<Department> departments)
        {
            var result = new List<DepartmentDto>();
            foreach (var department in departments)
                result.Add(new DepartmentDto { DepartmentId = department.DepartmentId });

            return result;
        }
    }
}
