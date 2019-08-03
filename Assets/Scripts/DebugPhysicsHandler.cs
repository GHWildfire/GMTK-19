using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPhysicsHandler : MonoBehaviour
{
    [SerializeField] private GameObject playerModel;
    [SerializeField] private GameObject slimeSpawnPointsParent;

    [Header("Slimes Models")]
    [SerializeField] private GameObject standardSlimeModel;
    [SerializeField] private GameObject fastSlimeModel;
    [SerializeField] private GameObject slowSlimeModel;

    private const float SPAWN_DURATION = 2;

    private float currentSpawnTimer;

    private SlimeManager slimeManager;

    private GameObject currentPlayer;

    private void Awake()
    {
        currentSpawnTimer = SPAWN_DURATION;

        currentPlayer = Instantiate(playerModel);

        slimeManager = new SlimeManager(standardSlimeModel, fastSlimeModel, slowSlimeModel, currentPlayer);

        ChangeSlimeSpawnPoints(slimeSpawnPointsParent.transform);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Manage slime spawn
        currentSpawnTimer -= Time.deltaTime;

        if (currentSpawnTimer <= 0)
        {
            currentSpawnTimer = SPAWN_DURATION;
            slimeManager.SpawnRandomSlime();
        }

        // Manage ignore collisions between bullet and slimes
        GameObject bulletRef = currentPlayer.GetComponent<PlayerController>().BulletRef;
        if (bulletRef != null)
        {
            foreach (GameObject item in slimeManager.Slimes)
            {
                Physics2D.IgnoreCollision(
                    bulletRef.GetComponent<BulletController>().MainCollider,
                    item.GetComponent<SlimeController>().MainCollider
                );
            }
        }
    }

    private void ChangeSlimeSpawnPoints(Transform slimeSpawnPointsParentTransform)
    {
        List<GameObject> slimeSpawnPoints = new List<GameObject>();

        for (int i = 0; i < slimeSpawnPointsParentTransform.childCount; i++)
        {
            slimeSpawnPoints.Add(slimeSpawnPointsParentTransform.GetChild(i).gameObject);
        }

        slimeManager.ChangeSpawnPoints(slimeSpawnPoints);
    }
}
