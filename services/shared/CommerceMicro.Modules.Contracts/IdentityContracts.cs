namespace CommerceMicro.Modules.Contracts;

public record UserCreatedEvent(long Id, string UserName, string FirstName, string LastName);
public record UserUpdatedEvent(long Id, string UserName, string FirstName, string LastName);
public record UserDeletedEvent(long Id, string UserName);
