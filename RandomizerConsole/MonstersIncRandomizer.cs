using System;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Microsoft.VisualBasic.ApplicationServices;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms.VisualStyles;

namespace MonstersIncRandomizer
{
	public class Randomizer
	{
		const int PROCESS_ALL_ACCESS = 0x1F0FFF;
		public static bool urbanUnlocked = false;
		public static bool desertUnlocked = false;
		public static bool arcticUnlocked = false;
		public static bool trampolineUnlocked = false;
		public static bool boostUnlocked = false;
		public static bool flingUnlocked = false;
		public static bool mikeAttackUnlocked = false;
		public static bool mikeJumpUnlocked = false;
		public static bool sullyAttackUnlocked = false;
		public static bool sullyJumpUnlocked = false;
		public static bool parkTreeDoorUnlocked = false;
		public static long parkOoze = 0;
		public static long downtownOoze = 0;
		public static long docksOoze = 0;
		public static long marketOoze = 0;
		public static long sphinxOoze = 0;
		public static long oasisOoze = 0;
		public static long tombOoze = 0;
		public static long pyramidOoze = 0;
		public static long shackOoze = 0;
		public static long skiliftOoze = 0;
		public static long icebergOoze = 0;
		public static long hotspringsOoze = 0;
		public static bool parkBronzeMedal = false;
		public static bool parkSilverMedal = false;
		public static bool parkGoldMedal = false;
		public static bool downtownBronzeMedal = false;
		public static bool downtownSilverMedal = false;
		public static bool downtownGoldMedal = false;
		public static bool docksBronzeMedal = false;
		public static bool docksSilverMedal = false;
		public static bool docksGoldMedal = false;
		public static bool marketBronzeMedal = false;
		public static bool marketSilverMedal = false;
		public static bool marketGoldMedal = false;
		public static bool sphinxBronzeMedal = false;
		public static bool sphinxSilverMedal = false;
		public static bool sphinxGoldMedal = false;
		public static bool oasisBronzeMedal = false;
		public static bool oasisSilverMedal = false;
		public static bool oasisGoldMedal = false;
		public static bool tombBronzeMedal = false;
		public static bool tombSilverMedal = false;
		public static bool tombGoldMedal = false;
		public static bool pyramidBronzeMedal = false;
		public static bool pyramidSilverMedal = false;
		public static bool pyramidGoldMedal = false;
		public static bool shackBronzeMedal = false;
		public static bool shackSilverMedal = false;
		public static bool shackGoldMedal = false;
		public static bool skiliftBronzeMedal = false;
		public static bool skiliftSilverMedal = false;
		public static bool skiliftGoldMedal = false;
		public static bool icebergBronzeMedal = false;
		public static bool icebergSilverMedal = false;
		public static bool icebergGoldMedal = false;
		public static bool hotspringsBronzeMedal = false;
		public static bool hotspringsSilverMedal = false;
		public static bool hotspringsGoldMedal = false;
		public static bool docksCraneBoxUnlcoked = false;
		public static bool marketSewerEntranceUnlocked = false;
		public static bool tombEntranceUnlocked = false;
		public static int parkNerveCount = 0;
		public static int downtownNerveCount = 0;
		public static int docksNerveCount = 0;
		public static int marketNerveCount = 0;
		public static int sphinxNerveCount = 0;
		public static int oasisNerveCount = 0;
		public static int tombNerveCount = 0;
		public static int pyramidNerveCount = 0;
		public static int shackNerveCount = 0;
		public static int skiliftNerveCount = 0;
		public static int icebergNerveCount = 0;
		public static int hotspringsNerveCount = 0;
		public static int parkTokenCount = 0;
		public static int downtownTokenCount = 0;
		public static int docksTokenCount = 0;
		public static int marketTokenCount = 0;
		public static int sphinxTokenCount = 0;
		public static int oasisTokenCount = 0;
		public static int tombTokenCount = 0;
		public static int pyramidTokenCount = 0;
		public static int shackTokenCount = 0;
		public static int skiliftTokenCount = 0;
		public static int icebergTokenCount = 0;
		public static int hotspringsTokenCount = 0;
		public static int oldTrampolineValue = 0;
		public static int oldBoostValue = 0;
		public static int oldFlingValue = 0;
		public static int itemIndex = 0;

		public static Process process = Process.GetProcessesByName("MonstersInc")[0];
		public static IntPtr processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);

		public static int bytesRead;
		public static byte[] characterOffsetBuffer;
		public static nint baseAddress;
		public static int address;
		public static int characterOffset;
		public static int levelAddress;
		public static byte[] levelBuffer;

		public static byte[] buffer;
		public static byte[] oldMemory;
		public static int bytesWritten = 0;
		public static int oozeAddress;
		public static int oozeOffset;
		public static string levelText;
		public static List<Reward> rewards;
		public static string[] loadedChecks;
		public static string path;
		public static string goal;
		public static long oozeAmount;

		[DllImport("Kernel32.dll")]
		public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll")]
		public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

		public static List<(int offset, int valueChange, Check check, Reward reward, string text, string[] needs)> addressesToCheck = new List<(int offset, int valueChange, Check check, Reward reward, string text, string[] needs)>();

		public static void Main(string[] args)
		{
            bytesRead = 0;
			characterOffsetBuffer = new byte[4];
			baseAddress = process.MainModule.BaseAddress;
			address = (int)baseAddress + 0x07383EB0;
			levelBuffer = new byte[20];
			levelAddress = (int)baseAddress + 0x07383FE7;
			buffer = new byte[4];
			//ReadProcessMemory((int)processHandle, levelAddress, buffer, buffer.Length, ref bytesRead);

			//levelAddress = BitConverter.ToInt32(buffer);

			ReadProcessMemory((int)processHandle, address, characterOffsetBuffer, characterOffsetBuffer.Length, ref bytesRead);
			characterOffset = BitConverter.ToInt32(characterOffsetBuffer);

			Console.WriteLine("Input Server: ");
			string server = Console.ReadLine();

			Console.WriteLine("Input Player Name: ");
			string player = Console.ReadLine();

			string[] splitServer = server.Split(":");

			var session = ArchipelagoSessionFactory.CreateSession(splitServer[0], Int32.Parse(splitServer[1]));
			session.MessageLog.OnMessageReceived += OnMessageReceived;

			string game = "Monsters, Inc. Scare Island";

			rewards = new List<Reward>();

			Initialize();

			LoginResult result;
			try
			{
				result = session.TryConnectAndLogin(game, player, ItemsHandlingFlags.AllItems);
			}

			catch (Exception e)
			{
				result = new LoginFailure(e.GetBaseException().Message);
			}

			if (!result.Successful)
			{
				LoginFailure failure = (LoginFailure)result;
				string errorMessage = $"Failed to Connect to game as bam:";
				foreach (string error in failure.Errors)
				{
					errorMessage += $"\n    {error}";
				}
				foreach (ConnectionRefusedError error in failure.ErrorCodes)
				{
					errorMessage += $"\n    {error}";
				}

				return; // Did not connect, show the user the contents of `errorMessage`
			}

			var loginSuccess = (LoginSuccessful)result;
			string[] keys = loginSuccess.SlotData.Keys.ToArray();
			foreach (string key in keys)
			{
				if(key == "goal")
				{
					if ((Int64)loginSuccess.SlotData[key] == 0)
					{
						goal = "bronze";
					}
					else if ((Int64)loginSuccess.SlotData[key] == 1)
					{
						goal = "silver";
					}
					else if ((Int64)loginSuccess.SlotData[key] == 2)
					{
						goal = "gold";
						Console.WriteLine("goal is gold");
					}
                }
				else if(key == "level_ooze")
				{
					oozeAmount = (Int64)loginSuccess.SlotData[key];
                }
				Console.WriteLine(key);
				Console.WriteLine(loginSuccess.SlotData[key]);
			}
			

			//Console.WriteLine(session.RoomState.Seed);
			session.Items.ItemReceived += HandleItem;

			ReadProcessMemory((int)processHandle, characterOffset + 0x1F4, buffer, buffer.Length, ref bytesRead);
			oldMemory = buffer;
			oldTrampolineValue = oldMemory[0];
			oldBoostValue = oldMemory[0];
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F5, buffer, buffer.Length, ref bytesRead);
			oldMemory = buffer;
			oldFlingValue = oldMemory[0];
			int bronzeCollected = 0;
			int silverCollected = 0;
			int goldCollected = 0;
			parkNerveCount = 0;
			downtownNerveCount = 0;
			docksNerveCount = 0;
			marketNerveCount = 0;
			sphinxNerveCount = 0;
			oasisNerveCount = 0;
			tombNerveCount = 0;
			pyramidNerveCount = 0;
			shackNerveCount = 0;
			skiliftNerveCount = 0;
			icebergNerveCount = 0;
			hotspringsNerveCount = 0;

			path = System.AppDomain.CurrentDomain.BaseDirectory + session.RoomState.Seed;


			if (File.Exists(path))
			{
				loadedChecks = File.ReadAllLines(path);
				foreach (string check in loadedChecks)
				{
					if (check.Contains("parkNerveCount")) { parkNerveCount = Int32.Parse(check.Substring(0, 1));}
					if (check.Contains("downtownNerveCount")) { downtownNerveCount = Int32.Parse(check.Substring(0, 1)); }
					if (check.Contains("docksNerveCount")) { docksNerveCount = Int32.Parse(check.Substring(0, 1)); }
					if (check.Contains("marketNerveCount")) { marketNerveCount = Int32.Parse(check.Substring(0, 1)); }
					if (check.Contains("sphinxNerveCount")) { sphinxNerveCount = Int32.Parse(check.Substring(0, 1)); }
					if (check.Contains("oasisNerveCount")) { oasisNerveCount = Int32.Parse(check.Substring(0, 1)); }
					if (check.Contains("tombNerveCount")) { tombNerveCount = Int32.Parse(check.Substring(0, 1)); }
					if (check.Contains("pyramidNerveCount")) { pyramidNerveCount = Int32.Parse(check.Substring(0, 1)); }
					if (check.Contains("shackNerveCount")) { shackNerveCount = Int32.Parse(check.Substring(0, 1)); }
					if (check.Contains("skiliftNerveCount")) { skiliftNerveCount = Int32.Parse(check.Substring(0, 1)); }
					if (check.Contains("icebergNerveCount")) { icebergNerveCount = Int32.Parse(check.Substring(0, 1)); }
                    if (check.Contains("hotspringsNerveCount")) { hotspringsNerveCount = Int32.Parse(check.Substring(0, 1)); }
                    if (check.Contains("Mike Attack")) { mikeAttackUnlocked = true; }
                    if (check.Contains("Mike Jump")) { mikeJumpUnlocked = true; }
                    if (check.Contains("Sully Attack")) { sullyAttackUnlocked = true; }
                    if (check.Contains("Sully Jump")) { sullyJumpUnlocked = true; }
                    if (check.Contains("Desert")) { desertUnlocked = true; }
                    if (check.Contains("Boost")) { boostUnlocked = true; }
                    if (check.Contains("Trampoline")) { trampolineUnlocked = true; }
                    //if (check.Contains("Arctic")) { arcticUnlocked = true; }
                    if (check.Contains("bronzeCollected") && check[1].ToString() != " ")
					{
						bronzeCollected = Int32.Parse(check.Substring(0, 2));
					}
                    else if (check.Contains("bronzeCollected"))

                    {
						bronzeCollected = Int32.Parse(check.Substring(0, 1));
					}
					if (check.Contains("silverCollected") && check[1].ToString() != " ")
					{
						silverCollected = Int32.Parse(check.Substring(0, 2));
					}
					else if (check.Contains("silverCollected"))

                    {
						silverCollected = Int32.Parse(check.Substring(0, 1));
					}
					if (check.Contains("goldCollected") && check[1].ToString() != " ")
					{
						goldCollected = Int32.Parse(check.Substring(0, 2));
					}
					else if (check.Contains("goldCollected"))

                    {
						goldCollected = Int32.Parse(check.Substring(0, 1));
					}

				}
			}

			while (true)
			{
				foreach ((int offset, int valueChange, Check check, Reward reward, string text, string[] needs) check in addressesToCheck)
				{
					int oldValue;
					ReadProcessMemory((int)processHandle, characterOffset + check.offset, buffer, buffer.Length, ref bytesRead);
					//Console.WriteLine(buffer.Length);
					int newValue = check.check.currValue + (int)check.valueChange;
					//Console.WriteLine(check.check.currValue);
					//Console.WriteLine(buffer[0]);
					if (newValue == buffer[0])
					{
						oldValue = buffer[0];
						ReadProcessMemory((int)processHandle, characterOffset + check.reward.address, buffer, buffer.Length, ref bytesRead);
						oldMemory = buffer;
						//Console.WriteLine(Convert.ToInt32(oldMemory[0]));
						bytesWritten = 0;
						oldMemory[0] = BitConverter.GetBytes(check.reward.changeValue + Convert.ToInt16(oldMemory[0]))[0];
						//WriteProcessMemory((int)processHandle, characterOffset + check.reward.address, oldMemory, oldMemory.Length, ref bytesWritten);
						check.check.currValue = oldValue;
						session.Locations.CompleteLocationChecks(session.Locations.GetLocationIdFromName(game, check.text));
						if (check.text.Contains("Bronze Medal Collected"))
						{
							bronzeCollected += 1;
							bool bronzeWritten = false;
							string[] save = File.ReadAllLines(path);
							for (int i = 0; i < save.Length; i++)
							{
								if (save[i].Contains("bronze")){
									string newString = save[i];
									if(newString.Substring(1, 1) == " ")
									{
                                        int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                        newString = (leadingNumber + 1).ToString() + newString.Substring(1);
                                    }
									else
									{
                                        int leadingNumber = Int32.Parse(newString.Substring(0, 2));
                                        newString = (leadingNumber + 1).ToString() + newString.Substring(2);
                                    }
									bronzeWritten = true;
									break;
                                }
							}
							if (!bronzeWritten)
							{
                                StreamWriter sw = new StreamWriter(path, true);
                                sw.WriteLine("1 bronzeCollected");
                                sw.Close();
                            }
						}
                        if (check.text.Contains("Gold Medal Collected"))
                        {
                            goldCollected += 1;
                            bool goldWritten = false;
                            string[] save = File.ReadAllLines(path);
                            for (int i = 0; i < save.Length; i++)
                            {
                                if (save[i].Contains("gold"))
                                {
                                    string newString = save[i];
                                    if (newString.Substring(1, 1) == " ")
                                    {
                                        int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                        newString = (leadingNumber + 1).ToString() + newString.Substring(1);
                                    }
                                    else
                                    {
                                        int leadingNumber = Int32.Parse(newString.Substring(0, 2));
                                        newString = (leadingNumber + 1).ToString() + newString.Substring(2);
                                    }
                                    goldWritten = true;
                                    break;
                                }
                            }
                            if (!goldWritten)
                            {
                                StreamWriter sw = new StreamWriter(path, true);
                                sw.WriteLine("1 goldCollected");
                                sw.Close();
                            }
                        }
                        if (check.text.Contains("Silver Medal Collected"))
                        {
                            silverCollected += 1;
                            bool silverWritten = false;
                            string[] save = File.ReadAllLines(path);
                            for (int i = 0; i < save.Length; i++)
                            {
                                if (save[i].Contains("silver"))
                                {
                                    string newString = save[i];
                                    if (newString.Substring(1, 1) == " ")
                                    {
                                        int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                        newString = (leadingNumber + 1).ToString() + newString.Substring(1);
                                    }
                                    else
                                    {
                                        int leadingNumber = Int32.Parse(newString.Substring(0, 2));
                                        newString = (leadingNumber + 1).ToString() + newString.Substring(2);
                                    }
                                    silverWritten = true;
                                    break;
                                }
                            }
                            if (!silverWritten)
                            {
                                StreamWriter sw = new StreamWriter(path, true);
                                sw.WriteLine("1 bronzeCollected");
                                sw.Close();
                            }
                        }
                        if (check.text.Contains("Park -") && (check.text.Contains("Token") && (!check.text.Contains("Tokens")))){
							parkTokenCount += 1;
						}
						if (check.text.Contains("Nerve") && (check.text.Contains("- Blue Nerve") || check.text.Contains("- Green Nerve") || check.text.Contains("- Yellow Nerve") || check.text.Contains("- Orange Nerve") || check.text.Contains("- Red Nerve")))
						{
							if (check.text.Contains("Park -"))
							{
								parkNerveCount += 1;
                                bool parkWritten = false;
                                string[] save = File.ReadAllLines(path);
                                for (int i = 0; i < save.Length; i++)
                                {
                                    if (save[i].Contains("parkN"))
                                    {
                                        string newString = save[i];
                                        if (newString.Substring(1, 1) == " ")
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(1);
                                        }
                                        else
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(2);
                                        }
										save[i] = newString;
										File.WriteAllLines(path, save);
                                        parkWritten = true;
                                        break;
                                    }
                                }
                                if (!parkWritten)
                                {
                                    StreamWriter sw = new StreamWriter(path, true);
                                    sw.WriteLine("1 parkNerveCount");
                                    sw.Close();
                                }
                            }
							else if (check.text.Contains("Downtown -"))
							{
								downtownNerveCount += 1;
                                bool downtownWritten = false;
                                string[] save = File.ReadAllLines(path);
                                for (int i = 0; i < save.Length; i++)
                                {
                                    if (save[i].Contains("downtownN"))
                                    {
                                        string newString = save[i];
                                        if (newString.Substring(1, 1) == " ")
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(1);
                                        }
                                        else
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(2);
                                        }
                                        save[i] = newString;
                                        File.WriteAllLines(path, save);
                                        downtownWritten = true;
                                        break;
                                    }
                                }
                                if (!downtownWritten)
                                {
                                    StreamWriter sw = new StreamWriter(path, true);
                                    sw.WriteLine("1 downtownNerveCount");
                                    sw.Close();
                                }
                            }
							else if (check.text.Contains("Docks -"))
							{
								docksNerveCount += 1;
                                bool docksWritten = false;
                                string[] save = File.ReadAllLines(path);
                                for (int i = 0; i < save.Length; i++)
                                {
                                    if (save[i].Contains("docksN"))
                                    {
                                        string newString = save[i];
                                        if (newString.Substring(1, 1) == " ")
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(1);
                                        }
                                        else
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(2);
                                        }
                                        save[i] = newString;
                                        File.WriteAllLines(path, save);
                                        docksWritten = true;
                                        break;
                                    }
                                }
                                if (!docksWritten)
                                {
                                    StreamWriter sw = new StreamWriter(path, true);
                                    sw.WriteLine("1 docksNerveCount");
                                    sw.Close();
                                }
                            }
							else if (check.text.Contains("Market -"))
							{
								marketNerveCount += 1;
                                bool marketWritten = false;
                                string[] save = File.ReadAllLines(path);
                                for (int i = 0; i < save.Length; i++)
                                {
                                    if (save[i].Contains("marketN"))
                                    {
                                        string newString = save[i];
                                        if (newString.Substring(1, 1) == " ")
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(1);
                                        }
                                        else
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(2);
                                        }
                                        save[i] = newString;
                                        File.WriteAllLines(path, save);
                                        marketWritten = true;
                                        break;
                                    }
                                }
                                if (!marketWritten)
                                {
                                    StreamWriter sw = new StreamWriter(path, true);
                                    sw.WriteLine("1 marketNerveCount");
                                    sw.Close();
                                }
                            }
							else if (check.text.Contains("Sphinx -"))
							{
								sphinxNerveCount += 1;
                                bool sphinxWritten = false;
                                string[] save = File.ReadAllLines(path);
                                for (int i = 0; i < save.Length; i++)
                                {
                                    if (save[i].Contains("sphinxN"))
                                    {
                                        string newString = save[i];
                                        if (newString.Substring(1, 1) == " ")
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(1);
                                        }
                                        else
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(2);
                                        }
                                        save[i] = newString;
                                        File.WriteAllLines(path, save);
                                        sphinxWritten = true;
                                        break;
                                    }
                                }
                                if (!sphinxWritten)
                                {
                                    StreamWriter sw = new StreamWriter(path, true);
                                    sw.WriteLine("1 sphinxNerveCount");
                                    sw.Close();
                                }
                            }
							else if (check.text.Contains("Oasis -"))
							{
								oasisNerveCount += 1;
                                bool oasisWritten = false;
                                string[] save = File.ReadAllLines(path);
                                for (int i = 0; i < save.Length; i++)
                                {
                                    if (save[i].Contains("oasisN"))
                                    {
                                        string newString = save[i];
                                        if (newString.Substring(1, 1) == " ")
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(1);
                                        }
                                        else
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(2);
                                        }
                                        save[i] = newString;
                                        File.WriteAllLines(path, save);
                                        oasisWritten = true;
                                        break;
                                    }
                                }
                                if (!oasisWritten)
                                {
                                    StreamWriter sw = new StreamWriter(path, true);
                                    sw.WriteLine("1 oasisNerveCount");
                                    sw.Close();
                                }
                            }
							else if (check.text.Contains("Tomb -"))
							{
								tombNerveCount += 1;
                                bool tombWritten = false;
                                string[] save = File.ReadAllLines(path);
                                for (int i = 0; i < save.Length; i++)
                                {
                                    if (save[i].Contains("tombN"))
                                    {
                                        string newString = save[i];
                                        if (newString.Substring(1, 1) == " ")
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(1);
                                        }
                                        else
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(2);
                                        }
                                        save[i] = newString;
                                        File.WriteAllLines(path, save);
                                        tombWritten = true;
                                        break;
                                    }
                                }
                                if (!tombWritten)
                                {
                                    StreamWriter sw = new StreamWriter(path, true);
                                    sw.WriteLine("1 tombNerveCount");
                                    sw.Close();
                                }
                            }
							else if (check.text.Contains("Pyramid -"))
							{
								pyramidNerveCount += 1;
                                bool pyramidWritten = false;
                                string[] save = File.ReadAllLines(path);
                                for (int i = 0; i < save.Length; i++)
                                {
                                    if (save[i].Contains("pyramidN"))
                                    {
                                        string newString = save[i];
                                        if (newString.Substring(1, 1) == " ")
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(1);
                                        }
                                        else
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(2);
                                        }
                                        save[i] = newString;
                                        File.WriteAllLines(path, save);
                                        pyramidWritten = true;
                                        break;
                                    }
                                }
                                if (!pyramidWritten)
                                {
                                    StreamWriter sw = new StreamWriter(path, true);
                                    sw.WriteLine("1 pyramidNerveCount");
                                    sw.Close();
                                }
                            }
							else if (check.text.Contains("Shack -"))
							{
								shackNerveCount += 1;
                                bool shackWritten = false;
                                string[] save = File.ReadAllLines(path);
                                for (int i = 0; i < save.Length; i++)
                                {
                                    if (save[i].Contains("shackN"))
                                    {
                                        string newString = save[i];
                                        if (newString.Substring(1, 1) == " ")
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(1);
                                        }
                                        else
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(2);
                                        }
                                        save[i] = newString;
                                        File.WriteAllLines(path, save);
                                        shackWritten = true;
                                        break;
                                    }
                                }
                                if (!shackWritten)
                                {
                                    StreamWriter sw = new StreamWriter(path, true);
                                    sw.WriteLine("1 shackNerveCount");
                                    sw.Close();
                                }
                            }
							else if (check.text.Contains("Ski Lift -"))
							{
								skiliftNerveCount += 1;
                                bool skiliftWritten = false;
                                string[] save = File.ReadAllLines(path);
                                for (int i = 0; i < save.Length; i++)
                                {
                                    if (save[i].Contains("skiliftN"))
                                    {
                                        string newString = save[i];
                                        if (newString.Substring(1, 1) == " ")
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(1);
                                        }
                                        else
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(2);
                                        }
                                        save[i] = newString;
                                        File.WriteAllLines(path, save);
                                        skiliftWritten = true;
                                        break;
                                    }
                                }
                                if (!skiliftWritten)
                                {
                                    StreamWriter sw = new StreamWriter(path, true);
                                    sw.WriteLine("1 skiliftNerveCount");
                                    sw.Close();
                                }
                            }
							else if (check.text.Contains("Iceberg -"))
							{
								icebergNerveCount += 1;
                                bool icebergWritten = false;
                                string[] save = File.ReadAllLines(path);
                                for (int i = 0; i < save.Length; i++)
                                {
                                    if (save[i].Contains("icebergN"))
                                    {
                                        string newString = save[i];
                                        if (newString.Substring(1, 1) == " ")
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(1);
                                        }
                                        else
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(2);
                                        }
                                        save[i] = newString;
                                        File.WriteAllLines(path, save);
                                        icebergWritten = true;
                                        break;
                                    }
                                }
                                if (!icebergWritten)
                                {
                                    StreamWriter sw = new StreamWriter(path, true);
                                    sw.WriteLine("1 icebergNerveCount");
                                    sw.Close();
                                }
                            }
							else if (check.text.Contains("Hotsprings -"))
							{
								hotspringsNerveCount += 1;
                                bool hotspringsWritten = false;
                                string[] save = File.ReadAllLines(path);
                                for (int i = 0; i < save.Length; i++)
                                {
                                    if (save[i].Contains("hotspringsN"))
                                    {
                                        string newString = save[i];
                                        if (newString.Substring(1, 1) == " ")
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(1);
                                        }
                                        else
                                        {
                                            int leadingNumber = Int32.Parse(newString.Substring(0, 1));
                                            newString = (leadingNumber + 1).ToString() + newString.Substring(2);
                                        }
                                        save[i] = newString;
                                        File.WriteAllLines(path, save);
                                        hotspringsWritten = true;
                                        break;
                                    }
                                }
                                if (!hotspringsWritten)
                                {
                                    StreamWriter sw = new StreamWriter(path, true);
                                    sw.WriteLine("1 hotspringsNerveCount");
                                    sw.Close();
                                }
                            }
						}
						foreach ((int offset, int valueChange, Check check, Reward reward, string text, string[] needs) check2 in addressesToCheck)
						{
							if (check2.offset == check.offset)
							{
								check2.check.currValue = oldValue;
							}
						}
					}
				}
				if (goal == "bronze" && bronzeCollected == 12)
				{
					session.SetGoalAchieved();
				}
				else if(goal == "silver" && silverCollected == 12)
				{
                    session.SetGoalAchieved();
                }
                else if (goal == "gold" && goldCollected == 12)
                {
                    session.SetGoalAchieved();
                }
                ReadProcessMemory((int)processHandle, oozeOffset + 0x21, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				ReadProcessMemory((int)processHandle, levelAddress, levelBuffer, levelBuffer.Length, ref bytesRead);
				levelText = System.Text.Encoding.UTF8.GetString(levelBuffer);
				if (levelText.Contains("MO04"))
				{
					oldMemory[0] = BitConverter.GetBytes(Math.Clamp(parkOoze, 0, 100))[0];
				}
				if (levelText.Contains("MO05"))
				{
					oldMemory[0] = BitConverter.GetBytes(Math.Clamp(downtownOoze, 0, 100))[0];
				}
				if (levelText.Contains("MO06"))
				{
					oldMemory[0] = BitConverter.GetBytes(Math.Clamp(docksOoze, 0, 100))[0];
				}
				if (levelText.Contains("MO07"))
				{
					oldMemory[0] = BitConverter.GetBytes(Math.Clamp(marketOoze, 0, 100))[0];
				}
				if (levelText.Contains("BD08"))
				{
					oldMemory[0] = BitConverter.GetBytes(Math.Clamp(sphinxOoze, 0, 100))[0];
				}
				if (levelText.Contains("BD09"))
				{
					oldMemory[0] = BitConverter.GetBytes(Math.Clamp(oasisOoze, 0, 100))[0];
				}
				if (levelText.Contains("BD10"))
				{
					oldMemory[0] = BitConverter.GetBytes(Math.Clamp(tombOoze, 0, 100))[0];
				}
				if (levelText.Contains("BD11"))
				{
					oldMemory[0] = BitConverter.GetBytes(Math.Clamp(pyramidOoze, 0, 100))[0];
				}
				if (levelText.Contains("SP12"))
				{
					oldMemory[0] = BitConverter.GetBytes(Math.Clamp(shackOoze, 0, 100))[0];
				}
				if (levelText.Contains("SP13"))
				{
					oldMemory[0] = BitConverter.GetBytes(Math.Clamp(skiliftOoze, 0, 100))[0];
				}
				if (levelText.Contains("SP14"))
				{
					oldMemory[0] = BitConverter.GetBytes(Math.Clamp(icebergOoze, 0, 100))[0];
				}
				if (levelText.Contains("SP15"))
				{
					oldMemory[0] = BitConverter.GetBytes(Math.Clamp(hotspringsOoze, 0, 100))[0];
				}
				WriteProcessMemory((int)processHandle, oozeOffset + 0x21, oldMemory, oldMemory.Length, ref bytesWritten);

				ReadProcessMemory((int)processHandle, characterOffset + 0xE1, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				oldMemory[0] = BitConverter.GetBytes(parkNerveCount)[0];
				WriteProcessMemory((int)processHandle, characterOffset + 0xE1, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xE2, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				oldMemory[0] = BitConverter.GetBytes(downtownNerveCount)[0];
				WriteProcessMemory((int)processHandle, characterOffset + 0xE2, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xE3, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				oldMemory[0] = BitConverter.GetBytes(docksNerveCount)[0];
				WriteProcessMemory((int)processHandle, characterOffset + 0xE3, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xE4, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				oldMemory[0] = BitConverter.GetBytes(marketNerveCount)[0];
				WriteProcessMemory((int)processHandle, characterOffset + 0xE4, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xE5, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				oldMemory[0] = BitConverter.GetBytes(sphinxNerveCount)[0];
				WriteProcessMemory((int)processHandle, characterOffset + 0xE5, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xE6, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				oldMemory[0] = BitConverter.GetBytes(oasisNerveCount)[0];
				WriteProcessMemory((int)processHandle, characterOffset + 0xE6, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xE7, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				oldMemory[0] = BitConverter.GetBytes(tombNerveCount)[0];
				WriteProcessMemory((int)processHandle, characterOffset + 0xE7, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xE8, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				oldMemory[0] = BitConverter.GetBytes(pyramidNerveCount)[0];
				WriteProcessMemory((int)processHandle, characterOffset + 0xE8, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xE9, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				oldMemory[0] = BitConverter.GetBytes(shackNerveCount)[0];
				WriteProcessMemory((int)processHandle, characterOffset + 0xE9, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xEA, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				oldMemory[0] = BitConverter.GetBytes(skiliftNerveCount)[0];
				WriteProcessMemory((int)processHandle, characterOffset + 0xEA, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xEB, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				oldMemory[0] = BitConverter.GetBytes(icebergNerveCount)[0];
				WriteProcessMemory((int)processHandle, characterOffset + 0xEB, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xEC, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				oldMemory[0] = BitConverter.GetBytes(hotspringsNerveCount)[0];
				WriteProcessMemory((int)processHandle, characterOffset + 0xEC, oldMemory, oldMemory.Length, ref bytesWritten);

				ReadProcessMemory((int)processHandle, characterOffset + 0xD4, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				WriteProcessMemory((int)processHandle, characterOffset + 0xD4, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xD5, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				WriteProcessMemory((int)processHandle, characterOffset + 0xD5, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xD6, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				WriteProcessMemory((int)processHandle, characterOffset + 0xD6, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xD7, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				WriteProcessMemory((int)processHandle, characterOffset + 0xD7, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xD8, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				WriteProcessMemory((int)processHandle, characterOffset + 0xD8, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xD9, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				WriteProcessMemory((int)processHandle, characterOffset + 0xD9, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xDA, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				WriteProcessMemory((int)processHandle, characterOffset + 0xDA, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xDB, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				WriteProcessMemory((int)processHandle, characterOffset + 0xDB, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xDC, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				WriteProcessMemory((int)processHandle, characterOffset + 0xDC, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xDD, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				WriteProcessMemory((int)processHandle, characterOffset + 0xDD, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xDE, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				WriteProcessMemory((int)processHandle, characterOffset + 0xDE, oldMemory, oldMemory.Length, ref bytesWritten);
				ReadProcessMemory((int)processHandle, characterOffset + 0xDF, buffer, buffer.Length, ref bytesRead);
				oldMemory = buffer;
				WriteProcessMemory((int)processHandle, characterOffset + 0xDF, oldMemory, oldMemory.Length, ref bytesWritten);

				ReadProcessMemory((int)processHandle, characterOffset + 0xB5, buffer, buffer.Length, ref bytesRead);
				int currCharacter = buffer[0];
				if (levelText.Substring(0, 9) == "ACTIM.BZE" || levelText.Substring(0, 8) == "ACTI.BZE")
				{
					ReadProcessMemory((int)processHandle, characterOffset + 0x20, buffer, buffer.Length, ref bytesRead);
					oldMemory = buffer;
					oldMemory[0] = BitConverter.GetBytes(128)[0];
					WriteProcessMemory((int)processHandle, characterOffset + 0x20, oldMemory, oldMemory.Length, ref bytesWritten);
					ReadProcessMemory((int)processHandle, characterOffset + 0x26, buffer, buffer.Length, ref bytesRead);
					oldMemory = buffer;
					oldMemory[0] = BitConverter.GetBytes(64)[0];
					WriteProcessMemory((int)processHandle, characterOffset + 0x26, oldMemory, oldMemory.Length, ref bytesWritten);
					ReadProcessMemory((int)processHandle, characterOffset + 0x48, buffer, buffer.Length, ref bytesRead);
					oldMemory = buffer;
					oldMemory[0] = BitConverter.GetBytes(128)[0];
					WriteProcessMemory((int)processHandle, characterOffset + 0x48, oldMemory, oldMemory.Length, ref bytesWritten);
					ReadProcessMemory((int)processHandle, characterOffset + 0x4E, buffer, buffer.Length, ref bytesRead);
					oldMemory = buffer;
					oldMemory[0] = BitConverter.GetBytes(64)[0];
					WriteProcessMemory((int)processHandle, characterOffset + 0x4E, oldMemory, oldMemory.Length, ref bytesWritten);
				}
				else
				{
					if (currCharacter == 1)
					{
						if (mikeAttackUnlocked == false)
						{
							ReadProcessMemory((int)processHandle, characterOffset + 0x20, buffer, buffer.Length, ref bytesRead);
							oldMemory = buffer;
							oldMemory[0] = BitConverter.GetBytes(0)[0];
							WriteProcessMemory((int)processHandle, characterOffset + 0x20, oldMemory, oldMemory.Length, ref bytesWritten);
						}
						else
						{
							ReadProcessMemory((int)processHandle, characterOffset + 0x20, buffer, buffer.Length, ref bytesRead);
							oldMemory = buffer;
							bytesWritten = 0;
							oldMemory[0] = BitConverter.GetBytes(128)[0];
							WriteProcessMemory((int)processHandle, characterOffset + 0x20, oldMemory, oldMemory.Length, ref bytesWritten);
						}
						if (mikeJumpUnlocked == false)
						{
							ReadProcessMemory((int)processHandle, characterOffset + 0x26, buffer, buffer.Length, ref bytesRead);
							oldMemory = buffer;
							oldMemory[0] = BitConverter.GetBytes(0)[0];
							WriteProcessMemory((int)processHandle, characterOffset + 0x26, oldMemory, oldMemory.Length, ref bytesWritten);
						}
						else
						{
							ReadProcessMemory((int)processHandle, characterOffset + 0x26, buffer, buffer.Length, ref bytesRead);
							oldMemory = buffer;
							bytesWritten = 0;
							oldMemory[0] = BitConverter.GetBytes(64)[0];
							WriteProcessMemory((int)processHandle, characterOffset + 0x26, oldMemory, oldMemory.Length, ref bytesWritten);
						}
					}
					else if (currCharacter == 0)
                    {
                        if (sullyAttackUnlocked == false)
                        {
                            ReadProcessMemory((int)processHandle, characterOffset + 0x20, buffer, buffer.Length, ref bytesRead);
                            oldMemory = buffer;
                            oldMemory[0] = BitConverter.GetBytes(0)[0];
                            WriteProcessMemory((int)processHandle, characterOffset + 0x20, oldMemory, oldMemory.Length, ref bytesWritten);
                        }
                        else
                        {
                            ReadProcessMemory((int)processHandle, characterOffset + 0x20, buffer, buffer.Length, ref bytesRead);
                            oldMemory = buffer;
                            bytesWritten = 0;
                            oldMemory[0] = BitConverter.GetBytes(128)[0];
                            WriteProcessMemory((int)processHandle, characterOffset + 0x20, oldMemory, oldMemory.Length, ref bytesWritten);
                        }
                        if (sullyJumpUnlocked == false)
                        {
                            ReadProcessMemory((int)processHandle, characterOffset + 0x26, buffer, buffer.Length, ref bytesRead);
                            oldMemory = buffer;
                            oldMemory[0] = BitConverter.GetBytes(0)[0];
                            WriteProcessMemory((int)processHandle, characterOffset + 0x26, oldMemory, oldMemory.Length, ref bytesWritten);
                        }
                        else
                        {
                            ReadProcessMemory((int)processHandle, characterOffset + 0x26, buffer, buffer.Length, ref bytesRead);
                            oldMemory = buffer;
                            bytesWritten = 0;
                            oldMemory[0] = BitConverter.GetBytes(64)[0];
                            WriteProcessMemory((int)processHandle, characterOffset + 0x26, oldMemory, oldMemory.Length, ref bytesWritten);
                        }
                    }
                }
				if (urbanUnlocked)
				{
					//Console.WriteLine("Urban Unlocked");
					ReadProcessMemory((int)processHandle, characterOffset + 0x22F, buffer, buffer.Length, ref bytesRead);
					oldMemory = buffer;
					bytesWritten = 0;
					oldMemory[0] = BitConverter.GetBytes(oldMemory[0] + 2)[0];
					WriteProcessMemory((int)processHandle, characterOffset + 0x22F, oldMemory, oldMemory.Length, ref bytesWritten);
					urbanUnlocked = false;
				}
				if (desertUnlocked)
				{
					//Console.WriteLine("Desert Unlocked");
					ReadProcessMemory((int)processHandle, characterOffset + 0xEF, buffer, buffer.Length, ref bytesRead);
					oldMemory = buffer;
					bytesWritten = 0;
					oldMemory[0] = BitConverter.GetBytes(1)[0];
					WriteProcessMemory((int)processHandle, characterOffset + 0xEF, oldMemory, oldMemory.Length, ref bytesWritten);
				}
				if (arcticUnlocked)
				{
					//Console.WriteLine("Arctic Unlocked");
					ReadProcessMemory((int)processHandle, characterOffset + 0xF0, buffer, buffer.Length, ref bytesRead);
					oldMemory = buffer;
					bytesWritten = 0;
					oldMemory[0] = BitConverter.GetBytes(oldMemory[0] + 1)[0];
					WriteProcessMemory((int)processHandle, characterOffset + 0xF0, oldMemory, oldMemory.Length, ref bytesWritten);
					arcticUnlocked = false;
				}
				if (parkTreeDoorUnlocked)
				{
					ReadProcessMemory((int)processHandle, characterOffset + 0x22A, buffer, buffer.Length, ref bytesRead);
					oldMemory = buffer;
					oldMemory[0] = BitConverter.GetBytes(oldMemory[0] + 8)[0];
					WriteProcessMemory((int)processHandle, characterOffset + 0x22A, oldMemory, oldMemory.Length, ref bytesWritten);
					parkTreeDoorUnlocked = false;
				}
				if (docksCraneBoxUnlcoked)
				{
					ReadProcessMemory((int)processHandle, characterOffset + 0x22F, buffer, buffer.Length, ref bytesRead);
					oldMemory = buffer;
					oldMemory[0] = BitConverter.GetBytes(oldMemory[0] + 1)[0];
					WriteProcessMemory((int)processHandle, characterOffset + 0x22F, oldMemory, oldMemory.Length, ref bytesWritten);
					docksCraneBoxUnlcoked = false;
				}
				if (tombEntranceUnlocked)
				{
					ReadProcessMemory((int)processHandle, characterOffset + 0x104, buffer, buffer.Length, ref bytesRead);
					oldMemory = buffer;
					oldMemory[0] = BitConverter.GetBytes(14)[0];
					WriteProcessMemory((int)processHandle, characterOffset + 0x104, oldMemory, oldMemory.Length, ref bytesWritten);
				}
				if (trampolineUnlocked)
				{
					if (boostUnlocked)
					{
						ReadProcessMemory((int)processHandle, characterOffset + 0x1F4, buffer, buffer.Length, ref bytesRead);
						oldMemory = buffer;
						oldMemory[0] = BitConverter.GetBytes(1)[0];
						WriteProcessMemory((int)processHandle, characterOffset + 0x1F4, oldMemory, oldMemory.Length, ref bytesWritten);
					}
					else
					{
						ReadProcessMemory((int)processHandle, characterOffset + 0x1F4, buffer, buffer.Length, ref bytesRead);
						oldMemory = buffer;
						oldMemory[0] = BitConverter.GetBytes(65)[0];
						WriteProcessMemory((int)processHandle, characterOffset + 0x1F4, oldMemory, oldMemory.Length, ref bytesWritten);
					}
				}
				else if (boostUnlocked)
				{
					if (!trampolineUnlocked)
					{
						ReadProcessMemory((int)processHandle, characterOffset + 0x1F4, buffer, buffer.Length, ref bytesRead);
						oldMemory = buffer;
						oldMemory[0] = BitConverter.GetBytes(128)[0];
						WriteProcessMemory((int)processHandle, characterOffset + 0x1F4, oldMemory, oldMemory.Length, ref bytesWritten);
					}

				}
				else
				{
					ReadProcessMemory((int)processHandle, characterOffset + 0x1F4, buffer, buffer.Length, ref bytesRead);
					oldMemory = buffer;
					oldMemory[0] = BitConverter.GetBytes(192)[0];
					WriteProcessMemory((int)processHandle, characterOffset + 0x1F4, oldMemory, oldMemory.Length, ref bytesWritten);
				}
				if (flingUnlocked)
				{
					ReadProcessMemory((int)processHandle, characterOffset + 0x1F5, buffer, buffer.Length, ref bytesRead);
					oldMemory = buffer;
					oldMemory[0] = BitConverter.GetBytes(128)[0];
					WriteProcessMemory((int)processHandle, characterOffset + 0x1F5, oldMemory, oldMemory.Length, ref bytesWritten);
				}
                ReadProcessMemory((int)processHandle, characterOffset + 0xC7, buffer, buffer.Length, ref bytesRead);
                oldMemory = buffer;
				if (oldMemory[0] == 9)
                {
                    oldMemory[0] = BitConverter.GetBytes(10)[0];
                    WriteProcessMemory((int)processHandle, characterOffset + 0xC7, oldMemory, oldMemory.Length, ref bytesWritten);
                    session.Locations.CompleteLocationChecks(session.Locations.GetLocationIdFromName(game, "Park - Slide First Token"));
                    session.Locations.CompleteLocationChecks(session.Locations.GetLocationIdFromName(game, "Park - Slide Second Token"));
                    session.Locations.CompleteLocationChecks(session.Locations.GetLocationIdFromName(game, "Park - Slide Third Token"));
                    session.Locations.CompleteLocationChecks(session.Locations.GetLocationIdFromName(game, "Park - Slide Fourth Token"));
                    session.Locations.CompleteLocationChecks(session.Locations.GetLocationIdFromName(game, "Park - Slide Fifth Token"));
                    session.Locations.CompleteLocationChecks(session.Locations.GetLocationIdFromName(game, "Park - Slide Sixth Token"));
                    session.Locations.CompleteLocationChecks(session.Locations.GetLocationIdFromName(game, "Park - Slide Seventh Token"));
                    session.Locations.CompleteLocationChecks(session.Locations.GetLocationIdFromName(game, "Park - Slide Eigth Token"));
                    session.Locations.CompleteLocationChecks(session.Locations.GetLocationIdFromName(game, "Park - Slide Ninth Token"));
                    session.Locations.CompleteLocationChecks(session.Locations.GetLocationIdFromName(game, "Park - Slide Tenth Token"));
                }

            }

			static void OnMessageReceived(LogMessage message)
			{
				DisplayOnScreen(message.ToString());
			}

            static void CurrentDomain_ProcessExit(object sender, EventArgs e)
            {
                Thread.Sleep(1000);
                Console.WriteLine("exit");
            }


            static void HandleItem(ReceivedItemsHelper itemHandler)
			{

				//Console.WriteLine(baseAddress);
				// The event is fired for every received item. Keep a local index to compare against in the case of continuing
				// an existing session after an earlier disconnection.
				var itemToHandle = itemHandler.DequeueItem();
				itemIndex += 1;
				//Console.WriteLine(itemToHandle.ItemDisplayName);
				if (!loadedChecks.Contains(itemToHandle.ItemName))
				{
					// Since we dequeued the current item already, the index on the handler is the index of the *next* upcoming item,
					// not the currently processing one.
					//Console.WriteLine(itemToHandle.ItemName);
					// Do whatever other item handling
					Reward reward = new Reward();
					buffer = new byte[4];
					foreach (Reward archipelagoReward in rewards)
					{
						if (archipelagoReward.text == itemToHandle.ItemName)
						{
							reward = archipelagoReward;
							break;
						}
					}
					if (reward.text.Contains("Urban"))
					{
						urbanUnlocked = true;
					}
					if (reward.text.Contains("Desert"))
					{
						desertUnlocked = true;
					}
					if (reward.text.Contains("Arctic"))
					{
						arcticUnlocked = true;
					}
					if (reward.text == "Mike Attack")
					{
						mikeAttackUnlocked = true;
					}
					if (reward.text == "Mike Jump")
					{
						mikeJumpUnlocked = true;
					}
                    if (reward.text == "Sully Attack")
                    {
                        sullyAttackUnlocked = true;
                    }
                    if (reward.text == "Sully Jump")
                    {
                        sullyJumpUnlocked = true;
                    }
                    if (reward.text.Contains("ParkTreeDoor"))
					{
						parkTreeDoorUnlocked = true;
					}
					if (reward.text.Contains("DocksCraneBox"))
					{
						docksCraneBoxUnlcoked = true;
					}
					if (reward.text.Contains("Trampoline"))
					{
						trampolineUnlocked = true;
					}
					if (reward.text.Contains("Boost"))
					{
						boostUnlocked = true;
					}
					if (reward.text.Contains("Flingshot"))
					{
						flingUnlocked = true;
					}
					if (reward.text.Contains("TombEntrance"))
					{
						tombEntranceUnlocked = true;
					}
					/*
					if (currCharacter == 0)
					{
						if (sullyAttackUnlocked == false)
						{
							ReadProcessMemory((int)processHandle, characterOffset + 0x20, buffer, buffer.Length, ref bytesRead);
							oldMemory = buffer;
							oldMemory[0] = BitConverter.GetBytes(0)[0];
							WriteProcessMemory((int)processHandle, characterOffset + 0x20, oldMemory, oldMemory.Length, ref bytesWritten);
						}
						if (sullyJumpUnlocked == false)
						{
							ReadProcessMemory((int)processHandle, characterOffset + 0x26, buffer, buffer.Length, ref bytesRead);
							oldMemory = buffer;
							oldMemory[0] = BitConverter.GetBytes(0)[0];
							WriteProcessMemory((int)processHandle, characterOffset + 0x26, oldMemory, oldMemory.Length, ref bytesWritten);
						}
					}*/
					if (reward.text.Contains("Ooze"))
					{
						if (reward.text.Contains("Park"))
						{
							parkOoze += oozeAmount;
						}
                        if (reward.text.Contains("Downtown"))
                        {
                            downtownOoze += oozeAmount;
                        }
                        if (reward.text.Contains("Docks"))
                        {
                            docksOoze += oozeAmount;
                        }
                        if (reward.text.Contains("Market"))
                        {
                            marketOoze += oozeAmount;
                        }
                        if (reward.text.Contains("Sphinx"))
                        {
                            sphinxOoze += oozeAmount;
                        }
                        if (reward.text.Contains("Oasis"))
                        {
                            oasisOoze += oozeAmount;
                        }
                        if (reward.text.Contains("Tomb"))
                        {
                            tombOoze += oozeAmount;
                        }
                        if (reward.text.Contains("Pyramid"))
                        {
                            pyramidOoze += oozeAmount;
                        }
                        if (reward.text.Contains("Shack"))
                        {
                            shackOoze += oozeAmount;
                        }
                        if (reward.text.Contains("SkiLift"))
                        {
                            skiliftOoze += oozeAmount;
                        }
                        if (reward.text.Contains("Iceberg"))
                        {
                            icebergOoze += oozeAmount;
                        }
                        if (reward.text.Contains("HotSprings"))
                        {
                            hotspringsOoze += oozeAmount;
                        }
                    }
					if (itemToHandle.ItemName != null && !loadedChecks.Contains(itemToHandle.ItemName) && !itemToHandle.ItemName.Contains("Ooze") && !itemToHandle.ItemName.Equals("Booga") && !itemToHandle.ItemName.Contains("Mike") && !itemToHandle.ItemName.Contains("Sully"))
					{
                        StreamWriter sw = new StreamWriter(path, true);
                        sw.WriteLine(itemToHandle.ItemName);
                        sw.Close();
                    }
					if(itemToHandle.ItemId < 100)
					{
						string text = itemToHandle.ItemId.ToString() + " meterAmount";
						if (!loadedChecks.Contains(text))
						{
							StreamWriter sw = new StreamWriter(path, true);
							sw.WriteLine(text);
							sw.Close();
						}
						oozeAmount = Convert.ToInt32(itemToHandle.ItemId);
                    }
				}
				//Console.WriteLine("Collected " + reward.text + " from " + itemToHandle.Player);

			}
		}

		private static void DisplayOnScreen(string message)
		{
			Console.WriteLine(message);
		}

		static void Initialize()
		{
			Process[] processArray = Process.GetProcessesByName("MonstersInc");
			//Console.WriteLine(processArray.Length);
			if (processArray.Length < 1)
			{
				Console.WriteLine("Monsters Inc. process not found");
			}
			while (processArray.Length < 1)
			{

			}
			ReadProcessMemory((int)processHandle, levelAddress, levelBuffer, levelBuffer.Length, ref bytesRead);
			//Console.WriteLine(System.Text.Encoding.UTF8.GetString(levelBuffer));

			bytesRead = 0;
			byte[] oozeBarBuffer = new byte[4];
			oozeAddress = (int)baseAddress + 0x00035EAC;
			ReadProcessMemory((int)processHandle, oozeAddress, oozeBarBuffer, oozeBarBuffer.Length, ref bytesRead);
			oozeOffset = BitConverter.ToInt32(oozeBarBuffer);
			ReadProcessMemory((int)processHandle, oozeOffset + 0x21, buffer, buffer.Length, ref bytesRead);
			//Console.WriteLine(buffer[0]);
			int seedHash;
			Random random;

			int hash1 = 5381;
			int hash2 = hash1;
			//Console.WriteLine(random.NextDouble());
			string[] checksList = {"ParkOoze1", "ParkOoze2", "ParkOoze3", "ParkOoze4", "ParkToken1", "ParkToken2", "ParkToken3", "ParkToken4", "ParkToken5", "ParkToken6", "ParkToken7", "ParkToken8", "ParkToken9", "ParkToken10", "ParkNerve1", "ParkNerve2", "ParkNerve3", "ParkNerve4", "ParkNerve5", "ParkNerve6", "ParkNerve7", "ParkNerve8", "ParkBronze", "ParkSilver", "ParkGold",
			"DowntownOoze1", "DowntownOoze2", "DowntownOoze3", "DowntownOoze4", "DowntownToken1", "DowntownToken2", "DowntownToken3", "DowntownToken4", "DowntownToken5", "DowntownToken6", "DowntownToken7", "DowntownToken8", "DowntownToken9", "DowntownToken10", "DowntownNerve1", "DowntownNerve2", "DowntownNerve3", "DowntownNerve4", "DowntownNerve5", "DowntownNerve6", "DowntownNerve7", "DowntownNerve8", "DowntownBronze", "DowntownSilver", "DowntownGold",
			"DocksOoze1", "DocksOoze2", "DocksOoze3", "DocksOoze4", "DocksToken1", "DocksToken2", "DocksToken3", "DocksToken4", "DocksToken5", "DocksToken6", "DocksToken7", "DocksToken8", "DocksToken9", "DocksToken10", "DocksNerve1", "DocksNerve2", "DocksNerve3", "DocksNerve4", "DocksNerve5", "DocksNerve6", "DocksNerve7", "DocksNerve8", "DocksBronze", "DocksSilver", "DocksGold",
			"MarketOoze1", "MarketOoze2", "MarketOoze3", "MarketOoze4", "MarketToken1", "MarketToken2", "MarketToken3", "MarketToken4", "MarketToken5", "MarketToken6", "MarketToken7", "MarketToken8", "MarketToken9", "MarketToken10", "MarketNerve1", "MarketNerve2", "MarketNerve3", "MarketNerve4", "MarketNerve5", "MarketNerve6", "MarketNerve7", "MarketNerve8", "MarketBronze", "MarketSilver", "MarketGold",
			"SphinxOoze1", "SphinxOoze2", "SphinxOoze3", "SphinxOoze4", "SphinxToken1", "SphinxToken2", "SphinxToken3", "SphinxToken4", "SphinxToken5", "SphinxToken6", "SphinxToken7", "SphinxToken8", "SphinxToken9", "SphinxToken10", "SphinxNerve1", "SphinxNerve2", "SphinxNerve3", "SphinxNerve4", "SphinxNerve5", "SphinxNerve6", "SphinxNerve7", "SphinxNerve8", "SphinxBronze", "SphinxSilver", "SphinxGold",
			"OasisOoze1", "OasisOoze2", "OasisOoze3", "OasisOoze4", "OasisToken1", "OasisToken2", "OasisToken3", "OasisToken4", "OasisToken5", "OasisToken6", "OasisToken7", "OasisToken8", "OasisToken9", "OasisToken10", "OasisNerve1", "OasisNerve2", "OasisNerve3", "OasisNerve4", "OasisNerve5", "OasisNerve6", "OasisNerve7", "OasisNerve8", "OasisBronze", "OasisSilver", "OasisGold",
			"TombOoze1", "TombOoze2", "TombOoze3", "TombOoze4", "TombToken1", "TombToken2", "TombToken3", "TombToken4", "TombToken5", "TombToken6", "TombToken7", "TombToken8", "TombToken9", "TombToken10", "TombNerve1", "TombNerve2", "TombNerve3", "TombNerve4", "TombNerve5", "TombNerve6", "TombNerve7", "TombNerve8", "TombBronze", "TombSilver", "TombGold",
			"PyramidOoze1", "PyramidOoze2", "PyramidOoze3", "PyramidOoze4", "PyramidToken1", "PyramidToken2", "PyramidToken3", "PyramidToken4", "PyramidToken5", "PyramidToken6", "PyramidToken7", "PyramidToken8", "PyramidToken9", "PyramidToken10", "PyramidNerve1", "PyramidNerve2", "PyramidNerve3", "PyramidNerve4", "PyramidNerve5", "PyramidNerve6", "PyramidNerve7", "PyramidNerve8", "PyramidBronze", "PyramidSilver", "PyramidGold",
			"ShackOoze1", "ShackOoze2", "ShackOoze3", "ShackOoze4", "ShackToken1", "ShackToken2", "ShackToken3", "ShackToken4", "ShackToken5", "ShackToken6", "ShackToken7", "ShackToken8", "ShackToken9", "ShackToken10", "ShackNerve1", "ShackNerve2", "ShackNerve3", "ShackNerve4", "ShackNerve5", "ShackNerve6", "ShackNerve7", "ShackNerve8", "ShackBronze", "ShackSilver", "ShackGold",
			"SkiLiftOoze1", "SkiLiftOoze2", "SkiLiftOoze3", "SkiLiftOoze4", "SkiLiftToken1", "SkiLiftToken2", "SkiLiftToken3", "SkiLiftToken4", "SkiLiftToken5", "SkiLiftToken6", "SkiLiftToken7", "SkiLiftToken8", "SkiLiftToken9", "SkiLiftToken10", "SkiLiftNerve1", "SkiLiftNerve2", "SkiLiftNerve3", "SkiLiftNerve4", "SkiLiftNerve5", "SkiLiftNerve6", "SkiLiftNerve7", "SkiLiftNerve8", "SkiLiftBronze", "SkiLiftSilver", "SkiLiftGold",
			"IcebergOoze1", "IcebergOoze2", "IcebergOoze3", "IcebergOoze4", "IcebergToken1", "IcebergToken2", "IcebergToken3", "IcebergToken4", "IcebergToken5", "IcebergToken6", "IcebergToken7", "IcebergToken8", "IcebergToken9", "IcebergToken10", "IcebergNerve1", "IcebergNerve2", "IcebergNerve3", "IcebergNerve4", "IcebergNerve5", "IcebergNerve6", "IcebergNerve7", "IcebergNerve8", "IcebergBronze", "IcebergSilver", "IcebergGold",
			"HotSpringsOoze1", "HotSpringsOoze2", "HotSpringsOoze3", "HotSpringsOoze4", "HotSpringsToken1", "HotSpringsToken2", "HotSpringsToken3", "HotSpringsToken4", "HotSpringsToken5", "HotSpringsToken6", "HotSpringsToken7", "HotSpringsToken8", "HotSpringsToken9", "HotSpringsToken10", "HotSpringsNerve1", "HotSpringsNerve2", "HotSpringsNerve3", "HotSpringsNerve4", "HotSpringsNerve5", "HotSpringsNerve6", "HotSpringsNerve7", "HotSpringsNerve8", "HotSpringsBronze", "HotSpringsSilver", "HotSpringsGold",
			"5Lives", "10Lives", "15Lives", "20Lives", "TombUnlocked"};
			//string[] levelList = {"ACCA", "AACAM", "ACTR1", "ACTR1M", "BD08", "BD08M", "BD09", "BD09M", "BD10", "BD10M", "BD11", "BD11M", "Credits", "LACCA", "LACCAM", "MO04", "MO04B", "MO04BM", "MO04M", "MO05", "MO05M", "MO06", "MO06M", "MO07", "MO07M", "Mp01", "Mp01M", "Mp02", "Mp02M", "Mp03", "Mp03M", "SP12", "SP12M", "SP13", "SP13B", "SP13BM", "SP13M", "SP14", "SP14M", "SP15", "SP15M", "SQ02", "SQ03", "SQ04", "SQ05", "SQ06", "SQ07", "SQ08", "SQFS", "SQSA", "SQTR"};
			//random.Shuffle(levelList);



			rewards = new List<Reward>();

			Level park = new Level();
			park.token1 = (0x1F5, 128, new Check(), new Reward(), "Park - Slide First Token", new string[] { "Jump", "Urban", "Attack", "Trampoline", "Park3Nerves", "ParkTreeDoor" }); //0
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F5, buffer, buffer.Length, ref bytesRead);
			park.token1.check.currValue = park.token1.check.oldValue = buffer[0];
			addressesToCheck.Add(park.token1);
			park.token2 = (0x1F6, 1, new Check(), new Reward(), "Park - Slide Second Token", new string[] { "Jump", "Urban", "Attack", "Trampoline", "Park3Nerves", "ParkTreeDoor" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F6, buffer, buffer.Length, ref bytesRead);
			park.token2.check.currValue = park.token2.check.oldValue = buffer[0];
			addressesToCheck.Add(park.token2);
			park.token3 = (0x1F6, 2, new Check(), new Reward(), "Park - Slide Third Token", new string[] { "Jump", "Urban", "Attack", "Trampoline", "Park3Nerves", "ParkTreeDoor" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F6, buffer, buffer.Length, ref bytesRead);
			park.token3.check.currValue = park.token3.check.oldValue = buffer[0];
			addressesToCheck.Add(park.token3);
			park.token4 = (0x1F6, 4, new Check(), new Reward(), "Park - Slide Fourth Token", new string[] { "Jump", "Urban", "Attack", "Trampoline", "Park3Nerves", "ParkTreeDoor" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F6, buffer, buffer.Length, ref bytesRead);
			park.token4.check.currValue = park.token4.check.oldValue = buffer[0];
			addressesToCheck.Add(park.token4);
			park.token5 = (0x1F6, 8, new Check(), new Reward(), "Park - Slide Fifth Token", new string[] { "Jump", "Urban", "Attack", "Trampoline", "Park3Nerves", "ParkTreeDoor" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F6, buffer, buffer.Length, ref bytesRead);
			park.token5.check.currValue = park.token5.check.oldValue = buffer[0];
			addressesToCheck.Add(park.token5);
			park.token6 = (0x1F6, 16, new Check(), new Reward(), "Park - Slide Sixth Token", new string[] { "Jump", "Urban", "Attack", "Trampoline", "Park3Nerves", "ParkTreeDoor" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F6, buffer, buffer.Length, ref bytesRead);
			park.token6.check.currValue = park.token6.check.oldValue = buffer[0];
			addressesToCheck.Add(park.token6);
			park.token7 = (0x1F6, 32, new Check(), new Reward(), "Park - Slide Seventh Token", new string[] { "Jump", "Urban", "Attack", "Trampoline", "Park3Nerves", "ParkTreeDoor" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F6, buffer, buffer.Length, ref bytesRead);
			park.token7.check.currValue = park.token7.check.oldValue = buffer[0];
			addressesToCheck.Add(park.token7);
			park.token8 = (0x1F6, 64, new Check(), new Reward(), "Park - Slide Eigth Token", new string[] { "Jump", "Urban", "Attack", "Trampoline", "Park3Nerves", "ParkTreeDoor" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F6, buffer, buffer.Length, ref bytesRead);
			park.token8.check.currValue = park.token8.check.oldValue = buffer[0];
			addressesToCheck.Add(park.token8);
			park.token9 = (0x1F6, 128, new Check(), new Reward(), "Park - Slide Ninth Token", new string[] { "Jump", "Urban", "Attack", "Trampoline", "Park3Nerves", "ParkTreeDoor" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F6, buffer, buffer.Length, ref bytesRead);
			park.token9.check.currValue = park.token9.check.oldValue = buffer[0];
			addressesToCheck.Add(park.token9);
			park.token10 = (0x1F7, 1, new Check(), new Reward(), "Park - Slide Tenth Token", new string[] { "Jump", "Urban", "Attack", "Trampoline", "Park3Nerves", "ParkTreeDoor" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F7, buffer, buffer.Length, ref bytesRead);
			park.token10.check.currValue = park.token10.check.oldValue = buffer[0];
			addressesToCheck.Add(park.token10);
			park.nerve1 = (0x205, 32, new Check(), new Reward(), "Park - Blue Nerve in Sandbox", new string[] { "Jump", "Urban", "Attack", "Park10Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x205, buffer, buffer.Length, ref bytesRead);
			park.nerve1.check.currValue = park.nerve1.check.oldValue = buffer[0];
			addressesToCheck.Add(park.nerve1);
			park.nerve2 = (0x205, 64, new Check(), new Reward(), "Park - Green Nerve on Monkeybars", new string[] { "Jump", "Urban", "Attack", "Park25Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x205, buffer, buffer.Length, ref bytesRead);
			park.nerve2.check.currValue = park.nerve2.check.oldValue = buffer[0];
			addressesToCheck.Add(park.nerve2);
			park.nerve3 = (0x205, 128, new Check(), new Reward(), "Park - Green Nerve in Sub Park", new string[] { "Jump", "Urban", "Attack", "Park3Nerves", "Park25Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x205, buffer, buffer.Length, ref bytesRead);
			park.nerve3.check.currValue = park.nerve3.check.oldValue = buffer[0];
			addressesToCheck.Add(park.nerve3);
			park.nerve4 = (0x206, 1, new Check(), new Reward(), "Park - Yellow Nerve Around Tree", new string[] { "Jump", "Urban", "Attack", "Park40Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x206, buffer, buffer.Length, ref bytesRead);
			park.nerve4.check.currValue = park.nerve4.check.oldValue = buffer[0];
			addressesToCheck.Add(park.nerve4);
			park.nerve5 = (0x206, 2, new Check(), new Reward(), "Park - Orange Nerve in Trash Can", new string[] { "Jump", "Urban", "Attack", "Park60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x206, buffer, buffer.Length, ref bytesRead);
			park.nerve5.check.currValue = park.nerve5.check.oldValue = buffer[0];
			addressesToCheck.Add(park.nerve5);
			park.nerve6 = (0x206, 4, new Check(), new Reward(), "Park - Red Nerve Locked by Tokens", new string[] { "Jump", "Urban", "Attack", "Park3Nerves", "ParkTreeDoor", "ParkSilverMedal", "Park100Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x206, buffer, buffer.Length, ref bytesRead);
			park.nerve6.check.currValue = park.nerve6.check.oldValue = buffer[0];
			addressesToCheck.Add(park.nerve6);
			park.nerve7 = (0x206, 8, new Check(), new Reward(), "Park - Red Nerve in Sub Park", new string[] { "Jump", "Urban", "Attack", "Park3Nerves", "Park100Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x206, buffer, buffer.Length, ref bytesRead);
			park.nerve7.check.currValue = park.nerve7.check.oldValue = buffer[0];
			addressesToCheck.Add(park.nerve7);
			park.nerve8 = (0x206, 16, new Check(), new Reward(), "Park - Red Nerve After Tree Slide", new string[] { "Jump", "Urban", "Attack", "Trampoline", "ParkTreeDoor", "Park3Nerves", "Park100Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x206, buffer, buffer.Length, ref bytesRead);
			park.nerve8.check.currValue = park.nerve8.check.oldValue = buffer[0];
			addressesToCheck.Add(park.nerve8);
			park.bronzeMedal = (0xF3, 1, new Check(), new Reward(), "Park - Bronze Medal Collected", new string[] { "Jump", "Urban", "Attack", "Park3Nerves", "Park60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF3, buffer, buffer.Length, ref bytesRead);
			park.bronzeMedal.check.currValue = park.bronzeMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(park.bronzeMedal);
			park.bronzeMedal2 = (0xF3, 1, new Check(), new Reward(), "Park - Bronze Medal", new string[] { "Jump", "Urban", "Attack", "Park3Nerves", "Park60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF3, buffer, buffer.Length, ref bytesRead);
			park.bronzeMedal2.check.currValue = park.bronzeMedal2.check.oldValue = buffer[0];
			addressesToCheck.Add(park.bronzeMedal2);
			park.silverMedal = (0xF3, 2, new Check(), new Reward(), "Park - Silver Medal Collected", new string[] { "Jump", "Urban", "Attack", "Park3Nerves", "ParkTreeDoor", "Trampoline" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF3, buffer, buffer.Length, ref bytesRead);
			park.silverMedal.check.currValue = park.silverMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(park.silverMedal);
			park.goldMedal = (0xF3, 4, new Check(), new Reward(), "Park - Gold Medal Collected", new string[] { "Jump", "Urban", "Attack", "Park3Nerves", "ParkTreeDoor", "Trampoline", "Park100Ooze", "ParkSilverMedal" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF3, buffer, buffer.Length, ref bytesRead);
			park.goldMedal.check.currValue = park.goldMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(park.goldMedal);

			Level downtown = new Level();
			downtown.token1 = (0x1F7, 2, new Check(), new Reward(), "Downtown - Conveyer Room Bottom Token", new string[] { "Jump", "Urban", "Attack", "Downtown4Nerves" }); //21
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F7, buffer, buffer.Length, ref bytesRead);
			downtown.token1.check.currValue = downtown.token1.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.token1);
			downtown.token2 = (0x1F7, 4, new Check(), new Reward(), "Downtown - Dead End by Car Token", new string[] { "Urban" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F7, buffer, buffer.Length, ref bytesRead);
			downtown.token2.check.currValue = downtown.token2.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.token2);
			downtown.token3 = (0x1F7, 8, new Check(), new Reward(), "Downtown - Mini Park by Bush Token", new string[] { "Jump", "Urban" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F7, buffer, buffer.Length, ref bytesRead);
			downtown.token3.check.currValue = downtown.token3.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.token3);
			downtown.token4 = (0x1F7, 16, new Check(), new Reward(), "Downtown - Toy Store Back Left Token", new string[] { "Jump", "Urban", "Attack", "Downtown3Nerves" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F7, buffer, buffer.Length, ref bytesRead);
			downtown.token4.check.currValue = downtown.token4.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.token4);
			downtown.token5 = (0x1F7, 32, new Check(), new Reward(), "Downtown - Toy Store Right Side Token", new string[] { "Jump", "Urban", "Attack", "Downtown3Nerves" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F7, buffer, buffer.Length, ref bytesRead);
			downtown.token5.check.currValue = downtown.token5.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.token5);
			downtown.token6 = (0x1F7, 64, new Check(), new Reward(), "Downtown - Conveyer Room Top Token", new string[] { "Jump", "Urban", "Attack", "Downtown4Nerves" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F7, buffer, buffer.Length, ref bytesRead);
			downtown.token6.check.currValue = downtown.token6.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.token6);
			downtown.token7 = (0x1F7, 128, new Check(), new Reward(), "Downtown - Outside Roof Jump Token", new string[] { "Jump", "Urban", "Attack", "Downtown4Nerves" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F7, buffer, buffer.Length, ref bytesRead);
			downtown.token7.check.currValue = downtown.token7.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.token7);
			downtown.token8 = (0x1F8, 1, new Check(), new Reward(), "Downtown - Outside Moving Platforms Token", new string[] { "Jump", "Urban", "Attack", "Downtown4Nerves" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F8, buffer, buffer.Length, ref bytesRead);
			downtown.token8.check.currValue = downtown.token8.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.token8);
			downtown.token9 = (0x1F8, 2, new Check(), new Reward(), "Downtown - Outside Bottom of Skyscraper Token", new string[] { "Jump", "Urban", "Attack", "Downtown4Nerves" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F8, buffer, buffer.Length, ref bytesRead);
			downtown.token9.check.currValue = downtown.token9.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.token9);
			downtown.token10 = (0x1F8, 4, new Check(), new Reward(), "Downtown - Outside Top of Skyscraper Token", new string[] { "Jump", "Urban", "Attack", "Trampoline", "Downtown4Nerves" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F8, buffer, buffer.Length, ref bytesRead);
			downtown.token10.check.currValue = downtown.token10.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.token10);
			downtown.nerve1 = (0x206, 32, new Check(), new Reward(), "Downtown - Blue Nerve Near Mini Park", new string[] { "Urban", "Attack", "Downtown10Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x206, buffer, buffer.Length, ref bytesRead);
			downtown.nerve1.check.currValue = downtown.nerve1.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.nerve1);
			downtown.nerve2 = (0x206, 64, new Check(), new Reward(), "Downtown - Green Nerve in Mini Park", new string[] { "Jump", "Urban", "Attack", "Downtown25Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x206, buffer, buffer.Length, ref bytesRead);
			downtown.nerve2.check.currValue = downtown.nerve2.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.nerve2);
			downtown.nerve3 = (0x206, 128, new Check(), new Reward(), "Downtown - Yellow Nerve in Toystore", new string[] { "Jump", "Urban", "Attack", "Downtown40Ooze", "Downtown3Nerves" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x206, buffer, buffer.Length, ref bytesRead);
			downtown.nerve3.check.currValue = downtown.nerve3.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.nerve3);
			downtown.nerve4 = (0x207, 1, new Check(), new Reward(), "Downtown - Orange Nerve in Toystore", new string[] { "Jump", "Urban", "Attack", "Downtown60Ooze", "Downtown3Nerves" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x207, buffer, buffer.Length, ref bytesRead);
			downtown.nerve4.check.currValue = downtown.nerve4.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.nerve4);
			downtown.nerve5 = (0x207, 2, new Check(), new Reward(), "Downtown - Red Nerve Locked Behind Tokens", new string[] { "Jump", "Urban", "Attack", "Downtown100Ooze", "Downtown4nerves", "DowntownSilverMedal" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x207, buffer, buffer.Length, ref bytesRead);
			downtown.nerve5.check.currValue = downtown.nerve5.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.nerve5);
			downtown.nerve6 = (0x207, 4, new Check(), new Reward(), "Downtown - Red Nerve Top of Skyscraper", new string[] { "Jump", "Urban", "Attack", "Downtown100Ooze", "Trampoline", "Downtown4Nerves" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x207, buffer, buffer.Length, ref bytesRead);
			downtown.nerve6.check.currValue = downtown.nerve6.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.nerve6);
			downtown.nerve7 = (0x207, 8, new Check(), new Reward(), "Downtown - Red Nerve Across the Rooftops", new string[] { "Jump", "Urban", "Attack", "Downtown100Ooze", "Downtown4Nerves" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x207, buffer, buffer.Length, ref bytesRead);
			downtown.nerve7.check.currValue = downtown.nerve7.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.nerve7);
			downtown.nerve8 = (0x207, 16, new Check(), new Reward(), "Downtown - Green Nerve in Sewers", new string[] { "Jump", "Urban", "Attack", "Downtown40Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x207, buffer, buffer.Length, ref bytesRead);
			downtown.nerve8.check.currValue = downtown.nerve8.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.nerve8);
			downtown.bronzeMedal = (0xF4, 1, new Check(), new Reward(), "Downtown - Bronze Medal Collected", new string[] { "Jump", "Urban", "Attack", "Downtown3Nerves", "Downtown60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF4, buffer, buffer.Length, ref bytesRead);
			downtown.bronzeMedal.check.currValue = downtown.bronzeMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.bronzeMedal);
			downtown.bronzeMedal2 = (0xF4, 1, new Check(), new Reward(), "Downtown - Bronze Medal", new string[] { "Jump", "Urban", "Attack", "Downtown3Nerves", "Downtown60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF4, buffer, buffer.Length, ref bytesRead);
			downtown.bronzeMedal2.check.currValue = downtown.bronzeMedal2.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.bronzeMedal2);
			downtown.silverMedal = (0xF4, 2, new Check(), new Reward(), "Downtown - Silver Medal Collected", new string[] { "Jump", "Urban", "Attack", "Downtown4Nerves", "Trampoline" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF4, buffer, buffer.Length, ref bytesRead);
			downtown.silverMedal.check.currValue = downtown.silverMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.silverMedal);
			downtown.goldMedal = (0xF4, 4, new Check(), new Reward(), "Downtown - Gold Medal Collected", new string[] { "Jump", "Urban", "Attack", "Trampoline", "Downtown100Ooze", "DowntownSilverMedal" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF4, buffer, buffer.Length, ref bytesRead);
			downtown.goldMedal.check.currValue = downtown.goldMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(downtown.goldMedal);


			Level docks = new Level();
			docks.token1 = (0x1F8, 8, new Check(), new Reward(), "Docks - By No Swimming Sign Token", new string[] { "Urban" }); //42
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F8, buffer, buffer.Length, ref bytesRead);
			docks.token1.check.currValue = docks.token1.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.token1);
			docks.token2 = (0x1F8, 16, new Check(), new Reward(), "Docks - In Air in Front of Boat", new string[] { "Jump", "Urban" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F8, buffer, buffer.Length, ref bytesRead);
			docks.token2.check.currValue = docks.token2.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.token2);
			docks.token3 = (0x1F8, 32, new Check(), new Reward(), "Docks - Trampoline in Alley Token", new string[] { "Jump", "Trampoline", "Urban" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F8, buffer, buffer.Length, ref bytesRead);
			docks.token3.check.currValue = docks.token3.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.token3);
			docks.token4 = (0x1F8, 64, new Check(), new Reward(), "Docks - Trampoline by Lighthouse Token", new string[] { "Jump", "Trampoline", "Urban" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F8, buffer, buffer.Length, ref bytesRead);
			docks.token4.check.currValue = docks.token4.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.token4);
			docks.token5 = (0x1F8, 128, new Check(), new Reward(), "Docks - Back of Boat Token", new string[] { "Jump", "Urban" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F8, buffer, buffer.Length, ref bytesRead);
			docks.token5.check.currValue = docks.token5.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.token5);
			docks.token6 = (0x1F9, 1, new Check(), new Reward(), "Docks - In Box by Entrance Token", new string[] { "Jump or Attack", "Urban" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F9, buffer, buffer.Length, ref bytesRead);
			docks.token6.check.currValue = docks.token6.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.token6);
			docks.token7 = (0x1F9, 2, new Check(), new Reward(), "Docks - In Box in Front of Boat Token", new string[] { "Jump or Attack", "Urban" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F9, buffer, buffer.Length, ref bytesRead);
			docks.token7.check.currValue = docks.token7.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.token7);
			docks.token8 = (0x1F9, 4, new Check(), new Reward(), "Docks - In Box on Middle Deck of Boat Token", new string[] { "Jump", "Urban" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F9, buffer, buffer.Length, ref bytesRead);
			docks.token8.check.currValue = docks.token8.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.token8);
			docks.token9 = (0x1F9, 8, new Check(), new Reward(), "Docks - In Box on Bottom Deck of Boat Token", new string[] { "Jump", "Urban" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F9, buffer, buffer.Length, ref bytesRead);
			docks.token9.check.currValue = docks.token9.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.token9);
			docks.token10 = (0x1F9, 16, new Check(), new Reward(), "Docks - Inside Boat Token", new string[] { "Jump", "Urban", "Attack", "Docks4Nerves" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F9, buffer, buffer.Length, ref bytesRead);
			docks.token10.check.currValue = docks.token10.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.token10);
			docks.nerve1 = (0x207, 32, new Check(), new Reward(), "Docks - Green Nerve by Lighthouse", new string[] { "Jump", "Urban", "Attack", "Docks25Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x207, buffer, buffer.Length, ref bytesRead);
			docks.nerve1.check.currValue = docks.nerve1.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.nerve1);
			docks.nerve2 = (0x207, 64, new Check(), new Reward(), "Docks - Green Nerve on Pier", new string[] { "Jump", "Urban", "Attack", "Docks25Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x207, buffer, buffer.Length, ref bytesRead);
			docks.nerve2.check.currValue = docks.nerve2.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.nerve2);
			docks.nerve3 = (0x207, 128, new Check(), new Reward(), "Docks - Yellow Nerve On Top of Lighthouse", new string[] { "Jump", "Urban", "Attack", "Docks40Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x207, buffer, buffer.Length, ref bytesRead);
			docks.nerve3.check.currValue = docks.nerve3.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.nerve3);
			docks.nerve4 = (0x208, 1, new Check(), new Reward(), "Docks - Orange Nerve On Front of Boat", new string[] { "Jump", "Urban", "Attack", "Docks60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x208, buffer, buffer.Length, ref bytesRead);
			docks.nerve4.check.currValue = docks.nerve4.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.nerve4);
			docks.nerve5 = (0x208, 2, new Check(), new Reward(), "Docks - Red Nerve Locked by Tokens", new string[] { "Jump", "Urban", "Attack", "Docks100Ooze", "DocksSilverMedal", "DocksCraneBox" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x208, buffer, buffer.Length, ref bytesRead);
			docks.nerve5.check.currValue = docks.nerve5.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.nerve5);
			docks.nerve6 = (0x208, 4, new Check(), new Reward(), "Docks - Red Nerve on Boat Mast", new string[] { "Jump", "Urban", "Attack", "Docks100Ooze", "DocksCraneBox" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x208, buffer, buffer.Length, ref bytesRead);
			docks.nerve6.check.currValue = docks.nerve6.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.nerve6);
			docks.nerve7 = (0x208, 8, new Check(), new Reward(), "Docks - Red Nerve Inside Boat", new string[] { "Jump", "Urban", "Attack", "Docks100Ooze", "Docks4Nerves" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x208, buffer, buffer.Length, ref bytesRead);
			docks.nerve7.check.currValue = docks.nerve7.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.nerve7);
			docks.nerve8 = (0x208, 16, new Check(), new Reward(), "Docks - Blue Nerve in Box at Spawn", new string[] { "Jump", "Urban", "Attack", "Docks10Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x208, buffer, buffer.Length, ref bytesRead);
			docks.nerve8.check.currValue = docks.nerve8.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.nerve8);
			docks.bronzeMedal = (0xF5, 1, new Check(), new Reward(), "Docks - Bronze Medal Collected", new string[] { "Jump", "Urban", "Attack", "Docks60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF5, buffer, buffer.Length, ref bytesRead);
			docks.bronzeMedal.check.currValue = docks.bronzeMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.bronzeMedal);
			docks.bronzeMedal2 = (0xF5, 1, new Check(), new Reward(), "Docks - Bronze Medal", new string[] { "Jump", "Urban", "Attack", "Docks60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF5, buffer, buffer.Length, ref bytesRead);
			docks.bronzeMedal2.check.currValue = docks.bronzeMedal2.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.bronzeMedal2);
			docks.silverMedal = (0xF5, 2, new Check(), new Reward(), "Docks - Silver Medal Collected", new string[] { "Jump", "Urban", "Attack", "Docks4Nerves", "Trampoline" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF5, buffer, buffer.Length, ref bytesRead);
			docks.silverMedal.check.currValue = docks.silverMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.silverMedal);
			docks.goldMedal = (0xF5, 4, new Check(), new Reward(), "Docks - Gold Medal Collected", new string[] { "Jump", "Urban", "Attack", "DocksCraneBox", "Docks100Ooze", "DocksSilverMedal" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF5, buffer, buffer.Length, ref bytesRead);
			docks.goldMedal.check.currValue = docks.goldMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(docks.goldMedal);

			Level market = new Level();
			market.token1 = (0x1F9, 32, new Check(), new Reward(), "Market - On Top of Buildings Token", new string[] { "Jump", "Trampoline", "Urban" }); //63
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F9, buffer, buffer.Length, ref bytesRead);
			market.token1.check.currValue = market.token1.check.oldValue = buffer[0];
			addressesToCheck.Add(market.token1);
			market.token2 = (0x1F9, 64, new Check(), new Reward(), "Market - In Box Back Right of Level Token", new string[] { "Jump", "Urban" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F9, buffer, buffer.Length, ref bytesRead);
			market.token2.check.currValue = market.token2.check.oldValue = buffer[0];
			addressesToCheck.Add(market.token2);
			market.token3 = (0x1F9, 128, new Check(), new Reward(), "Market - Left of Entrance Token", new string[] { "Jump", "Urban" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1F9, buffer, buffer.Length, ref bytesRead);
			market.token3.check.currValue = market.token3.check.oldValue = buffer[0];
			addressesToCheck.Add(market.token3);
			market.token4 = (0x1FA, 1, new Check(), new Reward(), "Market - Trampoline Ledge Behind Red Nerve Token", new string[] { "Jump", "Trampoline", "Urban" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FA, buffer, buffer.Length, ref bytesRead);
			market.token4.check.currValue = market.token4.check.oldValue = buffer[0];
			addressesToCheck.Add(market.token4);
			market.token5 = (0x1FA, 2, new Check(), new Reward(), "Market - In Box in Cut Through Token", new string[] { "Jump", "Urban" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FA, buffer, buffer.Length, ref bytesRead);
			market.token5.check.currValue = market.token5.check.oldValue = buffer[0];
			addressesToCheck.Add(market.token5);
			market.token6 = (0x1FA, 4, new Check(), new Reward(), "Market - Left of Sewer Entrance Token", new string[] { "Jump", "Urban" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FA, buffer, buffer.Length, ref bytesRead);
			market.token6.check.currValue = market.token6.check.oldValue = buffer[0];
			addressesToCheck.Add(market.token6);
			market.token7 = (0x1FA, 8, new Check(), new Reward(), "Market - Floating Box by Blue Nerve Token", new string[] { "Jump", "Urban" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FA, buffer, buffer.Length, ref bytesRead);
			market.token7.check.currValue = market.token7.check.oldValue = buffer[0];
			addressesToCheck.Add(market.token7);
			market.token8 = (0x1FA, 16, new Check(), new Reward(), "Market - In Sewer Token", new string[] { "Jump", "Urban", "Attack", "MarketSewerEntrance" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FA, buffer, buffer.Length, ref bytesRead);
			market.token8.check.currValue = market.token8.check.oldValue = buffer[0];
			addressesToCheck.Add(market.token8);
			market.token9 = (0x1FA, 32, new Check(), new Reward(), "Market - On Waternoose Statue Token", new string[] { "Jump", "Urban", "Attack", "MarketSewerEntrance" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FA, buffer, buffer.Length, ref bytesRead);
			market.token9.check.currValue = market.token9.check.oldValue = buffer[0];
			addressesToCheck.Add(market.token9);
			market.token10 = (0x1FA, 64, new Check(), new Reward(), "Market - Trampoline Ledge in Front of Red Nerve Token", new string[] { "Jump", "Trampoline", "Urban" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FA, buffer, buffer.Length, ref bytesRead);
			market.token10.check.currValue = market.token10.check.oldValue = buffer[0];
			addressesToCheck.Add(market.token10);
			market.nerve1 = (0x208, 32, new Check(), new Reward(), "Market - Red Nerve Above Trampoline", new string[] { "Jump", "Urban", "Attack", "Market100Ooze", "Trampoline" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x208, buffer, buffer.Length, ref bytesRead);
			market.nerve1.check.currValue = market.nerve1.check.oldValue = buffer[0];
			addressesToCheck.Add(market.nerve1);
			market.nerve2 = (0x208, 64, new Check(), new Reward(), "Market - Green Nerve by Spinning Top", new string[] { "Jump", "Urban", "Attack", "Market25Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x208, buffer, buffer.Length, ref bytesRead);
			market.nerve2.check.currValue = market.nerve2.check.oldValue = buffer[0];
			addressesToCheck.Add(market.nerve2);
			market.nerve3 = (0x208, 128, new Check(), new Reward(), "Market - Blue Nerve by Rivers", new string[] { "Jump", "Urban", "Attack", "Market10Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x208, buffer, buffer.Length, ref bytesRead);
			market.nerve3.check.currValue = market.nerve3.check.oldValue = buffer[0];
			addressesToCheck.Add(market.nerve3);
			market.nerve4 = (0x209, 1, new Check(), new Reward(), "Market - Yellow Nerve Before Sewer Entrance", new string[] { "Jump", "Urban", "Attack", "Market40Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x209, buffer, buffer.Length, ref bytesRead);
			market.nerve4.check.currValue = market.nerve4.check.oldValue = buffer[0];
			addressesToCheck.Add(market.nerve4);
			market.nerve5 = (0x209, 16, new Check(), new Reward(), "Market - Red Nerve Blocked by Tokens", new string[] { "Jump", "Urban", "Attack", "Market100Ooze", "MarketSewerEntrance", "MarketSilver" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x209, buffer, buffer.Length, ref bytesRead);
			market.nerve5.check.currValue = market.nerve5.check.oldValue = buffer[0];
			addressesToCheck.Add(market.nerve5);
			market.nerve6 = (0x209, 4, new Check(), new Reward(), "Market - Orange Nerve on Waternoose Statue", new string[] { "Jump", "Urban", "Attack", "Market60Ooze", "MarketSewerEntrance" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x209, buffer, buffer.Length, ref bytesRead);
			market.nerve6.check.currValue = market.nerve6.check.oldValue = buffer[0];
			addressesToCheck.Add(market.nerve6);
			market.nerve7 = (0x209, 8, new Check(), new Reward(), "Market - Yellow Nerve in Sewers", new string[] { "Jump", "Urban", "Attack", "Market40Ooze", "MarketSewerEntrance" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x209, buffer, buffer.Length, ref bytesRead);
			market.nerve7.check.currValue = market.nerve7.check.oldValue = buffer[0];
			addressesToCheck.Add(market.nerve7);
			market.nerve8 = (0x209, 2, new Check(), new Reward(), "Market - Red Nerve on Tall Rooftop", new string[] { "Jump", "Urban", "Attack", "Market100Ooze", "Trampoline" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x209, buffer, buffer.Length, ref bytesRead);
			market.nerve8.check.currValue = market.nerve8.check.oldValue = buffer[0];
			addressesToCheck.Add(market.nerve8);
			market.bronzeMedal = (0xF6, 1, new Check(), new Reward(), "Market - Bronze Medal Collected", new string[] { "Jump", "Urban", "Attack", "Market60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF6, buffer, buffer.Length, ref bytesRead);
			market.bronzeMedal.check.currValue = market.bronzeMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(market.bronzeMedal);
			market.bronzeMedal2 = (0xF6, 1, new Check(), new Reward(), "Market - Bronze Medal", new string[] { "Jump", "Urban", "Attack", "Market60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF6, buffer, buffer.Length, ref bytesRead);
			market.bronzeMedal2.check.currValue = market.bronzeMedal2.check.oldValue = buffer[0];
			addressesToCheck.Add(market.bronzeMedal2);
			market.silverMedal = (0xF6, 2, new Check(), new Reward(), "Market - Silver Medal Collected", new string[] { "Jump", "Urban", "Attack", "MarketSewerEntrance", "Trampoline" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF6, buffer, buffer.Length, ref bytesRead);
			market.silverMedal.check.currValue = market.silverMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(market.silverMedal);
			market.goldMedal = (0xF6, 4, new Check(), new Reward(), "Market - Gold Medal Collected", new string[] { "Jump", "Urban", "Attack", "Market100Ooze", "MarketSilver" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF6, buffer, buffer.Length, ref bytesRead);
			market.goldMedal.check.currValue = market.goldMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(market.goldMedal);

			Level sphinx = new Level();
			sphinx.token1 = (0x1FA, 128, new Check(), new Reward(), "Sphinx - By Dots Button Token", new string[] { "Jump", "Desert" }); //84
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FA, buffer, buffer.Length, ref bytesRead);
			sphinx.token1.check.currValue = sphinx.token1.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.token1);
			sphinx.token2 = (0x1FB, 1, new Check(), new Reward(), "Sphinx - Left Side Sphinx Base Token", new string[] { "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FB, buffer, buffer.Length, ref bytesRead);
			sphinx.token2.check.currValue = sphinx.token2.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.token2);
			sphinx.token3 = (0x1FB, 2, new Check(), new Reward(), "Sphinx - By Speed Boost Button Token", new string[] { "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FB, buffer, buffer.Length, ref bytesRead);
			sphinx.token3.check.currValue = sphinx.token3.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.token3);
			sphinx.token4 = (0x1FB, 4, new Check(), new Reward(), "Sphinx - By Fire Button Token", new string[] { "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FB, buffer, buffer.Length, ref bytesRead);
			sphinx.token4.check.currValue = sphinx.token4.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.token4);
			sphinx.token5 = (0x1FB, 8, new Check(), new Reward(), "Sphinx - On Top of Pillar Token", new string[] { "Jump", "Boost", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FB, buffer, buffer.Length, ref bytesRead);
			sphinx.token5.check.currValue = sphinx.token5.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.token5);
			sphinx.token6 = (0x1FB, 16, new Check(), new Reward(), "Sphinx - By Scarab Button Token", new string[] { "Jump", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FB, buffer, buffer.Length, ref bytesRead);
			sphinx.token6.check.currValue = sphinx.token6.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.token6);
			sphinx.token7 = (0x1FB, 32, new Check(), new Reward(), "Sphinx - Jump Off of High Platform Token", new string[] { "Jump", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FB, buffer, buffer.Length, ref bytesRead);
			sphinx.token7.check.currValue = sphinx.token7.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.token7);
			sphinx.token8 = (0x1FB, 64, new Check(), new Reward(), "Sphinx - By Sun Button Token", new string[] { "Jump", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FB, buffer, buffer.Length, ref bytesRead);
			sphinx.token8.check.currValue = sphinx.token8.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.token8);
			sphinx.token9 = (0x1FB, 128, new Check(), new Reward(), "Sphinx - Inside of Sphinx on Pillar Token", new string[] { "Jump", "Boost", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FB, buffer, buffer.Length, ref bytesRead);
			sphinx.token9.check.currValue = sphinx.token9.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.token9);
			sphinx.token10 = (0x1FC, 1, new Check(), new Reward(), "Sphinx - Back of Inside of Sphinx Token", new string[] { "Jump", "Boost", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FC, buffer, buffer.Length, ref bytesRead);
			sphinx.token10.check.currValue = sphinx.token10.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.token10);
			sphinx.nerve1 = (0x209, 32, new Check(), new Reward(), "Sphinx - Yellow Nerve by Hammer Scorpion", new string[] { "Jump", "Desert", "Attack", "Sphinx40Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x209, buffer, buffer.Length, ref bytesRead);
			sphinx.nerve1.check.currValue = sphinx.nerve1.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.nerve1);
			sphinx.nerve2 = (0x209, 64, new Check(), new Reward(), "Sphinx - Orange Nerve by Trampoline", new string[] { "Jump", "Desert", "Attack", "Sphinx60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x209, buffer, buffer.Length, ref bytesRead);
			sphinx.nerve2.check.currValue = sphinx.nerve2.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.nerve2);
			sphinx.nerve3 = (0x209, 128, new Check(), new Reward(), "Sphinx - Red Nerve Locked Behind Tokens", new string[] { "Jump", "Desert", "Attack", "Sphinx100Ooze", "SphinxSilverMedal", "Trampoline" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x209, buffer, buffer.Length, ref bytesRead);
			sphinx.nerve3.check.currValue = sphinx.nerve3.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.nerve3);
			sphinx.nerve4 = (0x20A, 1, new Check(), new Reward(), "Sphinx - Red Nerve First Inside Sphinx", new string[] { "Jump", "Desert", "Attack", "Sphinx100Ooze", "Boost" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20A, buffer, buffer.Length, ref bytesRead);
			sphinx.nerve4.check.currValue = sphinx.nerve4.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.nerve4);
			sphinx.nerve5 = (0x20A, 2, new Check(), new Reward(), "Sphinx - Red Nerve Second Inside Sphinx", new string[] { "Jump", "Desert", "Attack", "Sphinx100Ooze", "Boost" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20A, buffer, buffer.Length, ref bytesRead);
			sphinx.nerve5.check.currValue = sphinx.nerve5.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.nerve5);
			sphinx.nerve6 = (0x20A, 4, new Check(), new Reward(), "Sphinx - Blue Nerve First Puzzle", new string[] { "Jump", "Desert", "Attack", "Sphinx10Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20A, buffer, buffer.Length, ref bytesRead);
			sphinx.nerve6.check.currValue = sphinx.nerve6.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.nerve6);
			sphinx.nerve7 = (0x20A, 8, new Check(), new Reward(), "Sphinx - Green Nerve Second Puzzle", new string[] { "Jump", "Desert", "Attack", "Sphinx25Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20A, buffer, buffer.Length, ref bytesRead);
			sphinx.nerve7.check.currValue = sphinx.nerve7.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.nerve7);
			sphinx.nerve8 = (0x20A, 16, new Check(), new Reward(), "Sphinx - Yellow Nerve Third Puzzle", new string[] { "Jump", "Desert", "Attack", "Sphinx40Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20A, buffer, buffer.Length, ref bytesRead);
			sphinx.nerve8.check.currValue = sphinx.nerve8.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.nerve8);
			sphinx.bronzeMedal = (0xF7, 1, new Check(), new Reward(), "Sphinx - Bronze Medal Collected", new string[] { "Jump", "Desert", "Attack", "Sphinx60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF7, buffer, buffer.Length, ref bytesRead);
			sphinx.bronzeMedal.check.currValue = sphinx.bronzeMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.bronzeMedal);
			sphinx.bronzeMedal2 = (0xF7, 1, new Check(), new Reward(), "Sphinx - Bronze Medal", new string[] { "Jump", "Desert", "Attack", "Sphinx60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF7, buffer, buffer.Length, ref bytesRead);
			sphinx.bronzeMedal2.check.currValue = sphinx.bronzeMedal2.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.bronzeMedal2);
			sphinx.silverMedal = (0xF7, 2, new Check(), new Reward(), "Sphinx - Silver Medal Collected", new string[] { "Jump", "Desert", "Boost" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF7, buffer, buffer.Length, ref bytesRead);
			sphinx.silverMedal.check.currValue = sphinx.silverMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.silverMedal);
			sphinx.goldMedal = (0xF7, 4, new Check(), new Reward(), "Sphinx - Gold Medal Collected", new string[] { "Jump", "Desert", "Attack", "Sphinx100Ooze", "SphinxSilverMedal" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF7, buffer, buffer.Length, ref bytesRead);
			sphinx.goldMedal.check.currValue = sphinx.goldMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(sphinx.goldMedal);

			Level oasis = new Level();
			oasis.token1 = (0x1FC, 2, new Check(), new Reward(), "Oasis - Speed Boost Token", new string[] { "Jump", "Boost", "Desert" }); //105
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FC, buffer, buffer.Length, ref bytesRead);
			oasis.token1.check.currValue = oasis.token1.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.token1);
			oasis.token2 = (0x1FC, 4, new Check(), new Reward(), "Oasis - Lower Cave Token", new string[] { "Jump", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FC, buffer, buffer.Length, ref bytesRead);
			oasis.token2.check.currValue = oasis.token2.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.token2);
			oasis.token3 = (0x1FC, 8, new Check(), new Reward(), "Oasis - By Snake Token", new string[] { "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FC, buffer, buffer.Length, ref bytesRead);
			oasis.token3.check.currValue = oasis.token3.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.token3);
			oasis.token4 = (0x1FC, 16, new Check(), new Reward(), "Oasis - Floating Trampoline Token", new string[] { "Jump", "Trampoline", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FC, buffer, buffer.Length, ref bytesRead);
			oasis.token4.check.currValue = oasis.token4.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.token4);
			oasis.token5 = (0x1FC, 32, new Check(), new Reward(), "Oasis - Behind Entrance Token", new string[] { "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FC, buffer, buffer.Length, ref bytesRead);
			oasis.token5.check.currValue = oasis.token5.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.token5);
			oasis.token6 = (0x1FC, 64, new Check(), new Reward(), "Oasis - Jumping Platforms Token", new string[] { "Jump", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FC, buffer, buffer.Length, ref bytesRead);
			oasis.token6.check.currValue = oasis.token6.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.token6);
			oasis.token7 = (0x1FC, 128, new Check(), new Reward(), "Oasis - Above Log Rivers Token", new string[] { "Jump", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FC, buffer, buffer.Length, ref bytesRead);
			oasis.token7.check.currValue = oasis.token7.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.token7);
			oasis.token8 = (0x1FD, 1, new Check(), new Reward(), "Oasis - Top of Waterfall Token", new string[] { "Jump", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FD, buffer, buffer.Length, ref bytesRead);
			oasis.token8.check.currValue = oasis.token8.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.token8);
			oasis.token9 = (0x1FD, 2, new Check(), new Reward(), "Oasis - Above Cave Entrance Token", new string[] { "Jump", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FD, buffer, buffer.Length, ref bytesRead);
			oasis.token9.check.currValue = oasis.token9.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.token9);
			oasis.token10 = (0x1FD, 4, new Check(), new Reward(), "Oasis - Top Cave Token", new string[] { "Jump", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FD, buffer, buffer.Length, ref bytesRead);
			oasis.token10.check.currValue = oasis.token10.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.token10);
			oasis.nerve1 = (0x20A, 32, new Check(), new Reward(), "Oasis - Blue Nerve First Lilypad Puzzle", new string[] { "Jump", "Desert", "Attack", "Oasis10Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20A, buffer, buffer.Length, ref bytesRead);
			oasis.nerve1.check.currValue = oasis.nerve1.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.nerve1);
			oasis.nerve2 = (0x20A, 64, new Check(), new Reward(), "Oasis - Green Nerve by Sandcastle", new string[] { "Jump", "Desert", "Attack", "Oasis25Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20A, buffer, buffer.Length, ref bytesRead);
			oasis.nerve2.check.currValue = oasis.nerve2.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.nerve2);
			oasis.nerve3 = (0x20A, 128, new Check(), new Reward(), "Oasis - Yellow Nerve Second Lilypad Puzzle", new string[] { "Jump", "Desert", "Attack", "Oasis40Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20A, buffer, buffer.Length, ref bytesRead);
			oasis.nerve3.check.currValue = oasis.nerve3.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.nerve3);
			oasis.nerve4 = (0x20B, 1, new Check(), new Reward(), "Oasis - Orange Nerve Top of Waterfall", new string[] { "Jump", "Desert", "Attack", "Oasis60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20B, buffer, buffer.Length, ref bytesRead);
			oasis.nerve4.check.currValue = oasis.nerve4.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.nerve4);
			oasis.nerve5 = (0x20B, 2, new Check(), new Reward(), "Oasis - Orange Nerve on the Trees", new string[] { "Jump", "Desert", "Attack", "Oasis60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20B, buffer, buffer.Length, ref bytesRead);
			oasis.nerve5.check.currValue = oasis.nerve5.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.nerve5);
			oasis.nerve6 = (0x20B, 4, new Check(), new Reward(), "Oasis - Red Nerve Third Lilypad Puzzle", new string[] { "Jump", "Desert", "Attack", "Oasis100Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20B, buffer, buffer.Length, ref bytesRead);
			oasis.nerve6.check.currValue = oasis.nerve6.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.nerve6);
			oasis.nerve7 = (0x20B, 8, new Check(), new Reward(), "Oasis - Red Nerve Locked Behind Tokens", new string[] { "Jump", "Desert", "Boost", "Trampoline", "OasisSilverMedal" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20B, buffer, buffer.Length, ref bytesRead);
			oasis.nerve7.check.currValue = oasis.nerve7.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.nerve7);
			oasis.nerve8 = (0x20B, 16, new Check(), new Reward(), "Oasis - Red Nerve in Cave Cage", new string[] { "Jump", "Desert", "Attack", "Oasis100Ooze", "Boost" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20B, buffer, buffer.Length, ref bytesRead);
			oasis.nerve8.check.currValue = oasis.nerve8.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.nerve8);
			oasis.bronzeMedal = (0xF8, 1, new Check(), new Reward(), "Oasis - Bronze Medal Collected", new string[] { "Jump", "Desert", "Attack", "Oasis60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF8, buffer, buffer.Length, ref bytesRead);
			oasis.bronzeMedal.check.currValue = oasis.bronzeMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.bronzeMedal);
			oasis.bronzeMedal2 = (0xF8, 1, new Check(), new Reward(), "Oasis - Bronze Medal", new string[] { "Jump", "Desert", "Attack", "Oasis60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF8, buffer, buffer.Length, ref bytesRead);
			oasis.bronzeMedal2.check.currValue = oasis.bronzeMedal2.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.bronzeMedal2);
			oasis.silverMedal = (0xF8, 2, new Check(), new Reward(), "Oasis - Silver Medal Collected", new string[] { "Jump", "Desert", "Boost", "Trampoline" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF8, buffer, buffer.Length, ref bytesRead);
			oasis.silverMedal.check.currValue = oasis.silverMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.silverMedal);
			oasis.goldMedal = (0xF8, 4, new Check(), new Reward(), "Oasis - Gold Medal Collected", new string[] { "Jump", "Desert", "Attack", "Oasis100Ooze", "OasisSilverMedal" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF8, buffer, buffer.Length, ref bytesRead);
			oasis.goldMedal.check.currValue = oasis.goldMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(oasis.goldMedal);

			Level tomb = new Level();
			tomb.token1 = (0x1FD, 8, new Check(), new Reward(), "Tomb - High in the Sky Token", new string[] { "Jump", "Boost", "Desert" }); //126
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FD, buffer, buffer.Length, ref bytesRead);
			tomb.token1.check.currValue = tomb.token1.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.token1);
			tomb.token2 = (0x1FD, 16, new Check(), new Reward(), "Tomb - Pillar Back Left of Entrance Token", new string[] { "Jump", "Trampoline", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FD, buffer, buffer.Length, ref bytesRead);
			tomb.token2.check.currValue = tomb.token2.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.token2);
			tomb.token3 = (0x1FD, 32, new Check(), new Reward(), "Tomb - By Purple Box Token", new string[] { "Jump", "Desert", "Attack" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FD, buffer, buffer.Length, ref bytesRead);
			tomb.token3.check.currValue = tomb.token3.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.token3);
			tomb.token4 = (0x1FD, 64, new Check(), new Reward(), "Tomb - Climbing Platforms Token", new string[] { "Jump", "Desert", "Attack" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FD, buffer, buffer.Length, ref bytesRead);
			tomb.token4.check.currValue = tomb.token4.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.token4);
			tomb.token5 = (0x1FD, 128, new Check(), new Reward(), "Tomb - Platform Before Top of Climb Token", new string[] { "Jump", "Desert", "Attack" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FD, buffer, buffer.Length, ref bytesRead);
			tomb.token5.check.currValue = tomb.token5.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.token5);
			tomb.token6 = (0x1FE, 1, new Check(), new Reward(), "Tomb - Top of Climb Token", new string[] { "Jump", "Desert", "Attack" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FE, buffer, buffer.Length, ref bytesRead);
			tomb.token6.check.currValue = tomb.token6.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.token6);
			tomb.token7 = (0x1FE, 2, new Check(), new Reward(), "Tomb - Right Side of Water Room Token", new string[] { "Jump", "Desert", "Attack" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FE, buffer, buffer.Length, ref bytesRead);
			tomb.token7.check.currValue = tomb.token7.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.token7);
			tomb.token8 = (0x1FE, 4, new Check(), new Reward(), "Tomb - Next to Red Nerve Token", new string[] { "Jump", "Desert", "Attack" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FE, buffer, buffer.Length, ref bytesRead);
			tomb.token8.check.currValue = tomb.token8.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.token8);
			tomb.token9 = (0x1FE, 8, new Check(), new Reward(), "Tomb - Left in Water Room Token", new string[] { "Jump", "Desert", "Attack" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FE, buffer, buffer.Length, ref bytesRead);
			tomb.token9.check.currValue = tomb.token9.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.token9);
			tomb.token10 = (0x1FE, 16, new Check(), new Reward(), "Tomb - By Green Box in Water Room Token", new string[] { "Jump", "Desert", "Attack" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FE, buffer, buffer.Length, ref bytesRead);
			tomb.token10.check.currValue = tomb.token10.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.token10);
			tomb.nerve1 = (0x20B, 32, new Check(), new Reward(), "Tomb - Red Nerve Above Pool at Entrance", new string[] { "Jump", "Desert", "Attack", "Tomb100Ooze", "Flingshot", "Boost" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20B, buffer, buffer.Length, ref bytesRead);
			tomb.nerve1.check.currValue = tomb.nerve1.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.nerve1);
			tomb.nerve2 = (0x20B, 64, new Check(), new Reward(), "Tomb - Green Nerve Top of Outside Tomb", new string[] { "Jump", "Desert", "Attack", "Tomb25Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20B, buffer, buffer.Length, ref bytesRead);
			tomb.nerve2.check.currValue = tomb.nerve2.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.nerve2);
			tomb.nerve3 = (0x20B, 128, new Check(), new Reward(), "Tomb - Blue Nerve Outside Tomb", new string[] { "Jump", "Desert", "Attack", "Tomb10Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20B, buffer, buffer.Length, ref bytesRead);
			tomb.nerve3.check.currValue = tomb.nerve3.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.nerve3);
			tomb.nerve4 = (0x20C, 1, new Check(), new Reward(), "Tomb - Orange Nerve Inside Tomb Locked by Purple Box", new string[] { "Jump", "Desert", "Attack", "Tomb60Ooze", "TombEntrance" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20C, buffer, buffer.Length, ref bytesRead);
			tomb.nerve4.check.currValue = tomb.nerve4.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.nerve4);
			tomb.nerve5 = (0x20C, 2, new Check(), new Reward(), "Tomb - Orange Nerve Inside Tomb Locked by Red Box", new string[] { "Jump", "Desert", "Attack", "Tomb60Ooze", "TombEntrance" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20C, buffer, buffer.Length, ref bytesRead);
			tomb.nerve5.check.currValue = tomb.nerve5.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.nerve5);
			tomb.nerve6 = (0x20C, 4, new Check(), new Reward(), "Tomb - Red Nerve Locked Behind Tokens", new string[] { "Jump", "Desert", "Attack", "Tomb100Ooze", "TombSilverMedal" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20C, buffer, buffer.Length, ref bytesRead);
			tomb.nerve6.check.currValue = tomb.nerve6.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.nerve6);
			tomb.nerve7 = (0x20C, 8, new Check(), new Reward(), "Tomb - Red Nerve in Water Room", new string[] { "Jump", "Desert", "Attack", "Tomb100Ooze", "TombEntrance" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20C, buffer, buffer.Length, ref bytesRead);
			tomb.nerve7.check.currValue = tomb.nerve7.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.nerve7);
			tomb.nerve8 = (0x20C, 16, new Check(), new Reward(), "Tomb - Orange Nerve in Water Room Blocked by Green Box", new string[] { "Jump", "Desert", "Attack", "Tomb60Ooze", "TombEntrance" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20C, buffer, buffer.Length, ref bytesRead);
			tomb.nerve8.check.currValue = tomb.nerve8.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.nerve8);
			tomb.bronzeMedal = (0xF9, 1, new Check(), new Reward(), "Tomb - Bronze Medal Collected", new string[] { "Jump", "Desert", "Attack", "Tomb60Ooze", "TombEntrance" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF9, buffer, buffer.Length, ref bytesRead);
			tomb.bronzeMedal.check.currValue = tomb.bronzeMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.bronzeMedal);
			tomb.bronzeMedal2 = (0xF9, 1, new Check(), new Reward(), "Tomb - Bronze Medal", new string[] { "Jump", "Desert", "Attack", "Tomb60Ooze", "TombEntrance" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF9, buffer, buffer.Length, ref bytesRead);
			tomb.bronzeMedal2.check.currValue = tomb.bronzeMedal2.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.bronzeMedal2);
			tomb.silverMedal = (0xF9, 2, new Check(), new Reward(), "Tomb - Silver Medal Collected", new string[] { "Jump", "Desert", "Boost", "TombEntrance", "Flingshot", "Trampoline" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF9, buffer, buffer.Length, ref bytesRead);
			tomb.silverMedal.check.currValue = tomb.silverMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.silverMedal);
			tomb.goldMedal = (0xF9, 4, new Check(), new Reward(), "Tomb - Gold Medal Collected", new string[] { "Jump", "Desert", "Attack", "Tomb100Ooze", "TombSilverMedal" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xF9, buffer, buffer.Length, ref bytesRead);
			tomb.goldMedal.check.currValue = tomb.goldMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(tomb.goldMedal);

			Level pyramid = new Level();
			pyramid.token1 = (0x1FE, 32, new Check(), new Reward(), "Pyramid - On Left Tree Token", new string[] { "Jump", "Desert" }); //147
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FE, buffer, buffer.Length, ref bytesRead);
			pyramid.token1.check.currValue = pyramid.token1.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.token1);
			pyramid.token2 = (0x1FE, 64, new Check(), new Reward(), "Pyramid - Speed Boost Above Entrance Token", new string[] { "Jump", "Boost", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FE, buffer, buffer.Length, ref bytesRead);
			pyramid.token2.check.currValue = pyramid.token2.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.token2);
			pyramid.token3 = (0x1FE, 128, new Check(), new Reward(), "Pyramid - Back Right of Entrance Token", new string[] { "Jump", "Trampoline", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FE, buffer, buffer.Length, ref bytesRead);
			pyramid.token3.check.currValue = pyramid.token3.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.token3);
			pyramid.token4 = (0x1FF, 1, new Check(), new Reward(), "Pyramid - Eye Level Bottom Right Token", new string[] { "Jump", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FF, buffer, buffer.Length, ref bytesRead);
			pyramid.token4.check.currValue = pyramid.token4.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.token4);
			pyramid.token5 = (0x1FF, 2, new Check(), new Reward(), "Pyramid - Eye Level Top Trampoline Token", new string[] { "Jump", "Trampoline", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FF, buffer, buffer.Length, ref bytesRead);
			pyramid.token5.check.currValue = pyramid.token5.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.token5);
			pyramid.token6 = (0x1FF, 4, new Check(), new Reward(), "Pyramid - Snake Level Token", new string[] { "Jump", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FF, buffer, buffer.Length, ref bytesRead);
			pyramid.token6.check.currValue = pyramid.token6.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.token6);
			pyramid.token7 = (0x1FF, 8, new Check(), new Reward(), "Pyramid - Scarab Level Above Entrance Token", new string[] { "Jump", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FF, buffer, buffer.Length, ref bytesRead);
			pyramid.token7.check.currValue = pyramid.token7.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.token7);
			pyramid.token8 = (0x1FF, 16, new Check(), new Reward(), "Pyramid - Hub Level on Right Token", new string[] { "Jump", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FF, buffer, buffer.Length, ref bytesRead);
			pyramid.token8.check.currValue = pyramid.token8.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.token8);
			pyramid.token9 = (0x1FF, 32, new Check(), new Reward(), "Pyramid - Water Level Left Side Token", new string[] { "Jump", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FF, buffer, buffer.Length, ref bytesRead);
			pyramid.token9.check.currValue = pyramid.token9.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.token9);
			pyramid.token10 = (0x1FF, 64, new Check(), new Reward(), "Pyramid - Water Level Right Side Token", new string[] { "Jump", "Desert" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FF, buffer, buffer.Length, ref bytesRead);
			pyramid.token10.check.currValue = pyramid.token10.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.token10);
			pyramid.nerve1 = (0x20C, 32, new Check(), new Reward(), "Pyramid - Yellow Nerve on Tree", new string[] { "Jump", "Desert", "Attack", "Pyramid40Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20C, buffer, buffer.Length, ref bytesRead);
			pyramid.nerve1.check.currValue = pyramid.nerve1.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.nerve1);
			pyramid.nerve2 = (0x20C, 64, new Check(), new Reward(), "Pyramid - Red Nerve Locked Behind Tokens", new string[] { "Jump", "Desert", "Attack", "Pyramid100Ooze", "PyramidSilverMedal" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20C, buffer, buffer.Length, ref bytesRead);
			pyramid.nerve2.check.currValue = pyramid.nerve2.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.nerve2);
			pyramid.nerve3 = (0x20C, 128, new Check(), new Reward(), "Pyramid - Green Nerve in Eye Level", new string[] { "Jump", "Desert", "Attack", "Pyramid25Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20C, buffer, buffer.Length, ref bytesRead);
			pyramid.nerve3.check.currValue = pyramid.nerve3.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.nerve3);
			pyramid.nerve4 = (0x20D, 1, new Check(), new Reward(), "Pyramid - Blue Nerve in Eye Level", new string[] { "Jump", "Desert", "Attack", "Pyramid10Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20D, buffer, buffer.Length, ref bytesRead);
			pyramid.nerve4.check.currValue = pyramid.nerve4.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.nerve4);
			pyramid.nerve5 = (0x20D, 2, new Check(), new Reward(), "Pyramid - Red Nerve in Snake Level", new string[] { "Jump", "Desert", "Attack", "Pyramid100Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20D, buffer, buffer.Length, ref bytesRead);
			pyramid.nerve5.check.currValue = pyramid.nerve5.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.nerve5);
			pyramid.nerve6 = (0x20D, 4, new Check(), new Reward(), "Pyramid - Blue Nerve in Snake Level", new string[] { "Jump", "Desert", "Attack", "Pyramid10Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20D, buffer, buffer.Length, ref bytesRead);
			pyramid.nerve6.check.currValue = pyramid.nerve6.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.nerve6);
			pyramid.nerve7 = (0x20D, 8, new Check(), new Reward(), "Pyramid - Orange Nerve in Bug Level", new string[] { "Jump", "Desert", "Attack", "Pyramid60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20D, buffer, buffer.Length, ref bytesRead);
			pyramid.nerve7.check.currValue = pyramid.nerve7.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.nerve7);
			pyramid.nerve8 = (0x20D, 16, new Check(), new Reward(), "Pyramid - Red Nerve in Water Level", new string[] { "Jump", "Desert", "Pyramid100Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20D, buffer, buffer.Length, ref bytesRead);
			pyramid.nerve8.check.currValue = pyramid.nerve8.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.nerve8);
			pyramid.bronzeMedal = (0xFA, 1, new Check(), new Reward(), "Pyramid - Bronze Medal Collected", new string[] { "Jump", "Desert", "Attack", "Pyramid60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xFA, buffer, buffer.Length, ref bytesRead);
			pyramid.bronzeMedal.check.currValue = pyramid.bronzeMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.bronzeMedal);
			pyramid.bronzeMedal2 = (0xFA, 1, new Check(), new Reward(), "Pyramid - Bronze Medal", new string[] { "Jump", "Desert", "Attack", "Pyramid60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xFA, buffer, buffer.Length, ref bytesRead);
			pyramid.bronzeMedal2.check.currValue = pyramid.bronzeMedal2.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.bronzeMedal2);
			pyramid.silverMedal = (0xFA, 2, new Check(), new Reward(), "Pyramid - Silver Medal Collected", new string[] { "Jump", "Desert", "Boost" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xFA, buffer, buffer.Length, ref bytesRead);
			pyramid.silverMedal.check.currValue = pyramid.silverMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.silverMedal);
			pyramid.goldMedal = (0xFA, 4, new Check(), new Reward(), "Pyramid - Gold Medal Collected", new string[] { "Jump", "Desert", "Attack", "Pyramid100Ooze", "PyramidSilverMedal" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xFA, buffer, buffer.Length, ref bytesRead);
			pyramid.goldMedal.check.currValue = pyramid.goldMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(pyramid.goldMedal);

			Level shack = new Level();
			shack.token1 = (0x1FF, 128, new Check(), new Reward(), "Shack - Platforming Section Token", new string[] { "Jump", "Arctic" }); //168
			ReadProcessMemory((int)processHandle, characterOffset + 0x1FF, buffer, buffer.Length, ref bytesRead);
			shack.token1.check.currValue = shack.token1.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.token1);
			shack.token2 = (0x200, 1, new Check(), new Reward(), "Shack - Inside Shack Token", new string[] { "Jump", "Arctic", "Attack", "Shack3Nerves" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x200, buffer, buffer.Length, ref bytesRead);
			shack.token2.check.currValue = shack.token2.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.token2);
			shack.token3 = (0x200, 2, new Check(), new Reward(), "Shack - By Orange Nerve Token", new string[] { "Jump", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x200, buffer, buffer.Length, ref bytesRead);
			shack.token3.check.currValue = shack.token3.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.token3);
			shack.token4 = (0x200, 4, new Check(), new Reward(), "Shack - On Log Pile Token", new string[] { "Jump", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x200, buffer, buffer.Length, ref bytesRead);
			shack.token4.check.currValue = shack.token4.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.token4);
			shack.token5 = (0x200, 8, new Check(), new Reward(), "Shack - Second Floating Token", new string[] { "Jump", "Flingshot", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x200, buffer, buffer.Length, ref bytesRead);
			shack.token5.check.currValue = shack.token5.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.token5);
			shack.token6 = (0x200, 16, new Check(), new Reward(), "Shack - First Floating Token", new string[] { "Jump", "Flingshot", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x200, buffer, buffer.Length, ref bytesRead);
			shack.token6.check.currValue = shack.token6.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.token6);
			shack.token7 = (0x200, 32, new Check(), new Reward(), "Shack - Platform By Red Nerve Token", new string[] { "Jump", "Flingshot", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x200, buffer, buffer.Length, ref bytesRead);
			shack.token7.check.currValue = shack.token7.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.token7);
			shack.token8 = (0x200, 64, new Check(), new Reward(), "Shack - Floating Platform Token", new string[] { "Jump", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x200, buffer, buffer.Length, ref bytesRead);
			shack.token8.check.currValue = shack.token8.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.token8);
			shack.token9 = (0x200, 128, new Check(), new Reward(), "Shack - Fourth Floating Token", new string[] { "Jump", "Flingshot", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x200, buffer, buffer.Length, ref bytesRead);
			shack.token9.check.currValue = shack.token9.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.token9);
			shack.token10 = (0x201, 1, new Check(), new Reward(), "Shack - Third Floating Token", new string[] { "Jump", "Flingshot", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x201, buffer, buffer.Length, ref bytesRead);
			shack.token10.check.currValue = shack.token10.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.token10);
			shack.nerve1 = (0x20D, 32, new Check(), new Reward(), "Shack - Red Nerve Blocked By Tokens", new string[] { "Jump", "Arctic", "Attack", "Shack100Ooze", "ShackSilverMedal" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20D, buffer, buffer.Length, ref bytesRead);
			shack.nerve1.check.currValue = shack.nerve1.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.nerve1);
			shack.nerve2 = (0x20D, 64, new Check(), new Reward(), "Shack - Orange Nerve Along Skinny Platform", new string[] { "Jump", "Arctic", "Attack", "Shack60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20D, buffer, buffer.Length, ref bytesRead);
			shack.nerve2.check.currValue = shack.nerve2.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.nerve2);
			shack.nerve3 = (0x20D, 128, new Check(), new Reward(), "Shack - Green Nerve Throwing Rocks", new string[] { "Jump", "Arctic", "Attack", "Shack25Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20D, buffer, buffer.Length, ref bytesRead);
			shack.nerve3.check.currValue = shack.nerve3.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.nerve3);
			shack.nerve4 = (0x20E, 1, new Check(), new Reward(), "Shack - Yellow Nerve Throwing Rocks", new string[] { "Jump", "Arctic", "Attack", "Shack40Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20E, buffer, buffer.Length, ref bytesRead);
			shack.nerve4.check.currValue = shack.nerve4.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.nerve4);
			shack.nerve5 = (0x20E, 2, new Check(), new Reward(), "Shack - Yellow Nerve Platforming Above River", new string[] { "Jump", "Arctic", "Attack", "Shack40Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20E, buffer, buffer.Length, ref bytesRead);
			shack.nerve5.check.currValue = shack.nerve5.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.nerve5);
			shack.nerve6 = (0x20E, 4, new Check(), new Reward(), "Shack - Orange Nerve on Shack Roof", new string[] { "Jump", "Arctic", "Attack", "Shack60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20E, buffer, buffer.Length, ref bytesRead);
			shack.nerve6.check.currValue = shack.nerve6.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.nerve6);
			shack.nerve7 = (0x20E, 8, new Check(), new Reward(), "Shack - Red Nerve Accessed by Flingshot", new string[] { "Jump", "Arctic", "Attack", "Shack100Ooze", "Flingshot" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20E, buffer, buffer.Length, ref bytesRead);
			shack.nerve7.check.currValue = shack.nerve7.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.nerve7);
			shack.nerve8 = (0x20E, 16, new Check(), new Reward(), "Shack - Red Nerve in Cabin", new string[] { "Jump", "Arctic", "Attack", "Shack100Ooze", "Shack3Nerves" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20E, buffer, buffer.Length, ref bytesRead);
			shack.nerve8.check.currValue = shack.nerve8.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.nerve8);
			shack.bronzeMedal = (0xFB, 1, new Check(), new Reward(), "Shack - Bronze Medal Collected", new string[] { "Jump", "Arctic", "Attack", "Shack60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xFB, buffer, buffer.Length, ref bytesRead);
			shack.bronzeMedal.check.currValue = shack.bronzeMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.bronzeMedal);
			shack.bronzeMedal2 = (0xFB, 1, new Check(), new Reward(), "Shack - Bronze Medal", new string[] { "Jump", "Arctic", "Attack", "Shack60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xFB, buffer, buffer.Length, ref bytesRead);
			shack.bronzeMedal2.check.currValue = shack.bronzeMedal2.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.bronzeMedal2);
			shack.silverMedal = (0xFB, 2, new Check(), new Reward(), "Shack - Silver Medal Collected", new string[] { "Jump", "Arctic", "Flingshot", "Shack3Nerves" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xFB, buffer, buffer.Length, ref bytesRead);
			shack.silverMedal.check.currValue = shack.silverMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.silverMedal);
			shack.goldMedal = (0xFB, 4, new Check(), new Reward(), "Shack - Gold Medal Collected", new string[] { "Jump", "Arctic", "Attack", "Shack100Ooze", "ShackSilverMedal" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xFB, buffer, buffer.Length, ref bytesRead);
			shack.goldMedal.check.currValue = shack.goldMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(shack.goldMedal);

			Level skilift = new Level();
			skilift.token1 = (0x201, 2, new Check(), new Reward(), "Ski Lift - Behind Start Token", new string[] { "Jump", "Arctic" }); //189
			ReadProcessMemory((int)processHandle, characterOffset + 0x201, buffer, buffer.Length, ref bytesRead);
			skilift.token1.check.currValue = skilift.token1.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.token1);
			skilift.token2 = (0x201, 4, new Check(), new Reward(), "Ski Lift - Fish Zone Token", new string[] { "Jump", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x201, buffer, buffer.Length, ref bytesRead);
			skilift.token2.check.currValue = skilift.token2.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.token2);
			skilift.token3 = (0x201, 8, new Check(), new Reward(), "Ski Lift - Bottom of Lift Zone Token", new string[] { "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x201, buffer, buffer.Length, ref bytesRead);
			skilift.token3.check.currValue = skilift.token3.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.token3);
			skilift.token4 = (0x201, 16, new Check(), new Reward(), "Ski Lift - Top Lift Area by Red Nerve Token", new string[] { "Jump", "Arctic", "Trampoline", "Flingshot" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x201, buffer, buffer.Length, ref bytesRead);
			skilift.token4.check.currValue = skilift.token4.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.token4);
			skilift.token5 = (0x201, 32, new Check(), new Reward(), "Ski Lift - By Trampoline in Lift Area", new string[] { "Jump", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x201, buffer, buffer.Length, ref bytesRead);
			skilift.token5.check.currValue = skilift.token5.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.token5);
			skilift.token6 = (0x201, 64, new Check(), new Reward(), "Ski Lift - By Green Nerve Token", new string[] { "Jump", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x201, buffer, buffer.Length, ref bytesRead);
			skilift.token6.check.currValue = skilift.token6.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.token6);
			skilift.token7 = (0x201, 128, new Check(), new Reward(), "Ski Lift - First Token Right Slope Token", new string[] { "Jump", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x201, buffer, buffer.Length, ref bytesRead);
			skilift.token7.check.currValue = skilift.token7.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.token7);
			skilift.token8 = (0x202, 1, new Check(), new Reward(), "Ski Lift - Second Token Left Slope Token", new string[] { "Jump", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x202, buffer, buffer.Length, ref bytesRead);
			skilift.token8.check.currValue = skilift.token8.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.token8);
			skilift.token9 = (0x202, 2, new Check(), new Reward(), "Ski Lift - First Token Left Slope Token", new string[] { "Jump", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x202, buffer, buffer.Length, ref bytesRead);
			skilift.token9.check.currValue = skilift.token9.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.token9);
			skilift.token10 = (0x202, 4, new Check(), new Reward(), "Ski Lift - Second Token Right Slope Token", new string[] { "Jump", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x202, buffer, buffer.Length, ref bytesRead);
			skilift.token10.check.currValue = skilift.token10.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.token10);
			skilift.nerve1 = (0x20E, 32, new Check(), new Reward(), "Ski Lift - Red Nerve Blocked by Tokens", new string[] { "Jump", "Arctic", "Attack", "SkiLift100Ooze", "SkiLiftSilverMedal" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20E, buffer, buffer.Length, ref bytesRead);
			skilift.nerve1.check.currValue = skilift.nerve1.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.nerve1);
			skilift.nerve2 = (0x20E, 64, new Check(), new Reward(), "Ski Lift - Yellow Nerve by Snow Castle", new string[] { "Jump", "Arctic", "Attack", "SkiLift40Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20E, buffer, buffer.Length, ref bytesRead);
			skilift.nerve2.check.currValue = skilift.nerve2.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.nerve2);
			skilift.nerve3 = (0x20E, 128, new Check(), new Reward(), "Ski Lift - Blue Nerve on Ice", new string[] { "Jump", "Arctic", "Attack", "SkiLift10Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20E, buffer, buffer.Length, ref bytesRead);
			skilift.nerve3.check.currValue = skilift.nerve3.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.nerve3);
			skilift.nerve4 = (0x20F, 1, new Check(), new Reward(), "Ski Lift - Orange Nerve Lift Area", new string[] { "Jump", "Arctic", "Attack", "SkiLift60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20F, buffer, buffer.Length, ref bytesRead);
			skilift.nerve4.check.currValue = skilift.nerve4.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.nerve4);
			skilift.nerve5 = (0x20F, 2, new Check(), new Reward(), "Ski Lift - Blue Nerve Lift Area", new string[] { "Jump", "Arctic", "Attack", "SkiLift10Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20F, buffer, buffer.Length, ref bytesRead);
			skilift.nerve5.check.currValue = skilift.nerve5.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.nerve5);
			skilift.nerve6 = (0x20F, 4, new Check(), new Reward(), "Ski Lift - Green Nerve Lift Area", new string[] { "Jump", "Arctic", "Attack", "SkiLift25Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20F, buffer, buffer.Length, ref bytesRead);
			skilift.nerve6.check.currValue = skilift.nerve6.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.nerve6);
			skilift.nerve7 = (0x20F, 8, new Check(), new Reward(), "Ski Lift - Red Nerve Up High Lift Area", new string[] { "Jump", "Arctic", "Attack", "SkiLift100Ooze", "Trampoline", "Flingshot" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20F, buffer, buffer.Length, ref bytesRead);
			skilift.nerve7.check.currValue = skilift.nerve7.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.nerve7);
			skilift.nerve8 = (0x20F, 16, new Check(), new Reward(), "Ski Lift - Red Nerve by Lift Area Shortcut", new string[] { "Jump", "Arctic", "Attack", "SkiLift100Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20F, buffer, buffer.Length, ref bytesRead);
			skilift.nerve8.check.currValue = skilift.nerve8.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.nerve8);
			skilift.bronzeMedal = (0xFC, 1, new Check(), new Reward(), "Ski Lift - Bronze Medal Collected", new string[] { "Jump", "Arctic", "Attack", "SkiLift60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xFC, buffer, buffer.Length, ref bytesRead);
			skilift.bronzeMedal.check.currValue = skilift.bronzeMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.bronzeMedal);
			skilift.bronzeMedal2 = (0xFC, 1, new Check(), new Reward(), "Ski Lift - Bronze Medal", new string[] { "Jump", "Arctic", "Attack", "SkiLift60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xFC, buffer, buffer.Length, ref bytesRead);
			skilift.bronzeMedal2.check.currValue = skilift.bronzeMedal2.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.bronzeMedal2);
			skilift.silverMedal = (0xFC, 2, new Check(), new Reward(), "Ski Lift - Silver Medal Collected", new string[] { "Jump", "Arctic", "Flingshot", "Trampoline" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xFC, buffer, buffer.Length, ref bytesRead);
			skilift.silverMedal.check.currValue = skilift.silverMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.silverMedal);
			skilift.goldMedal = (0xFC, 4, new Check(), new Reward(), "Ski Lift - Gold Medal Collected", new string[] { "Jump", "Arctic", "Attack", "SkiLift100Ooze", "SkiLiftSilverMedal" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xFC, buffer, buffer.Length, ref bytesRead);
			skilift.goldMedal.check.currValue = skilift.goldMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(skilift.goldMedal);

			Level iceberg = new Level();
			iceberg.token1 = (0x202, 8, new Check(), new Reward(), "Iceberg - At the Start Token", new string[] { "Arctic" }); //210
			ReadProcessMemory((int)processHandle, characterOffset + 0x202, buffer, buffer.Length, ref bytesRead);
			iceberg.token1.check.currValue = iceberg.token1.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.token1);
			iceberg.token2 = (0x202, 16, new Check(), new Reward(), "Iceberg - Second Token on Igloo Slide Token", new string[] { "Jump", "Boost", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x202, buffer, buffer.Length, ref bytesRead);
			iceberg.token2.check.currValue = iceberg.token2.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.token2);
			iceberg.token3 = (0x202, 32, new Check(), new Reward(), "Iceberg - Flingshot to fish Area Token", new string[] { "Jump", "Flingshot", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x202, buffer, buffer.Length, ref bytesRead);
			iceberg.token3.check.currValue = iceberg.token3.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.token3);
			iceberg.token4 = (0x202, 64, new Check(), new Reward(), "Iceberg - By Speed Boosts Token", new string[] { "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x202, buffer, buffer.Length, ref bytesRead);
			iceberg.token4.check.currValue = iceberg.token4.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.token4);
			iceberg.token5 = (0x202, 128, new Check(), new Reward(), "Iceberg - Thin Platform Token", new string[] { "Jump", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x202, buffer, buffer.Length, ref bytesRead);
			iceberg.token5.check.currValue = iceberg.token5.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.token5);
			iceberg.token6 = (0x203, 1, new Check(), new Reward(), "Iceberg - Jumping Fish Token", new string[] { "Jump", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x203, buffer, buffer.Length, ref bytesRead);
			iceberg.token6.check.currValue = iceberg.token6.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.token6);
			iceberg.token7 = (0x203, 2, new Check(), new Reward(), "Iceberg - Inside Along Path Token", new string[] { "Jump", "Boost", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x203, buffer, buffer.Length, ref bytesRead);
			iceberg.token7.check.currValue = iceberg.token7.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.token7);
			iceberg.token8 = (0x203, 4, new Check(), new Reward(), "Iceberg - Right Inside Igloo Token", new string[] { "Jump", "Boost", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x203, buffer, buffer.Length, ref bytesRead);
			iceberg.token8.check.currValue = iceberg.token8.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.token8);
			iceberg.token9 = (0x203, 8, new Check(), new Reward(), "Iceberg - Igloo by Exit Token", new string[] { "Jump", "Boost", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x203, buffer, buffer.Length, ref bytesRead);
			iceberg.token9.check.currValue = iceberg.token9.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.token9);
			iceberg.token10 = (0x203, 16, new Check(), new Reward(), "Iceberg - First on Igloo Slide Token", new string[] { "Jump", "Boost", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x203, buffer, buffer.Length, ref bytesRead);
			iceberg.token10.check.currValue = iceberg.token10.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.token10);
			iceberg.nerve1 = (0x20F, 32, new Check(), new Reward(), "Iceberg - Yellow Nerve Top of Mini Iceberg Token", new string[] { "Jump", "Arctic", "Attack", "Iceberg40Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20F, buffer, buffer.Length, ref bytesRead);
			iceberg.nerve1.check.currValue = iceberg.nerve1.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.nerve1);
			iceberg.nerve2 = (0x20F, 64, new Check(), new Reward(), "Iceberg - Yellow Nerve Ice Skating Token", new string[] { "Jump", "Arctic", "Attack", "Iceberg40Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20F, buffer, buffer.Length, ref bytesRead);
			iceberg.nerve2.check.currValue = iceberg.nerve2.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.nerve2);
			iceberg.nerve3 = (0x20F, 128, new Check(), new Reward(), "Iceberg - Orange Nerve on Platforming Fish Section Token", new string[] { "Jump", "Arctic", "Attack", "Iceberg60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x20F, buffer, buffer.Length, ref bytesRead);
			iceberg.nerve3.check.currValue = iceberg.nerve3.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.nerve3);
			iceberg.nerve4 = (0x210, 1, new Check(), new Reward(), "Iceberg - Orange Nerve End of Skinny Path Token", new string[] { "Jump", "Arctic", "Attack", "Iceberg60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x210, buffer, buffer.Length, ref bytesRead);
			iceberg.nerve4.check.currValue = iceberg.nerve4.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.nerve4);
			iceberg.nerve5 = (0x210, 2, new Check(), new Reward(), "Iceberg - Red Nerve Blocked by Tokens", new string[] { "Jump", "Arctic", "Attack", "Iceberg100Ooze", "Trampoline", "Flingshot", "IcebergSilverMedal" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x210, buffer, buffer.Length, ref bytesRead);
			iceberg.nerve5.check.currValue = iceberg.nerve5.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.nerve5);
			iceberg.nerve6 = (0x210, 4, new Check(), new Reward(), "Iceberg - Green Nerve by Level Start", new string[] { "Jump", "Arctic", "Attack", "Iceberg25Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x210, buffer, buffer.Length, ref bytesRead);
			iceberg.nerve6.check.currValue = iceberg.nerve6.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.nerve6);
			iceberg.nerve7 = (0x210, 8, new Check(), new Reward(), "Iceberg - Red Nerve in Igloo on Ledge", new string[] { "Jump", "Arctic", "Attack", "Iceberg100Ooze", "Boost" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x210, buffer, buffer.Length, ref bytesRead);
			iceberg.nerve7.check.currValue = iceberg.nerve7.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.nerve7);
			iceberg.nerve8 = (0x210, 16, new Check(), new Reward(), "Iceberg - Red Nerve in Igloo that Runs Away", new string[] { "Jump", "Arctic", "Attack", "Boost", "Iceberg100Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x210, buffer, buffer.Length, ref bytesRead);
			iceberg.nerve8.check.currValue = iceberg.nerve8.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.nerve8);
			iceberg.bronzeMedal = (0xFD, 1, new Check(), new Reward(), "Iceberg - Bronze Medal Collected", new string[] { "Jump", "Arctic", "Attack", "Iceberg60Ooze", "Boost" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xFD, buffer, buffer.Length, ref bytesRead);
			iceberg.bronzeMedal.check.currValue = iceberg.bronzeMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.bronzeMedal);
			iceberg.bronzeMedal2 = (0xFD, 1, new Check(), new Reward(), "Iceberg - Bronze Medal", new string[] { "Jump", "Arctic", "Attack", "Iceberg60Ooze", "Boost" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xFD, buffer, buffer.Length, ref bytesRead);
			iceberg.bronzeMedal2.check.currValue = iceberg.bronzeMedal2.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.bronzeMedal2);
			iceberg.silverMedal = (0xFD, 2, new Check(), new Reward(), "Iceberg - Silver Medal Collected", new string[] { "Jump", "Arctic", "Boost" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xFD, buffer, buffer.Length, ref bytesRead);
			iceberg.silverMedal.check.currValue = iceberg.silverMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.silverMedal);
			iceberg.goldMedal = (0xFD, 4, new Check(), new Reward(), "Iceberg - Gold Medal Collected", new string[] { "Jump", "Arctic", "Attack", "Iceberg100Ooze", "IcebergSilverMedal", "Boost", "Flingshot" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xFD, buffer, buffer.Length, ref bytesRead);
			iceberg.goldMedal.check.currValue = iceberg.goldMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(iceberg.goldMedal);


			Level hotsprings = new Level();
			hotsprings.token1 = (0x203, 32, new Check(), new Reward(), "Hotsprings - Left Side Water Spout Token", new string[] { "Jump", "Arctic" }); //231
			ReadProcessMemory((int)processHandle, characterOffset + 0x203, buffer, buffer.Length, ref bytesRead);
			hotsprings.token1.check.currValue = hotsprings.token1.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.token1);
			hotsprings.token2 = (0x203, 64, new Check(), new Reward(), "Hotsprings - By Green Nerve Token", new string[] { "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x203, buffer, buffer.Length, ref bytesRead);
			hotsprings.token2.check.currValue = hotsprings.token2.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.token2);
			hotsprings.token3 = (0x203, 128, new Check(), new Reward(), "Hotsprings - Water Spout by Yellow Nerve Token", new string[] { "Jump", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x203, buffer, buffer.Length, ref bytesRead);
			hotsprings.token3.check.currValue = hotsprings.token3.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.token3);
			hotsprings.token4 = (0x204, 1, new Check(), new Reward(), "Hotsprings - Above Chasm Jump Token", new string[] { "Jump", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x204, buffer, buffer.Length, ref bytesRead);
			hotsprings.token4.check.currValue = hotsprings.token4.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.token4);
			hotsprings.token5 = (0x204, 2, new Check(), new Reward(), "Hotsprings - Open Air by Orange Nerve Token", new string[] { "Jump", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x204, buffer, buffer.Length, ref bytesRead);
			hotsprings.token5.check.currValue = hotsprings.token5.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.token5);
			hotsprings.token6 = (0x204, 4, new Check(), new Reward(), "Hotsprings - Mini Pools Chasm Token", new string[] { "Jump", "Arctic", "Trampoline" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x204, buffer, buffer.Length, ref bytesRead);
			hotsprings.token6.check.currValue = hotsprings.token6.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.token6);
			hotsprings.token7 = (0x204, 8, new Check(), new Reward(), "Hotsprings - Flingshot Canyon Token", new string[] { "Jump", "Flingshot", "Arctic", "Trampoline" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x204, buffer, buffer.Length, ref bytesRead);
			hotsprings.token7.check.currValue = hotsprings.token7.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.token7);
			hotsprings.token8 = (0x204, 16, new Check(), new Reward(), "Hotsprings - Near Speed Boost Hill Token", new string[] { "Jump", "Flingshot", "Arctic", "Trampoline" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x204, buffer, buffer.Length, ref bytesRead);
			hotsprings.token8.check.currValue = hotsprings.token8.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.token8);
			hotsprings.token9 = (0x204, 32, new Check(), new Reward(), "Hotsprings - By Highest Red Nerve Token", new string[] { "Jump", "Trampoline", "Flingshot", "Boost", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x204, buffer, buffer.Length, ref bytesRead);
			hotsprings.token9.check.currValue = hotsprings.token9.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.token9);
			hotsprings.token10 = (0x204, 64, new Check(), new Reward(), "Hotsprings - Trampoline by Red Nerve Token", new string[] { "Jump", "Trampoline", "Arctic" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x204, buffer, buffer.Length, ref bytesRead);
			hotsprings.token10.check.currValue = hotsprings.token10.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.token10);
			hotsprings.nerve1 = (0x210, 32, new Check(), new Reward(), "Hotsprings - Green Nerve Right Route", new string[] { "Jump", "Arctic", "Attack", "HotSprings25Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x210, buffer, buffer.Length, ref bytesRead);
			hotsprings.nerve1.check.currValue = hotsprings.nerve1.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.nerve1);
			hotsprings.nerve2 = (0x210, 64, new Check(), new Reward(), "Hotsprings - Blue Nerve Left Route", new string[] { "Jump", "Arctic", "Attack", "HotSprings10Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x210, buffer, buffer.Length, ref bytesRead);
			hotsprings.nerve2.check.currValue = hotsprings.nerve2.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.nerve2);
			hotsprings.nerve3 = (0x210, 128, new Check(), new Reward(), "Hotsprings - Yellow Nerve Above Green Nerve", new string[] { "Jump", "Arctic", "Attack", "HotSprings40Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x210, buffer, buffer.Length, ref bytesRead);
			hotsprings.nerve3.check.currValue = hotsprings.nerve3.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.nerve3);
			hotsprings.nerve4 = (0x211, 1, new Check(), new Reward(), "Hotsprings - Yellow Nerve by Trampoline to Next Tier", new string[] { "Jump", "Arctic", "Attack", "HotSprings40Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x211, buffer, buffer.Length, ref bytesRead);
			hotsprings.nerve4.check.currValue = hotsprings.nerve4.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.nerve4);
			hotsprings.nerve5 = (0x211, 2, new Check(), new Reward(), "Hotsprings - Orange Nerve Building Snowmen", new string[] { "Jump", "Arctic", "Attack", "HotSprings60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x211, buffer, buffer.Length, ref bytesRead);
			hotsprings.nerve5.check.currValue = hotsprings.nerve5.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.nerve5);
			hotsprings.nerve6 = (0x211, 4, new Check(), new Reward(), "Hotsprings - Red Nerve Blocked by Tokens", new string[] { "Jump", "Arctic", "Attack", "Trampoline", "HotSprings100Ooze", "HotSpringsSilverMedal" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x211, buffer, buffer.Length, ref bytesRead);
			hotsprings.nerve6.check.currValue = hotsprings.nerve6.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.nerve6);
			hotsprings.nerve7 = (0x211, 8, new Check(), new Reward(), "Hotsprings - Red Nerve by Water Spout 3rd Tier", new string[] { "Jump", "Arctic", "Attack", "HotSprings100Ooze", "Trampoline" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x211, buffer, buffer.Length, ref bytesRead);
			hotsprings.nerve7.check.currValue = hotsprings.nerve7.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.nerve7);
			hotsprings.nerve8 = (0x211, 16, new Check(), new Reward(), "Hotsprings - Red Nerve at the Highest Point", new string[] { "Jump", "Arctic", "Attack", "HotSprings100Ooze", "Trampoline", "Flingshot", "Boost" });
			ReadProcessMemory((int)processHandle, characterOffset + 0x211, buffer, buffer.Length, ref bytesRead);
			hotsprings.nerve8.check.currValue = hotsprings.nerve8.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.nerve8);
			hotsprings.bronzeMedal = (0xFE, 1, new Check(), new Reward(), "Hotsprings - Bronze Medal Collected", new string[] { "Jump", "Arctic", "Attack", "HotSprings60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xFE, buffer, buffer.Length, ref bytesRead);
			hotsprings.bronzeMedal.check.currValue = hotsprings.bronzeMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.bronzeMedal);
			hotsprings.bronzeMedal2 = (0xFE, 1, new Check(), new Reward(), "Hotsprings - Bronze Medal", new string[] { "Jump", "Arctic", "Attack", "HotSprings60Ooze" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xFE, buffer, buffer.Length, ref bytesRead);
			hotsprings.bronzeMedal2.check.currValue = hotsprings.bronzeMedal2.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.bronzeMedal2);
			hotsprings.silverMedal = (0xFE, 2, new Check(), new Reward(), "Hotsprings - Silver Medal Collected", new string[] { "Jump", "Arctic", "Boost", "Flingshot", "Trampoline" });
			ReadProcessMemory((int)processHandle, characterOffset + 0xFE, buffer, buffer.Length, ref bytesRead);
			hotsprings.silverMedal.check.currValue = hotsprings.silverMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.silverMedal);
			hotsprings.goldMedal = (0xFE, 4, new Check(), new Reward(), "Hotsprings - Gold Medal Collected", new string[] { "Jump", "Arctic", "Attack", "HotSprings100Ooze", "HotSpringsSilverMedal" }); //251
			ReadProcessMemory((int)processHandle, characterOffset + 0xFE, buffer, buffer.Length, ref bytesRead);
			hotsprings.goldMedal.check.currValue = hotsprings.goldMedal.check.oldValue = buffer[0];
			addressesToCheck.Add(hotsprings.goldMedal);

			rewards.Add(new Reward(0xEF, 1, "Desert Unlocked"));
			rewards.Add(new Reward(0xF0, 1, "Arctic Unlocked"));
			rewards.Add(new Reward(0x22F, 2, "Urban Unlocked"));

			//rewards.Add(new Reward(0x48, 128, "Sully Attack Unlocked"));
			//rewards.Add(new Reward(0x54, 64, "Sully Jump Unlocked"));

			rewards.Add(new Reward(0x20, 128, "Mike Attack"));
			rewards.Add(new Reward(0x26, 64, "Mike Jump"));
            rewards.Add(new Reward(0x20, 128, "Sully Attack"));
            rewards.Add(new Reward(0x26, 64, "Sully Jump"));

            rewards.Add(new Reward(0x1F4, -127, "Trampoline"));
			rewards.Add(new Reward(0x1F4, -64, "Boost"));
			rewards.Add(new Reward(0x1F5, -1, "Flingshot"));
			rewards.Add(new Reward(0xD4, 10, "ParkOoze"));
			rewards.Add(new Reward(0xD4, 10, "DowntownOoze"));
			rewards.Add(new Reward(0xD4, 10, "DocksOoze"));
			rewards.Add(new Reward(0xD4, 10, "MarketOoze"));
			rewards.Add(new Reward(0xD4, 10, "SphinxOoze"));
			rewards.Add(new Reward(0xD4, 10, "OasisOoze"));
			rewards.Add(new Reward(0xD4, 10, "TombOoze"));
			rewards.Add(new Reward(0xD4, 10, "PyramidOoze"));
			rewards.Add(new Reward(0xD4, 10, "ShackOoze"));
			rewards.Add(new Reward(0xD4, 10, "SkiLiftOoze"));
			rewards.Add(new Reward(0xD4, 10, "IcebergOoze"));
			rewards.Add(new Reward(0xD4, 10, "HotSpringsOoze"));
			rewards.Add(new Reward(0x22A, 8, "ParkTreeDoor"));
			rewards.Add(new Reward(0x22F, 1, "DocksCraneBox"));
			rewards.Add(new Reward(0x104, 14, "TombEntrance"));

			foreach ((int offset, int valueChange, Check check, Reward reward, string text, string[] needs) check in addressesToCheck)
			{
				ReadProcessMemory((int)processHandle, characterOffset + check.offset, buffer, buffer.Length, ref bytesRead);
				check.check.currValue = buffer[0];
			}
		}
	}
	public class Level
	{
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) nerve1;
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) nerve2;
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) nerve3;
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) nerve4;
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) nerve5;
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) nerve6;
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) nerve7;
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) nerve8;
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) token1;
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) token2;
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) token3;
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) token4;
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) token5;
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) token6;
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) token7;
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) token8;
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) token9;
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) token10;
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) bronzeMedal;
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) silverMedal;
		public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) goldMedal;
        public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) bronzeMedal2;
        public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) silverMedal2;
        public (int offset, int valueChange, Check check, Reward reward, string text, string[] needs) goldMedal2;
    }

	public class Check
	{
		public int currValue;
		public int oldValue;
		public bool completed;
	}

	public class Reward(int address = 0, int changeValue = 0, string text  ="Nothing")
	{
		public int address = address;
		public int changeValue = changeValue;
		public string text = text;
	}
}
