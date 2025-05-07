using System;

public enum SceneName
{
    Main,
    PrototypingEvironment,
    Forest
}

[Serializable]
public enum Scenarios
{
    None,
    Prototype1,
    ForestFlatGround,
    ForestUpHill,
    ForestDownHill
}

[Serializable]
public enum AIType
{
    Attack,
    Defence
}
