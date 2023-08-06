using Domain;

namespace Application.Common;

public interface IRepository
{
    public Monopoly FindGameById(string id);
    public bool IsExist(string id);
    public string Save(Monopoly game);
}