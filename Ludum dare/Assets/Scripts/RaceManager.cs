using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    private static RaceManager _instance = null;
    public static RaceManager Instance
    {
        get { return _instance; }
    }


    public List<Checkpoint> checkpoints;
    public float timeBetweenCheckpoints = 10.0f;
    public float timer = 0.0f;
    public int checkPointId = 0;
    public int nbOfCollision = 0;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    void Start()
    {
        timer = timeBetweenCheckpoints;
    }

    public void NextCheckPoint(Checkpoint reached)
    {
        Debug.Log("Checkpoint Reached");
        if(checkpoints[checkPointId] == reached)
        {
            if (checkPointId < checkpoints.Count - 1)
                checkPointId++;
            else
                checkPointId = 0;

            timer = timeBetweenCheckpoints - nbOfCollision;
            nbOfCollision = 0;
        }
    }

    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            //t nul
        }
    }
}
