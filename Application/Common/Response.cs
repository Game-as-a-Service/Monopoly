using Domain.Common;

namespace Application.Common;

public abstract record Response();
public abstract record CommandResponse(IReadOnlyList<DomainEvent> Events) : Response;