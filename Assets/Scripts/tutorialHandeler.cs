using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tutorialHandeler : MonoBehaviour
{
    public GameObject mainFrame;

    public Image tutorial1;
    public Image tutorial2;
    public Image tutorial3;

    public GameObject leftButton;
    public GameObject righttButton;

    private void Tutorial(int number)
    {
        switch (number)
        {
            case 1:
                mainFrame.GetComponent<Image>().sprite = tutorial1.sprite;
                break;
            case 2:
                mainFrame.GetComponent<Image>().sprite = tutorial2.sprite;
                break;
            case 3:
                mainFrame.GetComponent<Image>().sprite = tutorial3.sprite;
                break;
        }
    }

    public void NextImage()
    {
        var imageMainFrame = mainFrame.GetComponent<Image>().sprite;

        if (imageMainFrame == tutorial1.sprite)
        {
            Tutorial(2);
        } else if (imageMainFrame == tutorial2.sprite)
        {
            Tutorial(3);
        } else if (imageMainFrame == tutorial3.sprite)
        {
            Tutorial(1);
        }
    }
    
    public void PreviousImage()
        {
            var imageMainFrame = mainFrame.GetComponent<Image>().sprite;
    
            if (imageMainFrame == tutorial1.sprite)
            {
                Tutorial(3);
            } else if (imageMainFrame == tutorial2.sprite)
            {
                Tutorial(1);
            } else if (imageMainFrame == tutorial3.sprite)
            {
                Tutorial(2);
            }
        }
}
