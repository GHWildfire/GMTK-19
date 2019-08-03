using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SlimeManager
{
    public delegate void SpawmSlime();

    public enum SlimeType
    {
        STANDARD,
        FAST,
        SLOW
    }

    public List<GameObject> Slimes { get; private set; }

    private List<GameObject> slimeModels;
    private List<GameObject> spawnPoints;

    private GameObject player;

    public SlimeManager(GameObject standardSlimeModel, GameObject fastSlimeModel, GameObject slowSlimeModel,
        GameObject player)
    {
        slimeModels = new List<GameObject>()
        {
            standardSlimeModel,
            fastSlimeModel,
            slowSlimeModel
        };

        this.player = player;

        Slimes = new List<GameObject>();
    }

    public void ChangeSpawnPoints(Transform parent)
    {
        List<GameObject> slimeSpawnPoints = new List<GameObject>();
        for (int i = 0; i < parent.childCount; i++)
        {
            slimeSpawnPoints.Add(parent.GetChild(i).gameObject);
        }

        spawnPoints = slimeSpawnPoints;
    }

    public void SpawnStandard()
    {
        if (spawnPoints == null)
        {
            Debug.Log("No spawn points !");
            return;
        }

        SlimeType type = SlimeType.STANDARD;

        GameObject selectedSpawn = SelectSpawn(GetSpawnType(type));

        GameObject slime = Object.Instantiate(slimeModels[(int)type]);
        slime.transform.position = selectedSpawn.transform.position;

        SlimeController slimeController = slime.GetComponent<SlimeController>();
        slimeController.Type = type;
        slimeController.InitSpeed = GetInitSpeed(type);
        //Ignore collision between player and slime
        Physics2D.IgnoreCollision(slimeController.MainCollider, player.GetComponent<PlayerController>().MainCollider);

        RotateRandomly(slime);

        Slimes.Add(slime);
    }

    private void RotateRandomly(GameObject slime)
    {
        float rotation = Random.Range(0.0f, 360.0f);
        slime.transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    private float GetInitSpeed(SlimeType type)
    {
        switch (type)
        {
            case SlimeType.STANDARD:
                return 12;

            case SlimeType.FAST:
                return 16;

            case SlimeType.SLOW:
                return 8;
        }

        return 0;
    }

    private SpawnStorage.SpawnType GetSpawnType(SlimeType type)
    {
        switch (type)
        {
            case SlimeType.STANDARD:
            case SlimeType.FAST:
            case SlimeType.SLOW:
            default:
                return SpawnStorage.SpawnType.STANDARD;
        }
    }

    private GameObject SelectSpawn(SpawnStorage.SpawnType type)
    {
        List<GameObject> filteredSpawns = spawnPoints.Where(x => x.GetComponent<SpawnStorage>().CurrentSpawnType == type).ToList();

        return filteredSpawns[Random.Range(0, filteredSpawns.Count)];
    }
}
