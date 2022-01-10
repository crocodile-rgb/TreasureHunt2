using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapElement : SingleCoveredElement
{
    public override void Awake()
    {
        base.Awake();
        elementState = ElementState.Covered;
        elementContent = ElementContent.Trap;
    }

    public override void UncoveredElementSingle()
    {
        if (elementState == ElementState.Uncovered) return;
        RemoveFlag();
        elementState = ElementState.Uncovered;
        ClearShadow();
        Instantiate(GameManager.Instance.uncoveredEffect, transform);
        LoadSprite(GameManager.Instance.trapSprites[Random.Range(0, GameManager.Instance.trapSprites.Length)]);
    }

    public override void OnUncovered()
    {
        GameManager.Instance.DisplayAllTraps();
    }
}
