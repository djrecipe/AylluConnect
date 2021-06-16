using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using System.Collections.Generic;
using System.Linq;
using Android;
using Android.Widget;

namespace AbaciAndroid.Droid
{
    [Activity(Label = "AbaciAndroid", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		private readonly int REQUEST_BLUETOOTH_ID = 0;
		private readonly string[] PERMISSIONS_BLUETOOTH =
		{
			Manifest.Permission.AccessCoarseLocation,
			Manifest.Permission.AccessFineLocation,
			Manifest.Permission.Bluetooth,
			Manifest.Permission.BluetoothAdmin
		};
		private readonly string ERROR_BLE_PERMISSIONS = "Bluetooth permissions are required in order to scan and connect to XBee BLE devices.";
		/// <summary>
		/// Checks if the minimum permissions required to execute the 
		/// application are granted. If not, requests them.
		/// </summary>
		private void CheckPermissions()
		{
			List<string> permissionsToRequest = new List<string>();
			foreach (string permission in PERMISSIONS_BLUETOOTH)
			{
				if (CheckSelfPermission(permission) != Permission.Granted)
					permissionsToRequest.Add(permission);
			}
			// If the minimum permissions are not granted, request them to the user.
			if (permissionsToRequest.Count > 0)
				RequestPermissions(permissionsToRequest.ToArray(), REQUEST_BLUETOOTH_ID);
		}
		protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
			CheckPermissions();

			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
		{
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			if (requestCode != REQUEST_BLUETOOTH_ID)
				return;

			// Check if user declined any Bluetooth permission.
			bool anyPermissionDenied = false;
			foreach (var result in grantResults)
			{
				if (result == Permission.Denied)
				{
					anyPermissionDenied = true;
					break;
				}
			}
			// Display a message indicating that bluetooth permissions must be enabled.
			if (anyPermissionDenied)
				Toast.MakeText(this, ERROR_BLE_PERMISSIONS, ToastLength.Long).Show();
		}
    }
}