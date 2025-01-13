namespace CommerceMicro.Modules.Contracts;

public record ProductCreatedEvent(int Id, string Name, string Description, decimal Price, int StockQuantity);
public record ProductUpdatedEvent(int Id, string Name, string Description, decimal Price, int StockQuantity);
public record ProductDeletedEvent(int Id);
public record ChangeProductQuantityEvent(int Id, int QuantityChanged);
public record MarkProductOfStockEvent(int Id, bool IsOutOfStock);