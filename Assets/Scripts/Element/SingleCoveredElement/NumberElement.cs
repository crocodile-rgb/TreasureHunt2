using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberElement : SingleCoveredElement
{
    public override void Awake()
    {
        base.Awake();
        elementState = ElementState.Covered;
        elementContent = ElementContent.Number;
    }

    public override void OnMiddleMouseButton()
    {
        GameManager.Instance.UncoveredAdjacentElements(x, y);
    }

    public override void UncoveredElementSingle()
    {
        if (elementState == ElementState.Uncovered) return;
        RemoveFlag();
        elementState = ElementState.Uncovered;
        ClearShadow();
        Instantiate(GameManager.Instance.uncoveredEffect, transform);
        LoadSprite(GameManager.Instance.numberSprites[GameManager.Instance.CountAdjacentTraps(x, y)]);
    }

    public override void OnUncovered()
    {
        GameManager.Instance.FloodFillElement(x, y, new bool[GameManager.Instance.w, GameManager.Instance.h]);
    }
}
