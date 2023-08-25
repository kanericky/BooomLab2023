using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    public class CardHolder : MonoBehaviour
    {
        [SerializeField] private float yOffset = -6;
        [SerializeField] private float cardGap = 1;

        private void OnValidate()
        {
            SortChildTransforms();
        }

        public void InitCardHolderPos()
        {
            transform.position += new Vector3(0, yOffset, 0);
        }

        private void SortChildTransforms()
        {
            int childTransformCount = transform.childCount;

            int pivotIndex = childTransformCount / 2;

            for (int x = 0; x < childTransformCount; x++)
            {
                Transform currentTransform = transform.GetChild(x);
                MasterCard card = currentTransform.GetComponent<MasterCard>();
                
                currentTransform.position = Vector3.zero;
                Vector3 originPos = currentTransform.position;

                float xOffset = cardGap * (x - pivotIndex);
                float yOffset = 0;

                if (card != null)
                {
                    yOffset = - card.cardHeight / 2;
                }
                
                currentTransform.localPosition = new Vector3(originPos.x + xOffset, originPos.y + yOffset, originPos.z);
            }
        }

        public void AddCardToCardHolder(Transform card)
        {
            card.parent = transform;
            SortChildTransforms();
        }

        public void AddCardToEnemyCardHolder(Transform card)
        {
            card.parent = transform;
            //card.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
            SortChildTransforms();
        }

        public void ShowAllCards()
        {
            transform.DOLocalMoveY(transform.localPosition.y - yOffset, .2f);
        }

        public void RemoveOtherCards()
        {
            int childTransformCount = transform.childCount;

            for (int i = 0; i < childTransformCount; i++)
            {
                Transform card = transform.GetChild(i);
                StartCoroutine(IERemoveOtherCards(card));
            }
        }

        IEnumerator IERemoveOtherCards(Transform card)
        {
            yield return new WaitForSeconds(.2f);

            card.DOLocalMoveY(yOffset, .5f).onComplete += () =>
            {
                Destroy(card.gameObject);
            };
        }
    }
}