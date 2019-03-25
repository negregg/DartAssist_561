﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Speech;
using Android.Util;
using Android.Widget;
using Android.Media;
using System;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Collections.Generic;
using System.Text;

using Android.Support.Design.Widget;
using Android.Support.V7.App;

namespace DartAssistant.Droid.Source.Activities
{
	//, MainLauncher = true
	[Activity(Label = "AndroidNavActivity")]
	public class AndroidNavActivity : AppCompatActivity
	{
		SpeechRecognizer Recognizer { get; set; }
		Intent SpeechIntent { get; set; }

		bool isListeningPaused = false;
		int seconds = 0;
		bool singleUse = false;

		private AudioManager mAudioManager;
		private int mStreamVolume = 0;

		System.Collections.Generic.List<string> mList = new System.Collections.Generic.List<string>();

		BottomNavigationView bottomNavigation;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.NavGame);

			var BtnStartSpeech = FindViewById<Android.Widget.Button>(Resource.Id.btn_start_game);
			BtnStartSpeech.Click += BtnStartSpeech_Click;

			var recListener = new RecognitionListener();
			recListener.BeginSpeech += RecListener_BeginSpeech;
			recListener.EndSpeech += RecListener_EndSpeech;
			recListener.Error += RecListener_Error;
			recListener.Ready += RecListener_Ready;
			recListener.Recognized += RecListener_Recognized;

			Recognizer = SpeechRecognizer.CreateSpeechRecognizer(this);
			Recognizer.SetRecognitionListener(recListener);

			SpeechIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
			SpeechIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
			SpeechIntent.PutExtra(RecognizerIntent.ExtraCallingPackage, PackageName);

			//Debug Button
			//var BtnEndSpeech = FindViewById<Android.Widget.Button>(Resource.Id.btn_end_speech);
			//BtnEndSpeech.Click += BtnEndSpeech_Click;
			//BtnEndSpeech.Visibility = Android.Views.ViewStates.Invisible;

			var BtnGetOut = FindViewById<Android.Widget.Button>(Resource.Id.btn_GetOut);
			BtnGetOut.Click += BtnGetOut_Click;
			var BtnClearOut = FindViewById<Android.Widget.Button>(Resource.Id.btn_Clear);
			BtnClearOut.Click += BtnClearOut_Click;

			var txtYourScore = FindViewById<Android.Widget.EditText>(Resource.Id.YourScore);
			txtYourScore.Click += txtYourScore_Click;

			//var BtnSeeOutChart = FindViewById<Android.Widget.Button>(Resource.Id.btn_SeeOutChart);
			//BtnSeeOutChart.Click += delegate {
			//    StartActivity(typeof(OutChartActivity));
			//};

			Log.Debug(nameof(AndroidActivity), nameof(OnCreate));
			mList.Add("Start");
			Forms.Init(this, savedInstanceState);

			mAudioManager = (AudioManager)GetSystemService(Context.AudioService);

			var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
			if (toolbar != null)
			{
				SetSupportActionBar(toolbar);
				SupportActionBar.SetDisplayHomeAsUpEnabled(false);
				SupportActionBar.SetHomeButtonEnabled(false);

			}

			bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);


			bottomNavigation.NavigationItemSelected += BottomNavigation_NavigationItemSelected;
		}

		private void BottomNavigation_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
		{
			LoadFragment(e.Item.ItemId);
		}

		void LoadFragment(int id)
		{
			Android.Support.V4.App.Fragment fragment = null;
			switch (id)
			{
				case Resource.Id.menu_home:
					break;
				case Resource.Id.menu_audio:
					StartActivity(typeof(OutChartNavActivity));
					break;
				case Resource.Id.menu_video:
					StartActivity(typeof(Activity3));
					break;
				case Resource.Id.menu_scores:
					StartActivity(typeof(Activity4));
					break;
				case Resource.Id.menu_info:
					StartActivity(typeof(Activity5));
					break;
			}
			if (fragment == null)
				return;

			SupportFragmentManager.BeginTransaction()
			   .Replace(Resource.Id.content_frame, fragment)
			   .Commit();
		}

		private void BtnStartSpeech_Click(object sender, System.EventArgs e)
		{
			singleUse = false;

			Recognizer.StartListening(SpeechIntent);

			mStreamVolume = mAudioManager.GetStreamVolume(Stream.Music); // getting system volume into var for later un-muting 

			if (mStreamVolume == 0)
			{
				mStreamVolume = 9;
			}

			StartTimer();
		}

		private void txtYourScore_Click(object sender, System.EventArgs e)
		{
			EditText eText = (EditText)sender;
			eText.Text = "";
		}

		//Debug Button
		//private void BtnEndSpeech_Click(object sender, System.EventArgs e)
		//{
		//	foreach (String s in mList)
		//	{
		//		Log.Debug("BtnEndSpeech_Click", s);
		//	}

		//}

		private void BtnGetOut_Click(object sender, System.EventArgs e)
		{
			var txtOutLabel = FindViewById<Android.Widget.TextView>(Resource.Id.txtOutLabel);
			var txtYourScore = FindViewById<Android.Widget.EditText>(Resource.Id.YourScore);

			string scoreStr = txtYourScore.Text;
			Int16 score = 0;
			string text = "";

			bool Result = false;
			Result = Int16.TryParse(scoreStr, out score);

			//If not an Int
			if (true != Result)
			{
				text = "Invalid Out Number";
			}
			else
			{
				if (170 < score | 2 > score)
				{
					text = "Invalid Out Number";
				}
				else
				{
					text = GetAbbrevOut(score);
				}

			}
			if ("" == text)
			{
				text = "Unknown Out";
			}
			else
			{
				txtOutLabel.Text = text;
			}

		}
		private void BtnClearOut_Click(object sender, System.EventArgs e)
		{
			var txtOutLabel = FindViewById<Android.Widget.TextView>(Resource.Id.txtOutLabel);
			var txtYourScore = FindViewById<Android.Widget.EditText>(Resource.Id.YourScore);

			txtOutLabel.Text = "";
			txtYourScore.Text = "";

		}

		private void RecListener_Ready(object sender, Bundle e)
		{
			Log.Debug(nameof(AndroidActivity), nameof(RecListener_Ready));
			seconds = 1;
			mList.Add("RecListener_Ready");
			//Uncomment This to mute the tart/sotp listening tones
			//mAudioManager.SetStreamVolume(Stream.Music, 0, 0); // setting system volume to zero, muting

		}

		private void RecListener_BeginSpeech()
		{
			Log.Debug(nameof(AndroidActivity), nameof(RecListener_BeginSpeech));
			seconds = 2;
			mList.Add("RecListener_BeginSpeech");
		}
		private void RecListener_EndSpeech()
		{

			Log.Debug(nameof(AndroidActivity), nameof(RecListener_EndSpeech));

			if (!isListeningPaused)
			{
				seconds = -10;
			}

			mList.Add("RecListener_EndSpeech");
		}

		private void RecListener_Error(object sender, SpeechRecognizerError e)
		{
			Log.Debug(nameof(AndroidActivity), $"{nameof(RecListener_Error)}={e.ToString()}");
			mList.Add("RecListener_Error: " + e.ToString());

			if (!isListeningPaused)
			{
				seconds = -10;
			}
		}

		private void RecListener_Recognized(object sender, string recognized)
		{
			Log.Debug(nameof(AndroidActivity), nameof(RecListener_Recognized));

			var txtYourScore = FindViewById<Android.Widget.EditText>(Resource.Id.YourScore);

			// this method called when Speech Recognition is ready
			// also this is the right time to un-mute system volume because the annoying sound played already
			mAudioManager = (AudioManager)GetSystemService(Context.AudioService);
			mAudioManager.SetStreamVolume(Stream.Music, mStreamVolume, 0); // again setting the system volume back to the original, un-mutting

			Int16 TotalScore = 0;
			Int16 SingleScore = 0;
			string strRecommendedOut = "";
			string strAbbrevOut = "";

			string fmtInput = recognized.Trim().ToLower();

			if ("pause" == fmtInput.ToLower() || "stop" == fmtInput.ToLower())
			{
				seconds = 1;
				isListeningPaused = true;

				txtYourScore.Text = "stopped";
				Toast.MakeText(this, "stopped", ToastLength.Long).Show();

				return;
			}

			txtYourScore.Text = recognized;
			Toast.MakeText(this, recognized, ToastLength.Long).Show();

			if (fmtInput.Contains("out") && recognized.ToLower().IndexOf("out") > 1)
			{
				fmtInput = recognized.ToLower().Substring(0, (recognized.ToLower().IndexOf("out") - 1));

				bool Result = false;
				Result = Int16.TryParse(fmtInput, out TotalScore);

				//If not an Int set an out of range value for Total Score
				if (true != Result)
				{
					TotalScore = 200;
					strRecommendedOut = "Unknown Out";
				}
				else
				{
					strRecommendedOut = RecommendedOut(TotalScore);
					//strAbbrevOut = GetAbbrevOut(TotalScore);
				}

			}
			else if (fmtInput.Contains("score") && recognized.ToLower().IndexOf("score") > 1)
			{
				//TODO 
				//implement this after establishing total score tracking
				fmtInput = recognized.ToLower().Substring(0, (recognized.ToLower().IndexOf("score") - 1));

				bool Result = false;
				Result = Int16.TryParse(fmtInput, out SingleScore);

				// If not an Int set an out of range value for Total Score
				if (true != Result)
				{
					SingleScore = -1;
					strRecommendedOut = "Unknown Score";
				}
				else
				{
					strRecommendedOut = RecommendedOut(SingleScore);
					//strAbbrevOut = GetAbbrevOut(SingleScore);
				}

			}

			var txtOutLabel = FindViewById<Android.Widget.TextView>(Resource.Id.txtOutLabel);
			//txtOutLabel.Text = strAbbrevOut;
			txtOutLabel.Text = strRecommendedOut;

			TextToSpeech.SpeakAsync(strRecommendedOut);

			mList.Add("RecListener_Recognized");

			if (singleUse)
			{
				seconds = 1;
				isListeningPaused = true;
			}
			if (!isListeningPaused)
			{
				seconds = -5;
			}
		}

		private string RecommendedOut(int TotalOut)

		//This gets the out from the classes and vocalizes the words 
		{

			int dartCount = 0;

			OutCalculator clsOutCalc = new OutCalculator(InOutRule.Double);
			List<Dart> recOut = clsOutCalc.GetDartsForOut(TotalOut);

			StringBuilder sb = new StringBuilder();

			foreach (Dart mDart in recOut)
			{
				sb.Append(recOut[dartCount].ToString() + ", ");
				dartCount++;
			}

			string strOutText = "";

			if (0 == sb.Length)
			{
				strOutText = "Out Not Found";
			}
			else
			{
				sb = sb.Remove(sb.Length - 2, 2);
				strOutText = sb.ToString();
			}

			return strOutText;

		}

		private string GetAbbrevOut(int TotalOut)
		{

			int score = TotalOut;
			OutCalculator outCalculator = new OutCalculator(InOutRule.Double);
			List<Dart> outs = outCalculator.GetDartsForOut(score);

			StringBuilder stringBuilder = new StringBuilder();

			for (int i = 0; i < outs.Count; i++)
			{
				stringBuilder.Append(outs[i].Abbreviation);
				if (i != outs.Count - 1)
				{
					stringBuilder.Append(", ");
				}
			}

			string text = stringBuilder.ToString();

			return text;
		}
		private bool UpdateDateTime()
		{
			Log.Debug(nameof(AndroidActivity), "UpdateDateTime(): secs=" + seconds.ToString());

			if (seconds < -5 || seconds == 0)
			{
				Recognizer.StartListening(SpeechIntent);
				return true;
			}

			if (!isListeningPaused)
			{
				seconds += 1;
			}

			return true;
		}

		void StartTimer()
		{
			//Initiate the Timer
			Device.StartTimer(TimeSpan.FromSeconds(1), UpdateDateTime);

		}
	}
}