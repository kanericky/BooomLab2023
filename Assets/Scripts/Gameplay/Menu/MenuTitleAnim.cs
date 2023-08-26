using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MenuTitleAnim : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.DOMoveY(transform.position.y + .5f, 2f).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
