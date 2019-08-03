using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    [SerializeField] private GameObject handlerObject;

    private GameHandler handler;

    // Start is called before the first frame update
    void Start()
    {
        handler = handlerObject.GetComponent<GameHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
