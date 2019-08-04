using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{ 
    private GameObject[] spawns;
    private GameObject slimesObject;
    private List<(float, SlimeManager.SpawmSlime)> initSlimes;
    private List<(float, SlimeManager.SpawmSlime)> slimes;

    public Level(GameObject[] spawns, GameObject slimesObject, List<(float, SlimeManager.SpawmSlime)> slimes)
    {
        this.spawns = spawns;
        this.slimesObject = slimesObject;
        this.slimes = slimes;

        initSlimes = new List<(float, SlimeManager.SpawmSlime)>(slimes);
    }

    public void Update(float timePassed)
    {
        if (slimes.Count > 0)
        {
            (float, SlimeManager.SpawmSlime) slime = slimes[0];

            if (slime.Item1 - 1 <= timePassed)
            {
                slime.Item2();
            }

            if (slime.Item1 <= timePassed)
            {
                slimes.RemoveAt(0);
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
