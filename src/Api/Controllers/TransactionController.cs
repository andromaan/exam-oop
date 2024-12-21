using Api.DTOs;
using Application.Abstraction.Interfaces.Queries;
using Application.Abstraction.ViewModels;
using Application.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("transaction")]
[ApiController]
public class TransactionController(PayrollManager payrollManager, ITransactionQueries transactionQueries)
    : ControllerBase
{
    [HttpPost("add")]
    public async Task<ActionResult<TransactionVM>> Add(
        [FromBody] TransactionVM request)
    {
        var response = await payrollManager.CreateTransactionAsync(request);

        return Ok(response);
    }

    [HttpGet("get-all-by-employee/{employeeId:guid}")]
    public async Task<ActionResult<TransactionVM>> GetAllByEmployee(
        [FromRoute] Guid employeeId)
    {
        var transactions = await transactionQueries.GetAllForEmployee(employeeId);

        return Ok(transactions.Select(TransactionDTO.FromDomainModel).ToList());
    }
}