using Xamarin.Forms;

namespace CosmosHack
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new ViewOlivePage();
        }
    }
}
