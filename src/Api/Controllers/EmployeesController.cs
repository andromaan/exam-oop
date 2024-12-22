using Api.DTOs;
using Application.Abstraction.Interfaces.Queries;
using Application.Abstraction.Validatiors;
using Application.Abstraction.ViewModels;
using Application.Implementation.PayrollManager;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("employees")]
[ApiController]
public class EmployeesController(PayrollManager payrollManager, IEmployeeQueries employeeQueries) : ControllerBase
{
    [HttpPost("add")]
    public async Task<ActionResult<EmployeeDTO>> Add(
        [FromBody] EmployeeVM request)
    {
        var validator = new EmployeeValidator();
        var validation = await validator.ValidateAsync(request);
        
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors[0].ErrorMessage);
        }
        
        var respond = await payrollManager.AddEmployeeAsync(request);

        return Ok(EmployeeDTO.FromDomainModel(respond));
    }
    
    [HttpDelete("delete/{employeeId:guid}")]
    public async Task<ActionResult<EmployeeVM>> Delete(
        [FromRoute] Guid employeeId)
    {
        var respond = await payrollManager.DeleteEmployeeAsync(employeeId);

        return Ok(EmployeeDTO.FromDomainModel(respond));
    }
    
    [HttpGet("get-all")]
    public async Task<ActionResult<EmployeeDTO>> GetAll()
    {
        var employees = await employeeQueries.GetAll();

        return Ok(employees.Select(EmployeeDTO.FromDomainModel).ToList());
    }
}