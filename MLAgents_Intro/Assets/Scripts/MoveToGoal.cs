using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoal : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material lossMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;




    public override void OnEpisodeBegin() {

        float valueX = Random.Range(-3.5f, 3.5f);
        float valueZ = Random.Range(-3.5f, 3.5f);
        transform.localPosition = new Vector3(valueX, 0, valueZ);

        Vector3 targetPos;

        do
        {
            targetPos = new Vector3(Random.Range(-3.5f, 3.5f), 0, Random.Range(-3.5f, 3.5f));
        }
        while (Vector3.Distance(targetPos, transform.localPosition) < 2f);
        

        targetTransform.localPosition = targetPos;
    }
    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions) {

        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 10f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;


    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");


    }

    private void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent<Goal>(out Goal goal)) {
            Debug.Log("goal");
            SetReward(1f);
            floorMeshRenderer.sharedMaterial = winMaterial;
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall)) {
            Debug.Log("wall");
            floorMeshRenderer.sharedMaterial = lossMaterial;
            SetReward(-1f);
            EndEpisode();
        }
    }

}
