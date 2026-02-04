using Android.App;
using Android.Runtime;

namespace nfc_test_app
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override void OnCreate()
        {
            base.OnCreate();

            // Initialize NFC Plugin at application level
            Plugin.NFC.CrossNFC.Init(this);
        }
        
    }
}
