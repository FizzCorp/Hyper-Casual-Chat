//
//  CustomScrollRectContent.cs
//
//  Copyright (c) 2016 Fizz Inc
//
using UnityEngine;
using UnityEngine.EventSystems;

namespace Fizz.UI.Components.Extentions
{
	public class CustomScrollRectContent : UIBehaviour
	{
		public float spacing = 0;

		public void Setup (CustomScrollRect scrollRect)
		{
			this.scrollRect = scrollRect;

			if (this.scrollRect == null) {
				return;
			}

			lastSize = GetSize ();
		}

		public void SetHandleChangeInDimension (bool handle)
		{
			handleChangeInDimension = handle;
		}

		protected override void Awake ()
		{
			base.Awake ();
			rectTransform = gameObject.GetComponent<RectTransform> ();
		}

		protected override void OnRectTransformDimensionsChange ()
		{
			base.OnRectTransformDimensionsChange ();

			if (!handleChangeInDimension)
				return;

			if (scrollRect == null) {
				return;
			}

			float size = GetSize ();

			if (size != lastSize) {
				lastSize = size;

				//Refresh
				scrollRect.MarkForRefresh = true;
			}
		}

		private float GetSize ()
		{
			if (rectTransform == null)
				rectTransform = gameObject.GetComponent<RectTransform> ();

			if (this.scrollRect.vertical)
				return rectTransform.rect.width;
			else if (this.scrollRect.horizontal)
				return rectTransform.rect.height;

			return 0f;
		}

		private CustomScrollRect scrollRect;
		private RectTransform rectTransform;
		private float lastSize;
		private bool handleChangeInDimension = true;
	}
}