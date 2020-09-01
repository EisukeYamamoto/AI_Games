using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyGo_script : MonoBehaviour
{

    [SerializeField]
    float waitTime = 1.5f;
    [SerializeField]
    Text readyGoText = default;
    // [SerializeField]
    // GameObject Audio_Go = default;

    GameManager gameManager;

      // Start is called before the first frame update
      void Start()
      {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        StartCoroutine(ReadyGo());

      }

      // Update is called once per frame
      void Update()
      {

      }

      IEnumerator ReadyGo()
      {
          yield return new WaitForEndOfFrame();

          //プレイヤーを停止させる
          gameManager.game_stop_flg = true;
          gameManager.pause_flg = false;

          //yield return new WaitForSeconds(waitTime);

          readyGoText.text = "Ready?";

          yield return new WaitForSeconds(waitTime);

          readyGoText.text = "GO!!";
          // Instantiate(Audio_Go, transform.position, transform.rotation);

          //プレイヤーを移動可能にさせる
          gameManager.game_stop_flg = false;
          gameManager.pause_flg = true;


          yield return new WaitForSeconds(waitTime);

          readyGoText.enabled = false;

      }
}
