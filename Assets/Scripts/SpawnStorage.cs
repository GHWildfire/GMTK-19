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
}
