using System;

using Xamarin.Forms;

namespace CosmosHack
{
    public class ViewOlivePage : ContentPage
    {
        public ViewOlivePage()
        {
            var nameLabel = new Label();
            var furColorLabel = new Label();
            var getDataButton = new Button { Text = "Get data from Cosmos" };

            Content = new StackLayout
            {
                Padding = 20,
                VerticalOptions = LayoutOptions.Center,
                Children = {
                    nameLabel,
                    furColorLabel,
                    getDataButton
                }
            };

            getDataButton.Clicked += async (object sender, EventArgs e) =>
            {
                var dog = await DocumentDbService.GetDogByIdAsync("1");

                if (dog != null)
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        nameLabel.Text = dog.Name;
                        furColorLabel.Text = dog.FurColor;
                    });
            };
        }
    }
}