using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class SlimeManager
{
    public delegate void SpawmSlime(int slimeIdx, bool isSlimeReadyToSpawn);

    public List<GameObject> Slimes { get; private set; }

    private const float TIME_TO_SCALE_SPAWN = 1;

    private List<GameObject> slimeModels;
    private List<GameObject> spawnPoints;
    private Dictionary<int, GameObject> slimesSpawnPoints;

    private Transform slimesParent;

    private GameObject player;

    private float currentTimeToScaleSpawn;

    public SlimeManager(GameObject standardSlimeModel, GameObject fastSlimeModel, GameObject slowSlimeModel, 
        GameObject boss1SlimeModel, GameObject boss2SlimeModel, GameObject boss3SlimeModel, GameObject player)
    {
        slimeModels = new List<GameObject>()
        {
            standardSlimeModel,
            fastSlimeModel,
            slowSlimeModel,
            boss1SlimeModel,
            boss2SlimeModel,
            boss3SlimeModel
        };

        slimesSpawnPoints = new Dictionary<int, GameObject>();

        this.player = player;

        Slimes = new List<GameObject>();

        currentTimeToScaleSpawn = 0;
    }

    public void Init()
    {
        slimesSpawnPoints.Clear();
    }

    public void Update()
    {
        // Manage ignore collisions between bullet and slimes
        GameObject bulletRef = player.GetComponent<PlayerController>().BulletRef;
        if (bulletRef != null)
        {
            BulletController bulletController = bulletRef.GetComponent<BulletController>();

            for (int i = Slimes.Count - 1; i >= 0; i--)
            {
                GameObject slime = Slimes[i];

                // Prevent accessing to null object
                if (slime == null)
                {
                    Slimes.RemoveAt(i);
                    continue;
                }

                Physics2D.IgnoreCollision(
                    bulletRef.GetComponent<BulletController>().MainCollider,
                    slime.GetComponent<SlimeController>().MainCollider
                );
            }
        }
    }

    public void ChangeLevel(Transform spawnsParent, Transform slimesParent)
    {
        Slimes.Clear();

        List<GameObject> slimeSpawnPoints = new List<GameObject>();
        for (int i = 0; i < spawnsParent.childCount; i++)
        {
            slimeSpawnPoints.Add(spawnsParent.GetChild(i).gameObject);
        }

        spawnPoints = slimeSpawnPoints;

        this.slimesParent = slimesParent;
    }

    public void SpawnStandard(int slimeIdx, bool isSlimeReadyToSpawn)
    {
        ManageSpawnSlime(SlimeController.SlimeType.STANDARD, slimeIdx, isSlimeReadyToSpawn);
    }

    public void SpawnSlow(int slimeIdx, bool isSlimeReadyToSpawn)
    {
        ManageSpawnSlime(SlimeController.SlimeType.SLOW, slimeIdx, isSlimeReadyToSpawn);
    }

    public void SpawnFast(int slimeIdx, bool isSlimeReadyToSpawn)
    {
        ManageSpawnSlime(SlimeController.SlimeType.FAST, slimeIdx, isSlimeReadyToSpawn);
    }
    
    public void SpawnBoss1(int slimeIdx, bool isSlimeReadyToSpawn)
    {
        ManageSpawnSlime(SlimeController.SlimeType.BOSS1, slimeIdx, isSlimeReadyToSpawn);
    }

    public void SpawnBoss2(int slimeIdx, bool isSlimeReadyToSpawn)
    {
        ManageSpawnSlime(SlimeController.SlimeType.BOSS2, slimeIdx, isSlimeReadyToSpawn);
    }

    public void SpawnBoss3(int slimeIdx, bool isSlimeReadyToSpawn)
    {
        ManageSpawnSlime(SlimeController.SlimeType.BOSS3, slimeIdx, isSlimeReadyToSpawn);
    }

    private void ManageSpawnSlime(SlimeController.SlimeType type, int slimeIdx, bool isSlimeReadyToSpawn)
    {
        // Select slime spawn point if none exists
        if (!slimesSpawnPoints.ContainsKey(slimeIdx))
        {
            slimesSpawnPoints[slimeIdx] = SelectSpawn(SpawnStorage.GetSpawnType(type));
            currentTimeToScaleSpawn = 0;
        }

        // Check if the slime is ready to be spawned
        if (isSlimeReadyToSpawn)
        {
            SpawnSlime(type, slimesSpawnPoints[slimeIdx]);
        }
        else
        {
            PreSpawnSlime(type, slimesSpawnPoints[slimeIdx]);
        }
    }

    private void SpawnSlime(SlimeController.SlimeType type, GameObject selectedSpawn)
    {
        if (spawnPoints == null)
        {
            Debug.Log("No spawn points !");
            return;
        }

        GameObject slime = UnityEngine.Object.Instantiate(slimeModels[(int)type]);
        slime.transform.position = selectedSpawn.transform.position;
        slime.transform.SetParent(slimesParent);

        selectedSpawn.SetActive(false);

        SlimeController slimeController = slime.GetComponent<SlimeController>();
        slimeController.Init(type);
        //Ignore collision between player and slime
        Physics2D.IgnoreCollision(slimeController.MainCollider, player.GetComponent<PlayerController>().MainCollider);

        RotateRandomly(slime);

        Slimes.Add(slime);
    }

    private void PreSpawnSlime(SlimeController.SlimeType type, GameObject selectedSpawn)
    {
        currentTimeToScaleSpawn += Time.deltaTime;

        Vector3 finalScale = slimeModels[(int)type].transform.localScale * 0.8f;
        float scalePerc = currentTimeToScaleSpawn / TIME_TO_SCALE_SPAWN;

        selectedSpawn.transform.localScale = Vector3.Lerp(Vector3.zero, finalScale, scalePerc);
        selectedSpawn.SetActive(true);
    }

    private void RotateRandomly(GameObject slime)
    {
        float rotation = UnityEngine.Random.Range(0.0f, 360.0f);
        slime.GetComponent<SlimeController>().Body.transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    private GameObject SelectSpawn(SpawnStorage.SpawnType type)
    {
        List<GameObject> filteredSpawns = spawnPoints.Where(x => x.GetComponent<SpawnStorage>().CurrentSpawnType == type).ToList();

        return filteredSpawns[UnityEngine.Random.Range(0, filteredSpawns.Count)];
    }
}
