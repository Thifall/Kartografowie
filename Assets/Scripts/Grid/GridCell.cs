using System.Collections;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public CellType cellType = CellType.Default;

    private SpriteRenderer spriteRenderer;
    private Coroutine currentCoroutine;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnMouseDown()
    {
        if (IsRestricted())
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine); // Zatrzymanie poprzedniej korutyny
            }
            currentCoroutine = StartCoroutine(FlashRed());
            return;
        }
    }

    private IEnumerator FlashRed()
    {
        float elapsedTime = 0f;
        var originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        while (elapsedTime < 1.5f)
        {
            elapsedTime += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, originalColor, elapsedTime / 5f);
            yield return null;
        }
        spriteRenderer.color = originalColor;
    }

    private void OnValidate()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateVisual(); // Aktualizuje kolor po zmianie wartoœci w Inspectorze
    }

    private void UpdateVisual()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.color = Generals.CellTypeColors[cellType];
    }

    internal bool IsRestricted()
    {
        return cellType != CellType.Default;
    }

    public void SetCellType(CellType newType)
    {
        cellType = newType;
        UpdateVisual(); 
    }
}
