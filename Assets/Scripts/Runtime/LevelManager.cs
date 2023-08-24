using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

namespace Runtime
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;
        
        [Header("Data")] [SerializeField] private int cardNum = 3;
        
        [Header("Opponents")]
        [SerializeField] private Player playerController;
        [SerializeField] private Enemy enemyController;

        [SerializeField] private Deck cardDeck;
        [SerializeField] private CheckoutDetectZone checkoutDetectZone;

        private MasterCard _playerCard;
        private MasterCard _enemyCard;

        [Header("GameObject Template")] [SerializeField]
        private MasterCard cardTemplate;

        [Header("Runtime")] 
        [SerializeField] private Turn currentTurn;

        private void Start()
        {
            Instance = this;
            currentTurn = Turn.Start;
            StartCoroutine(SetupBattle());
            
        }

        private void PlayerTurn()
        {
            playerController.cardHolderComponent.InitCardHolderPos();
            
            CardData[] cardDataArray = cardDeck.RequestCardsFromDeck(cardNum);
            CardType[] cardTypeArray = cardDeck.RequestCardTypes(cardNum);
 
            for(int i = 0; i < cardNum; i++)
            {
                CardData cardData = cardDataArray[i];
                CardType cardType = cardTypeArray[i];
                
                MasterCard card = Instantiate(cardTemplate);
                card.InitCard(cardData, cardType);
                
                playerController.AddCardToHand(card);
            }
            
            playerController.cardHolderComponent.ShowAllCards();
        }

        private void EnemyTurn()
        {
            enemyController.cardHolderComponent.InitCardHolderPos();

            CardData[] cardDataArray = cardDeck.RequestCardsFromDeck(cardNum);
            CardType[] cardTypeArray = cardDeck.RequestCardTypes(cardNum);
 
            for(int i = 0; i < cardNum; i++)
            {
                CardData cardData = cardDataArray[i];
                CardType cardType = cardTypeArray[i];
                
                MasterCard card = Instantiate(cardTemplate);
                card.InitCard(cardData, cardType);
                
                enemyController.AddCardToHand(card);
            }
            
            enemyController.cardHolderComponent.ShowAllCards();
        }

        IEnumerator SetupBattle()
        {
            playerController = FindObjectOfType<Player>();
            enemyController = FindObjectOfType<Enemy>();
            cardDeck = FindObjectOfType<Deck>();

            checkoutDetectZone = GetComponentInChildren<CheckoutDetectZone>();

            yield return new WaitForSeconds(1f);

            currentTurn = Turn.Player;
            
            PlayerTurn();
            EnemyTurn();
        }
        

        public void CheckoutCard(MasterCard card)
        {
            _playerCard = card;

            card.KillIdleTweenAnimation();
            
            card.transform.parent = checkoutDetectZone.transform;

            card.transform.position = Vector3.zero;
            card.transform.localPosition -= new Vector3(0, card.cardHeight / 2, 0);
            
            playerController.cardHolderComponent.RemoveOtherCards();
            
            EnemyCheckoutCard();
        }

        public void EnemyCheckoutCard()
        {

            MasterCard card = enemyController.ChooseCard();
            
            _enemyCard = card;
            
            card.KillIdleTweenAnimation();
            
            enemyController.cardHolderComponent.RemoveOtherCards();
            
            StartCoroutine(PostCheck());
        }

        IEnumerator PostCheck()
        {
            yield return new WaitForSeconds(2f);
            //RemoveCardsOnTheTable();
            
            Destroy(_playerCard.gameObject);
            Destroy(_enemyCard.gameObject);
            
            // TODO
            
            StartCoroutine(SetupBattle());
        }

        private void RemoveCardsOnTheTable()
        {
            _playerCard.transform.DOLocalMoveY(-5f, .2f);
            _enemyCard.transform.DOLocalMoveY(10f, .2f);
        }
    }
}