public static class Consts {
    public const string VarScoreLast = "score_last";
    public const string VarScoreHigh = "score_high";
    public const string VarNumGames = "num_games";
    public const string SavePath = "user://game.save";
    public const string AudioBookDrop = "res://assets/audio/thud.wav";
    public const string AudioAddBook = "res://assets/audio/add_book.wav";
    public const string AudioLoseBook = "res://assets/audio/lose_book.wav";
    public const string AudioBeepShort = "res://assets/audio/beep_short.wav";
    public const string AudioBeepLong = "res://assets/audio/beep_long.wav";
    public const float AudioRepeatDelayMsec = 150;
    public const int SessionSeconds = 60;
    public const float MoveSpeed = 2.0f;
    public const float JumpVelocity = 3.0f;
    public const float CrouchSpeed = 10f;
    public const float BookDropSpeed = 2f;
    public const float BookForceSpeed = 2 * 20f;
    public const float BookRotateSpeed = 0.25f;
    public const float BookHoldDist = 0.5f;
    public const float MinBookHoldDist = 0.5f;
    public const float MaxBookHoldDist = 1f;
    public const float BookHoldDistChangeSpeed = 0.25f;
    public const float BookScoreSizeMul = 20f;
    public const float BookStillnessThreshold = 0.0001f;
    public const float BookCarryMass = 0.5f;
    public const float BookHitSoundMinVelocity = 0.1f;
    public const float BookHitSoundMinVelocitySqr = BookHitSoundMinVelocity * BookHitSoundMinVelocity;
}