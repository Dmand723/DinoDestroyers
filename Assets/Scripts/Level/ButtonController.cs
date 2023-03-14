using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ButtonController : MonoBehaviour
{

    public GameObject panel;
    public TextMeshProUGUI optionsText;
    public bool panelActve;
    


    // Start is called before the first frame update
    void Start()
    {
        
        panel.SetActive(false);
        panelActve = false;
        optionsText.text = "Options";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void onOptionsClick()
    {
        panelActve = !panelActve;
        panel.SetActive(panelActve);
        if (panelActve)
        {
            optionsText.text = "Close";
        }
        if (!panelActve)
        {
            optionsText.text = "Options";
        }
        
    }
}
