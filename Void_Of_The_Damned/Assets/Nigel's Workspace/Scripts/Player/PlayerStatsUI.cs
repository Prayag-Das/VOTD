using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;



public class PlayerStatsUI : MonoBehaviour
{
    public Slider sanitySlider;
    public Slider oxygenSlider;
    private SanitySystem sanitySystem;
    private OxygenSystem oxygenSystem;

    private void Start()
    {
        sanitySystem = FindFirstObjectByType<SanitySystem>();
        oxygenSystem = FindFirstObjectByType<OxygenSystem>();

        if (sanitySlider != null && sanitySystem != null)
        {
            sanitySlider.maxValue = sanitySystem.GetMaxSanity();
            sanitySlider.value = sanitySystem.GetCurrentSanity();
        }

        if (oxygenSlider != null && oxygenSystem != null)
        {
            oxygenSlider.maxValue = oxygenSystem.GetMaxOxygen();
            oxygenSlider.value = oxygenSystem.GetCurrentOxygen();
        }
    }

    private void Update()
    {
        if (sanitySystem != null)
            sanitySlider.value = sanitySystem.GetCurrentSanity();

        if (oxygenSystem != null && oxygenSlider.gameObject.activeSelf)
            oxygenSlider.value = oxygenSystem.GetCurrentOxygen();
    }
}