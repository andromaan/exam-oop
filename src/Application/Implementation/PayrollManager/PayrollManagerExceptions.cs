namespace Application.Implementation.PayrollManager;

public class PayrollManagerExceptions(string message, Exception? innerException = null)
    : Exception(message, innerException)
{
}

public class EmployeeNotFoundException(Guid employeeId) : PayrollManagerExceptions($"Employee under id: {employeeId} not found");

public class TransactionNotFoundException(Guid transactionId) : PayrollManagerExceptions($"Transaction under id: {transactionId} not found");

public class DatePeriodInvalidException(DateTime startDate, DateTime endDate) : PayrollManagerExceptions($"Date period from: {startDate}, to: {endDate} is invalid");