using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class Player_AI : Agent
{
     [SerializeField] public Vector3 velocity;
     [SerializeField] private float moveSpeed = 10.0f;        // 移動速度
     [SerializeField] private float applySpeed = 0.2f;       // 振り向きの適用速度
     private Animator anim;            //アニメーション
     private Rigidbody _rigidBody;     //リジッドボディ
     public Transform Target;
     public Transform Target2;
     public Transform Target3;
     public Transform Floor;
     public EnemyAI enemy;
     public Enemy_2AI enemy_2;
     public Enemy_3AI enemy_3;
     private float timenow;
     private float timelimit;
     private float distanceToTarget_before;
     private float distanceToTarget2_before;
     private float distanceToTarget3_before;
     private float Floor_X;
     private float Floor_Z;

     public int warpflag = 0;

     // スタート時に呼ばれる
     public override void Initialize()
     {
         _rigidBody = GetComponent<Rigidbody>();
         anim = GetComponent<Animator>();
         enemy = Target.GetComponent<EnemyAI>();
         enemy_2 = Target2.GetComponent<Enemy_2AI>();
         enemy_3 = Target3.GetComponent<Enemy_3AI>();
         timenow = enemy.timenow;
         timelimit = enemy.timelimit;
         Floor_X = Floor.localScale.x - 4;
         Floor_Z = Floor.localScale.z - 4;

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
            //timenow = 0f;
        }
        SetReward(0.0f);

        // Targetの位置のリセット
        // Target.localPosition = new Vector3(Random.value * Floor_X - Floor_X/2,
        //                                 0.5f,
        //                                 Random.value * Floor_Z - Floor_Z/2);
     }

     // 状態取得時に呼ばれる
     public override void CollectObservations(VectorSensor sensor)
     {
       // // Target and Agent positions
       sensor.AddObservation(Target.localPosition);
       sensor.AddObservation(Target2.localPosition);
       sensor.AddObservation(Target3.localPosition);
       sensor.AddObservation(this.transform.localPosition);
       sensor.AddObservation(this.transform.localRotation);
       //
       // // Agent velocity
       sensor.AddObservation(velocity.x);
       sensor.AddObservation(velocity.z);
       sensor.AddObservation(enemy.velocity.x);
       sensor.AddObservation(enemy.velocity.z);
       sensor.AddObservation(enemy_2.velocity.x);
       sensor.AddObservation(enemy_2.velocity.z);
       sensor.AddObservation(enemy_3.velocity.x);
       sensor.AddObservation(enemy_3.velocity.z);
     }

     // 行動実行時に呼ばれる
     public override void OnActionReceived(float[] vectorAction)
     {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        // Debug.Log(controlSignal);
        velocity = controlSignal * moveSpeed * Time.deltaTime;
        //Debug.Log(vectorAction[0]);
        //_rigidBody.AddForce(controlSignal * moveSpeed);


        timenow = enemy.timenow;

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);
        float distanceToTarget2 = Vector3.Distance(this.transform.localPosition, Target2.localPosition);
        float distanceToTarget3 = Vector3.Distance(this.transform.localPosition, Target3.localPosition);
        //SetReward(0.0f);

        if ( distanceToTarget_before  > distanceToTarget ||
             distanceToTarget2_before > distanceToTarget2 ||
             distanceToTarget3_before > distanceToTarget3){
               AddReward(0.03f);
        }





        // SetReward(0.1f);
        // if ( distanceToTarget_before > distanceToTarget){
        //   SetReward(-0.05f);
        // }
        // else {
        //   SetReward(0.05f);
        // }
        // if (distanceToTarget < 1.0f)
        // {
        //   SetReward(-1.0f);
        //   //timenow = 0f;
        //   EndEpisode();
        // }

        distanceToTarget_before = distanceToTarget;
        distanceToTarget2_before = distanceToTarget2;
        distanceToTarget3_before = distanceToTarget3;
        if (timelimit < enemy.timenow) {
          AddReward(15.0f);
          Debug.Log("Playerの勝ち");
          // timenow = 0f;
          EndEpisode();
        }

        if (distanceToTarget < 0.7f || distanceToTarget2 < 0.7f || distanceToTarget3 < 0.7f){
           SetReward(-30.0f);
           //Debug.Log("Enemyの勝ち！");
           //gamesetflag = true;
           // timenow = 0f;
           EndEpisode();
        }

        // Fell off platform
        if (this.transform.localPosition.y < 0)
        {
           //timenow = 0f;
           Debug.Log("Playerの落下");
           SetReward(-1.0f);
           EndEpisode();
        }
        if (this.transform.localPosition.y < 8.0f){
           AddReward(0.1f);
        }
     }

     private void FixedUpdate(){
       if (velocity.magnitude > 0) {
            anim.SetBool("Run",true);
            this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation,
                                 Quaternion.LookRotation(velocity),
                                 applySpeed);
            // CheckFloorDistance();
            // if (floorHitflg == true){
            //      velocity = Vector3.zero;
            // }

            //transform.position += transform.rotation * velocity;
            this.transform.localPosition += velocity;
       }
       else{
           anim.SetBool("Run",false);
       }
       AddReward(0.001f);
       // Debug.Log("Player" + timenow);
       // Debug.Log("transformあり" + Target.transform.localPosition);
       // Debug.Log("transform無し" + Target.localPosition);
       // Debug.Log("Enemy" + this.transform.localPosition);
       // Debug.Log("距離" + Vector3.Distance(this.transform.localPosition, Target.localPosition));


     }

     public override void Heuristic(float[] actionsOut)
     {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
     }
     private void OnCollisionEnter(Collision collision)
     {
          // Debug.Log(collision.gameObject.name);
          switch (collision.gameObject.name)
          {
              case "warpzone1":
                  //Debug.Log("1");
                  warpflag = 1;
                  break;
              case "warpzone2" :
                  //Debug.Log("2");
                  warpflag = 2;
                  break;
              case "warpzone3" :
                  //Debug.Log("3");
                  warpflag = 3;
                  break;
              case "warpzone4" :
                  //Debug.Log("4");
                  warpflag = 4;
                  break;
              case "warpzone5" :
                  //Debug.Log("5");
                  warpflag = 5;
                  break;
              case "warpzone6" :
                  //Debug.Log("6");
                  warpflag = 6;
                  break;
              case "warpzone7" :
                  //Debug.Log("7");
                  warpflag = 7;
                  break;
              case "warpzone8" :
                  //Debug.Log("8");
                  warpflag = 8;
                  break;
              default:
                  //Debug.Log("else");
                  warpflag = 0;
                  break;
          }
          if (collision.gameObject.tag == "Wall"){
               AddReward(-0.3f);
          }
          // else if (collision.gameObject.CompareTag("Enemy")){
          //      SetReward(-5.0f);
          //      // timenow = 0f;
          //      EndEpisode();
          // }

     }

     private void OnCollisionStay(Collision collision){
          if (collision.gameObject.CompareTag("Wall")){
               AddReward(-0.8f);
          }
          // else if (collision.gameObject.CompareTag("Enemy")){
          //      AddReward(-5.0f);
          //      // timenow = 0f;
          //      EndEpisode();
          // }
     }

}
