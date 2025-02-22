﻿using System;
using System.Drawing;
using System.Text;
using ReClassNET.Controls;
using ReClassNET.Extensions;
using ReClassNET.Nodes;
using ReClassNET.UI;
using XivReClassPlugin.Game;
using XivReClassPlugin.Resources;

namespace XivReClassPlugin.Nodes; 

public class Utf8StringNode : BaseTextPtrNode {
	public override int MemorySize => 0x68;
	public override Encoding Encoding => Encoding.UTF8;

	public override void GetUserInterfaceInfo(out string name, out Image icon) {
		name = "Utf8String";
		icon = XivReClassResources.Utf8StringIcon;
	}

	public override Size Draw(DrawContext context, int x, int y) {
		return CustomDraw(context, x, y, "UTF8");
		//return DrawText(context, x, y, "UTF8");
	}

	private Size CustomDraw(DrawContext context, int x, int y, string typeName) {
		if (IsHidden && !IsWrapped)
			return DrawHidden(context, x, y);

		var ptr = context.Memory.ReadIntPtr(Offset);
        var rawlen = (int)context.Memory.ReadInt64(Offset + 0x10);
        var strlen = rawlen > 2048 ? 2048 : rawlen;
        
        string text;
        if (Ffxiv.Settings.DecodeUtf8Strings) {
            try {
                var data = context.Process.ReadRemoteMemory(ptr, strlen) ?? [];
                text = Utf8Decoder.DecodeString(data);
            } catch (Exception ex) {
                text = $"Invalid Utf8String: {ex.Message}";
            }
        } else {
            text = context.Process.ReadRemoteString(ptr, Encoding, strlen) ?? string.Empty;
        }
        
		var origX = x;

		AddSelection(context, x, y, context.Font.Height);

		x = AddIconPadding(context, x);

		x = AddIcon(context, x, y, context.IconProvider.Text, HotSpot.NoneId, HotSpotType.None);
		x = AddAddressOffset(context, x, y);

		x = AddText(context, x, y, context.Settings.PluginColor, HotSpot.NoneId, typeName) + context.Font.Width;
		if (!IsWrapped)
			x = AddText(context, x, y, context.Settings.NameColor, HotSpot.NameId, Name) + context.Font.Width;

		x = AddText(context, x, y, context.Settings.TextColor, HotSpot.NoneId, "= '");
		x = AddText(context, x, y, context.Settings.TextColor, HotSpot.ReadOnlyId, text);
		x = AddText(context, x, y, context.Settings.TextColor, HotSpot.NoneId, "'") + context.Font.Width;

		x = AddComment(context, x, y);

		DrawInvalidMemoryIndicatorIcon(context, y);
		AddContextDropDownIcon(context, y);
		AddDeleteIcon(context, y);

		return new Size(x - origX, context.Font.Height);
	}
}