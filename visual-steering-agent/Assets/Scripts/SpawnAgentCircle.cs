using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAgentCircle : MonoBehaviour
{

    [SerializeField] private GameObject spawnPair;

    public float radius = 10f;
    public int amountToSpawn = 10;

    // Offset variable to spawn goal behind an agents
    private float goalPositionOffset = 2.2f;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 baseNewPos = new Vector3(Mathf.Cos(0) * radius, 0.0f, Mathf.Sin(0) * radius);
        GameObject baseVSAgent = spawnPair.transform.GetChild(0).gameObject;
        Vector3 baseLookDirection = Vector3.zero - baseNewPos;

        baseVSAgent.transform.position = baseNewPos;
        baseVSAgent.transform.rotation = Quaternion.LookRotation(baseLookDirection);

        GameObject baseAgentGoal = spawnPair.transform.GetChild(1).gameObject;

        Vector3 baseGoalPosition = baseNewPos + goalPositionOffset * baseLookDirection;

        baseAgentGoal.transform.position = baseGoalPosition;


        for (int i = 1; i < amountToSpawn; i++)
        {
            float angle = i * Mathf.PI * 2f / amountToSpawn;
            Vector3 newPos = new Vector3(Mathf.Cos(angle) * radius, 0.0f, Mathf.Sin(angle) * radius);

            GameObject newPair = Instantiate(spawnPair, Vector3.zero, Quaternion.identity);
            GameObject visualSteeringAgent = newPair.transform.GetChild(0).gameObject;

            Vector3 lookDirection = Vector3.zero - newPos;

            visualSteeringAgent.transform.position = newPos;
            visualSteeringAgent.transform.rotation = Quaternion.LookRotation(lookDirection);

            GameObject agentGoal = newPair.transform.GetChild(1).gameObject;

            Vector3 goalPosition = newPos + 2.2f * lookDirection;

            agentGoal.transform.position = goalPosition;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
