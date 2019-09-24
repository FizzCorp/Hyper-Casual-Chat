//
//  TextWithEmoji.cs
//
//  Copyright (c) 2016 Fizz Inc
//
using System.Collections.Generic;
using Fizz.UI.Extentions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Fizz.UI.Components
{
    public class TextWithEmoji : Text {
        [FormerlySerializedAs ("config")]
        [SerializeField]
        EmojiConfig m_Config;
        EmojiConfig config {
            get {
                return m_Config;
            }
            set {
                m_Config = value;
                if (shouldEmojilize) {
                    CreateEmojiCanvasRenderer ();
                }
            }
        }

        public bool showRawText = false;

        struct PosEmojiTuple {
            public int pos;
            public int emoji;
            public PosEmojiTuple (int p, int s) {
                this.pos = p;
                this.emoji = s;
            }
        }
        List<PosEmojiTuple> emojiReplacements = new List<PosEmojiTuple> ();

        public string rawText {
            get {
                return base.text;
            }
        }

        public override string text {
            get {
                return base.text;// UpdateReplacements (base.text);
            }
            set {
                base.text = UpdateReplacements (value);
                // if (base.text != value) {
                //     base.text = value;
                // }
            }
        }

        public float characterBaseline = 0;
        public float emojiBaseline = 0;

        CanvasRenderer emojiCanvasRenderer;

        bool shouldEmojilize {
            get {
                return config != null && !showRawText;
            }
        }

        void CreateEmojiCanvasRenderer () {
            if (shouldEmojilize && emojiCanvasRenderer == null) {
                var trans = transform.Find ("__emoji");
                if (trans != null) {
                    emojiCanvasRenderer = trans.GetComponent<CanvasRenderer> ();
                    trans.gameObject.hideFlags = HideFlags.HideAndDontSave;
                }
                if (emojiCanvasRenderer == null) {
                    var go = new GameObject ("__emoji");
                    emojiCanvasRenderer = go.AddComponent<CanvasRenderer> ();
                    emojiCanvasRenderer.hideFlags = HideFlags.HideAndDontSave;
                    go.hideFlags = HideFlags.HideAndDontSave;
                    go.transform.SetParent (transform, false);
                }

                supportRichText = true;
            }
        }

        protected override void OnDestroy () {
            if (emojiCanvasRenderer != null) {
                if (Application.isPlaying)
                    Destroy (emojiCanvasRenderer.gameObject);
                else
                    DestroyImmediate (emojiCanvasRenderer.gameObject);
                emojiCanvasRenderer = null;
            }
            base.OnDestroy ();
        }

        protected override void OnEnable () {
            base.OnEnable ();
            CreateEmojiCanvasRenderer ();
        }

        protected override void OnDisable () {
            base.OnDisable ();
            if (emojiCanvasRenderer != null)
                emojiCanvasRenderer.Clear ();
        }

        string UpdateReplacements (string inputString) {
            if (!shouldEmojilize)
                return inputString;

            inputString = UpdateEmojiReplacements (inputString);
            return inputString;
        }

        void InsertPosEmojiTuples (int index, int count) {
            if (count != 0) {
                for (int i = 0; i < emojiReplacements.Count; ++i) {
                    var e = emojiReplacements[i];
                    if (index <= e.pos) {
                        e.pos += count;
                        emojiReplacements[i] = e;
                        for (int j = i + 1; j < emojiReplacements.Count; ++j) {
                            e = emojiReplacements[j];
                            e.pos += count;
                            emojiReplacements[j] = e;
                        }
                        return;
                    }
                }
            }
        }

        void UpdatePosEmojiTuples (int index, int count) {
            if (count != 0) {
                for (int i = 0; i < emojiReplacements.Count; ++i) {
                    var e = emojiReplacements[i];
                    if (index < e.pos) {
                        e.pos += count;
                        emojiReplacements[i] = e;
                        for (int j = i + 1; j < emojiReplacements.Count; ++j) {
                            e = emojiReplacements[j];
                            e.pos += count;
                            emojiReplacements[j] = e;
                        }
                        return;
                    }
                }
            }
        }

        public static void UpdateEmojiReplacements (string inputString, EmojiConfig config, System.Action<string, int> onEmojiChar) {
            if (!string.IsNullOrEmpty (inputString)) {

                inputString = EmojiParser.ParseEmoji (inputString);

                int i = 0;
                while (i < inputString.Length) {
                    string singleChar = inputString.Substring (i, 1);

                    if (singleChar.Equals ("[")) {
                        int endIndex = inputString.IndexOf ("]", i);
                        if (endIndex > i) {
                            string emojiCode = inputString.Substring (i, endIndex - i + 1);
                            int emojiIndex;
                            if (config.map.TryGetValue (emojiCode.Substring (1, emojiCode.Length - 2), out emojiIndex)) {
                                onEmojiChar (emojiCode, emojiIndex);
                                i += emojiCode.Length;
                            } else {
                                onEmojiChar (singleChar, -1);
                                i++;
                            }
                        } else {
                            onEmojiChar (singleChar, -1);
                            i++;
                        }

                    } else {
                        onEmojiChar (singleChar, -1);
                        i++;
                    }
                }
            }
        }
        const char EMSPACE_CHARACTER = '\u2001';

        string UpdateEmojiReplacements (string inputString) {
            emojiReplacements.Clear ();

            var sb = new System.Text.StringBuilder ();

            UpdateEmojiReplacements (
                inputString, config,
                (emojiChar, emojiIndex) => {
                    if (emojiIndex != -1) {
                        sb.Append ("@");
                        emojiReplacements.Add (new PosEmojiTuple (sb.Length - 1, emojiIndex));
                    } else {
                        sb.Append (emojiChar);
                    }
                });
            return sb.ToString ();
        }

        readonly UIVertex[] tempVerts = new UIVertex[4];

        readonly static VertexHelper emojiVh = new VertexHelper ();

        static Mesh emojiWorkMesh_;
        static Mesh emojiWorkMesh {
            get {
                if (emojiWorkMesh_ == null) {
                    emojiWorkMesh_ = new Mesh ();
                    emojiWorkMesh_.name = "Shared Emoji Mesh";
                    emojiWorkMesh_.hideFlags = HideFlags.HideAndDontSave;
                }
                return emojiWorkMesh_;
            }
        }

        protected override void OnPopulateMesh (VertexHelper toFill) {
            base.OnPopulateMesh (toFill);
            if (shouldEmojilize) {
                UIVertex tempVert = new UIVertex ();

                if (characterBaseline != 0f) {
                    for (int i = 0; i < toFill.currentVertCount; ++i) {
                        toFill.PopulateUIVertex (ref tempVert, i);
                        tempVert.position = new Vector3 (tempVert.position.x, tempVert.position.y + characterBaseline, tempVert.position.z);
                        toFill.SetUIVertex (tempVert, i);
                    }
                }

                emojiVh.Clear ();
                for (int i = 0; i < emojiReplacements.Count; ++i) {
                    var r = emojiReplacements[i];
                    
                    var emojiPosInString = r.pos;
                    var emojiRect = config.rects[r.emoji];

                    int baseIndex = emojiPosInString * 4;
                    if (baseIndex <= toFill.currentVertCount - 4) {
                        for (int j = 0; j < 4; ++j) {
                            toFill.PopulateUIVertex (ref tempVert, baseIndex + j);
                            tempVert.position = new Vector3 (tempVert.position.x, tempVert.position.y + emojiBaseline, tempVert.position.z);
                            tempVerts[j] = tempVert;
                            tempVert.color = Color.clear;
                            toFill.SetUIVertex (tempVert, baseIndex + j);
                        }
                        tempVerts[0].color = Color.white;
                        tempVerts[0].uv0 = new Vector2 (emojiRect.x, emojiRect.yMax);
                        tempVerts[1].color = Color.white;
                        tempVerts[1].uv0 = new Vector2 (emojiRect.xMax, emojiRect.yMax);
                        tempVerts[2].color = Color.white;
                        tempVerts[2].uv0 = new Vector2 (emojiRect.xMax, emojiRect.y);
                        tempVerts[3].color = Color.white;
                        tempVerts[3].uv0 = new Vector2 (emojiRect.x, emojiRect.y);
                        emojiVh.AddUIVertexQuad (tempVerts);
                    }
                }
            }
        }

        protected override void UpdateGeometry () {
            base.UpdateGeometry ();
            if (shouldEmojilize) {
                CreateEmojiCanvasRenderer ();

                emojiVh.FillMesh (emojiWorkMesh);
                emojiCanvasRenderer.SetMesh (emojiWorkMesh);
            }
        }
        readonly static List<Component> components = new List<Component> ();
        Material GetModifiedEmojiMaterial (Material baseMaterial) {
            GetComponents (typeof (IMaterialModifier), components);
            var currentMat = baseMaterial;
            for (var i = 0; i < components.Count; i++)
                currentMat = (components[i] as IMaterialModifier).GetModifiedMaterial (currentMat);
            components.Clear ();
            return currentMat;
        }

        protected override void UpdateMaterial () {
            base.UpdateMaterial ();
            if (shouldEmojilize) {
                if (IsActive ()) {
                    CreateEmojiCanvasRenderer ();

                    emojiCanvasRenderer.materialCount = 1;
                    if (config.material != null)
                        emojiCanvasRenderer.SetMaterial (GetModifiedEmojiMaterial (config.material), 0);
                    else
                        emojiCanvasRenderer.SetMaterial (materialForRendering, 0);
                    emojiCanvasRenderer.SetTexture (config.texture);
                }
            } else {
                if (emojiCanvasRenderer != null) {
                    emojiCanvasRenderer.Clear ();
                }
            }
        }
    }
}