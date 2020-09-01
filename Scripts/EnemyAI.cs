using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class EnemyAI : Agent
{
     [SerializeField] public Vector3 velocity;
     [SerializeField] private float moveSpeed = 10.0f;        // 移動速度
     [SerializeField] private float applySpeed = 0.2f;       // 振り向きの適用速度
     private Animator anim;            //アニメーション
     private Rigidbody _rigidBody;     //リジッドボディ
     public Transform Target;
     public Transform Enemy_3;
     public Transform Enemy_2;
     public Transform Floor;
     public Transform MainField;
     // public Player_AI player_AI;
     public Enemy_3AI enemy_3;
     public Enemy_2AI enemy_2;
     public PlayerControll player_AI;
     public float timelimit = 15000f;
     public float timenow;
     private float distanceToTarget_before;
     private float distanceToTarget_before_par15;
     private float this_y_before;
     private float Floor_X;
     private float Floor_Z;
     private Vector3 targetPosition;
     private Vector3 enemy_pos_before;
     GameManager gamemanager;
     int floorMask;

     public int enemy_warpflag = 0;
     public bool gamesetflag = false;

     // スタート時に呼ばれる
     public override void Initialize()
     {
         _rigidBody = GetComponent<Rigidbody>();
         anim = GetComponent<Animator>();
         gamemanager = GameObject.Find("GameManager").GetComponent<GameManager>();
         enemy_3 = Enemy_3.GetComponent<Enemy_3AI>();
         enemy_2 = Enemy_2.GetComponent<Enemy_2AI>();
         // player_AI = Target.GetComponent<Player_AI>();
         player_AI = Target.GetComponent<PlayerControll>();
         timenow = 0f;
         floorMask = LayerMask.GetMask("Wall");
         Floor_X = Floor.localScale.x * MainField.localScale.x - 10f * MainField.localScale.x;
         Floor_Z = Floor.localScale.z * MainField.localScale.z - 10f * MainField.localScale.z;
     }

     // エピソード開始時に呼ばれる
     public override void OnEpisodeBegin()
     {
        // RollerAgentの落下時
        if (this.transform.localPosition.y < 0 || timelimit < timenow || gamesetflag == true || enemy_2.gamesetflag == true || enemy_3.gamesetflag == true)
        {
            // RollerAgentの位置と速度をリセット
            this._rigidBody.angularVelocity = Vector3.zero;
            this._rigidBody.velocity = Vector3.zero;
            // this.transform.localPosition = new Vector3( 0.0f, 0.5f, 0.0f);
            this.transform.localPosition = new Vector3(0f, 15f, 42f * MainField.localScale.z);
            enemy_2.transform.localPosition = new Vector3(42f * MainField.localScale.x, 15f, 0f);
            enemy_3.transform.localPosition = new Vector3(-42f * MainField.localScale.x, 15f, 0f);

            timenow = 0f;
            gamesetflag = false;
            enemy_2.gamesetflag = false;
            enemy_3.gamesetflag = false;
            // Debug.Log("Reset!");

        }

        // Targetの位置のリセット
        // Target.localPosition = new Vector3(Random.value * Floor_X - Floor_X/2f,
        //                                 15f,
        //                                 Random.value * Floor_Z - Floor_Z/2f);
        Target.localPosition = new Vector3(0f, 15f, -20f * MainField.localScale.z);
        distanceToTarget_before_par15 = Vector3.Distance(this.transform.localPosition, Target.localPosition);
        SetReward(0.0f);
     }

     // 状態取得時に呼ばれる
     public override void CollectObservations(VectorSensor sensor)
     {
       // // Target and Agent positions
       sensor.AddObservation(Target.localPosition);
       sensor.AddObservation(this.transform.localPosition);
       sensor.AddObservation(this.transform.localRotation);
       sensor.AddObservation(Enemy_2.localPosition);
       sensor.AddObservation(Enemy_3.localPosition);
       //
       // // Agent velocity
       sensor.AddObservation(velocity.x);
       // sensor.AddObservation(velocity.y);
       sensor.AddObservation(velocity.z);
       sensor.AddObservation(enemy_2.velocity.x);
       // sensor.AddObservation(velocity.y);
       sensor.AddObservation(enemy_2.velocity.z);
       sensor.AddObservation(enemy_3.velocity.x);
       // sensor.AddObservation(velocity.y);
       sensor.AddObservation(enemy_3.velocity.z);

       sensor.AddObservation(player_AI.velocity.x);
       sensor.AddObservation(player_AI.velocity.z);
       // sensor.AddObservation(player.velocity.x);
       // sensor.AddObservation(player.velocity.z);

     }

     // 行動実行時に呼ばれる
     public override void OnActionReceived(float[] vectorAction)
     {
       if (gamemanager.game_stop_flg == false){
         // Actions, size = 2
         Vector3 controlSignal = Vector3.zero;
         controlSignal.x = vectorAction[0];
         controlSignal.z = vectorAction[1];
         //Debug.Log(controlSignal);
         velocity = controlSignal * moveSpeed * Time.deltaTime;
         //Debug.Log(vectorAction[0]);
         //_rigidBody.AddForce(controlSignal * moveSpeed);


         timenow += Time.deltaTime;

         // Rewards
         float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

         if ( distanceToTarget_before > distanceToTarget){
           AddReward(0.01f);
         }
         else {
           AddReward(-0.01f);
         }
         // }

         if ( this.transform.localPosition.y < Target.localPosition.y ){
           if (this_y_before < this.transform.localPosition.y){
             AddReward(0.03f);
           }
         }

         if (Vector3.Distance(this.transform.localPosition, enemy_pos_before) < 0.8f){
           AddReward(-0.01f);
         }

         if (timenow % 15f == 0){
           if (distanceToTarget - distanceToTarget_before_par15 > -8.0f){
             AddReward(-0.5f);
           }
           distanceToTarget_before_par15 = distanceToTarget;
         }

         distanceToTarget_before = distanceToTarget;
         this_y_before = this.transform.localPosition.y;
         enemy_pos_before = this.transform.localPosition;


         // Fell off platform
         if (this.transform.localPosition.y < 0)
         {
           //timenow = 0f;
           Debug.Log("Enemyの落下");
           SetReward(-50.0f);
           this.transform.localPosition = new Vector3(0f, 15f, 0f);
           // EndEpisode();
           // gamesetflag = true;
         }

         if (gamesetflag == false && distanceToTarget < 0.7f){
           AddReward(30.0f);
           Debug.Log("Enemyの勝ち！ : " + timenow);
           gamesetflag = true;
           // timenow = 0f;
           //EndEpisode();
         }

         if (gamesetflag == false && (enemy_3.gamesetflag == true || enemy_2.gamesetflag == true)){
           AddReward(5.0f);
           gamesetflag = true;
           //EndEpisode();
         }

         if (timelimit + 0.005f < timenow) {
           AddReward(-5.0f);
           Debug.Log("時間切れ");
           // timenow = 0f;
           // EndEpisode();
           // gamesetflag = true;
           EndEpisode();
         }

         if (gamesetflag == true && enemy_2.gamesetflag == true && enemy_3.gamesetflag == true){
           EndEpisode();
         }
       }
     }

     private void FixedUpdate(){
       if (gamemanager.game_stop_flg == false){
         if (velocity.magnitude > 0) {
           anim.SetBool("Run",true);
           this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation,
           Quaternion.LookRotation(velocity),
           applySpeed);
           this.transform.localPosition += velocity;
         }
         else{
           anim.SetBool("Run",false);
         }
         AddReward(-0.05f);
       }
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
                  Debug.Log("1");
                  enemy_warpflag = 1;
                  break;
              case "warpzone2" :
                  Debug.Log("2");
                  enemy_warpflag = 2;
                  break;
              case "warpzone3" :
                  Debug.Log("3");
                  enemy_warpflag = 3;
                  break;
              case "warpzone4" :
                  Debug.Log("4");
                  enemy_warpflag = 4;
                  break;
              case "warpzone5" :
                  Debug.Log("5");
                  enemy_warpflag = 5;
                  break;
              case "warpzone6" :
                  Debug.Log("6");
                  enemy_warpflag = 6;
                  break;
              case "warpzone7" :
                  Debug.Log("7");
                  enemy_warpflag = 7;
                  break;
              case "warpzone8" :
                  Debug.Log("8");
                  enemy_warpflag = 8;
                  break;
              default:
                  //Debug.Log("else");
                  enemy_warpflag = 0;
                  break;
          }

          if (collision.gameObject.CompareTag("Wall")){
               SetReward(-0.3f);
          }

     }

     private void OnCollisionStay(Collision collision){
          if (collision.gameObject.CompareTag("Wall")){
               AddReward(-0.8f);
          }
     }

}
