using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public class Deck : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private CardData[] playerDeckCardDataArray;
        [SerializeField] private CardData[] enemyDeckCardDataArray;
        [SerializeField] private MasterCard masterCardTemplate;

        public CardData[] RequestCardsFromDeck(bool isEnemy, int cardNums)
        {
            CardData[] deckData = isEnemy ? enemyDeckCardDataArray : playerDeckCardDataArray;
            
            if (cardNums > deckData.Length)
            {
                Debug.LogError("Request card amount is larger than deck amount");
                return null;
            }
            
            CardData[] resultCardDataArray = new CardData[cardNums];
            List<CardData> deckCardDataList = deckData.ToList();

            for (int i = 0; i < cardNums; i++)
            {
                int randomSelectedIndex = Random.Range(0, deckCardDataList.Count);
                resultCardDataArray[i] = deckCardDataList[randomSelectedIndex];
                
                for (int j = deckCardDataList.Count - 1; j > -1; j-- )
                {
                    CardData card = deckCardDataList[j];
                    if (card == resultCardDataArray[i])
                    {
                        deckCardDataList.Remove(card);
                    }
                }
            }

            return resultCardDataArray;
        }

        public CardType[] RequestCardTypes(bool isEnemy, int cardNums)
        {
            CardData[] deckData = isEnemy ? enemyDeckCardDataArray : playerDeckCardDataArray;
            
            if (cardNums > deckData.Length)
            {
                Debug.LogError("Request card amount is larger than deck amount");
                return null;
            }

            CardType[] resultCartTypeArray = new CardType[cardNums];

            if (!isEnemy)
            {
                List<CardType> actionPool = new List<CardType>() {CardType.Attack, CardType.Defend, CardType.Treat};
                List<CardType> normalPool = new List<CardType>()
                {
                    CardType.Attack,
                    CardType.Attack,
                    CardType.Defend,
                    CardType.Treat,
                    CardType.Default,
                    CardType.Default,
                };

                resultCartTypeArray[0] = actionPool[Random.Range(0, 3)];

                for (int i = 1; i < cardNums; i++)
                {
                    int randomSelectedIndex = Random.Range(0, normalPool.Count);
                    resultCartTypeArray[i] = normalPool[randomSelectedIndex];
                }
            }
            else
            {
                List<CardType> normalPool = new List<CardType>()
                {
                    CardType.Attack,
                    CardType.Attack,
                    CardType.Attack,
                    CardType.Defend,
                    CardType.Treat,
                    CardType.Treat,
                    CardType.Default,
                };

                for (int i = 1; i < cardNums; i++)
                {
                    int randomSelectedIndex = Random.Range(0, normalPool.Count);
                    resultCartTypeArray[i] = normalPool[randomSelectedIndex];
                }
            }

            return resultCartTypeArray;
        }
        
    }
}