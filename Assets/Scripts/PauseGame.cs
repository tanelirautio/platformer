using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;

namespace pf
{
    public class PauseGame : MonoBehaviour
    {
        private Image fadeImage;
        private GameObject pauseBase;

        private void Awake()
        {
            fadeImage = transform.Find("Fade").GetComponentInChildren<Image>();
            pauseBase = transform.Find("PauseBase").gameObject;
        }

        void Start()
        {
            Reset();
        }

        private void OnDestroy()
        {
            print("Destroy all");
            DOTween.KillAll();
        }

        private void Reset()
        {
            fadeImage.DOFade(0, 0);
            pauseBase.transform.localScale = Vector3.zero;
            pauseBase.SetActive(false);
        }

        public void ShowPause()
        {
            FadeBackground();
            ShowPauseBase();
        }

        private void FadeBackground()
        {
            fadeImage.DOFade(0.8f, 1.0f);
        }

        private void ShowPauseBase()
        {
            pauseBase.SetActive(true);
            pauseBase.transform.DOScale(Defs.MENU_NORMAL_SCALE, 1.0f);
        }
    }
}
