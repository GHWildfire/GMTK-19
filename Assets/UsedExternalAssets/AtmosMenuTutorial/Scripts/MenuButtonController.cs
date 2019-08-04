using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage menu buttons
/// Improve and adapt this file from : https://github.com/atmosgames/unityMenuTutorial
/// 
/// Author : Alexander Wohlfahrt
/// Version : 0.1
/// </summary>
public class MenuButtonController : MonoBehaviour {
    
    public AudioSource ButtonAudioSource;
	[SerializeField] private int maxIndex = 0;
    [SerializeField] private bool isControlledOnX = false;

    public int Index
    {
        get
        {
            return index;
        }
        set
        {
            if (index > maxIndex)
            {
                index = 0;
            }
            else if (index < 0)
            {
                index = maxIndex;
            }

            index = value;
        }

    }

    private const float NEEDED_TIME = 0.2f;
    private const float OFFSET = 0.15f;

    private int index;

    private float elapsedTime;

    private string controlledAxis;

    // Update is called once per frame
    private void Update ()
    {
        elapsedTime += Time.deltaTime;

        if (isControlledOnX)
        {
            controlledAxis = "Horizontal";
        }
        else
        {
            controlledAxis = "Vertical";
        }

        float controllerSpeed = Input.GetAxisRaw(controlledAxis);
        if (Mathf.Abs(controllerSpeed) > OFFSET)
        {
            if (elapsedTime > NEEDED_TIME)
            {
                elapsedTime = 0;

                if (controllerSpeed < -OFFSET)
                {
					if(index < maxIndex)
                    {
                        index++;
					}
                    else
                    {
                        index = 0;
					}
				}
                else if(controllerSpeed > OFFSET)
                {
					if(index > 0)
                    {
                        index--; 
					}
                    else
                    {
                        index = maxIndex;
					}
				}
			}
		}
	}
}
