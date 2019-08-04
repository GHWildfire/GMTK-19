using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStorage : MonoBehaviour
{
    public enum SpawnType
    {
        STANDARD,
        BOSS
    }

    public SpawnType CurrentSpawnType;

    public static SpawnType GetSpawnType(SlimeController.SlimeType type)
    {
        switch (type)
        {
            case SlimeController.SlimeType.STANDARD:
                return SpawnType.STANDARD;
            case SlimeController.SlimeType.FAST:
                return SpawnType.STANDARD;
            case SlimeController.SlimeType.SLOW:
                return SpawnType.STANDARD;
            case SlimeController.SlimeType.BOSS1:
                return SpawnType.BOSS;
            default:
                return SpawnType.STANDARD;
        }
    }
}
