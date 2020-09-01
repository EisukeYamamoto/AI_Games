using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerControll : MonoBehaviour
{
    [SerializeField] public Vector3 velocity;
    [SerializeField] private float moveSpeed = 10.0f;        // 移動速度
    [SerializeField] private float applySpeed = 0.2f;       // 振り向きの適用速度
    [SerializeField] private CameraContoroller refCamera = default;   //カメラ

    private Rigidbody _rigidBody;     //リジッドボディ
    public Vector3 velocity_copy;
    private Vector3 normalVector = Vector3.zero;
    // private Vector3 onPlane = Vector3.zero;
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
    private const int _isGroundStateChange = 50;

    ///    プレイヤーと地面の間の距離
    ///    IsGround()が呼ばれるたびに更新される
    [SerializeField] private float _groundDistance = 0f;
    [SerializeField] private float _floorDistance = 0f;

    ///    _groundDistanceがこの値以下の場合接地していると判定する
    private const float _groundDistanceLimit = 0.5f;

    ///    判定元の原点が地面に極端に近いとrayがヒットしない場合があるので、
    ///    オフセットを設けて確実にヒットするようにする
    private Vector3 _raycastOffset  = new Vector3(0f, 0.05f, 0f);

    ///    プレイヤーキャラから下向きに地面判定のrayを飛ばす時の上限距離
    ///    ゲーム中でプレイヤーキャラと地面が最も離れると考えられる場面の距離に、
    ///    マージンを多少付けた値にしておくのが良
    ///    Mathf.Infinityを指定すれば無制限も可能だが重くなる可能性があるかも？
    private const float _raycastSearchDistance = 100f;
    private float moveSpeed_init;


    GameManager gamemanager;
    public int warpflag = 0;

    // Start is called before the first frame update
    void Start()
    {
          anim = GetComponent<Animator>();
          _rigidBody = GetComponent<Rigidbody>();
          velocity = Vector3.zero;
          velocity_copy = velocity;
          gamemanager = GameObject.Find("GameManager").GetComponent<GameManager>();
          moveSpeed_init = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
          if (gamemanager.game_stop_flg == false){
               velocity = Vector3.zero;
               if(Input.GetKey(KeyCode.W)){
                    //anim.SetBool("Walk",true);
                    velocity.z += 1;
               }
               if(Input.GetKey(KeyCode.S)){
                    //anim.SetBool("Walk",true);
                    velocity.z -= 1;
               }
               if(Input.GetKey(KeyCode.D)){
                    //anim.SetBool("Walk",true);
                    velocity.x += 1;
               }
               if(Input.GetKey(KeyCode.A)){
                    //anim.SetBool("Walk",true);
                    velocity.x -= 1;
               }
               // else{
               //      transform.rotation = Quaternion.Euler(0, 0, 0);
               // }


               if (velocity.magnitude > 0 && !_isJumping){
                    if (Input.GetMouseButton(0)){
                         moveSpeed = moveSpeed_init * 3;
                         anim.SetBool("Run",true);
                    }
                    else{
                         moveSpeed = moveSpeed_init;
                         anim.SetBool("Run",false);

                    }
               }

               CheckGroundDistance(() => {
               _jumpInput = false;
               _isJumping = false;
               });


               // 既にジャンプ入力が行われていたら、ジャンプ入力チェックを飛ばす
               if (_jumpInput || JumpInput()) _jumpInput = true;

               Debug.DrawRay(
               transform.position + _raycastOffset,
               transform.TransformDirection(Vector3.forward)* 100, Color.red, 0.5f, false);



          }
    }

    private void FixedUpdate(){
      if (gamemanager.game_stop_flg == false){
          // 速度ベクトルの長さを1秒でmoveSpeedだけ進むように調整します
          velocity = velocity.normalized * moveSpeed * Time.deltaTime;
          velocity_copy = velocity;
          //Debug.Log(climing);
          //Debug.Log(normalVector);
          if (velocity.magnitude > 0 && refCamera.rotateflg == false){
               // プレイヤーの回転(transform.rotation)の更新
               // 無回転状態のプレイヤーのZ+方向(後頭部)を、移動の反対方向(-velocity)に回す回転とします
               //transform.rotation = Quaternion.LookRotation(-velocity);
               anim.SetBool("Walk",true);
               transform.rotation = Quaternion.Slerp(transform.rotation,
                                    Quaternion.LookRotation(refCamera.hRotation * velocity),
                                    applySpeed);
               // CheckFloorDistance();
               // if (floorHitflg == true){
               //      velocity = Vector3.zero;
               // }
               transform.position += refCamera.hRotation * velocity;

               //Debug.Log(velocity);
           }
           else{
               anim.SetBool("Walk",false);
               anim.SetBool("Run",false);
           }

           if (_jumpInput) {
                if (!_isJumping) {
                     _isJumping = true;
                     DoJump();

                }
           }
        }
    }

    ///    ジャンプ入力チェック
    private bool JumpInput()
    {
           // ジャンプ最速入力のテスト用にGetButton
           //if (Input.GetButton("Jump")) return true;    // ジャンプキー押しっぱなしで連続ジャンプ
           //if (Input.GetButtonDown("Jump")) return true;    // ジャンプキーが押された時だけジャンプにする時はこっち
           if (Input.GetKeyDown(KeyCode.Space)) {
                //Debug.Log("jump");
                return true;
           }
           return false;
    }

    ///    ジャンプの強さ
    [SerializeField] private float jumpPower = 100f;
    //private const float jumpPower = 5f;

    ///    ジャンプのための上方向への加圧
    private void DoJump()
    {
           _rigidBody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
           anim.SetTrigger("Jump_up");
           anim.SetBool("Jump_down", false);

    }



    ///    接地判定
    private void CheckGroundDistance(UnityAction landingAction = null, UnityAction takeOffAction = null)
    {
           RaycastHit hit;
           var layerMask = LayerMask.GetMask("Ground");

           // プレイヤーの位置から下向きにRaycast
           // レイヤーマスクでGroundを設定しているので、
           // 地面のGameObjectにGroundのレイヤーを設定しておけば、
           // Groundのレイヤーを持つGameObjectで一番近いものが一つだけヒットする
           var isGroundHit = Physics.Raycast(
                             transform.position + _raycastOffset,
                             transform.TransformDirection(Vector3.down),
                             out hit,
                             _raycastSearchDistance,
                             layerMask
                             );

           if (isGroundHit) {
                _groundDistance = hit.distance;
           } else {
                // ヒットしなかった場合はキャラの下方に地面が存在しないものとして扱う
                _groundDistance = float.MaxValue;
           }
           // Debug.Log(_groundDistance);

           // 地面とキャラの距離は環境によって様々で
           // 完全にゼロにはならない時もあるため、
           // ジャンプしていない時の値に多少のマージンをのせた
           // 一定値以下を接地と判定する
           // 通常あり得ないと思われるが、オーバーフローされると再度アクションが実行されてしまうので、越えたところで止める
           if (_groundDistance < _groundDistanceLimit) {
                if (_isGround <= _isGroundStateChange) {
                     _isGround += 1;
                     _notGround = 0;

                }
                anim.SetBool("Jump_down", true);
                anim.SetBool("Jumping", false);

           } else {
                if (_notGround <= _isGroundStateChange) {
                     _isGround = 0;
                     _notGround += 1;

                }
                anim.SetBool("Jump_down", false);
                anim.SetBool("Jumping", true);


           }

           // 接地後またはジャンプ後、特定フレーム分状態の変化が無ければ、
           // 状態が安定したものとして接地処理またはジャンプ処理を行う
           if (_isGroundStateChange == _isGround && _notGround == 0) {
                if (landingAction != null){
                     landingAction();

                     //Debug.Log("landing");
                }
           } else {
                if (_isGroundStateChange == _notGround && _isGround == 0) {
                     if (takeOffAction != null){
                          takeOffAction();
                          //Debug.Log("takeOFF");
                     }
                }
           }
      }


      private void OnCollisionEnter(Collision collision)
      {
           // Debug.Log(collision.gameObject.name);
           switch (collision.gameObject.name)
           {
               case "warpzone1":
                   Debug.Log("1");
                   warpflag = 1;
                   break;
               case "warpzone2" :
                   Debug.Log("2");
                   warpflag = 2;
                   break;
               case "warpzone3" :
                   Debug.Log("3");
                   warpflag = 3;
                   break;
               case "warpzone4" :
                   Debug.Log("4");
                   warpflag = 4;
                   break;
               case "warpzone5" :
                   Debug.Log("5");
                   warpflag = 5;
                   break;
               case "warpzone6" :
                   Debug.Log("6");
                   warpflag = 6;
                   break;
               case "warpzone7" :
                   Debug.Log("7");
                   warpflag = 7;
                   break;
               case "warpzone8" :
                   Debug.Log("8");
                   warpflag = 8;
                   break;
               default:
                   // Debug.Log("else");
                   warpflag = 0;
                   break;
           }
           if (collision.gameObject.tag == "Enemy"){
               gamemanager.GameOver();
           }

      }


      // private void OnCollisionExit(Collision collision)
      // {
      //     //Debug.Log(collision);
      //     if (collision.gameObject.CompareTag("Wall")){
      //       climing = false;
      //     }
      //
      // }
}
