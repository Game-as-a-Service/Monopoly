namespace Domain.Interfaces;

public interface IDice
{
    public int Value { get; }

    public void Roll();
}