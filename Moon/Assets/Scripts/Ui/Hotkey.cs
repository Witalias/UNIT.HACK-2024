using TMPro;
using UnityEngine;

public class Hotkey : MonoBehaviour
{
    [SerializeField] private TMP_Text _keyText;
    [SerializeField] private TMP_Text _description;

    public void SetData(Data data)
    {
        _keyText.text = data.Key;
        _description.text = data.Description;
    }

    public struct Data
    {
        public string Key { get; set; }
        public string Description { get; set; }
    }
}
