using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public Transform player;
    Vector3 offset;


    void Start()
    {
        offset = transform.position - player.position;

    }

    void Update()
    {
        Vector3 targetpos = player.position + offset;
        targetpos.x = 0;
        targetpos.y = 4;
        transform.position = targetpos;
    }
}
