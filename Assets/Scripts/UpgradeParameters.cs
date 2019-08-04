
public static class UpgradeParameters
{
    public const bool DID_PLAYER_HEAL = false;
    public const float HEAL_PLAYER_FACTOR = 0.33f;
    public const float BULLET_SPEED = 30f;
    public const float BULLET_TIME = 1f;
    public const float BULLET_DAMAGES = 1f;
    public const float BULLET_SCALE_FACTOR = 1f;
    public const float PLAYER_SPEED = 20f;
    public const float PLAYER_LIFE = 2f;
    public const float ENNEMY_SPEED_FACTOR = 1f;

    public static bool DidPlayerHeal = false;
    public static float HealPlayerFactor = 0.33f;
    public static float BulletSpeed;
    public static float BulletTime;
    public static float BulletDamages;
    public static float BulletScaleFactor;
    public static float PlayerSpeed;
    public static float PlayerLife;
    public static float EnnemySpeedFactor;

    public static float BulletSpeedMult = 1.3f;
    public static float BulletTimeMult = 1.5f;
    public static float BulletDamagesMult = 1.3f;
    public static float BulletScaleFactorMult = 1.3f;
    public static float PlayerSpeedMult = 1.15f;
    public static float PlayerLifeMult = 2f;
    public static float EnnemySpeedFactorMult = 1.3f;

    public static void Init()
    {
        DidPlayerHeal = DID_PLAYER_HEAL;
        HealPlayerFactor = HEAL_PLAYER_FACTOR;
        BulletSpeed = BULLET_SPEED;
        BulletTime = BULLET_TIME;
        BulletDamages = BULLET_DAMAGES;
        BulletScaleFactor = BULLET_SCALE_FACTOR;
        PlayerSpeed = PLAYER_SPEED;
        PlayerLife = PLAYER_LIFE;
        EnnemySpeedFactor = ENNEMY_SPEED_FACTOR;
    }
}
