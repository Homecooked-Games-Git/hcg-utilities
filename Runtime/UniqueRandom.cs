namespace HCGames.Utilities
{
    using UnityEngine;
    using System.Collections.Generic;

    public class UniqueRandom<T>
    {
        private List<T> _availableItems;
        private readonly List<T> _originalItems;

        public UniqueRandom(IEnumerable<T> items)
        {
            _originalItems = new List<T>(items); // Create a copy of the list
            ResetItems();
        }

        private void ResetItems()
        {
            _availableItems = new List<T>(_originalItems); // Reset to the original list
        }

        public T Next()
        {
            if (_availableItems.Count == 0)
            {
                ResetItems();
            }
            var index = Random.Range(0, _availableItems.Count);
            var selectedItem = _availableItems[index];
            _availableItems.RemoveAt(index);
            return selectedItem;
        }
    }
}