using Domain.Interfaces;

namespace ServerTests.Usecases;

public class MockDiceService
{
    public IDice[] Dices { get; set; } = default!;
}

public class MockDice : IDice
{
    private int _value;
    public int Value => _value;
    public MockDice(int value)
    {
        _value = value;
    }
    public void Roll()
    {
    }
}