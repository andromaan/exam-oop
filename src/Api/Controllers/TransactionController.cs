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
        var transactions = await payrollManager.GetTransactionsByEmployeeAsync(employeeId);

        return Ok(transactions.Select(TransactionDTO.FromDomainModel).ToList());
    }
    
    [HttpGet("get-all")]
    public async Task<ActionResult<TransactionVM>> GetAll()
    {
        var transactions = await transactionQueries.GetAll();

        return Ok(transactions.Select(TransactionDTO.FromDomainModel).ToList());
    }
    
    [HttpGet("get-get-total-amount-by-period")]
    public async Task<ActionResult<decimal>> GetTotalAmountByPeriod(
        DateTime from, DateTime to)
    {
        var amount = await payrollManager.GetTotalPayoutsAsync(from, to);

        return Ok(amount);
    }
    
    [HttpDelete("delete/{transactionId:guid}")]
    public async Task<ActionResult<TransactionVM>> GetAll(
        [FromRoute] Guid transactionId)
    {
        var response = await payrollManager.DeleteTransactionAsync(transactionId);

        return Ok(response);
    }
}