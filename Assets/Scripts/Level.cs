using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{ 
    private GameObject[] spawns;
    private List<(float, Slime)> mobs;

    public Level(GameObject[] spawns, List<(float, Slime)> mobs)
    {
        this.spawns = spawns;
        this.mobs = mobs;
    }

    public void Update(float timePassed)
    {
        if (mobs.Count > 0)
        {
            (float, Slime) slime = mobs[0];
            if (slime.Item1 <= timePassed)
            {
                slime.Item2.Instantiate(NextSpawn());
                mobs.RemoveAt(0);
            }
        }
    }

    private Vector3 NextSpawn()
    {
        GameObject spawn = spawns[Random.Range(0, mobs.Count)];
        return spawn.transform.position;
    }
}
