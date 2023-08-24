using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime
{
    public class Deck : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private CardData[] deckCardDataArray;
        [SerializeField] private MasterCard masterCardTemplate;

        public CardData[] RequestCardsFromDeck(int cardNums)
        {
            if (cardNums > deckCardDataArray.Length)
            {
                Debug.LogError("Request card amount is larger than deck amount");
                return null;
            }
            
            CardData[] resultCardDataArray = new CardData[cardNums];
            List<CardData> deckCardDataList = deckCardDataArray.ToList();

            for (int i = 0; i < cardNums; i++)
            {
                int randomSelectedIndex = Random.Range(0, deckCardDataList.Count);
                resultCardDataArray[i] = deckCardDataList[randomSelectedIndex];
                deckCardDataList.RemoveAt(randomSelectedIndex);
            }

            return resultCardDataArray;
        }

        public CardType[] RequestCardTypes(int cardNums)
        {
            if (cardNums > deckCardDataArray.Length)
            {
                Debug.LogError("Request card amount is larger than deck amount");
                return null;
            }

            CardType[] resultCartTypeArray = new CardType[cardNums];

            List<CardType> actionPool = new List<CardType>() { CardType.Attack, CardType.Defend, CardType.Treat };
            List<CardType> normalPool = new List<CardType>()
            {
                CardType.Attack, 
                CardType.Defend, 
                CardType.Treat, 
                CardType.Default,
                CardType.Default,
                CardType.Default,
                CardType.Default,
                CardType.Default
            };

            resultCartTypeArray[0] = actionPool[Random.Range(0, 3)];
            
            for (int i = 1; i < cardNums; i++)
            {
                int randomSelectedIndex = Random.Range(0, normalPool.Count);
                resultCartTypeArray[i] = normalPool[randomSelectedIndex];
            }

            return resultCartTypeArray;
        }
        
    }
}