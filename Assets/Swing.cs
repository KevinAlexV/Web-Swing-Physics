using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour
{
    [SerializeField]
    public Pendulum pendulum;

    void Start()
    {
        pendulum.Initialize();
    }

    void Update()
    {
        transform.localPosition = pendulum.moveChar(transform.localPosition, Time.deltaTime);


    }

}
