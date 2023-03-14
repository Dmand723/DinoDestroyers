using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class SliderScript : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI sliderValue;




    // Start is called before the first frame update
    void Start()
    {
        slider.SetValueWithoutNotify(PlayerPrefs.GetInt("roundtimer", 100));
        sliderValue.text = slider.value.ToString();
        slider.onValueChanged.AddListener((v) =>
        {
            sliderValue.text = v.ToString();
            PlayerPrefs.SetInt("roundtimer", (int)v);
        });
    }

    // Update is called once per frame
    void Update()
    {
        print(PlayerPrefs.GetInt("roundtimer"));
    }
}
