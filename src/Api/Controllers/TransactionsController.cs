using Api.DTOs;
using Application.Abstraction.Interfaces.Queries;
using Application.Abstraction.Validatiors;
using Application.Abstraction.ViewModels;
using Application.Implementation;
using Application.Implementation.PayrollManager;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("transactions")]
[ApiController]
public class TransactionsController(PayrollManager payrollManager, ITransactionQueries transactionQueries)
    : ControllerBase
{
    [HttpPost("add")]
    public async Task<ActionResult<TransactionDTO>> Add(
        [FromBody] TransactionVM request)
    {
        var validator = new TransactionValidator();
        var validation = await validator.ValidateAsync(request);
        
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors[0].ErrorMessage);
        }
        
        var response = await payrollManager.CreateTransactionAsync(request);

        return Ok(TransactionDTO.FromDomainModel(response));
    }

    [HttpGet("get-all-by-employee/{employeeId:guid}")]
    public async Task<ActionResult<TransactionDTO>> GetAllByEmployee(
        [FromRoute] Guid employeeId)
    {
        var transactions = await payrollManager.GetTransactionsByEmployeeAsync(employeeId);

        return Ok(transactions.Select(TransactionDTO.FromDomainModel).ToList());
    }
    
    [HttpGet("get-all")]
    public async Task<ActionResult<TransactionDTO>> GetAll()
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
    public async Task<ActionResult<TransactionDTO>> GetAll(
        [FromRoute] Guid transactionId)
    {
        var response = await payrollManager.DeleteTransactionAsync(transactionId);

        return Ok(TransactionDTO.FromDomainModel(response));
    }
}