using Kartografowie.General;
using System.Collections;
using UnityEngine;

namespace Kartografowie.Grid
{
    public class GridCell : MonoBehaviour
    {
        public CellType CellType = CellType.Default;
        public bool HasRuins = false;
        public Vector2Int GridPosition = Vector2Int.zero;

        private float revealDuration = 0.5f;
        private float scaleFactor = 1.2f;
        private SpriteRenderer spriteRenderer;
        private Coroutine updateVisualCoroutine;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
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
            if (Generals.CellTypeIcons.ContainsKey(CellType))
            {
                spriteRenderer.sprite = Generals.CellTypeIcons[CellType];
                spriteRenderer.color = Color.white;
            }
            else
            {
                spriteRenderer.color = Generals.CellTypeColors[CellType];
            }
        }

        public bool IsRestricted()
        {
            return CellType != CellType.Default;
        }

        public void SetCellType(CellType newType)
        {
            CellType = newType;
            if (updateVisualCoroutine != null)
            {
                StopCoroutine(updateVisualCoroutine);
            }
            updateVisualCoroutine = StartCoroutine(RevealCoroutine());
        }

        private IEnumerator RevealCoroutine()
        {
            if (spriteRenderer == null)
                yield break;

            Vector3 originalScale = transform.localScale;
            Vector3 targetScale = originalScale * scaleFactor;

            Color startColor = Color.white;
            startColor.a = 0f;
            Color endColor;

            if (Generals.CellTypeIcons.ContainsKey(CellType))
            {
                spriteRenderer.sprite = Generals.CellTypeIcons[CellType];
                endColor = Color.white;
            }
            else
            {
                endColor = Generals.CellTypeColors[CellType];
            }
            spriteRenderer.color = startColor;

            float t = 0f;
            while (t < revealDuration)
            {
                t += Time.deltaTime;
                float normalized = t / revealDuration;

                // Lerp przezroczystoœci i skali
                spriteRenderer.color = Color.Lerp(startColor, endColor, normalized);
                transform.localScale = Vector3.Lerp(originalScale, targetScale, Mathf.Sin(normalized * Mathf.PI)); // pulsuj¹cy efekt

                yield return null;
            }

            // Upewnij siê, ¿e wartoœci koñcowe s¹ dok³adne
            spriteRenderer.color = endColor;
            transform.localScale = originalScale;

            updateVisualCoroutine = null;
        }
    }
}