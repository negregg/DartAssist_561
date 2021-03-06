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
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

using Android.Support.Design.Widget;
using Android.Support.V7.App;

namespace DartAssistant.Droid.Source.Activities
{
	//, MainLauncher = true
	[Activity(Label = "@string/app_name", MainLauncher = true)]
	public class AndroidNavActivity : AppCompatActivity
	{
		SpeechRecognizer Recognizer { get; set; }
		Intent SpeechIntent { get; set; }

		bool isListeningPaused = false;
		int seconds = 0;
		bool singleUse = false;

		private AudioManager mAudioManager;
		private int mStreamVolume = 0;

		//Variables for Maintaining State
		int startingScore = 0;
		//int LastScore = 0;
		int currentScore = 0;
		//string currentScoreText = "";
		//string doubleOutText = "";
		//int dartsRemaining = 0;
		//int turnState = 0;
		string turnClassSerial = "";
		string UIClassSerial = "";

		System.Collections.Generic.List<string> mList = new System.Collections.Generic.List<string>();

		BottomNavigationView bottomNavigation;

		Turn clsTurn = new Turn(InOutRule.Double);
		UIState clsUIState = new UIState();

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.NavGame);

			Log.Debug(GetType().FullName, "Activity A - OnCreate");

			turnClassSerial = Intent.GetStringExtra("turnClassSerial");
			System.Diagnostics.Debug.Print("-" + turnClassSerial);
			UIClassSerial = Intent.GetStringExtra("UIClassSerial");
			System.Diagnostics.Debug.Print("-" + UIClassSerial);

			var txtDartScore = FindViewById<Android.Widget.EditText>(Resource.Id.DartScore);
			var txtStartScore = FindViewById<Android.Widget.EditText>(Resource.Id.StartScore);
			var txtNewOut = FindViewById<Android.Widget.TextView>(Resource.Id.txtNewScore);
			var txtOutTurn = FindViewById<Android.Widget.TextView>(Resource.Id.txtOutTurn);

			//Saved Instance Takes precedence
			if (savedInstanceState != null)
			{
				string savedTurn = savedInstanceState.GetString("jsonSerialTurn", "");
				clsTurn = JsonConvert.DeserializeObject<Turn>(savedTurn);

				System.Diagnostics.Debug.Print("-" + clsTurn.CurrentScore.ToString());

				string savedUIState = savedInstanceState.GetString("jsonSerialUIState", "");
				clsUIState = JsonConvert.DeserializeObject<UIState>(savedUIState);

				System.Diagnostics.Debug.Print("-" + clsUIState.LastScore.ToString());

				if (TurnState.InProgress == clsTurn.State)
				{
					txtDartScore.Text = clsUIState.LastScore.ToString();
					txtStartScore.Text = clsTurn.StartingScore.ToString();
					txtNewOut.Text = clsUIState.CurrentScoreText.ToString();
					txtOutTurn.Text = clsUIState.DoubleOutText.ToString();

					currentScore = clsTurn.CurrentScore;
				}
				else
				{
					txtDartScore.Text = "";
					txtStartScore.Text = "";
					txtNewOut.Text = "";
					txtOutTurn.Text = "";

					currentScore = 0;
				}

				Log.Debug(GetType().FullName, "AndroidNavActivity - Recovered instance state");
			}
			else if (null != turnClassSerial && 0 < turnClassSerial.Trim().Length)
			{
				clsTurn = JsonConvert.DeserializeObject<Turn>(turnClassSerial);

				System.Diagnostics.Debug.Print("-" + clsTurn.CurrentScore.ToString());

				clsUIState = JsonConvert.DeserializeObject<UIState>(UIClassSerial);

				System.Diagnostics.Debug.Print("-" + clsUIState.LastScore.ToString());

				if (TurnState.InProgress == clsTurn.State)
				{
					txtDartScore.Text = clsUIState.LastScore.ToString();
					txtStartScore.Text = clsTurn.StartingScore.ToString();
					txtNewOut.Text = clsUIState.CurrentScoreText.ToString();
					txtOutTurn.Text = clsUIState.DoubleOutText.ToString();

					currentScore = clsTurn.CurrentScore;
				}
				else
				{
					txtDartScore.Text = "";
					txtStartScore.Text = "";
					txtNewOut.Text = "";
					txtOutTurn.Text = "";

					currentScore = 0;
				}

				Log.Debug(GetType().FullName, "AndroidNavActivity - Loaded Serial");
			}

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

			var BtnStartTurn = FindViewById<Android.Widget.Button>(Resource.Id.btn_StartTurn);
			BtnStartTurn.Click += BtnStartTurn_Click;
			
			var BtnDartScored = FindViewById<Android.Widget.Button>(Resource.Id.btn_DartScored);
			BtnDartScored.Click += BtnDartScored_Click;

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

			try
			{

				bottomNavigation.Menu.GetItem(0).SetChecked(true);

			}
			catch (System.Exception ex)
			{

				System.Diagnostics.Debug.Print("Heya:" + ex.Message);
			}

		}

		protected override void OnSaveInstanceState(Bundle outsInstanceState)
		{

			string savedTurn = JsonConvert.SerializeObject(clsTurn);
			outsInstanceState.PutString("jsonSerialTurn", savedTurn);

			string savedUIState = JsonConvert.SerializeObject(clsUIState);
			outsInstanceState.PutString("jsonSerialUIState", savedUIState);

			Log.Debug(GetType().FullName, "AndroidNavActivity- Saving instance state");

			// always call the base implementation!
			base.OnSaveInstanceState(outsInstanceState);
		}

		protected override void OnDestroy()
		{
			Log.Debug(GetType().FullName, "Activity A - On Destroy");
			base.OnDestroy();
		}

		protected override void OnPause()
		{
			Log.Debug(GetType().FullName, "Activity A - OnPause");
			base.OnPause();
		}

		protected override void OnRestart()
		{
			Log.Debug(GetType().FullName, "Activity A - OnRestart");
			base.OnRestart();
		}

		protected override void OnResume()
		{
			Log.Debug(GetType().FullName, "Activity A - OnResume");

			try
			{

				bottomNavigation.Menu.GetItem(0).SetChecked(true);

			}
			catch (System.Exception ex)
			{

				System.Diagnostics.Debug.Print("Heya:" + ex.Message);
			}

			base.OnResume();
		}

		protected override void OnStart()
		{
			Log.Debug(GetType().FullName, "Activity A - OnStart");
			base.OnStart();
		}

		protected override void OnStop()
		{
			Log.Debug(GetType().FullName, "Activity A - OnStop");
			base.OnStop();
		}
		private void BottomNavigation_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
		{
			LoadFragment(e.Item.ItemId);
		}

		void LoadFragment(int id)
		{
			Bundle mBundle = new Bundle();
			string turnClassSerial = JsonConvert.SerializeObject(clsTurn);
			string UIClassSerial = JsonConvert.SerializeObject(clsUIState);

			Android.Support.V4.App.Fragment fragment = null;

			switch (id)
			{
				case Resource.Id.menu_home:
					break;
				case Resource.Id.menu_chart:
					Intent ichartActivity = new Intent(this, typeof(OutChartNavActivity));

					ichartActivity.PutExtra("turnClassSerial", turnClassSerial);
					ichartActivity.PutExtra("UIClassSerial", UIClassSerial);
					StartActivity(ichartActivity);

					break;
				case Resource.Id.menu_rules:
					Intent irulesActivity = new Intent(this, typeof(Activity3));

					irulesActivity.PutExtra("turnClassSerial", turnClassSerial);
					irulesActivity.PutExtra("UIClassSerial", UIClassSerial);
					StartActivity(irulesActivity);
					break;
				case Resource.Id.menu_scores:
					Intent iscoresActivity = new Intent(this, typeof(Activity4));

					iscoresActivity.PutExtra("turnClassSerial", turnClassSerial);
					iscoresActivity.PutExtra("UIClassSerial", UIClassSerial);
					StartActivity(iscoresActivity);
					break;
					
				case Resource.Id.menu_info:
					Intent iinfoActivity = new Intent(this, typeof(Activity5));

					iinfoActivity.PutExtra("turnClassSerial", turnClassSerial);
					iinfoActivity.PutExtra("UIClassSerial", UIClassSerial);
					StartActivity(iinfoActivity);
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

			HideKeyboard();

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

		private void BtnStartTurn_Click(object sender, System.EventArgs e)
		{
			singleUse = false;
			string strRecommendedOut = "";

			HideKeyboard();

			var txtDartScore = FindViewById<Android.Widget.EditText>(Resource.Id.DartScore);
			txtDartScore.Text = "";

			var txtStartScore = FindViewById<Android.Widget.EditText>(Resource.Id.StartScore);

			if (txtStartScore.Text.Trim() != "")
			{
				
				bool Result = false;
				Result = int.TryParse(txtStartScore.Text, out startingScore);

				if (true != Result)
				{
					startingScore = -1;
					strRecommendedOut = "Unknown Score";
				}
				else
				{
					strRecommendedOut = RecommendedOut(startingScore);
				}

				bool mbOK = clsTurn.SetStartingScore(startingScore);

				var txtNewOut = FindViewById<Android.Widget.TextView>(Resource.Id.txtNewScore);
				txtNewOut.Text = txtStartScore.Text + " Left, Darts Remaining: 3";

				var txtOutTurn = FindViewById<Android.Widget.TextView>(Resource.Id.txtOutTurn);
				txtOutTurn.Text = "(" + txtStartScore.Text + ") " + strRecommendedOut;

				currentScore = startingScore;
				clsUIState.CurrentScoreText = txtNewOut.Text;
				clsUIState.DoubleOutText = txtOutTurn.Text;

				TextToSpeech.SpeakAsync(txtStartScore.Text + " Starting Score");
			}
			else
			{
				Recognizer.StopListening();

				isListeningPaused = false;

				Recognizer.StartListening(SpeechIntent);

				mStreamVolume = mAudioManager.GetStreamVolume(Stream.Music); // getting system volume into var for later un-muting 

				if (mStreamVolume == 0)
				{
					mStreamVolume = 9;
				}

				StartTimer();
			}
			
		}

		private void BtnDartScored_Click(object sender, System.EventArgs e)
		{
			singleUse = false;
			string strNewOut = "";

			HideKeyboard();

			var txtDartScore = FindViewById<Android.Widget.EditText>(Resource.Id.DartScore);

			if (txtDartScore.Text.Trim() != "")
			{
				int dartScore = 0;

				bool Result = false;
				Result = int.TryParse(txtDartScore.Text, out dartScore);

				if (true != Result)
				{
					dartScore = -1;
				}
				
				clsUIState.LastScore = dartScore;

				strNewOut = GetNewScore(txtDartScore.Text);

				
				var txtNewOut = FindViewById<Android.Widget.TextView>(Resource.Id.txtNewScore);
				txtNewOut.Text = strNewOut;
				
				if (1 == (int)clsTurn.State)
				{
					clsUIState.CurrentScoreText = strNewOut;
				}
			}

			TextToSpeech.SpeakAsync(strNewOut);

		}

		private void BtnGetOut_Click(object sender, System.EventArgs e)
		{
			var txtOutLabel = FindViewById<Android.Widget.TextView>(Resource.Id.txtOutLabel);
			var txtYourScore = FindViewById<Android.Widget.EditText>(Resource.Id.YourScore);

			string scoreStr = txtYourScore.Text;
			int score = 0;
			string text = "";

			HideKeyboard();

			bool Result = false;
			Result = int.TryParse(scoreStr, out score);

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
			
			txtOutLabel.Text = text;

		}
		private void BtnClearOut_Click(object sender, System.EventArgs e)
		{
			HideKeyboard();

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

			int TotalScore = 0;
			int SingleScore = 0;
			string strRecommendedOut = "";
			
			string fmtInput = recognized.Trim().ToLower();

			if ("pause" == fmtInput.ToLower() || "stop" == fmtInput.ToLower())
			{
				seconds = 1;
				isListeningPaused = true;

				txtYourScore.Text = "stopped";
				Toast.MakeText(this, "stopped", ToastLength.Long).Show();

				return;
			}

			Toast.MakeText(this, recognized, ToastLength.Long).Show();

			if (fmtInput.Contains("out") && recognized.ToLower().IndexOf("out") > 1)
			{
				txtYourScore.Text = recognized;

				fmtInput = recognized.ToLower().Substring(0, (recognized.ToLower().IndexOf("out") - 1));

				bool Result = false;
				Result = int.TryParse(fmtInput, out TotalScore);

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

				var txtOutLabel = FindViewById<Android.Widget.TextView>(Resource.Id.txtOutLabel);
				txtOutLabel.Text = strRecommendedOut;

				TextToSpeech.SpeakAsync(strRecommendedOut);

			}
			else if (fmtInput.Contains("score") && recognized.ToLower().IndexOf("score") > 1)
			{
				//TODO 
				//implement this after establishing total score tracking
				fmtInput = recognized.ToLower().Substring(0, (recognized.ToLower().IndexOf("score") - 1));

				bool Result = false;
				Result = int.TryParse(fmtInput, out SingleScore);

				// If not an Int set an out of range value for Total Score
				if (true != Result)
				{
					SingleScore = -1;
					strRecommendedOut = "Unknown Score";
				}
				else
				{
					var txtDartScore = FindViewById<Android.Widget.EditText>(Resource.Id.DartScore);
					txtDartScore.Text = SingleScore.ToString();
					clsUIState.LastScore = SingleScore;

					strRecommendedOut = GetNewScore(SingleScore.ToString());
					
					var txtNewOut = FindViewById<Android.Widget.TextView>(Resource.Id.txtNewScore);
					txtNewOut.Text = strRecommendedOut;
					clsUIState.CurrentScoreText = txtNewOut.Text;
				}

				TextToSpeech.SpeakAsync(strRecommendedOut);

			}
			else if (fmtInput.Contains("start") && recognized.ToLower().IndexOf("start") > 1)
			{
				int startingScore = 0;

				fmtInput = recognized.ToLower().Substring(0, (recognized.ToLower().IndexOf("start") - 1));

				bool Result = false;
				Result = int.TryParse(fmtInput, out startingScore);

				if (true != Result)
				{
					startingScore = -1;
					strRecommendedOut = "Unknown Score";
				}
				else
				{
					strRecommendedOut = RecommendedOut(startingScore);
				}

				bool mbOK = clsTurn.SetStartingScore(startingScore);

				if (mbOK)
				{
					var txtDartScore = FindViewById<Android.Widget.EditText>(Resource.Id.DartScore);
					txtDartScore.Text = "";
					clsUIState.LastScore = 0;

					var txtStartScore = FindViewById<Android.Widget.EditText>(Resource.Id.StartScore);
					txtStartScore.Text = startingScore.ToString();
					
					var txtNewOut = FindViewById<Android.Widget.TextView>(Resource.Id.txtNewScore);
					txtNewOut.Text = txtStartScore.Text + " Left, Darts Remaining: 3";
					currentScore = startingScore;
					clsUIState.CurrentScoreText = txtNewOut.Text;

					var txtOutTurn = FindViewById<Android.Widget.TextView>(Resource.Id.txtOutTurn);
					txtOutTurn.Text = "(" + txtStartScore.Text + ") " + strRecommendedOut;

					clsUIState.DoubleOutText = txtOutTurn.Text;

					TextToSpeech.SpeakAsync(txtStartScore.Text + " Starting Score");
					
				}
				
			}

			
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

		private void ResetGame()
		{
			seconds = 1;
			isListeningPaused = true;

			Recognizer.StopListening();

			var txtDartScore = FindViewById<Android.Widget.EditText>(Resource.Id.DartScore);
			txtDartScore.Text = "";
			clsUIState.LastScore = 0;

			var txtStartScore = FindViewById<Android.Widget.EditText>(Resource.Id.StartScore);
			txtStartScore.Text = "";
			startingScore = 0;

			currentScore = startingScore;
			clsUIState.CurrentScoreText = "";

			var txtOutTurn = FindViewById<Android.Widget.TextView>(Resource.Id.txtOutTurn);
			txtOutTurn.Text = "";
			clsUIState.DoubleOutText = txtOutTurn.Text;

		}

		private string GetNewScore(string ThrownScore)
		{
			int dartScore = 0;
			string strReturnValue = "";

			bool Result = false;
			Result = int.TryParse(ThrownScore, out dartScore);

			// If not an Int set an out of range value for Total Score
			if (true != Result)
			{
				dartScore = -1;
				strReturnValue = "Unknown Score";
			}
			else
			{
				bool mbOK = clsTurn.RecordPointsScored(dartScore);

				if (mbOK)
				{
					int intNewScore = clsTurn.CurrentScore;
					
					if (TurnState.Done == clsTurn.State)
					{
						strReturnValue = strReturnValue = intNewScore.ToString() + " Left, Turn Over";
						ResetGame();
					}
					else if (TurnState.Bust == clsTurn.State)
					{
						strReturnValue = "Hands Up. You're BUSTED!";
						ResetGame();
					}
					else if (TurnState.Win == clsTurn.State)
					{
						strReturnValue = "Winner Winner Chicken Dinner!";
						ResetGame();
					}
					else
					{
						strReturnValue = intNewScore.ToString() + " Left, Darts Remaining: " + clsTurn.DartsRemaining.ToString();
					}

				}
				else
				{
					if (TurnState.Done == clsTurn.State)
					{
						strReturnValue = "Turn Over";
						ResetGame();
					}
					else
					{
						strReturnValue = "Unknown Score";
					}
					
				}
			}


			return strReturnValue;
		}

		//This gets the out from the classes and vocalizes the words 
		private string RecommendedOut(int TotalOut)
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

		private void HideKeyboard()
		{
			try
			{
				Android.Views.InputMethods.InputMethodManager inputMethodManager = Application.GetSystemService(Context.InputMethodService) as Android.Views.InputMethods.InputMethodManager;
				inputMethodManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, Android.Views.InputMethods.HideSoftInputFlags.None);

			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.Print("Error- " + ex.Message);

			}
		}

		}
}