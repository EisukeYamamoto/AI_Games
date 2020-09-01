using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpManager : MonoBehaviour
{
    GameManager gamemanager;
    public GameObject warpzone1;
    public GameObject warpzone2;
    public GameObject warpzone3;
    public GameObject warpzone4;
    public GameObject warpzone5;
    public GameObject warpzone6;
    public GameObject warpzone7;
    public GameObject warpzone8;

    [SerializeField] public bool training;

    public GameObject player;
    private PlayerControll playercontroll;
    private Player_AI player_AI;

    public GameObject Enemy;
    private EnemyAI enemy_AI;
    public GameObject Enemy_2;
    private Enemy_2AI enemy_2AI;
    public GameObject Enemy_3;
    private Enemy_3AI enemy_3AI;

    private Vector3 _x_forward  = new Vector3(1f, 0f, 0f);
    private Vector3 _x_back  = new Vector3(-1f, 0f, 0f);
    private Vector3 _z_forward  = new Vector3(0f, 0f, 1f);
    private Vector3 _z_back  = new Vector3(0f, 0f, -1f);
    private float _Offset = 2f;
    // Start is called before the first frame update
    void Start()
    {
          gamemanager = GameObject.Find("GameManager").GetComponent<GameManager>();
          warpzone1 = GameObject.Find("warpzone1");
          warpzone2 = GameObject.Find("warpzone2");
          warpzone3 = GameObject.Find("warpzone3");
          warpzone4 = GameObject.Find("warpzone4");
          warpzone5 = GameObject.Find("warpzone5");
          warpzone6 = GameObject.Find("warpzone6");
          warpzone7 = GameObject.Find("warpzone7");
          warpzone8 = GameObject.Find("warpzone8");

          if (!training){
              //player = GameObject.Find("Player");
              playercontroll = player.GetComponent<PlayerControll>();
          }
          else {
              //player = GameObject.Find("Player_AI");
              player_AI = player.GetComponent<Player_AI>();
          }
          //Enemy = GameObject.Find("Enemy");
          enemy_AI = Enemy.GetComponent<EnemyAI>();
          enemy_2AI = Enemy_2.GetComponent<Enemy_2AI>();
          enemy_3AI = Enemy_3.GetComponent<Enemy_3AI>();
    }

    // Update is called once per frame
    void Update()
    {
          if (gamemanager.game_stop_flg == false){
               //Debug.Log(warpzone1.transform.localPosition);
               //Debug.Log(warpzone.transform.localPosition);
               if (!training){
                  switch (playercontroll.warpflag){
                       case 0:
                            break;
                       case 1:
                            // 衝突した面の、接触した点における法線を取得
                            player.transform.localPosition = warpzone7.transform.localPosition + _z_back * _Offset;
                            playercontroll.warpflag = 0;
                            break;
                       case 2:
                            player.transform.localPosition = warpzone8.transform.localPosition + _z_forward * _Offset;
                            playercontroll.warpflag = 0;
                            break;
                       case 3:
                            player.transform.localPosition = warpzone6.transform.localPosition + _x_back * _Offset;
                            playercontroll.warpflag = 0;
                            break;
                       case 4:
                            player.transform.localPosition = warpzone5.transform.localPosition + _x_forward * _Offset;
                            playercontroll.warpflag = 0;
                            break;
                       case 5:
                            player.transform.localPosition = warpzone4.transform.localPosition + _x_back * _Offset;
                            playercontroll.warpflag = 0;
                            break;
                       case 6:
                            player.transform.localPosition = warpzone3.transform.localPosition + _x_forward * _Offset;
                            playercontroll.warpflag = 0;
                            break;
                       case 7:
                            player.transform.localPosition = warpzone1.transform.localPosition + _z_forward * _Offset;
                            playercontroll.warpflag = 0;
                            break;
                       case 8:
                            player.transform.localPosition = warpzone2.transform.localPosition + _z_back * _Offset;
                            playercontroll.warpflag = 0;
                            break;
                       default:
                            break;
                    }
               }
               else{
                   switch (player_AI.warpflag){
                      case 0:
                           break;
                      case 1:
                           // 衝突した面の、接触した点における法線を取得
                           player.transform.localPosition = warpzone7.transform.localPosition + _z_back * _Offset;
                           player_AI.warpflag = 0;
                           break;
                      case 2:
                           player.transform.localPosition = warpzone8.transform.localPosition + _z_forward * _Offset;
                           player_AI.warpflag = 0;
                           break;
                      case 3:
                           player.transform.localPosition = warpzone6.transform.localPosition + _x_back * _Offset;
                           player_AI.warpflag = 0;
                           break;
                      case 4:
                           player.transform.localPosition = warpzone5.transform.localPosition + _x_forward * _Offset;
                           player_AI.warpflag = 0;
                           break;
                      case 5:
                           player.transform.localPosition = warpzone4.transform.localPosition + _x_back * _Offset;
                           player_AI.warpflag = 0;
                           break;
                      case 6:
                           player.transform.localPosition = warpzone3.transform.localPosition + _x_forward * _Offset;
                           player_AI.warpflag = 0;
                           break;
                      case 7:
                           player.transform.localPosition = warpzone1.transform.localPosition + _z_forward * _Offset;
                           player_AI.warpflag = 0;
                           break;
                      case 8:
                           player.transform.localPosition = warpzone2.transform.localPosition + _z_back * _Offset;
                           player_AI.warpflag = 0;
                           break;
                      default:
                           break;
                   }
               }
               switch (enemy_AI.enemy_warpflag){
                    case 0:
                       break;
                    case 1:
                       // 衝突した面の、接触した点における法線を取得
                       Enemy.transform.localPosition = warpzone7.transform.localPosition + _z_back * _Offset;
                       enemy_AI.enemy_warpflag = 0;
                       break;
                    case 2:
                       Enemy.transform.localPosition = warpzone8.transform.localPosition + _z_forward * _Offset;
                       enemy_AI.enemy_warpflag = 0;
                       break;
                    case 3:
                       Enemy.transform.localPosition = warpzone6.transform.localPosition + _x_back * _Offset;
                       enemy_AI.enemy_warpflag = 0;
                       break;
                    case 4:
                       Enemy.transform.localPosition = warpzone5.transform.localPosition + _x_forward * _Offset;
                       enemy_AI.enemy_warpflag = 0;
                       break;
                    case 5:
                       Enemy.transform.localPosition = warpzone4.transform.localPosition + _x_back * _Offset;
                       enemy_AI.enemy_warpflag = 0;
                       break;
                    case 6:
                       Enemy.transform.localPosition = warpzone3.transform.localPosition + _x_forward * _Offset;
                       enemy_AI.enemy_warpflag = 0;
                       break;
                    case 7:
                       Enemy.transform.localPosition = warpzone1.transform.localPosition + _z_forward * _Offset;
                       enemy_AI.enemy_warpflag = 0;
                       break;
                    case 8:
                       Enemy.transform.localPosition = warpzone2.transform.localPosition + _z_back * _Offset;
                       enemy_AI.enemy_warpflag = 0;
                       break;
                    default:
                       break;
               }
               switch (enemy_2AI.enemy_2_warpflag){
                    case 0:
                       break;
                    case 1:
                       // 衝突した面の、接触した点における法線を取得
                       Enemy_2.transform.localPosition = warpzone7.transform.localPosition + _z_back * _Offset;
                       enemy_2AI.enemy_2_warpflag = 0;
                       break;
                    case 2:
                       Enemy_2.transform.localPosition = warpzone8.transform.localPosition + _z_forward * _Offset;
                       enemy_2AI.enemy_2_warpflag = 0;
                       break;
                    case 3:
                       Enemy_2.transform.localPosition = warpzone6.transform.localPosition + _x_back * _Offset;
                       enemy_2AI.enemy_2_warpflag = 0;
                       break;
                    case 4:
                       Enemy_2.transform.localPosition = warpzone5.transform.localPosition + _x_forward * _Offset;
                       enemy_2AI.enemy_2_warpflag = 0;
                       break;
                    case 5:
                       Enemy_2.transform.localPosition = warpzone4.transform.localPosition + _x_back * _Offset;
                       enemy_2AI.enemy_2_warpflag = 0;
                       break;
                    case 6:
                       Enemy_2.transform.localPosition = warpzone3.transform.localPosition + _x_forward * _Offset;
                       enemy_2AI.enemy_2_warpflag = 0;
                       break;
                    case 7:
                       Enemy_2.transform.localPosition = warpzone1.transform.localPosition + _z_forward * _Offset;
                       enemy_2AI.enemy_2_warpflag = 0;
                       break;
                    case 8:
                       Enemy_2.transform.localPosition = warpzone2.transform.localPosition + _z_back * _Offset;
                       enemy_2AI.enemy_2_warpflag = 0;
                       break;
                    default:
                       break;
               }
               switch (enemy_3AI.enemy_3_warpflag){
                    case 0:
                       break;
                    case 1:
                       // 衝突した面の、接触した点における法線を取得
                       Enemy_3.transform.localPosition = warpzone7.transform.localPosition + _z_back * _Offset;
                       enemy_3AI.enemy_3_warpflag = 0;
                       break;
                    case 2:
                       Enemy_3.transform.localPosition = warpzone8.transform.localPosition + _z_forward * _Offset;
                       enemy_3AI.enemy_3_warpflag = 0;
                       break;
                    case 3:
                       Enemy_3.transform.localPosition = warpzone6.transform.localPosition + _x_back * _Offset;
                       enemy_3AI.enemy_3_warpflag = 0;
                       break;
                    case 4:
                       Enemy_3.transform.localPosition = warpzone5.transform.localPosition + _x_forward * _Offset;
                       enemy_3AI.enemy_3_warpflag = 0;
                       break;
                    case 5:
                       Enemy_3.transform.localPosition = warpzone4.transform.localPosition + _x_back * _Offset;
                       enemy_3AI.enemy_3_warpflag = 0;
                       break;
                    case 6:
                       Enemy_3.transform.localPosition = warpzone3.transform.localPosition + _x_forward * _Offset;
                       enemy_3AI.enemy_3_warpflag = 0;
                       break;
                    case 7:
                       Enemy_3.transform.localPosition = warpzone1.transform.localPosition + _z_forward * _Offset;
                       enemy_3AI.enemy_3_warpflag = 0;
                       break;
                    case 8:
                       Enemy_3.transform.localPosition = warpzone2.transform.localPosition + _z_back * _Offset;
                       enemy_3AI.enemy_3_warpflag = 0;
                       break;
                    default:
                       break;
               }
          }
          else {

          }
    }
}
