using Shared.Domain;

namespace SharedTests.Domain;

[TestClass]
public class PayTollTest
{
    [TestMethod]
    [Description(
        """
        Given:  玩家A, B
                玩家A持有的金額 1000, 房地產 A4, 地價 1000, 沒有房子
                玩家B持有的金額 1000
                B的回合移動到A4
        When:   B扣除過路費1000 * 5% = 50給A
        Then:   玩家A持有金額為1000+50 = 1050
                玩家A持有金額為1000-50 = 950
        """)]
    public void 玩家付過路費_無房_無同地段()
    {
        // Arrange
        var map = new Map(Utils.SevenXSevenMap());
        var game = new Game("Test", map, Utils.MockDice(2));

        // 玩家AB
        // A餘額1000, B餘額1000
        var player_a = new Player("A", 1000);
        var player_b = new Player("B", 1000);
        
        game.AddPlayer(player_b);
        game.AddPlayer(player_a);

        game.Initial();

        // A擁有A4
        Land A4 = (Land)map.FindBlockById("A4");
        game.SetPlayerToBlock(player_a, "A4", Map.Direction.Down);
        player_a.AddLandContract(new(player_a, A4));

        // B的回合移動到A4
        game.SetPlayerToBlock(player_b, "A2", Map.Direction.Right);
        game.PlayerRollDice(player_b.Id); // point:2

        Land location = (Land)game.GetPlayerPosition(player_b.Id);
        
        decimal amount = 0;
        Player? payee = game.GetOwner(location);
 
        bool enoughMoney = game.CalculateToll(location, player_b, payee!,out amount);
        Assert.AreEqual(true, enoughMoney);

        if(enoughMoney)
        {
            //Act
            game.PayToll(player_b, player_a, amount);

            // Assert
            // 1000 * 0.05 = 50
            Assert.AreEqual(1050, player_a.Money);
            Assert.AreEqual(950, player_b.Money);
        }
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A, B
                玩家A持有的金額 1000, 房地產 A1 A4, A4地價 1000, A4有2棟房子
                玩家B持有的金額 2000
                B的回合移動到A4
        When:   B扣除過路費1000 * 100% * 130% = 1300給A
        Then:   玩家A持有金額為1000+1300 = 2300
                玩家A持有金額為2000-1300 = 700
        """)]
    public void 玩家付過路費_有2房_有1同地段()
    {
        var map = new Map(Utils.SevenXSevenMap());
        var game = new Game("Test", map, Utils.MockDice(2));

        // 玩家AB
        // A餘額1000, B餘額1000
        var player_a = new Player("A", 1000);
        var player_b = new Player("B", 2000);
        
        game.AddPlayer(player_b);
        game.AddPlayer(player_a);

        game.Initial();

        // A擁有A1, A4, A4有2房子
        game.SetPlayerToBlock(player_a, "A1", Map.Direction.Right);
        Land A1 = (Land)map.FindBlockById("A1");
        player_a.AddLandContract(new(player_a, A1));

        game.SetPlayerToBlock(player_a, "A4", Map.Direction.Down);
        Land A4 = (Land)map.FindBlockById("A4");
        player_a.AddLandContract(new(player_a, A4));
        A4.Upgrade();
        A4.Upgrade();

        // B的回合移動到A4
        game.SetPlayerToBlock(player_b, "A2", Map.Direction.Right);
        game.PlayerRollDice(player_b.Id); // point:2

        Land location = (Land)game.GetPlayerPosition(player_b.Id);
        
        decimal amount = 0;
        Player? payee = game.GetOwner(location);
 
        bool enoughMoney = game.CalculateToll(location, player_b, payee!,out amount);
        Assert.AreEqual(true, enoughMoney);

        if(enoughMoney)
        {
            game.PayToll(player_b, player_a, amount);

            // 1000 * 100% * 130% = 1300
            Assert.AreEqual(2300, player_a.Money);
            Assert.AreEqual(700, player_b.Money);
        }
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A, B
                目前輪到A
                A在A1上，持有1000元
                B持有1000元
                B在監獄(Jail)
                A1是B的土地，價值1000元
        When:   A付過路費
        Then:   A無須付過路費
        """)]
    public void 地主在監獄無需付過路費()
    {
        // Arrange
        var map = new Map(Utils.SevenXSevenMap());
        var game = new Game("Test", map, Utils.MockDice(1));

        // 玩家AB
        // A餘額1000, B餘額1000
        var player_a = new Player("A", 1000);
        var player_b = new Player("B", 1000);
        
        game.AddPlayer(player_a);
        game.AddPlayer(player_b);

        game.Initial();

        //A1是B的土地，價值1000元
        Land A1 = (Land)map.FindBlockById("A1");
        game.SetPlayerToBlock(player_b, "A1", Map.Direction.Right);
        player_b.AddLandContract(new(player_b, A1));

        // B在監獄
        game.SetPlayerToBlock(player_b, "Jail", Map.Direction.Left);
        
        game.PlayerRollDice(player_a.Id); // point:1

        Land location = (Land)game.GetPlayerPosition(player_a.Id);
        
        decimal amount = 0;
        Player? payee = game.GetOwner(location);
 
        bool enoughMoney = game.CalculateToll(location, player_b, payee!,out amount);
        Assert.AreEqual(false, enoughMoney);
        Assert.AreEqual(0, amount);
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A, B
                目前輪到A
                A在A1上，持有1000元
                B持有1000元
                B在停車場
                A1是B的土地，價值500元
        When:   A付過路費
        Then:   A無須付過路費
        """)]
    public void 地主在停車場無需付過路費()
    {
        // Arrange
        var map = new Map(Utils.SevenXSevenMap());
        var game = new Game("Test", map, Utils.MockDice(1));

        // 玩家AB
        // A餘額1000, B餘額1000
        var player_a = new Player("A", 1000);
        var player_b = new Player("B", 1000);
        
        game.AddPlayer(player_a);
        game.AddPlayer(player_b);

        game.Initial();

        //A1是B的土地，價值1000元
        Land A1 = (Land)map.FindBlockById("A1");
        game.SetPlayerToBlock(player_b, "A1", Map.Direction.Right);
        player_b.AddLandContract(new(player_b, A1));

        // B在監獄
        game.SetPlayerToBlock(player_b, "ParkingLot", Map.Direction.Left);
        
        game.PlayerRollDice(player_a.Id); // point:1

        Land location = (Land)game.GetPlayerPosition(player_a.Id);
        
        decimal amount = 0;
        Player? payee = game.GetOwner(location);
 
        bool enoughMoney = game.CalculateToll(location, player_b, payee!,out amount);
        Assert.AreEqual(false, enoughMoney);
        Assert.AreEqual(0, amount);
    }
}