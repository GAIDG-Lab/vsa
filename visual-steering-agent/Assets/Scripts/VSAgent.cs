using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.AI;


public class VSAgent : Agent
{
    [SerializeField] private GameObject targetTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
        //Debug.Log(actions.DiscreteActions[0]);

        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float rotation = actions.ContinuousActions[2];

        float velocity = 4.0f;

        transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * velocity;
        Vector3 upVec = transform.up;
        transform.Rotate(upVec, rotation * 30 * Time.deltaTime);
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);

        float distanceToGoal = Vector3.Distance(targetTransform.transform.position, transform.position);

        sensor.AddObservation(distanceToGoal);
        sensor.AddObservation(targetTransform.transform.position - transform.position);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameObject.ReferenceEquals(other.gameObject, targetTransform))
        {
            Debug.Log("Hit goal");
            SetReward(+1f);
            EndEpisode();
        }

        if (other.CompareTag("Wall"))
        {
            Debug.Log("Hit wall");
            SetReward(-0.05f);
        }
        if (other.CompareTag("Agent"))
        {
            Debug.Log("Hit each other");
            SetReward(-0.025f);
        }
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        Vector3 randomPos = new Vector3(Random.Range(-9f, 9f), 0, Random.Range(-9f, 9f));
        NavMeshHit hit;

        Vector3 randomGoalPosition;
        while (!NavMesh.SamplePosition(randomPos, out hit, 3.0f, NavMesh.AllAreas))
        {
            randomPos = new Vector3(Random.Range(-9f, 9f), 0, Random.Range(-9f, 9f));
            Debug.Log("Have to re-roll for goal position");
        }

        randomGoalPosition = hit.position;
        randomGoalPosition.y = 0.1f;

        targetTransform.transform.position = randomGoalPosition;
        //Debug.Log("My goal position is at: " + targetTransform.transform.position);
    }
}
