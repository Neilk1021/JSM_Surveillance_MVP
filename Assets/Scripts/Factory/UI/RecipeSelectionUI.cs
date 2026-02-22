using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JSM.Surveillance.UI
{
    public class RecipeSelectionUI : MonoBehaviour
    {
        [SerializeField] private Image image; 
        [SerializeField] private TextMeshProUGUI recipeName;

        private Recipe _recipe;
        private ChangeRecipeUI _parent;
        public void Load(Recipe recipe, ChangeRecipeUI changeRecipeUI)
        {
            _recipe = recipe;
            _parent = changeRecipeUI;

            recipeName.text = _recipe.RecipeName;
            image.sprite = recipe.OutputVolume.resource.Sprite;
        }

        public void Select()
        {
            _parent.SelectRecipe(_recipe);
        }
    }
}