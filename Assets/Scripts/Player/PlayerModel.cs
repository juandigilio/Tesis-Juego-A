using System;

[Serializable]
public class PlayerSettings
{
    public float speed;
}

public class PlayerModel
{
    public PlayerSettings Settings { get; private set; }
    public float MoveXInputDir { get; private set; }

    public PlayerModel(PlayerSettings settings)
    {
        Settings = settings;
    }

    public void SetMoveXInput(float input)
    {
        MoveXInputDir = Math.Clamp(input, -1f, 1f);
    }
}