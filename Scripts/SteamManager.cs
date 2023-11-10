using CoreLauncher.Scripts.StoredData;
using CoreLauncher.Scripts.StoredData.StoredDataGroups;
using Godot;

namespace CoreLauncher.Scripts;

public static class SteamManager {
	public static string Path;
	
	public static void Init() {
		StoredDataManager.DeserializeStoredDataEvent += OnDeserializeStoredData;
		StoredDataManager.SerializeStoredDataEvent += OnSerializeStoredData;
	}
	
	public static void RunGame() {
		OS.Execute($"{FileUtil.GetPath(PathType.Project)}/Commands/RunGame.bat", new [] {GetPath()}, new Godot.Collections.Array());
	}

	public static string GetPath() {
		string steamPath = StoredDataManager.GetStoredDataGroup<PersistentDataGroup>().SteamPath;
		return steamPath != "" ? steamPath : FileUtil.GetPath(PathType.Steam);
	}
	
	private static void OnDeserializeStoredData() {
		Path = StoredDataManager.GetStoredDataGroup<PersistentDataGroup>().SteamPath;
	}

	private static void OnSerializeStoredData() {
		StoredDataManager.GetStoredDataGroup<PersistentDataGroup>().SteamPath = Path;
	}
}
