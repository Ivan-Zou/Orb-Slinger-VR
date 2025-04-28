using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ActionBasedControllerSubClass : ActionBasedController
{
    [Header("Custom Actions")]
    public InputActionReference pauseAction;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (pauseAction?.action != null)
        {
            pauseAction.action.Enable();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (pauseAction?.action != null)
        {
            pauseAction.action.Disable();
        }
    }
}
