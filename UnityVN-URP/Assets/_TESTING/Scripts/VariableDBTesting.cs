using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableDBTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        VariableStore.CreateDatabase("DB1");
        VariableStore.CreateDatabase("DB6");
        VariableStore.CreateDatabase("DB5");

        VariableStore.CreateVariable("DB6.num1", 1);

        VariableStore.PrintAllVariables();

        VariableStore.PrintAllDatabases();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
