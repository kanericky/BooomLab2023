using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime
{
    public class MasterCard : MonoBehaviour
    {
        // TODO - Test, need to be deleted
        public CardData cardData;

        public int CardPoint { get; private set; }
        public CardType CardType { get; private set; }

        [SerializeField] private GameObject cardTemplate;
        
        [SerializeField] private Transform completeCardSlot;
        [SerializeField] private Transform separateCardSlot;
        [SerializeField] private Transform detectionZone;

        private Card _separatedCardA;
        private Card _separatedCardB;

        private Vector3 _separatedCardAOriginalPos;
        private Vector3 _separatedCardBOriginalPos;
        
        private Transform _activatedSlot;

        private void Start()
        {
            InitCard();
        }

        private void InitCard()
        {
            completeCardSlot.gameObject.SetActive(true);
            separateCardSlot.gameObject.SetActive(false);
            detectionZone.gameObject.SetActive(false);
            _activatedSlot = completeCardSlot;

            // TODO - NEED TO BE DELELTED, ONLY FOR TESTING
            SpawnMainCard(cardData);
        }

        public void SpawnMainCard(CardData cardData)
        {
            GameObject spawnedGameObject = Instantiate(cardTemplate, _activatedSlot);
            Card spawnedCard = spawnedGameObject.GetComponent<Card>();
            
            if (spawnedCard == null) return;

            spawnedCard.SetupCardData(cardData, Runtime.CardType.Attack);
        }

        public void SeparateCard()
        {
            completeCardSlot.gameObject.SetActive(false);
            separateCardSlot.gameObject.SetActive(true);
            detectionZone.gameObject.SetActive(true);
            _activatedSlot = separateCardSlot;

            CardPair[] availableCardPairs = cardData.SeparatedCardPairs;
            int randomSelectIndex = Random.Range(0, availableCardPairs.Length);
            CardPair selectedCardPair = availableCardPairs[randomSelectIndex];
            
            CardData cardDataA = selectedCardPair.SeparatedCardDataA;
            CardData cardDataB = selectedCardPair.SeparatedCardDataB; 

            GameObject spawnedGameObjectA = Instantiate(cardTemplate, _activatedSlot);
            GameObject spawnedGameObjectB = Instantiate(cardTemplate, _activatedSlot);

            _separatedCardA = spawnedGameObjectA.GetComponent<Card>();
            _separatedCardB = spawnedGameObjectB.GetComponent<Card>();

            if (_separatedCardA == null || _separatedCardB == null) return;
            
            _separatedCardA.SetupCardData(cardDataA, CardType);
            _separatedCardB.SetupCardData(cardDataB, CardType.None);

            _separatedCardA.transform.localPosition += new Vector3(0f, _separatedCardB.CardHeight + 0.25f, 0f);
            _separatedCardA.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-3f, 3f)));

            _separatedCardB.transform.localPosition += new Vector3(0f, -0.25f, 0f);
            //_separatedCardB.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 5f));

            // Record the original pos to reset positions
            _separatedCardAOriginalPos = _separatedCardA.transform.position;
            _separatedCardBOriginalPos = _separatedCardB.transform.position;
        }

        public void EnterCombineModeA()
        {
            _separatedCardA.transform.DOMoveY(15f, .2f);
        }

        public void ExitCombineModeA()
        {
            _separatedCardA.transform.DOMoveY(_separatedCardAOriginalPos.y, .2f);
        }
        
        
        public void EnterCombineModeB()
        {
            _separatedCardA.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .2f);
            _separatedCardB.transform.DOMoveY(-15f, .2f);
            _separatedCardB.transform.DORotate(new Vector3(0f, 0f, Random.Range(-30f, 30f)), .2f);
        }
        
        public void ExitCombineModeB()
        {
            _separatedCardA.transform.DOScale(Vector3.one, .1f);
            _separatedCardA.transform.position = _separatedCardAOriginalPos;
            _separatedCardB.transform.DOMoveY(_separatedCardBOriginalPos.y, .1f);
            _separatedCardB.transform.DORotate(Vector3.zero, .1f);
        }

        public bool CheckCanCombine()
        {
            return false;
        }
    }
}
