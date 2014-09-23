using UnityEngine;
using System.Collections;
using MinMVC;

public class ChildViewMediator : Mediator<IChildView>  
{
    public override void OnRegister()
    {
        base.OnRegister();
        _view.OnButton += HandleOnButton;
    }

    void HandleOnButton (string name)
    {
        Debug.Log("CLICK "+name);
    }
}
