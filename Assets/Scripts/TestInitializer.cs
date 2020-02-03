using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInitializer : MonoBehaviour
{
    private PlanManager pm;

    // Start is called before the first frame update
    private void Awake()
    {
        pm = GetComponent<PlanManager>();
        pm.Setup(2, new TestLevelConfig());
   
    }

    private void Start()
    { 
        pm.Begin();
    }
}
