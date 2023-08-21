using System;
using UnityEngine;

namespace Runtime
{
    public class Card : MonoBehaviour
    {
        [Header("Data - Numeric")]
        public int cardPoint;

        [Header("Data - Art")] 
        [SerializeField] private Sprite iconSprite;
        [SerializeField] private Sprite patternSprite;
        [SerializeField] private Sprite backgroundSprite;

        [Header("Sprite Renderer Components")] 
        private SpriteRenderer _iconSpriteRenderer;
        private SpriteRenderer _patternSpriteRenderer;
        private SpriteRenderer _backgroundSpriteRenderer;
        
        public CardType CardType { get; private set; }
        private Color _cardColor;
        public int CardPoint { get; private set; }


        private void Start()
        {
            _iconSpriteRenderer = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
            _patternSpriteRenderer = gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();
            _backgroundSpriteRenderer = gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>();
            
            SetupCardData(cardPoint, CardType.Attack);
        }

        public void SetupCardData(int cardPoint, CardType cardType)
        {

            CardPoint = cardPoint;
            CardType = cardType;

            _cardColor = GameManager.Instance.AssignColor(cardType);
            AssignIcon(CardType);
            AssignPointPattern();
            AssignCardBackground();
        
        }

        private void AssignIcon(CardType type)
        {
            _iconSpriteRenderer.sprite = GameManager.Instance.AssignIcon(type);
            _iconSpriteRenderer.color = _cardColor;
        }

        private void AssignPointPattern()
        {
            _patternSpriteRenderer.sprite = patternSprite;
            _patternSpriteRenderer.color = _cardColor;
        }

        private void AssignCardBackground()
        {
            _backgroundSpriteRenderer.sprite = backgroundSprite;
        }
    
        private void OnMouseDown()
        {
            Debug.Log("This card has been separated");
        }

        private void OnMouseDrag()
        {
            Debug.Log("This card has been used");
        }
    }
}
