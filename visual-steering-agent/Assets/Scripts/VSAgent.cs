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

    private float episodeBeginDistance;

    [SerializeField] private GameObject enviromentBuilder;

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

        AddReward(-3f / MaxStep);

        float currentDistance = Vector3.Distance(targetTransform.transform.position, transform.position);
        float distanceReward = ExponentialRerwardFunction(currentDistance / episodeBeginDistance);

        AddReward(distanceReward / MaxStep);

        Vector3 directionToGoal = targetTransform.transform.position - transform.position;
        float offAngle = Vector3.Angle(directionToGoal, transform.forward);
        float rotationReward = ExponentialRerwardFunction(offAngle / 180f);

        AddReward(rotationReward / MaxStep);

    }


    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);

        float distanceToGoal = Vector3.Distance(targetTransform.transform.position, transform.position);

        sensor.AddObservation(distanceToGoal);


        Vector3 directionToGoal = targetTransform.transform.position - transform.position;
        sensor.AddObservation(directionToGoal.x/distanceToGoal);
        sensor.AddObservation(directionToGoal.z/distanceToGoal);

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = Input.GetAxisRaw("Vertical");

        //float currentDistance = Vector3.Distance(targetTransform.transform.position, transform.position);
        //Debug.Log(currentDistance/episodeBeginDistance);
        //Debug.Log("Exponential reward is:" + ExponentialRerwardFunction(currentDistance / episodeBeginDistance));

        //Vector3 directionToGoal = targetTransform.transform.position - transform.position;
        //Debug.Log(directionToGoal / currentDistance);
        //float angle = Vector3.Angle(directionToGoal, transform.forward);
        //Debug.Log(angle);
        //Debug.Log("Exponential reward is:" + ExponentialRerwardFunction(angle / 180f));

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

        var mapBuilder = enviromentBuilder.gameObject.GetComponent<BoundaryControl>();
        Vector3 randomPos = new Vector3(Random.Range(-mapBuilder.enviromentSize, mapBuilder.enviromentSize), 0, Random.Range(-mapBuilder.enviromentSize, mapBuilder.enviromentSize));

        NavMeshHit hit;

        Vector3 randomGoalPosition;
        while (!NavMesh.SamplePosition(randomPos, out hit, 3.0f, NavMesh.AllAreas))
        {

            randomPos = new Vector3(Random.Range(-mapBuilder.enviromentSize, mapBuilder.enviromentSize), 0, Random.Range(-mapBuilder.enviromentSize, mapBuilder.enviromentSize));
            Debug.Log("Have to re-roll for goal position");
        }

        randomGoalPosition = hit.position;
        randomGoalPosition.y = 0.1f;

        targetTransform.transform.position = randomGoalPosition;

        episodeBeginDistance = Vector3.Distance(targetTransform.transform.position, transform.position);
    }

    private float ExponentialRerwardFunction(float x)
    {
        return Mathf.Exp(-x * 2f);
    }
}

