using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay
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

        public override void UxSetup()
        {
            transform.position += new Vector3(0, 3, 0);
            transform.DOMoveY(transform.position.y - 3, .2f);
        }
    }
}