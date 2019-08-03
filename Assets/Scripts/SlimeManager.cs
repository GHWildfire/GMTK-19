using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SlimeManager
{
    public delegate void SpawmSlime();

    public List<GameObject> Slimes { get; private set; }

    private List<GameObject> slimeModels;
    private List<GameObject> spawnPoints;

    private Transform slimesParent;

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

    public void SpawnStandard()
    {
        if (spawnPoints == null)
        {
            Debug.Log("No spawn points !");
            return;
        }

        SpawnSlime(SlimeController.SlimeType.STANDARD);
    }

    public void SpawnSlow()
    {
        if (spawnPoints == null)
        {
            Debug.Log("No spawn points !");
            return;
        }

        SpawnSlime(SlimeController.SlimeType.SLOW);
    }

    private void SpawnSlime(SlimeController.SlimeType type)
    {
        GameObject selectedSpawn = SelectSpawn(SpawnStorage.GetSpawnType(type));

        GameObject slime = Object.Instantiate(slimeModels[(int)type]);
        slime.transform.position = selectedSpawn.transform.position;
        slime.transform.SetParent(slimesParent);

        SlimeController slimeController = slime.GetComponent<SlimeController>();
        slimeController.Init(type);
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

    private GameObject SelectSpawn(SpawnStorage.SpawnType type)
    {
        List<GameObject> filteredSpawns = spawnPoints.Where(x => x.GetComponent<SpawnStorage>().CurrentSpawnType == type).ToList();

        return filteredSpawns[Random.Range(0, filteredSpawns.Count)];
    }
}
