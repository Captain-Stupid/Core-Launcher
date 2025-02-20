using CoreLauncher.Scripts.Menus.Main;
using CoreLauncher.Scripts.StoredData;
using CoreLauncher.Scripts.Systems;
using CoreLauncher.Scripts.UI.Generic;
using Godot;

namespace CoreLauncher.Scripts.UI;

public partial class OptionsWindow : Window
{
	[Export] private FileLineEdit _steamPathLineEdit;
	[Export] private FileLineEdit _gamePathLineEdit;
	[Export] private FileLineEdit _serverPathLineEdit;
	[Export] private FileLineEdit _appDataPathLineEdit;

	public override void _Ready() {
		_steamPathLineEdit.SetText(GameManager.SteamExePath);
		_gamePathLineEdit.SetText(GameManager.SteamGamePath);
		_serverPathLineEdit.SetText(GameManager.SteamGameServerPath);
		_appDataPathLineEdit.SetText(GameManager.AppDataPath);
	}

	public void OnCloseRequested() {
		Save();
		
		Visible = false;
	}

	public void Save() {
		GameManager.SteamExePath = _steamPathLineEdit.Text;
		GameManager.SteamGamePath = _gamePathLineEdit.Text;
		GameManager.SteamGameServerPath = _serverPathLineEdit.Text;
		GameManager.AppDataPath = _appDataPathLineEdit.Text;
		StoredDataManager.Serialize();
	}
}
