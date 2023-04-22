using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBorder : MonoBehaviour
{
    [SerializeField] CameraMover mover;
    [SerializeField] BoxCollider2D border;

    private void OnMouseEnter()
    {
        mover.SetActiveCameraBorder(this);
    }

    private void OnMouseExit()
    {
        mover.SetActiveCameraBorder(null);
    }

    public BoxCollider2D GetBorder()
    {
        return border;
    }
}
