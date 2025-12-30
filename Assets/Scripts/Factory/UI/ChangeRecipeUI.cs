using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSM.Surveillance.UI
{
    public class ChangeRecipeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RectTransform contentTransform;
        [SerializeField] private RecipeSelectionUI recipeSelectionPrefab;
        
        private ProcessorUI _parent;
        private ProcessorInstance _processor;

        public void Load(ProcessorInstance processor)
        {
            _processor = processor;
            
            DestroyAllChildren();
            foreach (var recipe in _processor.AvailableRecipes)
            {
                var r = Instantiate(recipeSelectionPrefab, contentTransform);
                r.Load(recipe, this);
            }
        }

        private void DestroyAllChildren()
        {
            for (int i = contentTransform.childCount - 1; i >= 0; i--)
            {
                Destroy(contentTransform.GetChild(i).gameObject);
            }
        }
        public void SelectRecipe(Recipe recipe)
        {
            _processor.SetRecipe(recipe);
            _parent.Reload();
            gameObject.SetActive(false);
        }
        
        private void Awake()
        {
            _parent ??= GetComponentInParent<ProcessorUI>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _parent?.SetInside(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _parent?.SetInside(false);
        }
    }
}