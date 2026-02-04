using Android.App;
using Android.Content.PM;
using Android.Nfc;
using Android.OS;
using Android.Content; // Asegura que el tipo Intent esté disponible

namespace nfc_test_app
{
    [Activity(Theme = "@style/Maui.SplashTheme",
          MainLauncher = true,
          LaunchMode = LaunchMode.SingleTop,
          ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    [IntentFilter(new[] { NfcAdapter.ActionNdefDiscovered }, Categories = new[] { Intent.CategoryDefault })]
    [IntentFilter(new[] { NfcAdapter.ActionTagDiscovered }, Categories = new[] { Intent.CategoryDefault })]
    [IntentFilter(new[] { NfcAdapter.ActionTechDiscovered }, Categories = new[] { Intent.CategoryDefault })]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Initialize NFC Plugin
            Plugin.NFC.CrossNFC.Init(this);
        }

        protected override void OnResume()
        {
            base.OnResume();

            // Initialize NFC Plugin on resume
            Plugin.NFC.CrossNFC.OnResume();
        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);

            // Handle NFC intent
            Plugin.NFC.CrossNFC.OnNewIntent(intent);
        }
    }
}
