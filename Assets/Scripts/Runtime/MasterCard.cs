using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime
{
    public class MasterCard : MonoBehaviour
    {
        public Card mainCard;

        public CardPair[] separatedCardPairs;
        
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
            int randomSelectIndex = Random.Range(0, separatedCardPairs.Length);
            CardPair randomSelectedCardPair = separatedCardPairs[randomSelectIndex];
            
            Card separatedCardA = randomSelectedCardPair.separateCardA;
            Card separatedCardB = randomSelectedCardPair.separateCardB;

            Card spawnedSeparatedCardA = Instantiate(separatedCardA, separateCardSlot);
            Card spawnedSeparatedCardB = Instantiate(separatedCardB, separateCardSlot);

            spawnedSeparatedCardA.transform.localPosition += new Vector3(0f, (spawnedSeparatedCardB.CardHeight + spawnedSeparatedCardA.CardHeight) / 2 + .5f, 0f);

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
