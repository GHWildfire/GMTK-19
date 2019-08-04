using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{ 
    private GameObject[] spawns;
    private GameObject slimesObject;
    private List<(float, SlimeManager.SpawmSlime)> initSlimes;
    private List<(float, SlimeManager.SpawmSlime)> slimes;

    private float elapsedTime;

    public Level(GameObject[] spawns, GameObject slimesObject, List<(float, SlimeManager.SpawmSlime)> slimes)
    {
        this.spawns = spawns;
        this.slimesObject = slimesObject;
        this.slimes = slimes;

        initSlimes = new List<(float, SlimeManager.SpawmSlime)>(slimes);

        elapsedTime = 0;
    }

    public void Update()
    {
        elapsedTime += Time.deltaTime;

        if (slimes.Count > 0)
        {
            for (int i = slimes.Count - 1; i >= 0; i--)
            {
                (float, SlimeManager.SpawmSlime) slime = slimes[i];

                if (slime.Item1 - 2 <= elapsedTime)
                {
                    slime.Item2(i, false);
                }

                if (slime.Item1 <= elapsedTime)
                {
                    slime.Item2(i, true);

                    slimes.RemoveAt(slimes.Count - 1);
                }
            }
        }
    }

    public bool Ended()
    {
        return slimes.Count == 0 && slimesObject.transform.childCount == 0;
    }

    public void ClearSlimes()
    {
        foreach (Transform child in slimesObject.transform)
        {
            Object.Destroy(child.gameObject);
        }

        slimes = new List<(float, SlimeManager.SpawmSlime)>(initSlimes);
    }

    private Vector3 NextSpawn()
    {
        GameObject spawn = spawns[Random.Range(0, slimes.Count)];
        return spawn.transform.position;
    }
}
