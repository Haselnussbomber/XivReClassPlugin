﻿using System.Windows.Forms;
using BrightIdeasSoftware;
using ReClassNET;
using ReClassNET.Forms;
using ReClassNET.Nodes;
using XivReClassPlugin.Game;

namespace XivReClassPlugin.Forms; 

public partial class AgentListForm : IconForm {
	public AgentListForm() {
		InitializeComponent();

		ListViewAgents.ModelFilter = new ModelFilter(AgentFilter);
		ListViewAgents.ContextMenuStrip = ContextMenuMain;

		InitList();
	}
	
	private bool AgentFilter(object obj) {
		if (obj is not AgentInterface agent)
			return true;
		if (!string.IsNullOrWhiteSpace(TextBoxSearch.Text)) {
			var text = TextBoxSearch.Text.ToUpperInvariant();
			if (agent.ClassName.ToUpperInvariant().Contains(text))
				return true;
			if (agent.Address.ToString("X").Contains(text))
				return true;
			return false;
		}
		if (CheckBoxHideInactive.Checked)
			return agent.AddonId != 0;
		return true;
	}

	private void InitList() {
		ListViewAgents.ClearObjects();
		if (AgentModule.AgentList.Count == 0)
			return;
		ListViewAgents.AddObjects(AgentModule.AgentList);
		ListViewAgents.AutoResizeColumns(ListViewAgents.GetItemCount() == 0 ? ColumnHeaderAutoResizeStyle.HeaderSize : ColumnHeaderAutoResizeStyle.ColumnContent);
	}

	private void UpdateList() {
		ListViewAgents.UpdateObjects(AgentModule.AgentList);
	}

	private void ListUpdateTimer_Tick(object sender, System.EventArgs e) {
		if (ListViewAgents.GetItemCount() == 0 || AgentModule.AgentList.Count == 0)
			InitList();
		AtkUnitManager.Update();
		UpdateList();
	}

	private void ButtonUpdateList_Click(object sender, System.EventArgs e) {
		Ffxiv.Update();
		InitList();
	}

	private void TextBoxSearch_TextChanged(object sender, System.EventArgs e) {
		UpdateList();
	}

	private void TextBoxSearch_KeyDown(object sender, KeyEventArgs e) {
		UpdateList();
	}

	private void CreateClassMenuItem_Click(object sender, System.EventArgs e) {
		ClassNode? node = null;
		foreach (var obj in ListViewAgents.SelectedObjects) {
			if (obj is AgentInterface agent)
				node = agent.CreateClassNode();
		}
		if (node != null)
			Program.MainForm.CurrentClassNode = node;
	}

	private void CopyOffsetMenuItem_Click(object sender, System.EventArgs e) {
		var selectedObjects = ListViewAgents.SelectedObjects;
		if (selectedObjects.Count < 1)
			return;
		if (selectedObjects[0] is AgentInterface agent)
			Clipboard.SetText($"{agent.VTableOffset:X}");
	}

	private void ShowAgentMenuItem_Click(object sender, System.EventArgs e) {
		var selectedObjects = ListViewAgents.SelectedObjects;
		if (selectedObjects.Count < 1)
			return;
		if (selectedObjects[0] is AgentInterface agent)
			agent.Show();
	}

	private void HideAgentMenuItem_Click(object sender, System.EventArgs e) {
		var selectedObjects = ListViewAgents.SelectedObjects;
		if (selectedObjects.Count < 1)
			return;
		if (selectedObjects[0] is AgentInterface agent)
			agent.Hide();
	}
}