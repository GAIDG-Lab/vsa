using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

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
            EndEpisode();
        }

        if (other.CompareTag("Wall"))
        {
            Debug.Log("Hit wall");
        }
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();

        targetTransform.transform.position = new Vector3(Random.Range(-9f, 9f), targetTransform.transform.position.y, Random.Range(-9f, 9f));
    }
}
