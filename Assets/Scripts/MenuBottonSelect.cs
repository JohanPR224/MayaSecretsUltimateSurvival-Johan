using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonSelect : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    public static RectTransform Dagger;
    [SerializeField] private float offsetX = -60f; // Margen extra a la izquierda del borde

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
        UpdateDaggerPosition();
    }

    public void OnSelect(BaseEventData eventData)
    {
        UpdateDaggerPosition();
    }

    private void UpdateDaggerPosition()
    {
        if (Dagger != null)
        {
            RectTransform btnRect = GetComponent<RectTransform>();
            float halfWidth = btnRect != null ? (btnRect.rect.width * 0.5f) : 210f;

            // Calcula la posición exacta justo a la izquierda del borde del botón
            Vector3 pos = transform.position;
            pos.x -= (halfWidth - offsetX) * transform.lossyScale.x;
            Dagger.position = pos;
            Dagger.gameObject.SetActive(true);
        }
    }
}