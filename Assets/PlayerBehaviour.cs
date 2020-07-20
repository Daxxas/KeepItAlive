using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyNameSpace;

public class PlayerBehaviour : MonoBehaviour
{
    private PlayerMovementScript mvtScript;
    public LayerMask whatIsEnemy;
    public GameObject gmo;
    private GameManager gm;
    public CameraShake camShake;
    
    // Start is called before the first frame update
    void Start()
    {
        gm = gmo.GetComponent<GameManager>();
        mvtScript = GetComponent<PlayerMovementScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -9)
        {
            gm.GameLost();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((whatIsEnemy & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            other.GetComponent<EnemyBehaviour>().Death();
            StartCoroutine(camShake.Shake(0.1f, .4f));
        }
    }

}
