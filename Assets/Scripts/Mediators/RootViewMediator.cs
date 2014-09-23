using UnityEngine;
using System.Collections;
using MinMVC;
using UnityEngine.UI;

public class RootViewMediator : Mediator<IRootView> 
{
    public override void OnRegister ()
    {
        base.OnRegister ();

        var resource = Resources.Load("SimpleButton");

        for(var i = 0; i < 10; i++ )
        {
            var go = (GameObject)GameObject.Instantiate( resource );
            var button = go.GetComponentInChildren<Text>(); 
            button.text = i.ToString();
            _view.AddChildToCanvas( go );
        }
    }
}
