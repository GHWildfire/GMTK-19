﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{ 
    private GameObject[] spawns;
    private GameObject slimesObject;
    private List<(float, Slime)> initSlimes;
    private List<(float, Slime)> slimes;

    public Level(GameObject[] spawns, GameObject slimesObject, List<(float, Slime)> slimes)
    {
        this.spawns = spawns;
        this.slimesObject = slimesObject;
        this.slimes = slimes;

        initSlimes = new List<(float, Slime)>(slimes);

    }

    public void Update(float timePassed)
    {
        if (slimes.Count > 0)
        {
            (float, Slime) slime = slimes[0];
            if (slime.Item1 <= timePassed)
            {
                slime.Item2.Instantiate(NextSpawn(), slimesObject);
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

        slimes = new List<(float, Slime)>(initSlimes);
    }

    private Vector3 NextSpawn()
    {
        GameObject spawn = spawns[Random.Range(0, slimes.Count)];
        return spawn.transform.position;
    }
}