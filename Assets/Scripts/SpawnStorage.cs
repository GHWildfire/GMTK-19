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
            case SlimeController.SlimeType.FAST:
            case SlimeController.SlimeType.SLOW:
            default:
                return SpawnType.STANDARD;
        }
    }
}
