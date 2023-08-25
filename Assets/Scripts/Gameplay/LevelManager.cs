using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Gameplay
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

        [Header("GameObject Template")] 
        [SerializeField] private MasterCard cardTemplate;
        [SerializeField] private EnemyCard enemyCardTemplate;

        [Header("Runtime")] 
        [SerializeField] private Turn currentTurn;

        private void Start()
        {
            Instance = this;
            currentTurn = Turn.Start;
            
            UIManager.Instance.InitPlayerUIInfo(playerController.initHealth, playerController.initArmor);
            UIManager.Instance.InitEnemyUIInfo(enemyController.initHealth, enemyController.initArmor);

            GameObject[] uxSetups = FindObjectsOfType<GameObject>();
            foreach (var go in uxSetups)
            {
                IUxSetup uxSetup = go.GetComponent<IUxSetup>();
                if(uxSetup == null) continue;
                uxSetup.UxSetup();
            }
            
            StartCoroutine(SetupBattle());
            
        }

        private void PlayerTurn()
        {
            playerController.cardHolderComponent.InitCardHolderPos();
            
            CardData[] cardDataArray = cardDeck.RequestCardsFromDeck(isEnemy: false, cardNum);
            CardType[] cardTypeArray = cardDeck.RequestCardTypes(isEnemy: false, cardNum);
 
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

            CardData[] cardDataArray = cardDeck.RequestCardsFromDeck(isEnemy: true, cardNum);
            CardType[] cardTypeArray = cardDeck.RequestCardTypes(isEnemy: true, cardNum);
 
            for(int i = 0; i < cardNum; i++)
            {
                CardData cardData = cardDataArray[i];
                CardType cardType = cardTypeArray[i];
                
                MasterCard card = Instantiate(enemyCardTemplate);
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
            yield return new WaitForSeconds(1f);
            
            _enemyCard.GetComponent<EnemyCard>().RevealCard();
            
            yield return new WaitForSeconds(1f);

            int difference = _playerCard.masterCardPoint - _enemyCard.masterCardPoint;

            Transform playerSpriteTrans = playerController.spriteRendererComponent.transform;
            Transform enemySpriteTrans = enemyController.spriteRendererComponent.transform;
            
            // Player's card applied
            if (difference > 0)
            {
                _playerCard.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), .2f);
                _playerCard.transform.DOShakePosition(.2f);
                playerSpriteTrans.transform.DOLocalMoveZ(-2, .2f);

                yield return new WaitForSeconds(.2f);
                
                Vector3 cachePos = playerSpriteTrans.position;
                
                switch (_playerCard.masterCardType)
                {
                    case CardType.Attack:
                        enemyController.ApplyAttack(_playerCard.masterCardPoint);
                        
                        // Animation
                        playerSpriteTrans.DOScale(new Vector3(1.2f, 1.2f, 1.2f), .2f);
                        playerSpriteTrans.DOMove(enemySpriteTrans.position, 0.15f).onComplete += () =>
                        {
                            playerSpriteTrans.DOMove(cachePos, 0.2f).SetDelay(.05f);
                            playerSpriteTrans.DOMoveZ(playerSpriteTrans.position.z + 2, .1f).SetDelay(.1f);
                        };
                        enemySpriteTrans.DOShakeRotation(duration: 1f, strength: 20f).SetDelay(.15f);
                        
                        break;
                    
                    case CardType.Defend:
                        playerController.ApplyDefend(_playerCard.masterCardPoint);
                        break;
                    
                    case CardType.Treat:
                        playerController.ApplyTreat(_playerCard.masterCardPoint);
                        break;
                    
                    case CardType.Default:
                        break;
                }
            }

            // Enemy's card applied
            else if (difference < 0)
            {
                _enemyCard.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), .2f);
                _enemyCard.transform.DOLocalMoveY(_enemyCard.transform.localPosition.y - 3f, .2f);
                enemySpriteTrans.transform.DOLocalMoveZ(-1, .2f);
                
                yield return new WaitForSeconds(.2f);

                Vector3 cachePos = enemySpriteTrans.position;

                switch (_enemyCard.masterCardType)
                {
                    case CardType.Attack:
                        playerController.ApplyAttack(_enemyCard.masterCardPoint);
                        
                        // Animation
                        enemySpriteTrans.DOScale(new Vector3(1.2f, 1.2f, 1.2f), .2f);
                        enemySpriteTrans.DOMove(playerSpriteTrans.position, 0.15f).onComplete += () => {
                            enemySpriteTrans.DOMove(cachePos, 0.2f).SetDelay(.05f);
                            enemySpriteTrans.DOMoveZ(enemySpriteTrans.position.z + 1, .2f).SetDelay(.1f);
                        };
                        playerSpriteTrans.DOShakeRotation(duration: 1f, strength: 20f).SetDelay(.15f);;
                        break;
                    
                    case CardType.Defend:
                        enemyController.ApplyDefend(_enemyCard.masterCardPoint);
                        break;
                    
                    case CardType.Treat:
                        enemyController.ApplyTreat(_enemyCard.masterCardPoint);
                        break;
                    
                    case CardType.Default:
                        break;
                }
            }
            
            // Draw
            else
            {
                Debug.Log("DRAW");
            }
            
            yield return new WaitForSeconds(1f);

            _playerCard.transform.DOLocalMoveY(-10f, .1f).onComplete += () =>
            {
                Destroy(_playerCard.gameObject);
            };

            _enemyCard.transform.DOLocalMoveY(10f, .1f).onComplete += () =>
            {
                Destroy(_enemyCard.gameObject);
            };
            
            playerController.spriteRendererComponent.transform.DOScale(Vector3.one, .2f);
            enemyController.spriteRendererComponent.transform.DOScale(Vector3.one, .2f);

            if (playerController.currentHealth <= 0)
            {
                HandleGameOver(false);
                yield break;
            }

            UIManager.Instance.InitPlayerUIInfo(playerController.currentHealth, playerController.currentArmor);
            UIManager.Instance.InitEnemyUIInfo(enemyController.currentHealth, enemyController.currentArmor);
            
            if (enemyController.currentHealth <= 0)
            {
                HandleGameOver(true);
                yield break;
            }

            StartCoroutine(SetupBattle());
        }

        private void RemoveCardsOnTheTable()
        {
            _playerCard.transform.DOLocalMoveY(-5f, .2f);
            _enemyCard.transform.DOLocalMoveY(10f, .2f);
        }

        private void HandleGameOver(bool isPlayerWin)
        {
            if (isPlayerWin)
            {
                UIManager.Instance.HandleWin();
            }
            else
            {
                UIManager.Instance.HandleLose();
            }
        }
    }
}