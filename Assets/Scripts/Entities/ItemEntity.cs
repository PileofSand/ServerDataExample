using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FunCraftersTask.Entities
{
    public class ItemEntity : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _numberText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private GameObject _glow;
        [SerializeField] private Image _badgeIcon;
        [SerializeField] private Sprite _redBadgeSprite;
        [SerializeField] private Sprite _greenBadgeSprite;
        [SerializeField] private Sprite _blueBadgeSprite;

        public void Setup(DataItem data, int index)
        {
            int itemNumber = index + 1;
            _numberText.text = itemNumber.ToString();
            _descriptionText.text = data.Description;
            
            switch (data.Category)
            {
                case DataItem.CategoryType.RED:
                    _badgeIcon.sprite = _redBadgeSprite;
                    break;
                case DataItem.CategoryType.GREEN:
                    _badgeIcon.sprite = _greenBadgeSprite;
                    break;
                case DataItem.CategoryType.BLUE:
                    _badgeIcon.sprite = _blueBadgeSprite;
                    break;
            }
            
            _glow.SetActive(data.Special);
        }
    }
}