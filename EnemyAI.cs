using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class EnemyAI : Agent
{
     [SerializeField] private Vector3 velocity;
     [SerializeField] private float moveSpeed = 10.0f;        // 移動速度
     [SerializeField] private float applySpeed = 0.2f;       // 振り向きの適用速度
     private Rigidbody _rigidBody;     //リジッドボディ
     public Transform Target;

     // スタート時に呼ばれる
     public override void Initialize()
     {
         _rigidBody = GetComponent<Rigidbody>();
     }

     // エピソード開始時に呼ばれる
     public override void OnEpisodeBegin()
     {
        // RollerAgentの落下時
        if (this.transform.localPosition.y < 0)
        {
            // RollerAgentの位置と速度をリセット
            this._rigidBody.angularVelocity = Vector3.zero;
            this._rigidBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3( 0, 0.5f, 0);
            
        }

        // Targetの位置のリセット
        // Target.localPosition = new Vector3(Random.value * 8 - 4,
        //                                 0.5f,
        //                                 Random.value * 8 - 4);
     }

     // 状態取得時に呼ばれる
     public override void CollectObservations(VectorSensor sensor)
     {
       // Target and Agent positions
       sensor.AddObservation(Target.localPosition);
       sensor.AddObservation(this.transform.localPosition);

       // Agent velocity
       sensor.AddObservation(velocity.x);
       sensor.AddObservation(velocity.z);
     }

     // 行動実行時に呼ばれる
     public override void OnActionReceived(float[] vectorAction)
     {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        _rigidBody.AddForce(controlSignal * moveSpeed);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Reached target
        if (distanceToTarget < 1.0f)
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

}
