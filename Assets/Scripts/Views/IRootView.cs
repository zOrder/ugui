using UnityEngine;
using System.Collections;
using MinMVC;

public interface IRootView : IMediatedView
{
    void AddChildToCanvas( GameObject go );
}
