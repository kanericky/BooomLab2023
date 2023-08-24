using System;
using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(fileName = "Card Data A", menuName = "Card Data", order = 0)]
    public class CardData : ScriptableObject
    {
        [Header("Data - Numeric")] 
        public int cardPoint;
        public CardStatus cardStatus;
        
        [Header("Data - Art")] 
        public Sprite[] cardSprites;

        [Header("Data - Separate Cards")] 
        public CardPair[] SeparatedCardPairs;

        private void OnValidate()
        {
            if (cardStatus != CardStatus.Complete) SeparatedCardPairs = null;
        }
    }

    [Serializable]
    public struct CardPair
    {
        public CardData SeparatedCardDataA;
        public CardData SeparatedCardDataB;

    }
}