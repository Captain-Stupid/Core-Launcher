using System.Collections.Generic;
using System.IO.Compression;
using System.Threading.Tasks;
using CoreLauncher.Scripts.Menus.Main;
using CoreLauncher.Scripts.ModIO;
using CoreLauncher.Scripts.StoredData;
using CoreLauncher.Scripts.StoredData.StoredDataGroups;
using CoreLauncher.Scripts.UI;
using CoreLauncher.Scripts.UI.Generic;
using Godot;

namespace CoreLauncher.Scripts.Systems;

public static class GameManager {
	public static readonly string CoreKeeperRelativePath = "/steamapps/common/Core Keeper";
	public static readonly string CoreKeeperServerRelativePath = "/steamapps/common/Core Keeper Dedicated Server";
	public static readonly string ModsRelativePath = "/CoreKeeper_Data/StreamingAssets/Mods";
	public static readonly string ModsRelativeServerPath = "/CoreKeeperServer_Data/StreamingAssets/Mods";
	
	public static string SteamExePath;
	public static string SteamGamesPath;
	
	public static void Init() {
		StoredDataManager.DeserializeStoredDataEvent += OnDeserializeStoredData;
		StoredDataManager.SerializeStoredDataEvent += OnSerializeStoredData;
	}
	
	public static async void RunGame() {
		SelectableItemListEntry selectableEntry = InstanceManager.GetInstance<MainMenuManager>().ProfileList.GetSelectedEntry();

		if (selectableEntry == null) {
			InstanceManager.GetInstance<MainMenuManager>()?.PlayProgressBar.SetValue("Dependencies", 0.0, "No profile was selected...");
			return;
		}
		
		if (selectableEntry is ProfileListEntry profileEntry) {
			if (string.IsNullOrEmpty(profileEntry.GetName())) {
				TaskCompletionSource<string> popupTask = new TaskCompletionSource<string>();
				
				InstanceManager.GetInstance<MainMenuManager>().NameProfilePopup.Open(popupTask);
				
				string popupResult = await popupTask.Task;

				if (string.IsNullOrEmpty(popupResult)) {
					return;
				}
				
				profileEntry.SetName(popupResult);
			}
			
			await ModManager.ManageMods(profileEntry.Server, profileEntry.Mods);

			string osName = OS.GetName();

			if (osName == "Windows") {
				OS.Execute($"{FileUtil.GetPath(PathType.SteamExe)}/steam.exe", new string[] {"-applaunch", profileEntry.Server ? "1963720" : "1621690"}, new Godot.Collections.Array());
			}
			else if (osName == "Linux") {
				OS.Execute($"{FileUtil.GetPath(PathType.SteamExe)}/steam", new string[] {"-applaunch", profileEntry.Server ? "1963720" : "1621690"}, new Godot.Collections.Array());
			}
			else {
				GD.PrintErr($"Unrecognized operating system {osName}.");
			}
		}
		
		await Task.Delay(2000);
			
		InstanceManager.GetInstance<MainMenuManager>()?.PlayProgressBar.Reset();
	}

	public static string GetCoreKeeperPath() {
		return FileUtil.GetPath(PathType.SteamGames) + CoreKeeperRelativePath;
	}
	
	public static string GetCoreKeeperServerPath() {
		return FileUtil.GetPath(PathType.SteamGames) + CoreKeeperServerRelativePath;
	}
	
	public static string GetModsPath(bool server) {
		return server ? GetCoreKeeperServerPath() + ModsRelativeServerPath : GetCoreKeeperPath() + ModsRelativePath;
	}
	
	private static void OnDeserializeStoredData() {
		SteamExePath = StoredDataManager.GetStoredDataGroup<PersistentDataGroup>().SteamExePath;
		SteamGamesPath = StoredDataManager.GetStoredDataGroup<PersistentDataGroup>().SteamGamesPath;
	}

	private static void OnSerializeStoredData() {
		StoredDataManager.GetStoredDataGroup<PersistentDataGroup>().SteamExePath = SteamExePath;
		StoredDataManager.GetStoredDataGroup<PersistentDataGroup>().SteamGamesPath = SteamGamesPath;
	}
}
