namespace Shared.Domain;

public record LandContract(Player? Owner, Land Land)
{
    internal Player? Owner { get; set; } = Owner;
}