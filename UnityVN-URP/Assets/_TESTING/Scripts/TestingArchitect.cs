using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING {
    public class TestingArchitect : MonoBehaviour
    {
        DialogController dc;
        TextArchitect arch;

        public TextArchitect.BuildMethod bm = TextArchitect.BuildMethod.instant;

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
            arch = new TextArchitect(dc.dialogContainer.dialogText);
            arch.buildMethod = TextArchitect.BuildMethod.fade;
        }

        // Update is called once per frame
        void Update()
        {
            /*
            if (bm != arch.buildMethod) {
                arch.buildMethod = bm;
                arch.Stop();
            }

            if (Input.GetKeyDown("s"))
                arch.Stop();

            // Debug.Log(string.Format("Is pressing A Key: {0}", Input.GetKey(KeyCode.A)));
            if (Input.GetKeyDown("k")) {
                //arch.Build(lines[UnityEngine.Random.Range(0, lines.Length)]);
                if (arch.isBuilding)
                {
                    if (!arch.shouldSpeedUp)
                    {
                        arch.shouldSpeedUp = true;
                    }
                    else
                        arch.ForceComplete();
                }
                else
                    arch.Build(lines[UnityEngine.Random.Range(0, lines.Length)]);
            }
            else if (Input.GetKeyDown("a"))
            {
                arch.Append(lines[UnityEngine.Random.Range(0, lines.Length)]);
            }*/
        }
    }
}
