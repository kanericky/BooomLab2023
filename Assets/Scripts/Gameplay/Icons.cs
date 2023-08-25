using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    public class Icons : MonoBehaviour, IUxSetup
    {
        public void UxSetup()
        {
            transform.position += new Vector3(10, 0, 0);
            transform.DOMoveX(transform.position.x - 10, .4f);
        }
    }
}