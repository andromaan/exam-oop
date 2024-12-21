﻿using Api.DTOs;
using Application.Abstraction.Interfaces.Queries;
using Application.Abstraction.ViewModels;
using Application.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("employee")]
[ApiController]
public class EmployeeController(PayrollManager payrollManager, IEmployeeQueries employeeQueries) : ControllerBase
{
    [HttpPost("add")]
    public async Task<ActionResult<EmployeeVM>> Add(
        [FromBody] EmployeeVM request)
    {
        var respond = await payrollManager.AddEmployeeAsync(request);

        return Ok(respond);
    }
    
    [HttpDelete("delete/{employeeId:guid}")]
    public async Task<ActionResult<EmployeeVM>> Delete(
        [FromRoute] Guid employeeId)
    {
        var respond = await payrollManager.DeleteEmployeeAsync(employeeId);

        return Ok(respond);
    }
    
    [HttpGet("get-all")]
    public async Task<ActionResult<EmployeeDTO>> GetAll()
    {
        var employees = await employeeQueries.GetAll();

        return Ok(employees.Select(EmployeeDTO.FromDomainModel).ToList());
    }
}