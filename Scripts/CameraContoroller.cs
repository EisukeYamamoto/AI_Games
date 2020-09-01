using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraContoroller : MonoBehaviour
{
    [SerializeField] private float turnSpeed = 5.0f;   // 回転速度
    [SerializeField] private float applySpeed = 0.6f;       // 振り向きの適用速度
    [SerializeField] private float maxangle = 50;
    [SerializeField] private float minangle = 320;
    [SerializeField] private Transform player = default;          // 注視対象プレイヤー

    [SerializeField] private float distance = 1.0f;    // 注視対象プレイヤーからカメラを離す距離
    [SerializeField] public Quaternion vRotation;      // カメラの垂直回転(見下ろし回転)
    [SerializeField] public  Quaternion hRotation;      // カメラの水平回転
    [SerializeField] private PlayerControll p_motion = default;

    private float mouseY;
    private float playerangle;
    private float cameraangle;
    private float cameraangle_x;
    private float push_r_angle;
    private float push_r_angle_x;
    private float rotatespeed;
    private float rotatespeed_x;
    private float rot_x;
    private float camRayLength = 2f;
    private float hit_distance_now;
    public bool rotateflg;
    private Vector3 targetPosition;
    int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
    GameManager gamemanager;

    // Start is called before the first frame update
    void Start()
    {
      // 回転の初期化
      rotateflg = false;
      vRotation = Quaternion.Euler(10, 0, 0);         // 垂直回転(X軸を軸とする回転)は、30度見下ろす回転
      hRotation = Quaternion.Euler(0,player.transform.localEulerAngles.y, 0);                // 水平回転(Y軸を軸とする回転)は、無回転
      transform.rotation = hRotation * vRotation;     // 最終的なカメラの回転は、垂直回転してから水平回転する合成回転
      floorMask = LayerMask.GetMask("Wall");
      gamemanager = GameObject.Find("GameManager").GetComponent<GameManager>();
      //p_motion = gameObject.GetComponent<PlayMotion>();

      // 位置の初期化
        // player位置から距離distanceだけ手前に引いた位置を設定します
        transform.localPosition = player.localPosition + new Vector3(0, 1.5f, 0) -
                             transform.rotation * Vector3.forward * distance;
    }

    // Update is called once per frame
    void Update()
    {
         //Debug.Log(gamemanager.game_stop_flg);
         if (gamemanager.game_stop_flg == false){
              mouseY = Input.GetAxis("Mouse Y");
              rot_x = transform.localEulerAngles.x;
              if (rotateflg == false){
                   if (Input.GetMouseButton(1)){
                        hRotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * turnSpeed, 0);
                        if (transform.localEulerAngles.x > minangle || transform.localEulerAngles.x < maxangle){
                             vRotation *= Quaternion.Euler(-mouseY * turnSpeed, 0, 0);
                             //Debug.Log(transform.localEulerAngles.x);
                        }
                        else if (transform.localEulerAngles.x <= minangle && transform.localEulerAngles.x >= 180){
                             vRotation *= Quaternion.Euler(0.5f, 0 ,0);
                        }
                        else if(transform.localEulerAngles.x >= maxangle && transform.localEulerAngles.x < 180){
                             vRotation *= Quaternion.Euler(-0.5f, 0 ,0);
                        }
                        //vRotation *= Quaternion.Euler(-mouseY * turnSpeed, 0, 0);
                   }
                   if (Input.GetKeyDown(KeyCode.R)){   // すぐに背後へ向く
                        rotateflg = true;
                        playerangle = player.transform.localEulerAngles.y;  //プレイヤーの向き
                   }
                   // //Debug.Log(transform.localEulerAngles.x);
             }
             else {
                   cameraangle = transform.localEulerAngles.y - 180;  //カメラの向き
                   cameraangle_x = transform.localEulerAngles.x;
                   if (cameraangle_x >= 180){
                        cameraangle_x = cameraangle_x - 360;
                   }
                   push_r_angle = playerangle + 180 - cameraangle;  //プレイヤーとカメラの向きの差
                   push_r_angle_x = 10 - cameraangle_x;
                   //Debug.Log(push_r_angle_x);
                   //補正
                   if (push_r_angle >= 180){
                        push_r_angle -= 360;
                   }
                   if (push_r_angle < -180){
                        push_r_angle += 360;
                   }
                   //振り向き
                   if (push_r_angle < 0){
                        rotatespeed = -14.0f;
                   }
                   if (push_r_angle > 0){
                        rotatespeed = 14.0f;
                   }
                   //振り向き
                   if (push_r_angle_x < 0){
                        rotatespeed_x = -7.0f;
                   }
                   if (push_r_angle_x > 0){
                        rotatespeed_x = 7.0f;
                   }

                   if (Mathf.Abs(push_r_angle) < 20){
                        if (push_r_angle < 0){
                             rotatespeed = -1.5f;
                        }
                        if (push_r_angle >= 0){
                             rotatespeed = 1.5f;
                        }
                   }

                   if (Mathf.Abs(push_r_angle_x) < 10){
                        if (push_r_angle_x < 0){
                             rotatespeed_x = -0.75f;
                        }
                        if (push_r_angle_x >= 0){
                             rotatespeed_x = 0.75f;
                        }
                        //Debug.Log("<20");
                   }

                   hRotation *= Quaternion.Euler(0, rotatespeed* applySpeed, 0);
                   vRotation *= Quaternion.Euler(rotatespeed_x*applySpeed, 0, 0);

                   if (Mathf.Abs(push_r_angle) < 5 && Mathf.Abs(push_r_angle_x) < 2){
                        rotateflg = false;
                        playerangle = 0;
                        cameraangle = 0;
                        cameraangle_x = 0;
                        push_r_angle = 0;
                        push_r_angle_x = 0;
                        //Debug.Log("<5");
                   }
                   rotatespeed = 0;
                   rotatespeed_x = 0;

             }

             // カメラとマウスの位置を元にRayを準備
             // Ray camRay = new Ray(player.position, transform.TransformDirection(Vector3.forward));
             // Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.yellow);
             // Physics.queriesHitBackfaces = true;

             RaycastHit floorHit;
             targetPosition = new Vector3(player.localPosition.x, player.localPosition.y + 0.5f, player.localPosition.z);
             //
             if (Physics.Raycast (targetPosition, transform.localPosition - targetPosition, out floorHit, Vector3.Distance(targetPosition, transform.localPosition), floorMask, QueryTriggerInteraction.Ignore))
             {
                  // // Debug.Log(floorHit.distance);
                  // //レイを可視化
                  // hit_distance_now = floorHit.distance;
                  // //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * floorHit.distance, Color.yellow);
                  //
                  // if (p_motion.velocity_copy.z <= 0){
                  //      distance -= hit_distance_now;
                  // }
                  transform.localPosition = floorHit.point;

             }
             if (distance < 2.0f && p_motion.velocity_copy.z > 0){
                  distance += p_motion.velocity_copy.magnitude;
             }


        }
        else{

        }
    }

    private void FixedUpdate(){
            // カメラの回転(transform.rotation)の更新
            // 方法1 : 垂直回転してから水平回転する合成回転とします
            transform.rotation = hRotation * vRotation;
            // Debug.Log(player.localPosition);

            // カメラの位置(transform.position)の更新
            // player位置から距離distanceだけ手前に引いた位置を設定します(位置補正版)
            //transform.position = player.position - transform.rotation * Vector3.forward * distance;
            transform.position = player.localPosition + new Vector3(0, 1.0f, 0) -
                                 transform.rotation * Vector3.forward * distance;

    }


}
