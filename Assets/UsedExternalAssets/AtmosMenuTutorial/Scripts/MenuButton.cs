using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Manage the menu behaviour.
/// Improve and adapt this file from : https://github.com/atmosgames/unityMenuTutorial
/// 
/// Author : Alexander Wohlfahrt
/// Version : 0.1
/// </summary>
public class MenuButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField] private MenuButtonController menuButtonController = null;
	[SerializeField] private Animator animator = null;
	[SerializeField] private AnimatorFunctions animatorFunctions = null;
	[SerializeField] private int thisIndex = 0;
    [SerializeField] private UnityEvent clickEvent = null;

    private bool isPointerDown;

    public void OnPointerClick(PointerEventData eventData)
    {
        isPointerDown = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        menuButtonController.Index = thisIndex;
    }

    private void OnEnable()
    {
        isPointerDown = false;
    }

    // Update is called once per frame
    void Update()
    {
		if(menuButtonController.Index == thisIndex)
		{
			animator.SetBool ("selected", true);

			if(Input.GetAxisRaw("Submit") == 1 ||
                isPointerDown)
            {
                if (clickEvent != null &&
                    !animator.GetBool("pressed"))
                {
                    clickEvent.Invoke();
                }

                animator.SetBool("pressed", true);
            }
            else if (animator.GetBool ("pressed"))
            {
                animator.SetBool ("pressed", false);
				animatorFunctions.disableOnce = true;
			}
		}
        else
        {
            animator.SetBool ("selected", false);
            animator.SetBool("pressed", false);
        }

        isPointerDown = false;
    }
}
