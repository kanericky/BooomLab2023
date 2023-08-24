using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime
{
    public class Enemy : Player
    {
        public override void AddCardToHand(MasterCard card)
        {
            cardHolderComponent.AddCardToEnemyCardHolder(card.transform);
        }

        public MasterCard ChooseCard()
        {
            int i = cardHolderComponent.transform.childCount;
            int randomSelectedIndex = Random.Range(0, i);

            MasterCard card = cardHolderComponent.transform.GetChild(randomSelectedIndex).GetComponent<MasterCard>();
            card.transform.parent = transform;

            return card;
        }
    }
}