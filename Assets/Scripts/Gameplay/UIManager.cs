using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

namespace Gameplay
{
    
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [Header("Texts")] 
        [SerializeField] private TMP_Text playerHealthText;
        [SerializeField] private TMP_Text playerArmorText;
        [SerializeField] private TMP_Text enemyHealthText;
        [SerializeField] private TMP_Text enemyArmorText;


        [SerializeField] private Transform winGroup;
        [SerializeField] private Image winBg;
        
        [SerializeField] private Transform loseGroup;
        [SerializeField] private Image loseBg;

        private void Start()
        {
            Instance = this;
            
            winGroup.gameObject.SetActive(false);
            loseGroup.gameObject.SetActive(false);
        }

        public void InitPlayerUIInfo(float playerHealth, float playerArmor)
        {
            playerHealthText.text = playerHealth.ToString();
            playerArmorText.text = playerArmor.ToString();
        }

        public void InitEnemyUIInfo(float enemyHealth, float enemyArmor)
        {
            enemyHealthText.text = enemyHealth.ToString();
            enemyArmorText.text = enemyArmor.ToString(); 
        }

        public void HandleWin()
        {
            winGroup.gameObject.SetActive(true);
            var localScale = winBg.transform.localScale;
            localScale = new Vector3(localScale.x, 0, localScale.y);
            winBg.transform.localScale = localScale;

            winBg.transform.DOScaleY(1f, .4f).SetEase(Ease.OutCirc);;
            
            StartCoroutine(ReturnToMain());
        }

        public void HandleLose()
        {
            loseGroup.gameObject.SetActive(true);
            var localScale = loseBg.transform.localScale;
            localScale = new Vector3(localScale.x, 0, localScale.y);
            loseBg.transform.localScale = localScale;

            loseBg.transform.DOScaleY(1f, .4f).SetEase(Ease.OutCirc);

            StartCoroutine(ReturnToMain());
        }

        IEnumerator ReturnToMain()
        {
            yield return new WaitForSeconds(5f);
            GameManager.Instance.LoadScene(0);
        }
    }
}