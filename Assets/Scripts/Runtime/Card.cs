using System;
using DG.Tweening;
using UnityEngine;

namespace Runtime
{
    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public class Card : MonoBehaviour
    {
        [Header("Data")]
        public int cardPoint;
        public CardStatus cardStatus;

        [Header("Components")] 
        private SpriteRenderer _cardSpriteRendererComponent;
        private BoxCollider2D _collider2DComponent;
        private MasterCard _masterCardComponent;
        
        // Dynamic assigned
        public CardType CardType { get; private set; }
        
        private Sprite _cardSprite;
        
        
        // Card graphic data
        public float CardHeight { get; private set; }
        public float CardWidth { get; private set; }
        
        
        // Local variables
        private float _holdTime = 0f;
        private float _moveDistance = 0f;
        private bool _isMouseDown = false;
        private bool _isInCombineMode = false;

        private Vector3 _originalPos;
        private Vector3 _offset;
        

        private void OnValidate()
        {
            AssignComponentReference();
        }

        private void Update()
        {
            if (_isMouseDown)
            {
                _holdTime += Time.deltaTime;

                if (_holdTime > 1f && cardStatus == CardStatus.Complete)
                {
                    if (_moveDistance > .2f) return;
                    _masterCardComponent.SeparateCard();
                }
            }
        }


        private void AssignComponentReference()
        {
            _cardSpriteRendererComponent = GetComponent<SpriteRenderer>();
            _collider2DComponent = GetComponent<BoxCollider2D>();
            _masterCardComponent = gameObject.GetComponentInParent<MasterCard>();
        }
        
        
        // Setup card data dynamically
        public void SetupCardData(CardData cardData, CardType cardType)
        {
            cardPoint = cardData.cardPoint;
            cardStatus = cardData.cardStatus;
            CardType = cardType;
            _cardSprite = cardData.cardSprite;

            AssignComponentReference();
            AssignCardSprite();
            
            // Set card width and height...
            var bounds = _cardSpriteRendererComponent.bounds;
            CardHeight = bounds.size.y;
            CardWidth = bounds.size.x;
            
            SetupCardCollision();
        }

        private void SetupCardCollision()
        {
            _collider2DComponent.offset = new Vector2(0, CardHeight / 2);
            _collider2DComponent.size = new Vector2(CardWidth, CardHeight);
        }

        private void AssignCardSprite()
        {
            _cardSpriteRendererComponent.sprite = _cardSprite;
        }

        private void OnMouseOver()
        {
            
        }

        private void OnMouseExit()
        {
            
        }

        protected virtual void OnMouseDown()
        {
            _isMouseDown = true;
            _originalPos = transform.position;

            if (Camera.main == null) return;
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            _offset = _originalPos - mousePos;

        }

        private void OnMouseUp()
        {
            _isMouseDown = false;
            _holdTime = 0f;
            
            //TODO - Integrate success logic
            if (_isInCombineMode)
            {
                _masterCardComponent.ExitCombineModeB();
                _isInCombineMode = false;
            }
            
        }

        private void OnMouseDrag()
        {
            if (Camera.main == null) return;
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            
            if(cardStatus == CardStatus.SeparateTop)
            {
                transform.position = mousePos + _offset;

                if (!_isInCombineMode)
                {
                    _masterCardComponent.EnterCombineModeB();
                    _isInCombineMode = true;
                }
            }

            _moveDistance = Vector3.Distance(transform.position, _originalPos);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("3");
            GameObject otherGameObject = other.gameObject;

            if (otherGameObject.CompareTag("Top Detection") && otherGameObject.transform.parent != _masterCardComponent.transform)
            {
                Debug.Log("1");
                otherGameObject.GetComponentInParent<MasterCard>().EnterCombineModeA();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Debug.Log("4");
            GameObject otherGameObject = other.gameObject;
            if (otherGameObject.CompareTag("Top Detection") && otherGameObject.transform.parent != _masterCardComponent.transform)
            {
                Debug.Log("2");
                otherGameObject.GetComponentInParent<MasterCard>().ExitCombineModeA();
                _isInCombineMode = false;
            }
        }
    }
}
