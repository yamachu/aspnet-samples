using JSComponentGeneration.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace JSComponentGeneration.Vue
{
    public static class JSComponentConfigurationExtensions
    {
        public static void RegisterForVue<TComponent>(this IJSComponentConfiguration configuration) where TComponent : IComponent
        {
            var typeNameKebabCase = CasingUtilities.ToKebabCase(typeof(TComponent).Name);
            configuration.RegisterForJavaScript<TComponent>($"{typeNameKebabCase}-vue");
        }
    }
}
