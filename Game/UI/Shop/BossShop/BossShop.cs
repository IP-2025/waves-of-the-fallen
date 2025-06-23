using System;
using Godot;
using System.Collections.Generic;
using System.Diagnostics;

public partial class BossShop : CanvasLayer
{
	[Signal] 
	public delegate void WeaponChosenEventHandler(Weapon weapon);

	private List<Weapon> _shuffled;
	private Weapon _selected = null;
	private Timer _timer;
	private RichTextLabel _timeLabel;


	private static readonly List<Weapon> allWeapons = new()
{
	new Bow(),
	new Crossbow(),
	new Kunai(),
	new FireStaff(),
	new Lightningstaff(),
	new Healstaff(),
	new Dagger(),
	new Sword(),
	new WarHammer(),
	new DoubleBlade(),
	new MedicineBag()
};
	
	

	private void PopulateWeapon(string containerPath, Weapon weapon, int index)
	{
		var box = GetNode<Node>(containerPath);

		box.GetNode<TextureRect>("texture").Texture = GD.Load<Texture2D>(weapon.IconPath);

		var stats = weapon.BaseStats();

		box.GetNode<RichTextLabel>("name").Text = weapon.GetType().Name;
		box.GetNode<RichTextLabel>("damage").Text = weapon is Healstaff || weapon is MedicineBag ? $"Heal:   {stats.dmg}" : $"Damage:   {stats.dmg}";
		box.GetNode<RichTextLabel>("range").Text = $"Range:    {(int)stats.range}";
		box.GetNode<RichTextLabel>("delay").Text = $"Attacks/s:    {Math.Round(1f/stats.delay, 2)}";
		box.GetNode<RichTextLabel>("piercing").Text = weapon is RangedWeapon ? $"Piercing: {stats.piercing}" : "";
		
		var btn = GetNode<Button>(containerPath);
		var w = weapon; // closure copy
		btn.Pressed += () => OnWeaponButtonUp(weapon);
		
	}

	public override void _Ready()
	{
		var rng = new RandomNumberGenerator();
		rng.Randomize();

		_shuffled = new List<Weapon>(allWeapons);
		for (int i = _shuffled.Count - 1; i > 0; i--)
		{
			int j = rng.RandiRange(0, i);
			(_shuffled[i], _shuffled[j]) = (_shuffled[j], _shuffled[i]);
		}

		for (int slot = 0; slot < 3 && slot < _shuffled.Count; slot++)
		{
			string path = $"MarginContainer/HBoxContainer/weapon{slot + 1}";
			PopulateWeapon(path, _shuffled[slot], slot + 1);
		}
		
		_timer = GetNode<Timer>("Timer");
		_timeLabel = GetNode<RichTextLabel>("TimeLabel");
		

		// Timer starten
		_timer.Start();
	}
	
	public override void _Process(double delta)
	{
		// Nur anzeigen, wenn der Timer läuft
		if (_timer != null && _timer.IsStopped() == false)
		{
			
			double time = _timer.TimeLeft;
			_timeLabel.Text = $"Time left: {time:F2}s";
		}
	}

	public void OnWeaponButtonUp(Weapon chosen)
	{
		Debug.Print($"Du hast gewählt: {chosen.GetType().Name}");
		_selected = chosen;
		
	}

	public void OnTimerTimeout()
	{
		
		if (_selected == null)
		{
			var rng = new RandomNumberGenerator();
			rng.Randomize();
		
			int displayCount = Math.Min(_shuffled.Count, 3);
			int idx = rng.RandiRange(0, displayCount - 1);
			_selected = _shuffled[idx];
		
			Debug.Print($"Zeit abgelaufen – zufällige Waffe: {_selected.GetType().Name}");
			
		}
		
		Debug.Print($"Waffe hinzugefügt: {_selected.GetType().Name}");
		EmitSignal(SignalName.WeaponChosen, _selected);
		
	}
}
