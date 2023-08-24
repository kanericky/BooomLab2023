using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime
{
    [RequireComponent(typeof(Collider2D))]
    public class MasterCard : MonoBehaviour
    {
        public CardData cardData;

        [Header("Runtime")]
        public int masterCardPoint;
        public CardType masterCardType;
        public CardStatus masterCardStatus;
        public float cardHeight;

        [Header("Prefab Data")]
        [SerializeField] private GameObject cardTemplate;
        
        [SerializeField] private Transform completeCardSlot;
        [SerializeField] private Transform separateCardSlot;
        [SerializeField] private Transform detectionZone;

        [SerializeField] private float separateGap;
        [SerializeField] private float combineGapFactor;
        
        private bool _canCheckout = false;

        private Card _completeCard;
        private Card _separatedCardA;
        private Card _separatedCardB;

        private Vector3 _separatedCardAOriginalPos;
        private Vector3 _separatedCardBOriginalPos;
        private Vector3 _originalPos;
        private Vector3 _offset;
        private Vector3 _defaultPos;
        
        private Transform _activatedSlot;
        
        private Stack<Tween> _tweensPool = new Stack<Tween>();
        private Tween _randomShakePos, _randomShakeRot;
        private Tween _dragMoveAnimation;

        private BoxCollider2D _boxCollider2DComponent;

        private void Start()
        {
            _defaultPos = transform.localPosition;
            StartIdleTweenAnimation();
        }
        

        public void InitCard(CardData cardData, CardType cardType)
        {
            completeCardSlot.gameObject.SetActive(true);
            separateCardSlot.gameObject.SetActive(false);
            detectionZone.gameObject.SetActive(false);
            _activatedSlot = completeCardSlot;

            _boxCollider2DComponent = GetComponent<BoxCollider2D>();

            this.cardData = cardData;
            
            SpawnMainCard(cardData, cardType);

            masterCardStatus = CardStatus.Complete;
            RefreshMasterCardData();
        }

        private void SpawnMainCard(CardData cardData, CardType cardType)
        {
            GameObject spawnedGameObject = Instantiate(cardTemplate, _activatedSlot);
            _completeCard = spawnedGameObject.GetComponent<Card>();
            
            if (_completeCard == null) return;

            _completeCard.SetupCardData(cardData, cardType);
        }

        private void RefreshMasterCardData()
        {
            if (masterCardStatus == CardStatus.Complete)
            {
                masterCardPoint = _completeCard.cardPoint;
                masterCardType = _completeCard.CardType;
                cardHeight = _completeCard.CardHeight;
            }

            if (masterCardStatus == CardStatus.Separated)
            {
                masterCardPoint = _separatedCardA.cardPoint + _separatedCardB.cardPoint;
                masterCardType = _separatedCardA.CardType;
            }

            if (masterCardStatus == CardStatus.Combined)
            {
                masterCardPoint = _separatedCardA.cardPoint + _separatedCardB.cardPoint;
                masterCardType = _separatedCardA.CardType;
            }
        }
        
        public void CombineCards(Card otherCard)
        {
            Destroy(_separatedCardA.gameObject); // Destroy the current _separatedCardA
            _separatedCardA = otherCard; // Assign the new top-separated card to it
            
            otherCard.gameObject.transform.SetParent(separateCardSlot); // Re-parent the top separated card
            
            _separatedCardA.cardStatus = CardStatus.Combined; // Update the separated card's status
            _separatedCardB.cardStatus = CardStatus.Combined;
            
            // Update collisions
            _separatedCardA.DisableCollision2D();
            _separatedCardB.DisableCollision2D();

            cardHeight = _separatedCardA.CardHeight + _separatedCardB.CardHeight;
            _boxCollider2DComponent.offset = new Vector2(0, cardHeight / 2);
            _boxCollider2DComponent.size = new Vector2(_separatedCardA.CardWidth, cardHeight);

            _separatedCardB.RefreshCardSprite(_separatedCardA.CardType); // Update the bottom card color
            
            Destroy(otherCard.GetMasterCard().gameObject); // Destroy the instigator's master card game object because we do not need it anymore...
            Destroy(detectionZone.gameObject); // Destroy the combine detect zone since we do not need it anymore
            
            masterCardStatus = CardStatus.Combined;
            RefreshMasterCardData();
            
            // Handle animation
            _separatedCardA.transform.localPosition = _separatedCardAOriginalPos;
            _separatedCardA.transform.DOLocalMoveY(_separatedCardAOriginalPos.y - separateGap * combineGapFactor, .1f);
            _separatedCardB.transform.DOLocalMoveY(_separatedCardBOriginalPos.y + separateGap * combineGapFactor, .1f);
            
            StartIdleTweenAnimation();
        }

        public void SeparateCard()
        {
            completeCardSlot.gameObject.SetActive(false);
            separateCardSlot.gameObject.SetActive(true);
            detectionZone.gameObject.SetActive(true);
            _activatedSlot = separateCardSlot;

            // Get all separated possibilities
            CardPair[] availableCardPairs = cardData.SeparatedCardPairs;
            int randomSelectIndex = Random.Range(0, availableCardPairs.Length);
            CardPair selectedCardPair = availableCardPairs[randomSelectIndex];
            
            CardData cardDataA = selectedCardPair.SeparatedCardDataA;
            CardData cardDataB = selectedCardPair.SeparatedCardDataB; 

            // Spawn two separated cards
            GameObject spawnedGameObjectA = Instantiate(cardTemplate, _activatedSlot);
            GameObject spawnedGameObjectB = Instantiate(cardTemplate, _activatedSlot);

            _separatedCardA = spawnedGameObjectA.GetComponent<Card>();
            _separatedCardB = spawnedGameObjectB.GetComponent<Card>();

            if (_separatedCardA == null || _separatedCardB == null) return;
            
            _separatedCardA.SetupCardData(cardDataA, masterCardType);
            _separatedCardB.SetupCardData(cardDataB, masterCardType);

            _separatedCardA.transform.localPosition += new Vector3(0f, _separatedCardB.CardHeight - separateGap / 2, 0f);
            _separatedCardA.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-1f, 1f)));

            _separatedCardB.transform.localPosition += new Vector3(0f, - separateGap / 2, 0f);
            //_separatedCardB.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 5f));

            // Record the original pos to reset positions
            _separatedCardAOriginalPos = _separatedCardA.transform.localPosition;
            _separatedCardBOriginalPos = _separatedCardB.transform.localPosition;
            
            // Destroy the complete card since we do not need it anymore...
            Destroy(completeCardSlot.GetChild(0).gameObject);
            
            // Refresh master card's data
            masterCardStatus = CardStatus.Separated;    
            RefreshMasterCardData();
            
            StartIdleTweenAnimation();
        }

        public void EnterCombineModeA()
        {
            _separatedCardA.transform.DOLocalMoveY(15f, .2f);
        }

        public void ExitCombineModeA()
        {
            _separatedCardA.transform.DOLocalMoveY(_separatedCardAOriginalPos.y, .2f);
        }
        
        public void EnterCombineModeB()
        {
            _separatedCardA.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .2f);
            _separatedCardB.transform.DOLocalMoveY(-15f, .2f);
            _separatedCardB.transform.DORotate(new Vector3(0f, 0f, Random.Range(-30f, 30f)), .2f);
        }
        
        public void ExitCombineModeB()
        {
            _separatedCardA.transform.DOScale(Vector3.one, .1f);
            _separatedCardA.transform.localPosition = _separatedCardAOriginalPos;
            
            _separatedCardB.transform.DOLocalMoveY(_separatedCardBOriginalPos.y, .1f);
            _separatedCardB.transform.DORotate(Vector3.zero, .1f);
        }

        public void StartIdleTweenAnimation()
        {
            _randomShakePos = transform.
                DOShakePosition(duration: 5f, strength: .1f, vibrato: 1, fadeOut: false).
                SetLoops(-1, LoopType.Restart);
            _randomShakeRot = transform.
                DORotate(new Vector3(0, 0, Random.Range(-2, 2)), 2.5f).
                SetEase(Ease.Linear).
                SetLoops(-1, LoopType.Yoyo);
            
            _tweensPool.Push(_randomShakePos);
            _tweensPool.Push(_randomShakeRot);
        }
        
        public bool GetCanCheckout()
        {
            return _canCheckout;
        }
        
        public void KillIdleTweenAnimation()
        {
            transform.DOKill();
        }

        private void OnDestroy()
        {
            while (_tweensPool.Count != 0)
            {
                DOTween.Kill(_tweensPool.Pop());
            }
        }
        
        private void OnMouseExit()
        {
            if (masterCardStatus == CardStatus.Combined)
            {
                _tweensPool.Push(transform.DOScale(Vector3.one, .1f));
            }
        }

        private void OnMouseDown()
        {
            _originalPos = transform.position;

            if (Camera.main == null) return;
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            _offset = _originalPos - mousePos;
        }

        private void OnMouseDrag()
        {
            if (Camera.main == null) return;
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            if (masterCardStatus == CardStatus.Combined)
            {
                KillIdleTweenAnimation();
                _dragMoveAnimation = transform.DOMove(mousePos + _offset, .2f);
                _tweensPool.Push(_dragMoveAnimation);
            }
        }

        private void OnMouseUp()
        {
            if (_canCheckout)
            {
                LevelManager.Instance.CheckoutCard(this);
                KillIdleTweenAnimation();
                masterCardStatus = CardStatus.Checkedout;
                return;
            }
            
            if (masterCardStatus == CardStatus.Combined)
            {
                _dragMoveAnimation.Kill();
                _dragMoveAnimation = transform.DOLocalMove(_defaultPos, .2f);
                _tweensPool.Push(_dragMoveAnimation);
                _dragMoveAnimation.Kill();
                transform.localPosition = _defaultPos;
            }
            
        }

        private void OnMouseOver()
        {
            if (masterCardStatus == CardStatus.Combined)
            {
                _tweensPool.Push(transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .1f));
            }
        }

        public void SetCanCheckout(bool state)
        {
            _canCheckout = state;
        }
    }
}
