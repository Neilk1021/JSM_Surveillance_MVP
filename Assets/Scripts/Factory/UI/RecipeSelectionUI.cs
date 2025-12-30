using TMPro;
using UnityEngine;

namespace JSM.Surveillance.UI
{
    public class RecipeSelectionUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI recipeName;

        private Recipe _recipe;
        private ChangeRecipeUI _parent;
        public void Load(Recipe recipe, ChangeRecipeUI changeRecipeUI)
        {
            _recipe = recipe;
            _parent = changeRecipeUI;

            recipeName.text = _recipe.RecipeName;
        }

        public void Select()
        {
            _parent.SelectRecipe(_recipe);
        }
    }
}