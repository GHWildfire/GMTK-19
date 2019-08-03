using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeManager
{
    public enum SlimeType
    {
        STANDARD,
        FAST,
        SLOW
    }

    public List<GameObject> Slimes { get; private set; }

    private List<GameObject> slimeModels;
    private List<GameObject> slimeSpawnPoints;

    private GameObject player;

    public SlimeManager(GameObject standardSlimeModel, GameObject fastSlimeModel, GameObject slowSlimeModel,
        GameObject player, List<GameObject> slimeSpawnPoints = null)
    {
        slimeModels = new List<GameObject>()
        {
            standardSlimeModel,
            fastSlimeModel,
            slowSlimeModel
        };

        this.player = player;

        if (slimeSpawnPoints == null)
        {
            this.slimeSpawnPoints = new List<GameObject>();
        }
        else
        {
            this.slimeSpawnPoints = slimeSpawnPoints;
        }

        Slimes = new List<GameObject>();
    }

    public bool SpawnRandomSlime()
    {
        if (slimeSpawnPoints == null)
        {
            return false;
        }

        int slimeIdx = Random.Range(0, slimeModels.Count);
        int spawnPointIdx = Random.Range(0, slimeSpawnPoints.Count);

        GameObject slime = Object.Instantiate(slimeModels[slimeIdx]);
        slime.transform.position = slimeSpawnPoints[spawnPointIdx].transform.position;
        //Ignore collision between player and slime
        Physics2D.IgnoreCollision(slime.GetComponent<SlimeController>().MainCollider, player.GetComponent<PlayerController>().MainCollider);

        RotateRandomly(slime);

        Slimes.Add(slime);

        return true;
    }

    public bool SpawnSlime(SlimeType type)
    {
        if (slimeSpawnPoints == null)
        {
            return false;
        }

        int spawnPointIdx = Random.Range(0, slimeSpawnPoints.Count);

        GameObject slime = Object.Instantiate(slimeModels[(int)type]);
        slime.transform.position = slimeSpawnPoints[spawnPointIdx].transform.position;
        //Ignore collision between player and slime
        Physics2D.IgnoreCollision(slime.GetComponent<SlimeController>().MainCollider, player.GetComponent<PlayerController>().MainCollider);

        RotateRandomly(slime);

        Slimes.Add(slime);

        return true;
    }

    public void ChangeSpawnPoints(List<GameObject> spawnPoints)
    {
        if (slimeSpawnPoints == null)
        {
            return;
        }

        slimeSpawnPoints.Clear();
        slimeSpawnPoints.AddRange(spawnPoints);
    }

    private void RotateRandomly(GameObject slime)
    {
        float rotation = Random.Range(0.0f, 360.0f);
        slime.transform.rotation = Quaternion.Euler(0, 0, rotation);
    }
}
