using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;


public class RollerAgent : Agent
{
    Rigidbody rBody;


    // スタート時に呼ばれる
    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public Transform Target;

    // エピソード開始時に呼ばれる
    public override void OnEpisodeBegin()
    {
       // RollerAgentの落下時
       if (this.transform.localPosition.y < 0)
       {
           // RollerAgentの位置と速度をリセット
           this.rBody.angularVelocity = Vector3.zero;
           this.rBody.velocity = Vector3.zero;
           this.transform.localPosition = new Vector3( 0, 0.5f, 0);
       }

       // Targetの位置のリセット
       Target.localPosition = new Vector3(Random.value * 8 - 4,
                                       0.5f,
                                       Random.value * 8 - 4);
    }

    // 状態取得時に呼ばれる
    public override void CollectObservations(VectorSensor sensor)
    {
      // Target and Agent positions
      sensor.AddObservation(Target.localPosition);
      sensor.AddObservation(this.transform.localPosition);

      // Agent velocity
      sensor.AddObservation(rBody.velocity.x);
      sensor.AddObservation(rBody.velocity.z);
    }

    public float speed = 10;
    // 行動実行時に呼ばれる
    public override void OnActionReceived(float[] vectorAction)
    {
       // Actions, size = 2
       Vector3 controlSignal = Vector3.zero;
       controlSignal.x = vectorAction[0];
       controlSignal.z = vectorAction[1];
       rBody.AddForce(controlSignal * speed);

       // Rewards
       float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

       // Reached target
       if (distanceToTarget < 1.42f)
       {
          SetReward(1.0f);
          EndEpisode();
       }

       // Fell off platform
       if (this.transform.localPosition.y < 0)
       {
          EndEpisode();
       }
    }

    public override void Heuristic(float[] actionsOut)
    {
       actionsOut[0] = Input.GetAxis("Horizontal");
       actionsOut[1] = Input.GetAxis("Vertical");
    }


}
