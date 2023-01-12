using Shared.Repositories;

namespace Shared.Usecases;

public class MoveChessUsecase
{
    private readonly IRepository _repository;

    public MoveChessUsecase(IRepository jsonRepository)
    {
        _repository = jsonRepository;
    }

    public void Execute(Input input, Presenter presenter)
    {
        //查
        var game = _repository.FindGameById(input.GameId);
        //改
        game.PlayerMoveChess(input.PlayerId);
        //存
        _repository.Save(game);
        //推
        presenter.ChessPosition = game.GetPlayerPosition(input.PlayerId).Id;
    }

    public record Input(string GameId, string PlayerId);

    public class Presenter
    {
        public Presenter()
        {
            ChessPosition = string.Empty;
        }

        public string ChessPosition { get; set; }
    }
}