using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    [SerializeField] GameObject buildMenu;

    public void OpenBuildMenu()
    {
        buildMenu.SetActive(!buildMenu.activeInHierarchy);
    }
}
