using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenScript : MonoBehaviour {
    GameObject camera;
    AudioUtility script;
    Renderer rend;

    void Start () {
        camera = GameObject.Find("ARCamera");
        script = camera.GetComponent<AudioUtility>();
        rend = GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Custom/circleC");
    }

	void Update () {
        float vol = script.dbTemp;
        rend.material.SetFloat("_A", vol);

        int res = script.int_result;
        if (res == 0 || res == 1)
        {
            rend.enabled = false;
        }
        else if (res == 2)
        {
            rend.enabled = true;
        }
    }
}
