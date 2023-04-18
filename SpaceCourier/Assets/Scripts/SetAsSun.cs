using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAsSun : MonoBehaviour {
    
    public Light ownLight;

    void Start() {

        RenderSettings.sun = ownLight;
    }
}
