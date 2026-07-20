namespace ChroniclesAPI.Exceptions;

public class ConflictException(string message) : Exception(message);

public class NotFoundException(string message) : Exception(message);
