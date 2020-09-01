using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeCounter : MonoBehaviour
{

    [SerializeField] public TextMeshProUGUI timeText = default;
    public float countdown;

    private int minites;
    private int seconds;
    private int mseconds;
    GameManager gamemanager;
    EnemyAI enemy;

    // Start is called before the first frame update
    void Start()
    {
        gamemanager = GameObject.Find("GameManager").GetComponent<GameManager>();
        enemy = GameObject.Find("Enemy").GetComponent<EnemyAI>();
        countdown = enemy.timenow;

    }

    // Update is called once per frame
    void Update()
    {

      if(gamemanager.game_stop_flg == false){
         // countdown -= Time.deltaTime;
         countdown = 180f - enemy.timenow;

         if(countdown <= 0f){
             countdown = 0.0f;
             gamemanager.GameOver();
         }

         minites = Mathf.FloorToInt(countdown / 60F);

         seconds = Mathf.FloorToInt(countdown - minites * 60);

         mseconds = Mathf.FloorToInt((countdown - minites * 60 - seconds) * 100);
       }

         //Debug.Log(minites);

         timeText.text = "Time : " + string.Format("{0:00}:{1:00}:{2:00}", minites, seconds, mseconds);
   }

}
