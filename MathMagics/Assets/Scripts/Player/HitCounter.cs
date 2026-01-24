using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCounter : MonoBehaviour
{
    public static HitCounter Instance;

    
    [SerializeField] private int numHits;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddOne()
    {
        numHits++;
    }

    public int GetNumHits()
    {
        return numHits;
    }
}
