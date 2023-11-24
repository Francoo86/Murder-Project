using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //CommandController.Instance.Execute("test_moving", "right");
        //CommandController.Instance.Execute("Printear", "xdd");
        //CommandController.Instance.Execute("");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) {
            CommandController.Instance.Execute("test_moving", "left");
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            CommandController.Instance.Execute("test_moving", "right");
        }
    }


}
