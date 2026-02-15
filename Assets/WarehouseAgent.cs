using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class WarehouseAgent : Agent
{
    public Transform targetTransform;
    public float rollForce = 20f;
    private Rigidbody rb;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // Reset physics
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        // Reset Agent position
        transform.localPosition = new Vector3(0, 0.5f, 0);

        // Move Target to random position
        targetTransform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 3 Observations: Relative position to target
        sensor.AddObservation(targetTransform.localPosition - transform.localPosition);
        // 3 Observations: Current velocity
        sensor.AddObservation(rb.linearVelocity);
        // Total = 6
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int action = actions.DiscreteActions[0];
        Vector3 forceDir = Vector3.zero;

        if (action == 1) forceDir = Vector3.forward;
        else if (action == 2) forceDir = Vector3.back;
        else if (action == 3) forceDir = Vector3.right;
        else if (action == 4) forceDir = Vector3.left;

        rb.AddForce(forceDir * rollForce);

        // Fell off platform
        if (transform.localPosition.y < -1.0f)
        {
            AddReward(-1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = 0; // Idle

        if (Input.GetKey(KeyCode.W)) discreteActions[0] = 1;
        else if (Input.GetKey(KeyCode.S)) discreteActions[0] = 2;
        else if (Input.GetKey(KeyCode.D)) discreteActions[0] = 3;
        else if (Input.GetKey(KeyCode.A)) discreteActions[0] = 4;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            AddReward(1.0f);
            EndEpisode();
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.1f);
        }
    }
}