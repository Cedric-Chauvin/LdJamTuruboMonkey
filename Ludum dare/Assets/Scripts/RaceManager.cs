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
    public int nbLaps = 0;
    public float totalTimer = 0.0f;
    public List<obstacle> obstaclesToReset = new List<obstacle>();
    bool canCollide = true;
    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        Time.timeScale = 1.0f;
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
            {
                checkPointId = 0;
                nbLaps++;
            }

            foreach (obstacle o in obstaclesToReset)
                o.Reset();

            obstaclesToReset.Clear();
            timer = checkpoints[checkPointId].timeToReach - nbOfCollision;
            nbOfCollision = 0;
        }
    }

    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            UIManager.Instance.updateTimer(timer,timeBetweenCheckpoints);
            totalTimer += Time.deltaTime;
        }
        else
        {
            Time.timeScale = 0.0f;
        }
    }

    public void AddCollision()
    {
        if(canCollide)
        {
            canCollide = false;
            StartCoroutine("CollideCoroutine");
        }
    }

    private IEnumerator CollideCoroutine()
    {
        nbOfCollision++;
        yield return new WaitForSeconds(1f);
        canCollide = true;
    }
}
