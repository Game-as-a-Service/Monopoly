using Domain;

namespace Application.Common;

public interface IRepository
{
    public Monopoly FindGameById(string id);
    public string[] GetRooms();
    public bool IsExist(string id);
    public string Save(Monopoly game);
}