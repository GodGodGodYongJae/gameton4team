using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    public float speed;

    void Start()
    {
        Invoke("DestoryArrow", 3);
    }

    void Update()
    {
        transform.Translate(transform.right * -1f * speed * Time.deltaTime);
    }

}
