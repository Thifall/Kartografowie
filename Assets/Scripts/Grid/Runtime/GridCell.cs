using Kartografowie.Assets.Scripts.Grid.Core;
using Kartografowie.General;
using System.Collections;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Grid.Runtime
{
    public class GridCell : MonoBehaviour
    {
        private const float _animationRevealDuration = 0.5f;
        private const float _animationScaleFactor = 1.2f;
        private SpriteRenderer spriteRenderer;
        private Coroutine updateVisualCoroutine;
        public Vector2Int GridPosition = Vector2Int.zero;

        [SerializeField]
        private CellConfig _cellConfig;

        public CellState CellState { get; private set; }

        public CellType CurrentCellType => CellState.CellType;
        public bool HasRuins => CellState.HasRuins;



        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            CellState = new CellState(_cellConfig);
            UpdateVisualFromState();
        }

        private void OnValidate()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            UpdateVisualFromState(); // Aktualizuje kolor po zmianie wartoœci w Inspectorze
        }

        private void UpdateVisualFromState()
        {
            if (spriteRenderer == null) return;
            if (CellState is null) return;
            if (Generals.CellTypeIcons.ContainsKey(CellState.CellType))
            {
                spriteRenderer.sprite = Generals.CellTypeIcons[CellState.CellType];
                spriteRenderer.color = Color.white;
            }
            else
            {
                spriteRenderer.color = Generals.CellTypeColors[CellState.CellType];
            }
        }

        public void SetCellType(CellType newType)
        {
            CellState.SetCellType(newType);
            if (updateVisualCoroutine != null)
            {
                StopCoroutine(updateVisualCoroutine);
            }
            updateVisualCoroutine = StartCoroutine(TypeTransitionCoroutine());
        }

        private IEnumerator TypeTransitionCoroutine()
        {
            if (spriteRenderer == null)
                yield break;

            Vector3 originalScale = transform.localScale;
            Vector3 targetScale = originalScale * _animationScaleFactor;

            Color startColor = Color.white;
            startColor.a = 0f;
            Color endColor;

            if (Generals.CellTypeIcons.ContainsKey(CellState.CellType))
            {
                spriteRenderer.sprite = Generals.CellTypeIcons[CellState.CellType];
                endColor = Color.white;
            }
            else
            {
                endColor = Generals.CellTypeColors[CellState.CellType];
            }
            spriteRenderer.color = startColor;

            float t = 0f;
            while (t < _animationRevealDuration)
            {
                t += Time.deltaTime;
                float normalized = t / _animationRevealDuration;

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