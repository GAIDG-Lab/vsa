using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MapBuilder : MonoBehaviour
{
    [SerializeField] private GameObject NorthWall;
    [SerializeField] private GameObject SouthWall;
    [SerializeField] private GameObject EastWall;
    [SerializeField] private GameObject WestWall;

    [SerializeField] private GameObject obstacle;
    [SerializeField] private GameObject spawnPair;

    [SerializeField] NavMeshSurface navMeshSurface;

    public int enviromentSize = 10;

    public int numberOfAgents = 5;


    private List<GameObject> pairList = new List<GameObject>();
    private List<GameObject> obstacleList = new List<GameObject>();

    //private float debugTime = 0f;

    private int numberOfBeginAgents = 0;

    public bool isHeuristic = false;


    private void Awake()
    {
        NorthWall.transform.position = new Vector3(NorthWall.transform.position.x, NorthWall.transform.position.y, enviromentSize);
        SouthWall.transform.position = new Vector3(SouthWall.transform.position.x, SouthWall.transform.position.y, enviromentSize * -1.0f);
        EastWall.transform.position = new Vector3(enviromentSize, EastWall.transform.position.y, EastWall.transform.position.z);
        WestWall.transform.position = new Vector3(enviromentSize * -1.0f, WestWall.transform.position.y, WestWall.transform.position.z);

        GenerateObstacle();

    }

    // Start is called before the first frame update
    void Start()
    {
        navMeshSurface.BuildNavMesh();

        SpawnAgents();

        pairList.Add(spawnPair);
    }

    // Update is called once per frame
    void Update()
    {
/*        debugTime += Time.deltaTime;

        if (debugTime >= 5f)
        {
            RebuildEnviroment();
            debugTime = 0;
        }*/

    }


    void GenerateObstacle()
    {
        if (!isHeuristic) {
            for (int x = -enviromentSize + 1; x <= enviromentSize - 1; x += 1)
            {
                for (int y = -enviromentSize + 1; y <= enviromentSize - 1; y += 1)
                {
                    if (Random.value > 0.8f)
                    {
                        Vector3 pos = new Vector3(x, 1f, y);
                        GameObject cubeObstacle = Instantiate(obstacle, pos, Quaternion.identity, transform);
                        obstacleList.Add(cubeObstacle);
                    }
                }
            }
        }
    }

    public void SendBeginSignal() {
        numberOfBeginAgents++;

        Debug.Log("Send Begin Signal");

        if (numberOfBeginAgents > numberOfAgents) {
            RebuildEnviroment();
            numberOfBeginAgents = 0;

            Debug.Log("All signal recieve! Restart!");

        }
    }

    private void SpawnAgents()
    {
        for (int i = 0; i < numberOfAgents; i++)
        {

            GameObject newPair = Instantiate(spawnPair, Vector3.zero, Quaternion.identity);
            GameObject visualSteeringAgent = newPair.transform.GetChild(0).gameObject;


            float floatSize = enviromentSize * 1f - 1f;

            Vector3 randomPos = new Vector3(Random.Range(-floatSize, floatSize), 0, Random.Range(-floatSize, floatSize));

            NavMeshHit hit;

            Vector3 randomAgentPosition;

            //Find nearest NavMesh position for Agent, if don't find any, re-randomize and try again
            while (!NavMesh.SamplePosition(randomPos, out hit, 2.0f, NavMesh.AllAreas))
            {
                randomPos = new Vector3(Random.Range(-floatSize, floatSize), 0, Random.Range(-floatSize, floatSize));
                Debug.Log("Have to re-roll for agent position");
            }

            //Assign randomize position to Agent
            randomAgentPosition = hit.position;
            randomAgentPosition.y = 0.1f;
            visualSteeringAgent.transform.position = randomAgentPosition;
            visualSteeringAgent.transform.Rotate(visualSteeringAgent.transform.up, Random.Range(-180f, 180f));

            GameObject agentGoal = newPair.transform.GetChild(1).gameObject;

            Vector3 randomGoalPos = new Vector3(Random.Range(-floatSize, floatSize), 0, Random.Range(-floatSize, floatSize));

            NavMeshHit goalHit;
            Vector3 randomGoalPosition;

            //Find nearest NavMesh position for Goal, if don't find any, re-randomize and try again
            while (!NavMesh.SamplePosition(randomGoalPos, out goalHit, 2.0f, NavMesh.AllAreas))
            {
                randomGoalPos = new Vector3(Random.Range(-floatSize, floatSize), 0, Random.Range(-floatSize, floatSize));
                Debug.Log("Have to re-roll for agent position");
            }

            randomGoalPosition =  goalHit.position;
            randomGoalPosition.y = 0.1f;

            agentGoal.transform.position = randomGoalPosition;


            pairList.Add(newPair);
        }
    }


    private void RebuildEnviroment() {
        List<GameObject> newObstacleList = new List<GameObject>();

        foreach (var obs in obstacleList) {
            Destroy(obs);
        }

        obstacleList.Clear();

        //Instantiate(obstacle, Vector3.zero, Quaternion.identity, transform);

        GenerateObstacle();

        navMeshSurface.BuildNavMesh();


        foreach (var pair in pairList) {
            GameObject agent = pair.transform.GetChild(0).gameObject;
            GameObject goal = pair.transform.GetChild(1).gameObject;

            float floatSize = enviromentSize * 1f - 1f;

            Vector3 randomAgentVec = new Vector3(Random.Range(-floatSize, floatSize), 0, Random.Range(-floatSize, floatSize));
            Vector3 randomGoalVec = new Vector3(Random.Range(-floatSize, floatSize), 0, Random.Range(-floatSize, floatSize));

            NavMeshHit agentHit;
            NavMeshHit goalHit;

            Vector3 randomAgentPosition;
            Vector3 randomGoalPosition;


            //Find nearest NavMesh position for Agent, if don't find any, re-randomize and try again
            while (!NavMesh.SamplePosition(randomAgentVec, out agentHit, 2.0f, NavMesh.AllAreas))
            {
                randomAgentVec = new Vector3(Random.Range(-(floatSize-1), floatSize-1), 0, Random.Range(-floatSize, floatSize));
                Debug.Log("Have to re-roll for agent position");
            }


            randomAgentPosition = agentHit.position;
            randomAgentPosition.y = 0.1f;
            agent.transform.position = randomAgentPosition;
            agent.transform.Rotate(agent.transform.up, Random.Range(-180f, 180f));


            //Find nearest NavMesh position for Goal, if don't find any, re-randomize and try again
            while (!NavMesh.SamplePosition(randomGoalVec, out goalHit, 2.0f, NavMesh.AllAreas))
            {
                randomGoalVec = new Vector3(Random.Range(-floatSize, floatSize), 0, Random.Range(-floatSize, floatSize));
                Debug.Log("Have to re-roll for agent position");
            }

            randomGoalPosition = goalHit.position;
            randomGoalPosition.y = 0.1f;

            goal.transform.position = randomGoalPosition;

        }

    }

}



