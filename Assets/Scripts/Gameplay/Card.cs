using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Gameplay
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

        private Stack<Tween> _tweensPool = new Stack<Tween>();


        // Card graphic data
        public float CardHeight { get; private set; }
        public float CardWidth { get; private set; }
        
        
        // Local variables
        private float _holdTime = 0f;
        private float _moveDistance = 0f;
        private bool _isMouseDown = false;
        private bool _isInCombineMode = false;
        private bool _canCombine = false;


        private Vector3 _originalPos;
        private Vector3 _offset;
        private Vector3 _defaultPos;

        private MasterCard _otherMasterCard;
        private CardData _cardData;

        private Tween _dragMoveAnimation;

        private void OnValidate()
        {
            AssignComponentReference();
        }

        private void Start()
        {
            _defaultPos = transform.localPosition;
        }

        private void Update()
        {
            if (_isMouseDown)
            {
                _holdTime += Time.deltaTime;

                if (_holdTime > 1f && cardStatus == CardStatus.Complete)
                {
                    if (_moveDistance > .1f) return;
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
            _cardData = cardData;
            cardPoint = cardData.cardPoint;
            cardStatus = cardData.cardStatus;
            CardType = cardType;
            _cardSprite = GetCardSpriteByType(cardData, cardType);

            AssignComponentReference();
            AssignCardSprite();
            
            // Set card width and height...
            var bounds = _cardSpriteRendererComponent.bounds;
            CardHeight = bounds.size.y;
            CardWidth = bounds.size.x;
            
            SetupCardCollision();
        }

        private Sprite GetCardSpriteByType(CardData cardData, CardType cardType)
        {
            Sprite[] cardSprites = cardData.cardSprites;
                
            switch (cardType)
            {
                case CardType.Attack:
                    return cardSprites[0];
                
                case CardType.Defend:
                    return cardSprites[1];
                
                case CardType.Treat:
                    return cardSprites[2];
                
                case CardType.Default:
                    return cardSprites[3];
            }

            return null;
        }

        public void RefreshCardSprite(CardType cardType)
        {
            _cardSpriteRendererComponent.sprite = GetCardSpriteByType(_cardData, cardType);
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

        public MasterCard GetMasterCard()
        {
            return _masterCardComponent == null ? null : _masterCardComponent;
        }


        private void OnMouseOver()
        {
            if (cardStatus == CardStatus.Complete || cardStatus == CardStatus.SeparateTop)
            {
                _tweensPool.Push(transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .1f));
            }
        }

        private void OnMouseExit()
        {
            if (cardStatus == CardStatus.Complete || cardStatus == CardStatus.SeparateTop)
            {
                _tweensPool.Push(transform.DOScale(Vector3.one, .1f));
            }
        }


        private void OnMouseDown()
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

            if (cardStatus == CardStatus.SeparateTop)
            {
                if (_isInCombineMode && _canCombine && _otherMasterCard != null)
                {
                    _tweensPool.Push(transform.DOScale(Vector3.one, .1f));
                    _otherMasterCard.CombineCards(this);
                    _canCombine = false;
                    
                }else if (_isInCombineMode)
                {
                    _masterCardComponent.ExitCombineModeB();
                    _isInCombineMode = false;
                }
            }

            if (cardStatus == CardStatus.Complete)
            {
                _dragMoveAnimation = transform.DOLocalMove(_defaultPos, .2f);
                _tweensPool.Push(_dragMoveAnimation);
                transform.localPosition = _defaultPos;
                _masterCardComponent.StartIdleTweenAnimation();

                if (_masterCardComponent.GetCanCheckout())
                {
                    LevelManager.Instance.CheckoutCard(_masterCardComponent);
                    cardStatus = CardStatus.Checkedout;
                }
            }
            
        }

        private void OnMouseDrag()
        {
            if (Camera.main == null) return;
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            
            if (cardStatus == CardStatus.Complete)
            {
                if (_moveDistance < .05f)
                {
                    _tweensPool.Push(transform.DOShakePosition(duration: .1f, strength: _holdTime * .2f));
                }
                else
                {
                    _masterCardComponent.KillIdleTweenAnimation();
                    _dragMoveAnimation = transform.DOMove(mousePos + _offset, .2f);
                    _tweensPool.Push(_dragMoveAnimation);
                }
            }
            
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

        public void DisableCollision2D()
        {
            _collider2DComponent.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            GameObject otherGameObject = other.gameObject;

            if (otherGameObject.CompareTag("Top Detection") && otherGameObject.transform.parent != _masterCardComponent.transform && cardStatus == CardStatus.SeparateTop)
            {
                _otherMasterCard = otherGameObject.GetComponentInParent<MasterCard>();
                if (_otherMasterCard != null && _otherMasterCard.masterCardStatus == CardStatus.Separated)
                {
                    _otherMasterCard.EnterCombineModeA();
                    _canCombine = true;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            GameObject otherGameObject = other.gameObject;
            if (otherGameObject.CompareTag("Top Detection") && otherGameObject.transform.parent != _masterCardComponent.transform)
            {
                _otherMasterCard = otherGameObject.GetComponentInParent<MasterCard>();
                if (_otherMasterCard != null && _otherMasterCard.masterCardStatus == CardStatus.Separated)
                {
                    _otherMasterCard.ExitCombineModeA();
                    _otherMasterCard = null;
                    _isInCombineMode = false;
                    _canCombine = false;
                }
            }
        }

        private void OnDestroy()
        {
            while (_tweensPool.Count != 0)
            {
                _tweensPool.Pop().Kill();
            }
        }

        public void CoverCard(Sprite sprite)
        {
            _cardSpriteRendererComponent.sprite = sprite;
        }


    }
}
