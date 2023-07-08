using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenu : MonoBehaviour
{
    [SerializeField] ObjectPool objectPool;
    [SerializeField] InputHandler inputHandler;
    List<ConstructionButton> buttons = new List<ConstructionButton>();

    private void OnEnable()
    {
        SetUpConstructionButtons();
    }

    private void SetUpConstructionButtons()
    {
        float yOffset = 0;
        TileBuilding[] tileBuildings = objectPool.GetTileBuildingPrefabs();
        Faction activePlayer = inputHandler.GetActivePlayer();
        foreach (TileBuilding tileBuilding in tileBuildings)
        {
            if (tileBuilding.IsBuildingAvailable(activePlayer))
            {
                ConstructionButton button = objectPool.GetConstructionButtonFromPool();
                button.SetUpButton(tileBuilding);
                button.transform.position = new Vector2(transform.position.x, transform.position.y + yOffset);
                button.transform.SetParent(transform, true);
                button.gameObject.SetActive(true);
                buttons.Add(button);
                yOffset += 125;
            }
        }
    }

    private void OnDisable()
    {
        foreach(ConstructionButton button in buttons)
        {
            gameObject.SetActive(false);
        }

        buttons.Clear();
    }
}
