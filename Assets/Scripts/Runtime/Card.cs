using System;
using UnityEngine;

namespace Runtime
{
    public class Card : MonoBehaviour
    {
        [Header("Data - Numeric")]
        public int cardPoint;
        public CardStatus cardStatus;

        [Header("Data - Art")]
        [SerializeField] private Sprite patternSprite;
        [SerializeField] private Sprite backgroundSprite;

        [Header("Sprite Renderer Components")] 
        private SpriteRenderer _iconSpriteRenderer;
        private SpriteRenderer _patternSpriteRenderer;
        private SpriteRenderer _backgroundSpriteRenderer;
        private MasterCard _masterCardComponent;
        
        // Dynamic assigned
        public CardType CardType { get; private set; }
        private Color _cardColor;
        
        public float CardHeight { get; private set; }


        private void Start()
        {
            _iconSpriteRenderer = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
            _patternSpriteRenderer = gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();
            _backgroundSpriteRenderer = gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>();
            _masterCardComponent = gameObject.GetComponentInParent<MasterCard>();
            
            SetupCardData(cardPoint, CardType.Attack);

            CardHeight = _backgroundSpriteRenderer.sprite.bounds.size.y;
        }

        private void OnValidate()
        {
            _iconSpriteRenderer = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
            _patternSpriteRenderer = gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();
            _backgroundSpriteRenderer = gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>();
            
            _patternSpriteRenderer.sprite = patternSprite;
            _backgroundSpriteRenderer.sprite = backgroundSprite;
        }

        // Setup CardData dynamically
        public void SetupCardData(int cardPoint, CardType cardType)
        {

            this.cardPoint = cardPoint;
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
            _masterCardComponent.SeparateCard();
        }

        private void OnMouseDrag()
        {
            Debug.Log("This card has been used");
        }
    }
}
