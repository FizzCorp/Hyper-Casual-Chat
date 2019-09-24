using Fizz.Common;
using Fizz.UI.Components;
using Fizz.UI.Core;
using Fizz.UI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Fizz.UI
{
    public class FizzChannelsView : FizzBaseComponent
    {
        [SerializeField] Text TitleLabel; 
        [SerializeField] FizzChannelView ChannelPrefab;
        [SerializeField] FizzChannelGroupView ChannelGroupPrefab;
        [SerializeField] RectTransform Container;

        public FizzChannelItemSelectedEvent OnChannelSelected;

        public FizzChannel CurrentSelectedChannel { get; private set; }

        private const string DEFAULT_NO_GROUP = "001D3F4U1T_9O_GRP";

        private Dictionary<string, FizzChannelView> _channelsLookup;
        private Dictionary<string, FizzChannelGroupView> _groupsLookup;
        private List<string> _channelWatchList;

        private bool _initialized = false;

        public void SetVisibility (bool visible)
        {
            CanvasGroup cg = gameObject.GetComponent<CanvasGroup> ();
            if (cg != null)
            {
                cg.alpha = visible ? 1 : 0;
            }
        }

        public void AddChannel (string channelId, bool select = false)
        {
            if (!_initialized)
            {
                Initialize ();
            }

            FizzChannel channel = GetChannelById (channelId);

            if (channel == null)
            {
                if (!_channelWatchList.Contains (channelId)) _channelWatchList.Add (channelId);
                FizzLogger.W ("Channel not found, please add channel [" + channelId + "] to FizzService first.");
                return;
            }

            if (!_channelsLookup.ContainsKey (channel.Id))
            {
                AddChannelInternal (channel);
            }

            if (_channelsLookup.ContainsKey (channel.Id) && select)
            {
                HandleChannelSelected (channel);
            }
        }

        public void RemoveChannel (string channelId)
        {
            if (!_initialized)
            {
                Initialize ();
            }

            _channelWatchList.Remove (channelId);

            if (RemoveChannelInternal (channelId) && CurrentSelectedChannel == null && _channelsLookup.Count > 0)
            {
                HandleChannelSelected (_channelsLookup.Values.First ().GetChannel ());
            }
        }

        public bool SetChannel (string channelId)
        {
            if (!_initialized)
            {
                Initialize ();
            }

            FizzChannel channel = GetChannelById (channelId);

            if (channel == null)
                return false;

            if (_channelsLookup.ContainsKey (channel.Id))
            {
                HandleChannelSelected (channel);
                return true;
            }
            else
            {
                FizzLogger.W ("FizzChatView: Unable to set channel, add channel first");
                return false;
            }
        }

        public void Reset ()
        {
            if (!_initialized)
            {
                Initialize ();
            }

            CurrentSelectedChannel = null;

            foreach (KeyValuePair<string, FizzChannelView> pair in _channelsLookup)
            {
                FizzChannelView button = pair.Value;
                if (button != null)
                {
                    Destroy (button.gameObject);
                }
            }

            foreach (KeyValuePair<string, FizzChannelGroupView> pair in _groupsLookup)
            {
                FizzChannelGroupView group = pair.Value;
                if (group != null)
                {
                    Destroy (group.gameObject);
                }
            }

            _channelsLookup.Clear ();
            _groupsLookup.Clear ();
            _channelWatchList.Clear ();
        }

        protected override void OnEnable ()
        {
            base.OnEnable ();

            try
            {
                FizzService.Instance.OnChannelSubscribed += HandleOnChannelSubscribe;
                FizzService.Instance.OnChannelUnsubscribed += HandleOnChannelUnsubscribe;
            }
            catch
            {
                FizzLogger.E ("Unable to call FizzService.");
            }

            SyncViewState ();
        }

        protected override void OnDisable ()
        {
            base.OnDisable ();

            try
            {
                FizzService.Instance.OnChannelSubscribed -= HandleOnChannelSubscribe;
                FizzService.Instance.OnChannelUnsubscribed -= HandleOnChannelUnsubscribe;
            }
            catch
            {
                FizzLogger.E ("Unable to call FizzService.");
            }
        }

        private void Awake ()
        {
            if (!_initialized)
                Initialize ();
        }

        private void Initialize ()
        {
            if (_initialized) return;

            TitleLabel.SetLocalizedText ("Channels_Title");

            _channelsLookup = new Dictionary<string, FizzChannelView> ();
            _groupsLookup = new Dictionary<string, FizzChannelGroupView> ();

            _channelWatchList = new List<string> ();

            _initialized = true;
        }

        private void SyncViewState ()
        {
            try
            {
                foreach (FizzChannel channel in FizzService.Instance.Channels)
                {
                    if (_channelWatchList.Contains (channel.Id) && !_channelsLookup.ContainsKey (channel.Id))
                    {
                        AddChannelInternal (channel);
                        _channelWatchList.Remove (channel.Id);
                    }
                }

                foreach (string channelId in _channelsLookup.Keys)
                {
                    if (FizzService.Instance.GetChannel (channelId) == null)
                    {
                        RemoveChannelInternal (channelId);
                    }
                }
            }
            catch
            {
                FizzLogger.E ("Something went wrong while calling Channels of FizzService.");
            }
        }

        private void HandleOnChannelSubscribe (string channelId)
        {
            if (string.IsNullOrEmpty (channelId))
                return;

            if (_channelsLookup.ContainsKey (channelId))
                return;

            if (_channelWatchList.Contains (channelId))
            {
                AddChannelInternal (GetChannelById (channelId));
                _channelWatchList.Remove (channelId);
            }
        }

        private void HandleOnChannelUnsubscribe (string channelId)
        {
            if (string.IsNullOrEmpty (channelId))
                return;

            if (!_channelsLookup.ContainsKey (channelId))
                return;

            RemoveChannelInternal (channelId);
        }

        private void HandleChannelSelected (FizzChannel data)
        {
            if (CurrentSelectedChannel != null)
            {
                if (CurrentSelectedChannel.Id.Equals (data.Id))
                {
                    if (OnChannelSelected != null)
                        OnChannelSelected.Invoke (data);
                    return;
                }

                FizzChannelView currentButton = _channelsLookup[CurrentSelectedChannel.Id];
                currentButton.SetSelected (false);
            }

            CurrentSelectedChannel = data;
            if (OnChannelSelected != null)
                OnChannelSelected.Invoke (data);

            FizzChannelView barButton = _channelsLookup[data.Id];
            barButton.SetSelected (true);
        }

        private bool AddChannelInternal (FizzChannel _item)
        {
            if (_item == null) return false;

            bool _added = false;
            if (!_channelsLookup.ContainsKey (_item.Id))
            {
                FizzChannelView _button = Instantiate (ChannelPrefab);
                _button.gameObject.SetActive (true);
                _button.transform.SetParent (GetChannelGroup (_item).transform, false);
                _button.transform.SetAsLastSibling ();
                _button.transform.localScale = Vector3.one;
                _button.SetData (_item, HandleChannelSelected);
                _channelsLookup.Add (_item.Id, _button);
                _button.gameObject.name = _item.Name;

                if (!string.IsNullOrEmpty (_item.Meta.Group) && _groupsLookup.ContainsKey (_item.Meta.Group))
                {
                    _groupsLookup[_item.Meta.Group].ChannelCount++;
                }

                _added = true;
            }
            return _added;
        }

        private bool RemoveChannelInternal (string channelId)
        {
            bool _removed = false;
            if (!string.IsNullOrEmpty (channelId) && _channelsLookup.ContainsKey (channelId))
            {
                if (CurrentSelectedChannel != null && CurrentSelectedChannel.Id.Equals (channelId))
                {
                    CurrentSelectedChannel = null;
                }

                FizzChannel channel = _channelsLookup[channelId].GetChannel ();
                if (channel != null && !string.IsNullOrEmpty (channel.Meta.Group) && _groupsLookup.ContainsKey (channel.Meta.Group))
                {
                    _groupsLookup[channel.Meta.Group].ChannelCount--;
                }

                Destroy (_channelsLookup[channelId].gameObject);
                _channelsLookup.Remove (channelId);
                _removed = true;

            }

            if (_removed)
            {
                RemoveChannelGroup (channelId);
            }

            return _removed;
        }

        private RectTransform GetChannelGroup (FizzChannel channel)
        {
            string channelGroup = (string.IsNullOrEmpty (channel.Meta.Group) ? DEFAULT_NO_GROUP : channel.Meta.Group);
            
            RectTransform parentRect = Container;
            if (_groupsLookup.ContainsKey (channelGroup))
            {
                parentRect = _groupsLookup[channelGroup].RectTransform;
            }
            else
            {
                parentRect = AddChannelGroup (channel, channelGroup);
            }
            return parentRect;
        }

        private RectTransform AddChannelGroup (FizzChannel channel, string channelGroup)
        {
            RectTransform groupRect = null;
            if (!_groupsLookup.ContainsKey (channelGroup))
            {
                FizzChannelGroupView channelGroupView = Instantiate (ChannelGroupPrefab);
                channelGroupView.gameObject.SetActive (true);
                channelGroupView.transform.SetParent (Container.transform, false);
                channelGroupView.transform.localScale = Vector3.one;
                channelGroupView.SetData (channel.Meta.Group);
                _groupsLookup.Add (channelGroup, channelGroupView);
                groupRect = channelGroupView.RectTransform;
                channelGroupView.gameObject.name = channelGroup;
            }
            return groupRect;
        }

        private void RemoveChannelGroup (string channelId)
        {
            string groupToRemove = string.Empty;
            foreach (KeyValuePair<string, FizzChannelGroupView> pair in _groupsLookup)
            {
                if (pair.Value.ChannelCount < 1)
                {
                    groupToRemove = pair.Key;
                    break;
                }
            }

            if (!string.IsNullOrEmpty (groupToRemove))
            {
                Destroy (_groupsLookup[groupToRemove].gameObject);
                _groupsLookup.Remove (groupToRemove);
            }
        }

        private

        FizzChannel GetChannelById (string channelId)
        {
            try
            {
                return FizzService.Instance.GetChannel (channelId);
            }
            catch (Exception)
            {
                FizzLogger.W ("ChannelList unable to get channel with id " + channelId);
            }
            return null;
        }

        [Serializable]
        public class FizzChannelItemSelectedEvent : UnityEvent<FizzChannel>
        { }
    }
}