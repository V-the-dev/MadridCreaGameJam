using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SimplePanelScript : MonoBehaviour
{
    [SerializeField] private GameObject firstButton;
    private GameObject previousButton = null;

    private void OnEnable()
    {
        previousButton = EventSystem.current.currentSelectedGameObject;

        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    private void OnDisable()
    {
        if (EventSystem.current == null)
            return;

        EventSystem.current.SetSelectedGameObject(previousButton);
    }

    
}
