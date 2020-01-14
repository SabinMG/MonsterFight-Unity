using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace SpaceOrigin.MonsterFight
{
    public class HealthBarUI : MonoBehaviour
    {
        public Slider healthSlider;
        public void SetValue(float value) // expecting normalized value here 0 to 1
        {
            healthSlider.value = value;
        }
    }
}
