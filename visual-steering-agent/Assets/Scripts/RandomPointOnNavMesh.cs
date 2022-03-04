// RandomPointOnNavMesh
using UnityEngine;
using UnityEngine.AI;

public class RandomPointOnNavMesh : MonoBehaviour
{
    public int numberOfAgent = 10;

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
        for (int i = 0; i < numberOfAgent; i++) {
            //Instantiate pair 
            GameObject newPair = Instantiate(testObject, Vector3.zero, Quaternion.identity);
            GameObject visualSteeringAgent = newPair.transform.GetChild(0).gameObject;


            //visualSteeringAgent.transform.position = new Vector3(0f, 0.1f, -6f);
            //goal.transform.position = new Vector3(0f, 0.1f, 7f);

            Vector3 randomPos = new Vector3(Random.Range(-9f, 9f), 0, Random.Range(-9f, 9f));
            NavMeshHit hit;

            Vector3 randomAgentPosition;

            //Find nearest NavMesh position for Agent, if don't find any, re-randomize and try again
            while (!NavMesh.SamplePosition(randomPos, out hit, 3.0f, NavMesh.AllAreas))
            {
                randomPos = new Vector3(Random.Range(-9f, 9f), 0, Random.Range(-9f, 9f));
                Debug.Log("Have to re-roll for agent position");
            }

            //Assign randomize position to Agent
            randomAgentPosition = hit.position;
            randomAgentPosition.y = 0.1f;
            visualSteeringAgent.transform.position = randomAgentPosition;
        }
    }

}