using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private GameObject TargetGameObject;
    private Transform target;
    private GameObject gmo;
    private GameManager gm;
    public ParticleSystem deathParticle;

    [SerializeField] private float movespeed;
    // Start is called before the first frame update
    void Start()
    {
        gmo = GameObject.FindGameObjectWithTag("gameManager");
        gm = gmo.GetComponent<GameManager>();
        TargetGameObject = GameObject.FindGameObjectWithTag("Player");
        target = TargetGameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, movespeed * Time.deltaTime);
    }

    public void Death()
    {
        Instantiate(deathParticle, transform.position, Quaternion.identity);
        gm.AddScore(100);
        Destroy(this.gameObject);
    }
}
