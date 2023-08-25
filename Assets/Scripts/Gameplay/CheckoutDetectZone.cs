using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class CheckoutDetectZone : MonoBehaviour
    {

        private void OnTriggerStay2D(Collider2D other)
        {
            MasterCard card = other.GetComponentInParent<MasterCard>();

            if (card == null) return;

            if (card.masterCardStatus == CardStatus.Complete || card.masterCardStatus == CardStatus.Combined)
            {
                card.SetCanCheckout(true);
            }

        }

        private void OnTriggerExit2D(Collider2D other)
        {
            MasterCard card = other.GetComponentInParent<MasterCard>();

            if (card.masterCardStatus == CardStatus.Complete || card.masterCardStatus == CardStatus.Combined)
            {
                card.SetCanCheckout(false);
            }

        }
    }
}