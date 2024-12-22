namespace Application.Implementation;

public class PayrollManagerExceptions(string message, Exception? innerException = null)
    : Exception(message, innerException)
{
}

public class EmployeeNotFoundException(Guid employeeId) : PayrollManagerExceptions($"Employee under id: {employeeId} not found");

public class TransactionNotFoundException(Guid transactionId) : PayrollManagerExceptions($"Transaction under id: {transactionId} not found");