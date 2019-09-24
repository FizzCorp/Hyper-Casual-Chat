using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fizz.UI.Components.Extentions {
    [ExecuteInEditMode]
    public class CustomScrollRect : ScrollRect, IPointerDownHandler, IPointerUpHandler {
        public PullDirection pullDirection = PullDirection.Up;
        public UnityEvent onPullToRefresh;

        public UnityEvent onPointerDown;
        public UnityEvent onPointerUp;

        private bool initialized;
        private ICustomScrollRectDataSource dataSourceInterface;
        private int itemCount;
        private bool isVertical;
        private GameObject pooledObjContainer;
        private List<ScrollItemInfo> visibleObjects;
        private List<ScrollItemInfo> reuseableObjects;
        private Dictionary<int, float> cachedSizes;
        private PointerEventData lastDragEventData;
        private int endDragTempCount;

        private float _dragPositionThreshold = 150.0f;
        private bool _isPullToRefreshValid = false;
        // private float _dragStartNormlizedValue = int.MaxValue;
        private Vector2 _dragStartPosition = Vector2.zero;

        private ScrollInfo scrollInfo;
        private CustomScrollRectContent scrollRectContent;
        private bool markForRefresh = false;

        #region Properties

        public bool MarkForRefresh { get { return markForRefresh; } set { markForRefresh = value; } }

        public int ItemCount { get { return itemCount; } }

        public bool HandleChangeInContentDimension { set { scrollRectContent.SetHandleChangeInDimension (value); } }

        private static double SystemTimeInMilliseconds { get { return (System.DateTime.UtcNow - new System.DateTime (1970, 1, 1)).TotalMilliseconds; } }

        private float SpaceAbove { get { return -ContentPosition; } }

        private float SpaceBelow { get { return ContentSize - SpaceAbove - ViewportSize; } }

        private float ContentPosition {
            get { return isVertical ? -content.anchoredPosition.y : content.anchoredPosition.x; }
            set { content.anchoredPosition = isVertical ? new Vector2 (content.anchoredPosition.x, -value) : new Vector2 (value, content.anchoredPosition.y); }
        }

        public float ContentSize {
            get { return isVertical ? content.rect.height : content.rect.width; }
            set { content.sizeDelta = isVertical ? new Vector2 (content.sizeDelta.x, value) : new Vector2 (value, content.sizeDelta.y); }
        }

        private float ViewportPosition { get { return -ContentPosition; } }

        public float ViewportSize {
            get {
                RectTransform rectT = (viewport == null) ? (RectTransform) transform : viewport;
                return isVertical ? rectT.rect.height : rectT.rect.width;
            }
        }

        #endregion

        #region Unity Methods

        protected override void Start () {
            base.Start ();

            if (Application.isPlaying) {
                Initialize (dataSourceInterface);
            }
        }

        protected virtual void Update () {
            if (!Application.isPlaying) {
                return;
            }

            if (!initialized) {
                return;
            }

            if (markForRefresh) {
                RefreshContent ();
                markForRefresh = false;
            }

            if (scrollInfo.scrollToIndex != -1) {
                float itemSize = 0;

                scrollInfo.scrollTo = GetScrollItemPosition (scrollInfo.scrollToIndex, out itemSize) + scrollInfo.scrollOffset;

                switch (scrollInfo.scrollAnchor) {
                    case ContentAnchor.Middle:
                        scrollInfo.scrollTo -= (isVertical ? ViewportSize / 2f - itemSize / 2f : -ViewportSize / 2f + itemSize / 2f);
                        break;
                    case ContentAnchor.End:
                        scrollInfo.scrollTo -= (isVertical ? ViewportSize - itemSize : -ViewportSize + itemSize);
                        break;
                }

                double timePassed = SystemTimeInMilliseconds - scrollInfo.scrollingStart;
                float delta = (float) (timePassed / scrollInfo.scrollDelta);
                float distanceLeft = Mathf.Abs (scrollInfo.scrollTo - ViewportPosition);

                if (distanceLeft <= 0.5) {
                    scrollInfo.ResetScroll ();
                } else {
                    ContentPosition = ContentPosition - Mathf.Lerp (0, distanceLeft, delta) * (scrollInfo.scrollTo < ViewportPosition ? -1 : 1);

                    UpdateScrollItems ();

                    if (SpaceBelow < 0 || SpaceAbove < 0) {
                        ContentPosition = ContentPosition - (SpaceBelow < 0 ? SpaceBelow : -SpaceAbove);

                        scrollInfo.ResetScroll ();

                        UpdateScrollItems ();
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public bool Initialize (ICustomScrollRectDataSource dataSource) {
            if (initialized) {
                return true;
            }

            if (dataSource == null) {
                return false;
            }

            dataSourceInterface = dataSource;

            if (dataSourceInterface == null) {
                return false;
            }

            if (!isActiveAndEnabled) {
                return false;
            }

            scrollInfo = new ScrollInfo ();
            visibleObjects = new List<ScrollItemInfo> ();
            reuseableObjects = new List<ScrollItemInfo> ();
            cachedSizes = new Dictionary<int, float> ();

            onValueChanged.AddListener (Scrolled);

            pooledObjContainer = new GameObject ("Pool");
            pooledObjContainer.SetActive (false);
            pooledObjContainer.transform.SetParent (transform, false);

            if (content != null) {
                scrollRectContent = content.gameObject.GetComponent<CustomScrollRectContent> ();
                if (scrollRectContent == null) {
                    scrollRectContent = content.gameObject.AddComponent<CustomScrollRectContent> ();
                }
                scrollRectContent.Setup (this);
            }

            initialized = true;

            RebuildContent ();

            return true;
        }

        public void RebuildContent () {
            if (!initialized) {
                return;
            } else if (dataSourceInterface == null) {
                return;
            } else {
                DestroyAllPoolObjects ();

                isVertical = vertical;

                if (isVertical) {
                    vertical = true;
                } else {
                    horizontal = true;
                }

                SetupRectTransform (content);

                RefreshContent (0, 0);

                scrollInfo.ResetScroll ();
            }
        }

        public void RefreshContent () {
            if (!initialized) {
                return;
            } else if (visibleObjects == null) {
                return;
            } else {
                float topPosValue = isVertical ? content.anchoredPosition.y : -content.anchoredPosition.x;
                int lowestIndex = (visibleObjects.Count == 0) ? 0 : int.MaxValue;
                float contentOffset = 0;

                for (int i = 0; i < visibleObjects.Count; i++) {
                    if (lowestIndex > visibleObjects[i].index) {
                        RectTransform objRectT = visibleObjects[i].obj.transform as RectTransform;
                        lowestIndex = visibleObjects[i].index;
                        contentOffset = topPosValue - (isVertical ? -objRectT.anchoredPosition.y : objRectT.anchoredPosition.x);
                    }
                }

                contentOffset *= (isVertical ? 1 : -1);

                RefreshContent (lowestIndex, contentOffset + (isVertical ? 1 : -1) * (lowestIndex != 0 ? scrollRectContent.spacing : 0));

                scrollInfo.ResetScroll ();
            }
        }

        public void GoToScrollItem (int itemIndex) {
            if (!initialized) {
                return;
            }

            if (itemIndex < 0) {
                return;
            } else if (itemIndex >= itemCount) {
                return;
            }

            float contentOffset = (isVertical ? 0 : -0) + (isVertical ? 1 : -1) * (itemIndex != 0 ? scrollRectContent.spacing : 0);

            RefreshContent (itemIndex, contentOffset);
        }

        public void GoToLastScrollItem () {
            if (!initialized) {
                return;
            }

            if (itemCount > 0) {
                GoToScrollItem (itemCount - 1);
            }
        }
        
        public void GoToFirstScrollItem () {
            if (!initialized) {
                return;
            }

            if (itemCount > 0) {
                GoToScrollItem (0);
            }
        }

        public void ScrollToScrollItem (int itemIndex, ContentAnchor anchor = ContentAnchor.Beginning, float offset = 0, float scrollDelta = 10000) {
            if (!initialized) {
                return;
            }

            if (itemIndex < 0) {
                return;
            } else if (itemIndex >= itemCount) {
                return;
            }

            scrollInfo.ResetScroll ();

            scrollInfo.scrollAnchor = anchor;
            scrollInfo.scrollOffset = offset;
            scrollInfo.scrollDelta = scrollDelta;
            scrollInfo.scrollToIndex = itemIndex;
            scrollInfo.scrollingStart = SystemTimeInMilliseconds;
            scrollInfo.scrollFrom = ViewportPosition;
        }

        public override void OnBeginDrag (PointerEventData eventData) {
            base.OnBeginDrag (eventData);

            _isPullToRefreshValid = false;
            // _dragStartNormlizedValue = verticalNormalizedPosition;
            _dragStartPosition = eventData.position;
        }

        public override void OnDrag (PointerEventData eventData) {
            base.OnDrag (eventData);
            lastDragEventData = eventData;

            if (pullDirection == PullDirection.Up) {
                // Debug.Log ("_dragStartPosition " + _dragStartPosition.y + " _dragPositionThreshold" +_dragPositionThreshold + ", eventData.position " + eventData.position.y);
                if (_dragStartPosition.y - _dragPositionThreshold > eventData.position.y) {
                    _isPullToRefreshValid = ( /*_dragStartNormlizedValue >= 1 && */ verticalNormalizedPosition >= 1);
                    // Debug.Log ("_dragStartNormlizedValue " + _dragStartNormlizedValue + " verticalNormalizedPosition" +verticalNormalizedPosition);
                }
            } else if (pullDirection == PullDirection.Down) {
                if (_dragStartPosition.y + _dragPositionThreshold < eventData.position.y)
                    _isPullToRefreshValid = ( /*_dragStartNormlizedValue <= 0 && */ verticalNormalizedPosition <= 0);
            }

            scrollInfo.ResetScroll ();
        }

        public override void OnEndDrag (PointerEventData eventData) {
            base.OnEndDrag (eventData);

            lastDragEventData = null;
            if (_isPullToRefreshValid && onPullToRefresh != null)
                onPullToRefresh.Invoke ();
        }

        #endregion

        #region Private Methods

        private void Scrolled (Vector2 scrollPos) {
            if (!Application.isPlaying || content == null || dataSourceInterface == null || scrollInfo.scrollToIndex != -1) {
                return;
            }

            if (visibleObjects.Count == 0 && itemCount > 0) {
                RebuildContent ();
                return;
            }

            UpdateScrollItems ();
        }

        private void RefreshContent (int itemIndex, float contentOffset) {
            if (content == null || dataSourceInterface == null) {
                return;
            }

            EndDrag ();

            ReturnAllToPool ();

            cachedSizes.Clear ();

            content.sizeDelta = new Vector2 (isVertical ? content.sizeDelta.x : 0, isVertical ? 0 : content.sizeDelta.y);

            SetAnchoredPosition (content, 0);

            scrollInfo.Reset ();

            itemCount = dataSourceInterface.GetItemCount ();

            if (itemCount == 0) {
                return;
            }

            if (itemIndex >= itemCount) {
                itemIndex = itemCount - 1;
                contentOffset = 0;
            }

            if (contentOffset != 0) {
                ShiftAnchoredPosition (content, contentOffset);
            }

            GenerateScrollItem (itemIndex, isVertical ? 0 : 0, false);

            if (itemIndex != 0) {
                float width = isVertical ? content.sizeDelta.x : content.sizeDelta.x + scrollRectContent.spacing;
                float height = isVertical ? content.sizeDelta.y + scrollRectContent.spacing : content.sizeDelta.y;

                content.sizeDelta = new Vector2 (width, height);
            }

            float val1;
            float diff;

            if (isVertical) {
                val1 = content.rect.height - content.anchoredPosition.y;
                diff = ViewportSize - val1;
            } else {
                val1 = content.rect.width + content.anchoredPosition.x;
                diff = ViewportSize - val1;
            }

            if (diff > 0) {
                ShiftAnchoredPosition (content, isVertical ? -diff : diff);

                UpdateScrollItems ();

                if ((isVertical && content.anchoredPosition.y < 0) ||
                    (!isVertical && content.anchoredPosition.x > 0)) {
                    ShiftAnchoredPosition (content, -content.anchoredPosition.y);
                }
            }

            scrollInfo.savedTotalSize = scrollInfo.currentTotalSize;
            scrollInfo.savedCount = scrollInfo.currentCount;
            scrollInfo.usedSavedSize = true;

            int lowestIndex = int.MaxValue;
            int highestIndex = int.MinValue;

            for (int i = 0; i < visibleObjects.Count; i++) {
                lowestIndex = System.Math.Min (lowestIndex, visibleObjects[i].index);
                highestIndex = System.Math.Max (highestIndex, visibleObjects[i].index);
            }

            while (cachedSizes.ContainsKey (lowestIndex - 1)) {
                lowestIndex--;
            }

            float extraSize1 = 0;
            float extraSize2 = 0;

            if (lowestIndex > 0) {
                extraSize1 += scrollInfo.SavedAvgSize * lowestIndex;
            }

            if (highestIndex < itemCount - 1) {
                extraSize2 += scrollInfo.SavedAvgSize * (itemCount - highestIndex - 1);
            }

            if (extraSize1 + extraSize2 > 0) {
                content.sizeDelta = (isVertical ? new Vector2 (content.sizeDelta.x, content.sizeDelta.y + extraSize1 + extraSize2) : new Vector2 (content.sizeDelta.x + extraSize1 + extraSize2, content.sizeDelta.y));

                if (extraSize1 > 0) {
                    ShiftContentBy (isVertical ? extraSize1 : -extraSize1);
                }
            }

            BeginDrag ();
        }

        private void GenerateScrollItem (int index, float position, bool topItem) {
            int itemType = dataSourceInterface.GetItemType (index);
            ScrollItemInfo scrollItemInfo = GetPooledObj (itemType);

            scrollItemInfo.index = index;
            scrollItemInfo.obj = dataSourceInterface.GetListItem (index, itemType, scrollItemInfo.obj);

            if (scrollItemInfo.obj == null) {
                return;
            }

            scrollItemInfo.obj.SetActive (true);
            scrollItemInfo.obj.transform.SetParent (content, false);

            SetupRectTransform (scrollItemInfo.obj.transform as RectTransform);

            visibleObjects.Add (scrollItemInfo);

            if (!CanvasUpdateRegistry.IsRebuildingLayout ()) {
                Canvas.ForceUpdateCanvases ();
            }

            PositionScrollItem (scrollItemInfo, position, topItem);

            UpdateScrollItems ();
        }

        private bool UpdateScrollItems () {
            if (itemCount <= 0)
                return false;

            float topPosValue = isVertical ? content.anchoredPosition.y : -content.anchoredPosition.x;
            float bottomPosValue = topPosValue + ViewportSize;

            int highestIndex = int.MaxValue;
            int lowestIndex = int.MinValue;

            float highestVal = float.MinValue;
            float lowestVal = float.MaxValue;

            for (int i = visibleObjects.Count - 1; i >= 0; i--) {
                ScrollItemInfo listItemInfo = visibleObjects[i];
                RectTransform childRectT = listItemInfo.obj.transform as RectTransform;

                float val1 = isVertical ? -1 * childRectT.anchoredPosition.y : childRectT.anchoredPosition.x;
                float val2 = isVertical ? childRectT.rect.height : childRectT.rect.width;

                if (val1 < lowestVal) {
                    lowestVal = val1;
                    lowestIndex = listItemInfo.index;
                } else if (val1 == lowestVal && lowestIndex > listItemInfo.index) {
                    lowestIndex = listItemInfo.index;
                }

                if (val1 + val2 > highestVal) {
                    highestVal = val1 + val2;
                    highestIndex = listItemInfo.index;
                } else if (val1 + val2 == highestVal && highestIndex < listItemInfo.index) {
                    highestIndex = listItemInfo.index;
                }

                if (val1 + val2 <= topPosValue || val1 >= bottomPosValue) {
                    ReturnPooledObj (listItemInfo);
                }
            }

            if (lowestVal >= bottomPosValue) {
                float gapSize = lowestVal - topPosValue;
                int numItemsInGap = Mathf.CeilToInt (gapSize / scrollInfo.CurrentAvgSize);

                int gotoIndex = lowestIndex - numItemsInGap;

                if (gotoIndex < 0) {
                    gotoIndex = 0;
                }

                GoToScrollItem (gotoIndex);

                return true;
            }

            if (lowestVal - scrollRectContent.spacing > topPosValue && lowestIndex > 0) {
                GenerateScrollItem (lowestIndex - 1, isVertical ? -lowestVal : lowestVal, true);

                return true;
            }

            if (highestVal <= topPosValue) {
                float gapSize = topPosValue - highestVal;
                int numItemsInGap = Mathf.CeilToInt (gapSize / scrollInfo.CurrentAvgSize);

                int gotoIndex = highestIndex + numItemsInGap;

                if (gotoIndex >= itemCount) {
                    gotoIndex = itemCount - 1;
                }

                GoToScrollItem (gotoIndex);

                return true;
            }

            if (highestVal + scrollRectContent.spacing < bottomPosValue && highestIndex < itemCount - 1) {
                GenerateScrollItem (highestIndex + 1, isVertical ? -highestVal : highestVal, false);

                return true;
            }

            return false;
        }

        private void PositionScrollItem (ScrollItemInfo listItemInfo, float listPos, bool addedItemAtBeginning) {
            RectTransform objRectT = listItemInfo.obj.transform as RectTransform;
            float objSize = isVertical ? objRectT.rect.height : objRectT.rect.width;

            float spacingOffset = 0;

            if (addedItemAtBeginning) {
                spacingOffset = isVertical ? scrollRectContent.spacing : -scrollRectContent.spacing;
            } else if (listItemInfo.index != 0) {
                spacingOffset = isVertical ? -scrollRectContent.spacing : scrollRectContent.spacing;
            }

            float sizeOffset = (addedItemAtBeginning ? objSize : 0);
            float pos = isVertical ? listPos + spacingOffset + sizeOffset : listPos + spacingOffset - sizeOffset;

            SetAnchoredPosition (objRectT, pos);

            if (!cachedSizes.ContainsKey (listItemInfo.index) || objSize != cachedSizes[listItemInfo.index]) {
                float defaultSize = scrollInfo.usedSavedSize ? scrollInfo.SavedAvgSize : 0;
                float sizeDifference = objSize - (!cachedSizes.ContainsKey (listItemInfo.index) ? defaultSize : cachedSizes[listItemInfo.index]);
                Vector2 contentsNewSize = new Vector2 (content.sizeDelta.x, content.sizeDelta.y);
                bool includeSpacing = (!cachedSizes.ContainsKey (listItemInfo.index) && listItemInfo.index != itemCount - 1);

                if (isVertical) {
                    contentsNewSize.y += sizeDifference + (includeSpacing ? scrollRectContent.spacing : 0);
                } else {
                    contentsNewSize.x += sizeDifference + (includeSpacing ? scrollRectContent.spacing : 0);
                }

                content.sizeDelta = contentsNewSize;

                if (addedItemAtBeginning) {
                    sizeDifference += (!cachedSizes.ContainsKey (listItemInfo.index) && listItemInfo.index != 0) ? scrollRectContent.spacing : 0;
                    sizeDifference *= isVertical ? 1 : -1;
                    ShiftContentBy (sizeDifference);
                }

                scrollInfo.currentTotalSize -= !cachedSizes.ContainsKey (listItemInfo.index) ? 0 : cachedSizes[listItemInfo.index];
                scrollInfo.currentTotalSize += objSize;
                scrollInfo.currentCount += !cachedSizes.ContainsKey (listItemInfo.index) ? 1 : 0;

                cachedSizes[listItemInfo.index] = objSize;
            }
        }

        private void ShiftContentBy (float amount) {
            EndDrag ();

            ShiftAnchoredPosition (content, amount);

            for (int i = 0; i < visibleObjects.Count; i++) {
                ShiftAnchoredPosition (visibleObjects[i].obj.transform as RectTransform, -amount);
            }

            BeginDrag ();
        }

        private float GetScrollItemPosition (int itemIndex, out float size) {
            int lowestIndex = int.MaxValue;
            int highestIndex = int.MinValue;

            float lowestPos = 0;
            float highestPos = 0;

            for (int i = 0; i < visibleObjects.Count; i++) {
                RectTransform objRectT = visibleObjects[i].obj.transform as RectTransform;
                float objPos = (isVertical ? -objRectT.anchoredPosition.y : objRectT.anchoredPosition.x);

                if (itemIndex == visibleObjects[i].index) {
                    size = isVertical ? objRectT.rect.height : objRectT.rect.width;
                    return objPos;
                }

                if (lowestIndex > visibleObjects[i].index) {
                    lowestIndex = visibleObjects[i].index;
                    lowestPos = objPos;
                }

                if (highestIndex < visibleObjects[i].index) {
                    highestIndex = visibleObjects[i].index;
                    highestPos = objPos;
                }
            }

            size = 0;

            if (itemIndex < lowestIndex) {
                return lowestPos - scrollInfo.CurrentAvgSize * (lowestIndex - itemIndex);
            } else if (itemIndex > highestIndex) {
                return highestPos + scrollInfo.CurrentAvgSize * (itemIndex - highestIndex);
            }

            return 0;
        }

        private ScrollItemInfo GetPooledObj (int itemType) {
            lock (rObjLock) {
                ScrollItemInfo scrollItemInfo = null;

                for (int i = 0; i < reuseableObjects.Count; i++) {
                    if (itemType == reuseableObjects[i].itemType) {
                        scrollItemInfo = reuseableObjects[i];
                        reuseableObjects.RemoveAt (i);
                        break;
                    }
                }

                if (scrollItemInfo == null) {
                    scrollItemInfo = new ScrollItemInfo ();
                    scrollItemInfo.itemType = itemType;
                }

                return scrollItemInfo;
            }
        }

        private void ReturnPooledObj (ScrollItemInfo scrollItemInfo) {
            lock (rObjLock) {
                scrollItemInfo.obj.SetActive (false);
                scrollItemInfo.obj.transform.SetParent (pooledObjContainer.transform, false);
                visibleObjects.Remove (scrollItemInfo);
                reuseableObjects.Add (scrollItemInfo);
            }
        }

        private static System.Object rObjLock = new System.Object ();

        private void DestroyAllPoolObjects () {
            for (int i = 0; i < visibleObjects.Count; i++) {
                GameObject.Destroy (visibleObjects[i].obj);
            }

            for (int i = 0; i < reuseableObjects.Count; i++) {
                GameObject.Destroy (reuseableObjects[i].obj);
            }

            visibleObjects.Clear ();
            reuseableObjects.Clear ();
        }

        private void ReturnAllToPool () {
            for (int i = visibleObjects.Count - 1; i >= 0; i--) {
                ReturnPooledObj (visibleObjects[i]);
            }
        }

        private void EndDrag () {
            if (lastDragEventData == null) {
                return;
            }

            if (endDragTempCount == 0) {
                base.OnEndDrag (lastDragEventData);
            }

            endDragTempCount++;
        }

        private void BeginDrag () {
            if (lastDragEventData == null || endDragTempCount == 0) {
                return;
            }

            endDragTempCount--;

            if (endDragTempCount == 0) {
                base.OnBeginDrag (lastDragEventData);
            }
        }

        private void ShiftAnchoredPosition (RectTransform rectT, float amount) {
            Vector2 anchoredPosition = new Vector2 ();

            if (isVertical) {
                anchoredPosition.x = rectT.anchoredPosition.x;
                anchoredPosition.y = rectT.anchoredPosition.y + amount;
            } else {
                anchoredPosition.x = rectT.anchoredPosition.x + amount;
                anchoredPosition.y = rectT.anchoredPosition.y;
            }

            rectT.anchoredPosition = anchoredPosition;
        }

        private void SetAnchoredPosition (RectTransform rectT, float newPos) {
            Vector2 anchoredPosition = new Vector2 ();

            if (isVertical) {
                anchoredPosition.x = rectT.anchoredPosition.x;
                anchoredPosition.y = newPos;
            } else {
                anchoredPosition.x = newPos;
                anchoredPosition.y = rectT.anchoredPosition.y;
            }

            rectT.anchoredPosition = anchoredPosition;
        }

        private void SetupRectTransform (RectTransform rectT) {
            if (isVertical) {
                rectT.anchorMin = new Vector2 (rectT.anchorMin.x, 1);
                rectT.anchorMax = new Vector2 (rectT.anchorMax.x, 1);
                rectT.pivot = new Vector2 (rectT.pivot.x, 1);
            } else {
                rectT.anchorMin = new Vector2 (0, rectT.anchorMin.y);
                rectT.anchorMax = new Vector2 (0, rectT.anchorMax.y);
                rectT.pivot = new Vector2 (0, rectT.pivot.y);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (onPointerDown != null) {
				onPointerDown.Invoke ();
			}
        }

        public void OnPointerUp (PointerEventData eventData)
        {
            if (onPointerUp != null)
            {
                onPointerUp.Invoke ();
            }
        }

        #endregion

        #region Inner Classes

        private class ScrollItemInfo {
            public int index;
            public int itemType;
            public GameObject obj;
        }

        private class ScrollInfo {
            public float currentTotalSize;
            public int currentCount;
            public float savedTotalSize;
            public int savedCount;
            public bool usedSavedSize;

            public int scrollToIndex;
            public double scrollingStart;
            public float scrollFrom;
            public float scrollTo;
            public float scrollDelta;
            public float scrollOffset;
            public ContentAnchor scrollAnchor;

            public float CurrentAvgSize { get { return currentTotalSize / currentCount; } }

            public float SavedAvgSize { get { return savedTotalSize / savedCount; } }

            public ScrollInfo () {
                Reset ();
                ResetScroll ();
            }

            public void Reset () {
                currentTotalSize = 0;
                currentCount = 0;
                savedTotalSize = 0;
                savedCount = 0;
                usedSavedSize = false;
            }

            public void ResetScroll () {
                scrollToIndex = -1;
                scrollingStart = 0;
                scrollFrom = 0;
                scrollTo = 0;
                scrollDelta = 0;
                scrollOffset = 0;
            }
        }

        #endregion

        public enum ContentAnchor {
            Beginning,
            Middle,
            End
        }

        public enum PullDirection {
            Up,
            Down
        }
    }
}