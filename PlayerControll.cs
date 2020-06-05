using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float moveSpeed = 10.0f;        // 移動速度
    [SerializeField] private float applySpeed = 0.2f;       // 振り向きの適用速度
    //[SerializeField] private PlayerFollowCamera refCamera = default;   //カメラ

    private Rigidbody _rigidBody;     //リジッドボディ
    public Vector3 velocity_copy;
    private Animator anim;            //アニメーション

    ///    ジャンプ入力フラグ
    ///    ジャンプ入力が一度でもあったらON、着地したらOFF
    private bool _jumpInput = false;

    ///    ジャンプ処理中フラグ
    ///    ジャンプ処理が開始されたらON、着地したらOFF
    private bool _isJumping = false;


    ///    接地してから何フレーム経過したか
    ///    接地してない間は常にゼロとする
    private int _isGround = 0;

    ///    接地してない間、何フレーム経過したか
    ///    接地している間は常にゼロとする
    private int _notGround = 0;

    ///    このフレーム数分接地していたらor接地していなかったら
    ///    状態が変わったと認識する（ジャンプ開始したor着地した）
    ///    接地してからキャラの状態が安定するまでに数フレーム用するため、
    ///    キャラが安定する前に再ジャンプ入力を受け付けてしまうとバグる（ジャンプ出来なくなる）
    ///    筆者PCでは 3 で安定するが、安全をとって今回は 5 とした
    private const int _isGroundStateChange = 5;

    ///    プレイヤーと地面の間の距離
    ///    IsGround()が呼ばれるたびに更新される
    [SerializeField] private float _groundDistance = 0f;
    [SerializeField] private float _floorDistance = 0f;

    ///    _groundDistanceがこの値以下の場合接地していると判定する
    private const float _groundDistanceLimit = 0.8f;

    ///    判定元の原点が地面に極端に近いとrayがヒットしない場合があるので、
    ///    オフセットを設けて確実にヒットするようにする
    private Vector3 _raycastOffset  = new Vector3(0f, 0.05f, 0f);

    ///    プレイヤーキャラから下向きに地面判定のrayを飛ばす時の上限距離
    ///    ゲーム中でプレイヤーキャラと地面が最も離れると考えられる場面の距離に、
    ///    マージンを多少付けた値にしておくのが良
    ///    Mathf.Infinityを指定すれば無制限も可能だが重くなる可能性があるかも？
    private const float _raycastSearchDistance = 100f;

    GameManager gamemanager;

    // Start is called before the first frame update
    void Start()
    {
          anim = GetComponent<Animator>();
          _rigidBody = GetComponent<Rigidbody>();
          velocity = Vector3.zero;
          velocity_copy = velocity;
          gamemanager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
          if (gamemanager.game_stop_flg == false){
               velocity = Vector3.zero;
               if(Input.GetKey(KeyCode.W)){
                     anim.SetBool("Walk",true);
                     velocity.z += 1;
               }
               if(Input.GetKey(KeyCode.S)){
                     anim.SetBool("Walk",true);
                     velocity.z -= 1;
               }
               if(Input.GetKey(KeyCode.D)){
                     anim.SetBool("Walk",true);
                     velocity.x += 1;
               }
               if(Input.GetKey(KeyCode.A)){
                     anim.SetBool("Walk",true);
                     velocity.x -= 1;
               }
               else{
                     anim.SetBool("Walk",false);
               }

          }
    }
}
