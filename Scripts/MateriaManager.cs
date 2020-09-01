using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MateriaManager : MonoBehaviour
{

    [Header("Set Materia Prefab")]
    //マテリアプレハブ
    public GameObject MateriaPrefab;
    public Transform Player;
    GameObject materia;
    GameManager gamemanager;

    public int materia_step;
    private float distanceToMateria;
    // Start is called before the first frame update
    void Start()
    {
        gamemanager = GameObject.Find("GameManager").GetComponent<GameManager>();
        materia = Instantiate(MateriaPrefab);
        materia.transform.position = new Vector3(42f, 1f, -42f);
        materia_step = 0;
        distanceToMateria = Vector3.Distance(Player.transform.position, materia.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (gamemanager.game_stop_flg == false){
            distanceToMateria = Vector3.Distance(Player.transform.position, materia.transform.position);
            // Debug.Log(distanceToMateria);
            if (distanceToMateria < 0.8f){
                switch(materia_step){
                    case 0:
                        materia.transform.position = new Vector3(-42f, 1f, 42f);
                        break;
                    case 1:
                        materia.transform.position = new Vector3(-42f, 1f, -42f);
                        break;
                    case 2:
                        materia.transform.position = new Vector3(42f, 1f, 42f);
                        break;
                    case 3:
                        materia.transform.position = new Vector3(0f, 10.5f, 0f);
                        break;
                    default:
                        break;
                }
                materia_step += 1;
            }

            if (materia_step >= 5){
                gamemanager.GameClear();
            }
        }

    }
}
