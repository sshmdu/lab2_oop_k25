namespace lab2;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override bool OnBackButtonPressed()
    {
        ConfirmExitOnBack();
        return true;
    }

    private async void ConfirmExitOnBack()
    {
        bool answer = await DisplayAlert(
            "Exit confirmation",
            "Do you really want to exit the program?",
            "Yes",
            "No");

        if (answer)
        {
            Application.Current?.Quit();
        }
    }
}