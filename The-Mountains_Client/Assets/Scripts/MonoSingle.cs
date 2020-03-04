using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingle : MonoBehaviour
{
    private static MonoSingle instance;
    public static MonoSingle Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }
}
