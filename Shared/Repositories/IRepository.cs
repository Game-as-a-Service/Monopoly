using Shared.Domain;

namespace Shared.Repositories;

public interface IRepository
{
    public Game FindGameById(string id);
    public void Save(Game game);
}

