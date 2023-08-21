using System;
using UnityEngine;

namespace Runtime
{
    public class MasterCard : MonoBehaviour
    {
        public Card mainCard;

        public CardPair[] SeparatedCardPairs;
        
        [SerializeField] private Transform completeCardSlot;
        [SerializeField] private Transform separateCardSlot;

        private void Start()
        {
            InitCard();
        }

        private void InitCard()
        {
            completeCardSlot.gameObject.SetActive(true);
            separateCardSlot.gameObject.SetActive(false);
            
            SpawnMainCard();
            
        }

        private void SpawnMainCard()
        {
            Card spawnedCard = Instantiate(mainCard, completeCardSlot);
        }

        public void SeparateCard()
        {
            completeCardSlot.gameObject.SetActive(false);
            separateCardSlot.gameObject.SetActive(true);
        }
    }

    [Serializable]
    public struct CardPair
    {
        public Card separateCardA;
        public Card separateCardB;

    }
}
