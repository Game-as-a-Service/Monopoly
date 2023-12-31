﻿namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerCannotBuildHouseEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required string LandId { get; init; }
}