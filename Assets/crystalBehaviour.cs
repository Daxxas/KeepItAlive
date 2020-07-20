using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crystalBehaviour : MonoBehaviour
{
    public GameObject gmo;
    private GameManager gm;
    public LayerMask whatIsEnemy;
    
    // Start is called before the first frame update
    void Start()
    {
        gm = gmo.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((whatIsEnemy & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            gm.GameLost();
        }
    }
}
