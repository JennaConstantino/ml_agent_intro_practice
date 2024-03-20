using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
//MaxStep Slider: how long an agent episode will last, period of time agent can gain rewards
public class BallBalanceAgent : Agent
{
    // Agent abstract contains 5 definitions to override (Initialize, CollectObservations, OnActionReceived, Heuristic, OnEpisodeBegin)

    public GameObject ball; // Assigned in inspector
    Rigidbody ballRigidBody;
    EnvironmentParameters defaultParameters; // Access to dataset on the Academy, sets and gets default size of ball

    public override void Initialize() // Similar to start
    { 
        ballRigidBody = ball.GetComponent<ballRigidBody>(); // Gets rb of ball and sets
        defaultParameters = Academy.Instance.EnvironmentParameters;
        ResetScene();
    }

    public override void CollectObservations(VectorSensor sensor) // Send observations to academy, what observations the agent needs to collect
    {
        sensor.AddObservation(ballRigidBody.velocity); // Tells academy to observe values in this vector3
        sensor.AddObservation(ballRigidBody.transform.position);
        sensor.AddObservation(transform.rotation.z);
        sensor.AddObservation(transform.rotation.x);
    }

    //public override void OnActionReceived(float[] vectorAction) // Get action from Academy
    //{
        // Telling to only rotate if it hasn't rotated more than 25 deg in either direction in either axis
    //    var z_angle = 2f * Mathf.Clamp(vectorAction[0], -1f, 1f);
    //    var x_angle = 2f * Mathf.Clamp(vectorAction[1], -1f, 1f);

    //    if ((gameObject.transform.rotation.z < 0.25f && z_angle > 0f) || (gameObject.transform.rotation.z > -0.25f && z_angle < 0f))
    //    {
    //        gameObject.transform.Rotate(new Vector3(0, 0, 1), z_angle);
    //    }
    //    if ((gameObject.transform.rotation.x < 0.25f && x_angle > 0f) || (gameObject.transform.rotation.x > -0.25f && x_angle < 0f))
    //    {
    //        gameObject.transform.Rotate(new Vector3(1, 0, 0), x_angle);
    //    }

        // Checks if ball has fallen off agent by subtracting transforms
        // If balls is on, small reward each frame
        // If ball is off, subtract entire point

    //    if ((ball.transform.position.y - gameObject.transform.position.y) < -2.5f || Mathf.Abs(ball.transform.position.x - gameObject.transform.position.x) > 3f || Mathf.Abs(ball.transform.position.z - gameObject.transform.position.z) > 3f)
    //    {
    //        SetReward(-1f);
    //        EndEpisode();
    //    }
    //    else
    //    {
    //        SetReward(0.1f);
    //    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Extract continuous actions from the actions buffer
        var continuousActions = actions.ContinuousActions;

        // Telling to only rotate if it hasn't rotated more than 25 deg in either direction in either axis
        var z_angle = 2f * Mathf.Clamp(continuousActions[0], -1f, 1f);
        var x_angle = 2f * Mathf.Clamp(continuousActions[1], -1f, 1f);

        if ((gameObject.transform.rotation.z < 0.25f && z_angle > 0f) || (gameObject.transform.rotation.z > -0.25f && z_angle < 0f))
        {
            gameObject.transform.Rotate(new Vector3(0, 0, 1), z_angle);
        }
        if ((gameObject.transform.rotation.x < 0.25f && x_angle > 0f) || (gameObject.transform.rotation.x > -0.25f && x_angle < 0f))
        {
            gameObject.transform.Rotate(new Vector3(1, 0, 0), x_angle);
        }

        // Checks if ball has fallen off agent by subtracting transforms
        // If balls is on, small reward each frame
        // If ball is off, subtract entire point

        if ((ball.transform.position.y - gameObject.transform.position.y) < -2.5f || Mathf.Abs(ball.transform.position.x - gameObject.transform.position.x) > 3f || Mathf.Abs(ball.transform.position.z - gameObject.transform.position.z) > 3f)
        {
            SetReward(-1f);
            EndEpisode();
        }
        else
        {
            SetReward(0.1f);
        }
    }


    //}

    //public override void Heuristic(float[] actionsOut) // Agent runs whatever is in this method when set to heuristic
    //{
    //    actionsOut[0] = -Input.GetAxis("Horizontal");
    //    actionsOut[1] = Input.GetAxis("Vertical");
    //}
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Extract the continuous actions buffer from actionsOut
        var continuousActions = actionsOut.ContinuousActions;

        // Populate the continuousActions buffer with heuristic actions
        continuousActions[0] = -Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }



    public override void OnEpisodeBegin()
    {
        gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        gameObject.transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
        gameObject.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));
        ball.Rigidbody.velocity = new Vector3(0f, 0f, 0f);
        ball.transform.position = new Vector3(Random.Range(-1.5f, 1.5f), 5f, Random.Range(-1.5f, 1.5f)) + gameObject.transform.position;
        ResetScene();
    }

    void ResetScene() // To end and restart episode if ball falls off
    {
        ballRigidBody.mass = defaultParameters.GetWithDefault("mass", 1.0f); // Sets mass to default
        var scale = defaultParameters.GetWithDefault("scale", 1.0f);
        ball.transform.localScale = new Vector3(scale, scale, scale); // Sets scale of ball to default

        // Gets value from Academys environment parameters
    }
}
