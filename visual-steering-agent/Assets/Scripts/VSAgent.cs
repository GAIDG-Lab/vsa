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


    public bool isTesting = false;

    //Foward Vector toggle:


    //enviromentalScale: because of collision system is set up, when a collision happen, 
    //a collision is registered thrice, thus a variable to scale down reward happen on collision

    private float enviromentalScale = 1 / 3f;

    //Scale variables:
    // Wg: goalReachingWeight
    // Ww: wallCollisionWeight
    // Wca: agentCollisionWeight
    // Wd: distanceEncouragementWeight
    // Wh: rotationEncouragementWeight


    public float goalReachingWeight = 0.9f;
    public float wallCollisionWeight = 0.025f;
    public float agentCollisionWeight = 0.025f;
    public float distanceEncouragementWeight = 0.025f;
    public float rotationEncouragementWeight = 0.025f;


    private LineRenderer lr;

    private Vector3 previousMove;



    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 1.2f;
        //Debug.DrawLine(transform.position, transform.position + forward, Color.blue);
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + forward);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);

        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float rotation = actions.ContinuousActions[2];

        float velocity = 4.0f;

        transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * velocity;
        Vector3 upVec = transform.up;
        transform.Rotate(upVec, rotation * 90 * Time.deltaTime);

        previousMove = new Vector3(moveX, 0, moveZ);

        float currentDistance = Vector3.Distance(targetTransform.transform.position, transform.position);
        float distanceReward = ExponentialRerwardFunction(currentDistance);


        AddReward(distanceReward * distanceEncouragementWeight);

        Vector3 directionToGoal = targetTransform.transform.position - transform.position;
        float offAngle = Vector3.Angle(directionToGoal, transform.forward);
        float rotationReward = ExponentialRerwardFunction(offAngle / 180f);

        AddReward(rotationReward * rotationEncouragementWeight);

    }


    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);

        float distanceToGoal = Vector3.Distance(targetTransform.transform.position, transform.position);

        sensor.AddObservation(distanceToGoal);


        Vector3 directionToGoal = targetTransform.transform.position - transform.position;
        sensor.AddObservation(directionToGoal.normalized.x);
        sensor.AddObservation(directionToGoal.normalized.z);


        //Add forward Vector
        sensor.AddObservation(transform.forward.x);
        sensor.AddObservation(transform.forward.z);


        //Add last step Vector
        sensor.AddObservation(previousMove.normalized.x);
        sensor.AddObservation(previousMove.normalized.z);
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
            if (!isTesting)
            {
                Debug.Log("Hit goal");
                SetReward(+1f * enviromentalScale * goalReachingWeight);
                var mapBuilder = enviromentBuilder.gameObject.GetComponent<MapBuilder>();

                float floatSize = mapBuilder.enviromentSize * 1f - 1f;


                Vector3 randomGoalPos = new Vector3(Random.Range(-floatSize, floatSize), 0, Random.Range(-floatSize, floatSize));

                NavMeshHit goalHit;
                Vector3 randomGoalPosition;

                //Find nearest NavMesh position for Goal, if don't find any, re-randomize and try again
                while (!NavMesh.SamplePosition(randomGoalPos, out goalHit, 2.0f, NavMesh.AllAreas))
                {
                    randomGoalPos = new Vector3(Random.Range(-floatSize, floatSize), 0, Random.Range(-floatSize, floatSize));
                    Debug.Log("Have to re-roll for agent position");
                }

                randomGoalPosition = goalHit.position;
                randomGoalPosition.y = 0.1f;

                targetTransform.transform.position = randomGoalPosition;
            }
            else {
                transform.gameObject.SetActive(false);
            }


            //EndEpisode();
        }

        if (other.CompareTag("Wall"))
        {
            Debug.Log("Hit wall");
            SetReward(-1f * enviromentalScale * wallCollisionWeight);
        }
        if (other.CompareTag("Agent"))
        {
            Debug.Log("Hit each other");
            SetReward(-1f * enviromentalScale * agentCollisionWeight);
        }
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();

        var mapBuilder = enviromentBuilder.gameObject.GetComponent<MapBuilder>();

        if (!mapBuilder.isHeuristic) {
            mapBuilder.SendBeginSignal();
        }

        /*Vector3 randomPos = new Vector3(Random.Range(-mapBuilder.enviromentSize, mapBuilder.enviromentSize), 0, Random.Range(-mapBuilder.enviromentSize, mapBuilder.enviromentSize));

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

        episodeBeginDistance = Vector3.Distance(targetTransform.transform.position, transform.position);*/
    }

    private float ExponentialRerwardFunction(float x)
    {
        return Mathf.Exp(-x);

        //e^(-x)
    }
}

