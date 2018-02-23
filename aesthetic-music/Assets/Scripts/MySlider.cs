using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MySlider : Slider
{

    public void MySet(float value, bool callback)
    {
        Set(value, callback);
    }
}

