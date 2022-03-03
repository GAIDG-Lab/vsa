// RandomPointOnNavMesh
using UnityEngine;
using UnityEngine.AI;

public class RandomPointOnNavMesh : MonoBehaviour
{
    public float range = 10.0f;

    public GameObject testObject;

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    void Start()
    {
        //Instantiate pair 
        Quaternion emptyRotation = new Quaternion();
        GameObject newPair = Instantiate(testObject, Vector3.zero, emptyRotation);


        GameObject visualSteeringAgent = newPair.transform.GetChild(0).gameObject;
        GameObject goal = newPair.transform.GetChild(1).gameObject;


        //visualSteeringAgent.transform.position = new Vector3(0f, 0.1f, -6f);
        //goal.transform.position = new Vector3(0f, 0.1f, 7f);

        Vector3 randomPos = new Vector3(Random.Range(-9f, 9f), 0, Random.Range(-9f, 9f));
        NavMeshHit hit;

        Vector3 randomAgentPosition;
        Vector3 randomGoalPosition;

        //Find nearest NavMesh position for Agent, if don't find any, re-randomize and try again
        while (!NavMesh.SamplePosition(randomPos, out hit, 3.0f, NavMesh.AllAreas)) {
            randomPos = new Vector3(Random.Range(-9f, 9f), 0, Random.Range(-9f, 9f));
            Debug.Log("Have to re-roll for agent position");
        }

        //Assign randomize position to Agent
        randomAgentPosition = hit.position;
        randomAgentPosition.y = 0.1f;
        visualSteeringAgent.transform.position = randomAgentPosition;

        randomPos = new Vector3(Random.Range(-9f, 9f), 0, Random.Range(-9f, 9f));

        //Find nearest NavMesh position for Goal, if don't find any, re-randomize and try again
        while (!NavMesh.SamplePosition(randomPos, out hit, 3.0f, NavMesh.AllAreas))
        {
            randomPos = new Vector3(Random.Range(-9f, 9f), 0, Random.Range(-9f, 9f));
            Debug.Log("Have to re-roll for goal position");

        }

        //Assign randomize position to Goal
        randomGoalPosition = hit.position;
        randomGoalPosition.y = 0.1f;
        goal.transform.position = randomGoalPosition;


    }

    void Update()
    {
/*        Vector3 point;
        if (RandomPoint(transform.position, range, out point))
        {
            Debug.DrawRay(point, Vector3.up, Color.blue, 5.0f);
        }*/
    }
}