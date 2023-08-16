using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatSliderUi : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private TMP_Text label;

    [SerializeField]
    private TMP_InputField valueLabel;

    public event Action<float> OnValueChanged;

    public void Init(string name, float minValue, float maxValue, float defaultValue)
    {
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.value = defaultValue;

        valueLabel.onValueChanged.AddListener((value) =>
        {
            float v = float.Parse(value);
            slider.value = v;
            OnValueChanged?.Invoke(v);
        });

        label.text = name;
        valueLabel.text = defaultValue.ToString("0.00");
        OnValueChanged?.Invoke(slider.value);
    }

    private void Start()
    {
        slider.onValueChanged.AddListener((value) =>
        {
            valueLabel.text = value.ToString("0.00");
            OnValueChanged?.Invoke(value);
        });
    }
}
