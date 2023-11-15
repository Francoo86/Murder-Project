using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING {
    public class TestingArchitect : MonoBehaviour
    {
        DialogController dc;
        TextArchitect arch;

        string[] lines = new string[5] {
            "Hola me presento",
            "Lorem Ipsum bla bla",
            "Testeando un texto gigante porque se puede pasar este ramo",
            "Unity solos",
            "Speedrunneando esta cosa"
        }; 

        // Start is called before the first frame update
        void Start()
        {
            dc = DialogController.Instance;
            arch = new TextArchitect(dc.dialog.dialogText);
            arch.buildMethod = TextArchitect.BuildMethod.typewriter;
        }

        // Update is called once per frame
        void Update()
        {
            // Debug.Log(string.Format("Is pressing A Key: {0}", Input.GetKey(KeyCode.A)));
            if (Input.GetKeyDown("space")) {
                arch.Build(lines[UnityEngine.Random.Range(0, lines.Length)]);
            }
            else if (Input.GetKeyDown("a"))
            {
                arch.Append(lines[UnityEngine.Random.Range(0, lines.Length)]);
            }
        }
    }
}
