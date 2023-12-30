using TMPro;
using UnityEngine;

namespace FunCraftersTask.Entities
{
    public class ItemEntity : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _numberText;
        [SerializeField] private TextMeshProUGUI _descriptionText;

        public void Setup(DataItem data, int index)
        {
            int itemNumber = index + 1;
            _numberText.text = itemNumber.ToString();
            _descriptionText.text = data.Description;
        }
    }
}