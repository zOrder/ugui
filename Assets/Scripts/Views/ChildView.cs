using UnityEngine;
using MinMVC;
using UnityEngine.UI;

public class ChildView : MediatedView, IChildView, ILayoutElement
{
    [SerializeField] Button button;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] private float scaleY = 1;
    public event OnButtonClick OnButton;
    private bool _buttonExtended;
    private Canvas _root;

    private float _initialWidth;
    private float _initialHeight;

    public override void OnMediate ()
    {
        button.onClick.AddListener( OnClick );
        _initialHeight = rectTransform.rect.height;
        _initialWidth = rectTransform.rect.width;
//
//        Animator animator = GetComponent<Animator>() as Animator;
//        animator.SetTrigger("Create");   
        GetRootCanvas();
    }

    private void GetRootCanvas()
    {       
        Canvas []c = GetComponentsInParent<Canvas>();
        foreach( Canvas canvas in c)
        {
            if( canvas.isRootCanvas )
            {
                _root = canvas;
                return;
            }
        }
    }

    void Update()
    {
        if(animation.isPlaying)
        {
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }
    }

    void OnClick()
    {
        if( OnButton != null )
        {
            var text = button.GetComponentInChildren<Text>();                                  
            OnButton( text.text );
        }

        if( _buttonExtended )
        {
            animation.PlayBackwards();
        }
        else
        {
            animation.Play();
        }
        _buttonExtended = !_buttonExtended;
    }

    #region ILayoutElement implementation
    public void CalculateLayoutInputHorizontal ()
    {
    }
    public void CalculateLayoutInputVertical ()
    {
    }      

    public  float minWidth {
        get {
            return _initialWidth * _root.scaleFactor;
        }
    }
    public  float preferredWidth {
        get {            
            return _initialWidth * _root.scaleFactor;
        }
    }
    public  float flexibleWidth {
        get {
            return _initialWidth * _root.scaleFactor;
        }
    }
    public  float minHeight {
        get {
            return _initialHeight * _root.scaleFactor * scaleY;
        }
    }
    public  float preferredHeight {
        get {
            
            return _initialHeight * _root.scaleFactor * scaleY;
        }
    }
    public   float flexibleHeight {
        get {
            return _initialHeight * _root.scaleFactor * scaleY;
        }
    }
    public  int layoutPriority {
        get {
            return  1;
        }
    }
    #endregion
}

