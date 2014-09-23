using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BaconGroup : HorizontalOrVerticalLayoutGroup  
{
    public override void CalculateLayoutInputHorizontal ()
    {
        base.CalculateLayoutInputHorizontal ();
        CalculateAlongAxis( 0, true );
    }

    public override void CalculateLayoutInputVertical()
    {
        CalculateAlongAxis( 1, true );
    }
    public override void SetLayoutHorizontal()
    {
        SetBaconChildrenAlongAxis( 0, true );
    }

    public override void SetLayoutVertical()
    {
        SetBaconChildrenAlongAxis(1, true);
    }

    protected void CalculateAlongAxis( int axis, bool isVertical )
    {
        var num = ( float )( ( axis != 0 ) ? base.padding.vertical : base.padding.horizontal);                
        var flag = isVertical ^ axis == 1;
        for( int i = 0; i < base.rectChildren.Count; i++ )
        {
            RectTransform rect = base.rectChildren[ i ];
            var preferredSize = LayoutUtility.GetPreferredSize( rect, axis );
            if( flag )
            {
                num = Mathf.Max(preferredSize + num, num);
            }
            else
            {
                num += preferredSize + this.spacing;             
            }
        }
        if( !flag && base.rectChildren.Count > 0 )
        {
            num -= this.spacing;
        }
     
        base.SetLayoutInputForAxis( num, num, num, axis );
    }

    protected void SetBaconChildrenAlongAxis( int axis, bool isVertical )
    {
        var flag = isVertical ^ axis == 1;
        RectTransform rect;
        if( flag )
        {
            for( int i = 0; i < base.rectChildren.Count; i++ )
            {
                rect = base.rectChildren[ i ];
                var offset = rectTransform.sizeDelta.x / 2 - rect.sizeDelta.x / 2;
                var minSize = LayoutUtility.GetPreferredSize( rect, axis );
                
                base.SetChildAlongAxis(rect, axis, offset, minSize); // width
            }
        }
        else
        {
            var num4 = ( float )( ( axis != 0 ) ? base.padding.top : base.padding.left );
            for( int j = 0; j < base.rectChildren.Count; j++ )
            {
                rect = base.rectChildren[ j ];
                var minSize2 = LayoutUtility.GetPreferredSize( rect, axis );
                base.SetChildAlongAxis(rect, axis, num4, minSize2); //height
                num4 += minSize2 + this.spacing;
            }
        }
    }
}
