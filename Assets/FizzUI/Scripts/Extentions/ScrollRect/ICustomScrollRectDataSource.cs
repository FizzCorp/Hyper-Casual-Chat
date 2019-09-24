//
//  ICustomScrollRectDataSource.cs
//
//  Copyright (c) 2016 Fizz Inc
//
using UnityEngine;

namespace Fizz.UI.Components.Extentions
{
	public interface ICustomScrollRectDataSource
	{
		GameObject GetListItem (int index, int itemType, GameObject obj);

		int GetItemCount ();

		int GetItemType (int index);
	}
}