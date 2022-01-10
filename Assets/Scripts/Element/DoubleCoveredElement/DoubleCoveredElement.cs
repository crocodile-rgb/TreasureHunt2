using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleCoveredElement : SingleCoveredElement
{
    public bool isHide = true;
    //kai shi dao ju fan kai de gai lv
    public override void Awake()
    {
        base.Awake();
        elementType = ElementType.DoubleCovered;
        if (Random.value < GameManager.Instance.uncoveredProbability)
        {
            UncoveredElementSingle();
        }
    }

    public override void OnPlayerStand()
    {
        switch (elementState)
        {
            case ElementState.Covered:
                if (isHide == true)
                {
                    UncoveredElementSingle();
                }else
                {
                    UncoveredElement();
                }
                break;
            case ElementState.Uncovered:
                return;
            case ElementState.Marked:
                if (isHide == true)
                {
                    RemoveFlag();
                }
                break;
        }
    }

    public override void OnMiddleMouseButton()
    {
        GameManager.Instance.UncoveredAdjacentElements(x, y);
    }

    public override void OnRightMouseButton()
    {
        switch (elementState)
        {
            case ElementState.Covered:
                if (isHide == true)
                {
                    AddFlag();
                }
                break;
            case ElementState.Uncovered:
                return;
            case ElementState.Marked:
                if (isHide == true)
                {
                    RemoveFlag();
                }
                break;
        }
    }

    public override void UncoveredElementSingle()
    {
        if (elementState == ElementState.Uncovered) return;
        isHide = false;
        RemoveFlag();
        ClearShadow();
        ConfirmSprite();
    }

    public override void OnUncovered()
    {
        elementState = ElementState.Uncovered;
        ToNumberElement();
    }

    public virtual void ConfirmSprite()
    {

    }
}
