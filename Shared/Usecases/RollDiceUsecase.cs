using Shared.Repositories;

namespace Shared.Usecases;

public class RollDiceUsecase
{
    private readonly IRepository _repository;

    public RollDiceUsecase(IRepository repository)
    {
        _repository = repository;
    }

    public void Execute(Input input, Presenter presenter)
    {
        //查
        var game = _repository.FindGameById(input.GameId);
        //改
        game.PlayerRollDice(input.PlayerId);
        //存
        _repository.Save(game);
        //推
        presenter.CurrentDice = game.CurrentDice;
    }

    public class Presenter
    {
        public int[]? CurrentDice { get; set; }
    }

    public record Input(string GameId, string PlayerId);

}