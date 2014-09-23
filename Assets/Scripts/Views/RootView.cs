using UnityEngine;
using System.Collections;
using MinMVC;
using UnityEngine.UI;

public class RootView : MediatedView, IRootView 
{	
    [SerializeField] VerticalLayoutGroup group;
    [SerializeField] BaconGroup baconGroup;

    override protected void Start () 
	{
		IInjector injector = new Injector();

        injector.Register<IMediators, Mediators>();
        injector.Register<ICommands, Commands>();

        var mediators = injector.Get<IMediators>();
        mediators.Map<IRootView, RootViewMediator>();
        mediators.Map<IChildView, ChildViewMediator>();

        mediators.Mediate( this );		
	}

    public void AddChildToCanvas( GameObject go )
    {
        go.transform.parent = baconGroup.transform;
    }
}
