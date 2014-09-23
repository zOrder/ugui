using UnityEngine;
using System.Collections;
using MinMVC;

public delegate void OnButtonClick( string name );
public interface IChildView : IMediatedView 
{
    event OnButtonClick OnButton;
}
