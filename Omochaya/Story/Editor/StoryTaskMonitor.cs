// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTaskMonitor.cs" company="Omochaya">
//   Copyright (c) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
//   Provides a custom Unity EditorWindow interface to track, filter, sort, and inspect
//   the live status and execution order of automated and manual tasks during runtime.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG

// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
// これ以降は間接的に使用されます。利用者が直接使用することは想定していません
// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
namespace Omochaya.HiddenStory
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;
    using UnityEngine.UIElements.Experimental;

    class TaskMonitorForDebug : EditorWindow
    {
        /// <summary>Opens or focuses the custom Omochaya Task Monitor diagnostic editor window.</summary>
        [MenuItem("Window/Omochaya/Task Monitor")]
        public static void ShowWindow()
        {
            var window = GetWindow<TaskMonitorForDebug>("Task Monitor");
            window.minSize = new Vector2(400, 300);
        }

        // UI要素
        ListView listView;
        Label infoLabel;
        HelpBox playModeHelpBox;
        VisualElement headerContainer;
        VisualElement footer;
        ToolbarSearchField searchField;
        ToolbarButton btnSortOrder;

        // データ管理用
        readonly List<Story.Task> allTasks = new();
        readonly List<Story.Task> filteredTasks = new();
        
        // ★ ドメインリロード等での揮発を防ぐためシリアライズ
        [SerializeField] string searchString = ""; 
        bool autoRefresh = true;
        double lastRefreshTime;
        
        // ソート設定
        bool sortAscending = false; // デフォルトは降順（実行位置が新しいもの上にするため）

        // エディタ保存用のキー
        const string TRACKING_PREF_KEY = "Omochaya.Story.EnableTaskTracking";

        // エディタ起動時 および プレイモード開始時（ドメインリロード後）に自動で呼ばれる
        [InitializeOnLoadMethod]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RestoreTrackingState()
        {
            // エディタに保存された設定を読み込み、ランタイム側の高速な static bool に同期する
            Dev.EnableTaskTracking = EditorPrefs.GetBool(TRACKING_PREF_KEY, false);
        }

        void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            // Playモード開始時（編集モード退出時）に検索状態をクリーンアップ
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                this.searchString = "";
                if (this.searchField != null)
                {
                    this.searchField.SetValueWithoutNotify("");
                }
                
                this.filteredTasks.Clear();
                this.listView?.RefreshItems();
            }
        }

        void CreateGUI()
        {
            var root = this.rootVisualElement;

            // 1. ツールバーの構築
            var toolbar = new Toolbar();
            
            var btnRefresh = new ToolbarButton(() => RefreshData()) { text = Messages.EditorUI.TaskMonitor_Refresh };
            toolbar.Add(btnRefresh);

            var toggleAuto = new ToolbarToggle { text = Messages.EditorUI.TaskMonitor_AutoRefresh, value = this.autoRefresh };
            toggleAuto.RegisterValueChangedCallback(evt => this.autoRefresh = evt.newValue);
            toolbar.Add(toggleAuto);

            // ★ スタックトレース追跡用のトグルを追加
            var toggleTracking = new ToolbarToggle { text = "Enable Tracking", value = Dev.EnableTaskTracking };
            toggleTracking.RegisterValueChangedCallback(evt => 
            {
                // ランタイム用のフラグを更新
                Dev.EnableTaskTracking = evt.newValue;
                // エディタ側に状態を保存（次回以降のプレイモードや再起動で復元される）
                EditorPrefs.SetBool(TRACKING_PREF_KEY, evt.newValue);
            });
            toolbar.Add(toggleTracking);

            // 検索フィールド
            this.searchField = new ToolbarSearchField();
            this.searchField.value = this.searchString; // 生成時に前回の状態を復元（同期ズレ防止）
            this.searchField.RegisterValueChangedCallback(evt => 
            {
                this.searchString = evt.newValue ?? "";
                RefreshData();
            });
            toolbar.Add(this.searchField);

            root.Add(toolbar);

            // 2. プレイモード警告ボックス
            this.playModeHelpBox = new HelpBox(Messages.EditorUI.TaskMonitor_PlayModeOnly, HelpBoxMessageType.Info);
            root.Add(this.playModeHelpBox);

            // ----------------------------------------------------
            // ▼ ここからレイアウト変更：SplitViewの導入 ▼
            // ----------------------------------------------------

            // 画面を上下に分割し、境界線をドラッグ可能にするコンポーネント
            // (インデックス1=下側 を固定枠とし、初期高さを 120 に設定)
            var splitView = new TwoPaneSplitView(1, 120, TwoPaneSplitViewOrientation.Vertical)
            {
                style = { flexGrow = 1 }
            };

            // 【上部ペイン】ヘッダーとListViewを格納
            var topPane = new VisualElement { style = { flexGrow = 1 } };
            
            // 3. ヘッダー（ソート機能を含む）
            this.headerContainer = new VisualElement();
            this.headerContainer.style.flexDirection = FlexDirection.Row;
            this.headerContainer.style.height = 24;
            this.headerContainer.style.flexShrink = 0;
            this.headerContainer.style.borderBottomWidth = 1;
            this.headerContainer.style.borderBottomColor = Color.gray;
            this.headerContainer.style.backgroundColor = new Color(0.25f, 0.25f, 0.25f, 0.3f);
            this.headerContainer.style.alignItems = Align.Center;

            // カラムヘッダーを追加
            this.headerContainer.Add(new Label("Band") { style = { width = 60, paddingLeft = 4, unityFontStyleAndWeight = FontStyle.Bold } });
            this.headerContainer.Add(new Label("Offset") { style = { width = 60, paddingLeft = 4, unityFontStyleAndWeight = FontStyle.Bold } });
            this.headerContainer.Add(new Label("ID [Age]") { style = { width = 80, paddingLeft = 4, unityFontStyleAndWeight = FontStyle.Bold } });
            this.headerContainer.Add(new Label("Link") { style = { width = 60, paddingLeft = 4, unityFontStyleAndWeight = FontStyle.Bold } });
            this.headerContainer.Add(new Label("Task Type") { style = { flexGrow = 1, paddingLeft = 4, unityFontStyleAndWeight = FontStyle.Bold } });
            this.headerContainer.Add(new Label("Owner") { style = { width = 150, paddingLeft = 4, unityFontStyleAndWeight = FontStyle.Bold } });

            var sortContainer = new VisualElement { style = { flexDirection = FlexDirection.Row, alignItems = Align.Center } };
            this.headerContainer.Add(sortContainer);

            this.btnSortOrder = new ToolbarButton(() => 
            {
                this.sortAscending = !this.sortAscending;
                this.btnSortOrder.text = this.sortAscending ? "▲" : "▼";
                RefreshData();
            }) { text = this.sortAscending ? "▲" : "▼", style = { width = 25, unityTextAlign = TextAnchor.MiddleCenter } };
            sortContainer.Add(this.btnSortOrder);

            topPane.Add(this.headerContainer);

            // 4. ListView の初期化と設定
            this.listView = new ListView
            {
                style = { flexGrow = 1 },
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                fixedItemHeight = 20,
                itemsSource = this.filteredTasks
            };

            this.listView.makeItem = () =>
            {
                var row = new VisualElement { style = { flexDirection = FlexDirection.Row } };
                row.Add(new Label { name = "Band", style = { width = 60, paddingLeft = 4, unityTextAlign = TextAnchor.MiddleLeft } });
                row.Add(new Label { name = "Offset", style = { width = 60, paddingLeft = 4, unityTextAlign = TextAnchor.MiddleLeft } });
                row.Add(new Label { name = "ID", style = { width = 80, paddingLeft = 4, unityTextAlign = TextAnchor.MiddleLeft } });
                row.Add(new Label { name = "Link", style = { width = 60, paddingLeft = 4, unityTextAlign = TextAnchor.MiddleLeft } });
                row.Add(new Label { name = "Task", style = { flexGrow = 1, paddingLeft = 4, unityTextAlign = TextAnchor.MiddleLeft } });
                row.Add(new Label { name = "Owner", style = { width = 175, paddingLeft = 4, unityTextAlign = TextAnchor.MiddleLeft } });
                return row;
            };

            this.listView.bindItem = (element, index) =>
            {
                if (index >= 0 && index < this.filteredTasks.Count)
                {
                    var taskStr = this.filteredTasks[index].ToString();
                    var parts = taskStr.Split('|');
                    var offsetText = parts.Length > 1 ? parts[1].Trim() : string.Empty;

                    element.Q<Label>("Band").text = parts.Length > 0 ? parts[0].Trim() : string.Empty;
                    element.Q<Label>("Offset").text = offsetText;
                    element.Q<Label>("ID").text = parts.Length > 2 ? parts[2].Trim() : string.Empty;
                    element.Q<Label>("Link").text = parts.Length > 3 ? parts[3].Trim() : string.Empty;
                    element.Q<Label>("Task").text = parts.Length > 4 ? parts[4].Trim() : string.Empty;
                    element.Q<Label>("Owner").text = parts.Length > 5 ? parts[5].Trim() : string.Empty;

                    var isNumeric = int.TryParse(offsetText, out _);
                    element.style.color = new StyleColor(isNumeric ? Color.white : Color.gray5);
                }
            };

            this.listView.RegisterCallback<ContextClickEvent>(evt =>
            {
                int index = this.listView.selectedIndex;
                if (index >= 0 && index < this.filteredTasks.Count)
                {
                    var task = this.filteredTasks[index];
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(Messages.EditorUI.TaskMonitor_MenuPingOwner), false, () =>
                    {
                        Component owner = null;
                        DevForEditor.TaskMonitorAPI.ExtractOwner(ref owner, task);
                        if (owner != null) EditorGUIUtility.PingObject(owner);
                    });

                    menu.AddItem(new GUIContent(Messages.EditorUI.TaskMonitor_MenuForceFree), false, () =>
                    {
                        task.Stop();
                        RefreshData();
                    });
                    menu.ShowAsContext();
                }
            });

            topPane.Add(this.listView);
            splitView.Add(topPane); // 上部ペインをSplitViewに登録

            // 【下部ペイン】詳細情報を格納
            var bottomPane = new VisualElement { style = { flexGrow = 1 } };

            var detailPane = new ScrollView
            {
                style = { 
                    flexGrow = 1, // 高さを固定せずSplitViewに委ねる
                    borderTopWidth = 1, 
                    borderTopColor = Color.gray, 
                    backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f),
                    paddingLeft = 4, paddingRight = 4, paddingTop = 4, paddingBottom = 4 // コピーボタンがフッタに移動したため paddingTop を調整
                }
            };

            var detailLabel = new Label
            {
                enableRichText = true,
                style = { whiteSpace = WhiteSpace.Normal, color = new Color(0.8f, 0.8f, 0.8f) }
            };
            
            detailLabel.RegisterCallback<PointerDownLinkTagEvent>(evt =>
            {
                if (evt.button == 0)
                {
                    var linkId = evt.linkID;
                    var lastColon = linkId.LastIndexOf(':');
                    if (lastColon >= 0)
                    {
                        var filePath = linkId.Substring(0, lastColon);
                        var lineNumber = int.Parse(linkId.Substring(lastColon + 1));
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(filePath, lineNumber);
                    }
                }
            });

            detailPane.Add(detailLabel);
            bottomPane.Add(detailPane);

            // ★ コピー機能と現在表示中のトレース保持変数
            string currentRawTrace = "";

            var copyButton = new Button(() => 
            {
                if (!string.IsNullOrEmpty(currentRawTrace))
                {
                    // クリップボードに入れる前に <link> や <u> などのタグを正規表現で綺麗に削除する
                    var cleanTrace = System.Text.RegularExpressions.Regex.Replace(currentRawTrace, @"<.*?>", "");
                    EditorGUIUtility.systemCopyBuffer = cleanTrace;
                }
            }) 
            { 
                text = "Copy", 
                style = { position = Position.Absolute, top = 2, right = 4, height = 20, width = 60 } 
            };
            copyButton.SetEnabled(false); // 初期状態は無効

            splitView.Add(bottomPane); // 下部ペインをSplitViewに登録
            root.Add(splitView);       // SplitViewを大元に追加


            // リストの選択が変わった時のイベント
#if UNITY_2022_2_OR_NEWER
            this.listView.selectionChanged += (IEnumerable<object> selection) =>
#else
            this.listView.onSelectionChange += (IEnumerable<object> selection) =>
#endif
            {
                detailLabel.text = "";
                currentRawTrace = "";
                foreach (var item in selection)
                {
                    var task = (Story.Task)item;
                    DevForEditor.TaskMonitorAPI.ExtractCreationTrace(ref currentRawTrace, task);

                    if (string.IsNullOrEmpty(currentRawTrace))
                    {
                        detailLabel.text = "Creation trace is not recorded. Please enable 'Enable Tracking' in the toolbar.";
                    }
                    else
                    {
                        detailLabel.text = currentRawTrace;
                    }
                }

                // コピーボタンの有効/無効を更新
                copyButton.SetEnabled(!string.IsNullOrEmpty(currentRawTrace));
            };

            // 5. フッター（ステータス情報ラベルとコピーボタン）
            this.footer = new VisualElement();
            this.footer.style.height = 24;
            this.footer.style.flexShrink = 0;
            this.footer.style.borderTopWidth = 1;
            this.footer.style.borderTopColor = Color.gray;
            this.footer.style.paddingLeft = 6;
            this.footer.style.justifyContent = Justify.Center;
            this.footer.style.backgroundColor = new Color(0.25f, 0.25f, 0.25f, 0.3f);
            root.Add(this.footer);

            this.infoLabel = new Label { style = { unityFontStyleAndWeight = FontStyle.Bold } };
            this.footer.Add(this.infoLabel);

            // フッターにコピーボタンを追加
            this.footer.Add(copyButton);

            // 初回描画
            RefreshData();
        }

        void Update()
        {
            // UI生成前の実行をガード
            if (this.playModeHelpBox == null || this.listView == null) return;

            var isPlaying = Application.isPlaying;
            
            this.playModeHelpBox.style.display = isPlaying ? DisplayStyle.None : DisplayStyle.Flex;

            if (!isPlaying || EditorApplication.isPaused) return;

            // 0.5秒に1回の自動更新
            if (this.autoRefresh && EditorApplication.timeSinceStartup - this.lastRefreshTime > 0.5f)
            {
                RefreshData();
                this.lastRefreshTime = EditorApplication.timeSinceStartup;
            }
        }

        /// <summary>
        /// データの構築とListViewへの反映
        /// </summary>
        void RefreshData()
        {
            if (!Application.isPlaying || this.infoLabel == null) return;

            // UIと内部変数の同期ズレを確実に防ぐため、毎回UIから値を取得する
            if (this.searchField != null)
            {
                this.searchString = this.searchField.value ?? "";
            }

            var autoCount = 0;
            var manualCount = 0;
            var lateCount = 0;
            var fixedCount = 0;
            DevForEditor.TaskMonitorAPI.FetchAutoCount(ref autoCount);
            DevForEditor.TaskMonitorAPI.FetchManualCount(ref manualCount);
            DevForEditor.TaskMonitorAPI.FetchLateCount(ref lateCount);
            DevForEditor.TaskMonitorAPI.FetchFixedCount(ref fixedCount);
            DevForEditor.TaskMonitorAPI.GetTaskList(this.allTasks);

            var waitCount = this.allTasks.Count - (autoCount + manualCount + lateCount + fixedCount);

            // フッター用文字列の生成
            string sAuto = string.Format(Messages.EditorUI.TaskMonitor_StatAuto, autoCount);
            string sManual = string.Format(Messages.EditorUI.TaskMonitor_StatManual, manualCount);
            string sLate = string.Format(Messages.EditorUI.TaskMonitor_StatLate, lateCount);
            string sFixed = string.Format(Messages.EditorUI.TaskMonitor_StatFixed, fixedCount);
            string sWait = string.Format(Messages.EditorUI.TaskMonitor_StatWait, waitCount);
            string sTotal = string.Format(Messages.EditorUI.TaskMonitor_StatTotal, this.allTasks.Count);

            this.infoLabel.text = $"{sAuto}    |    {sManual}    |    {sLate}    |    {sFixed}    |    {sWait}    |    {sTotal}";

            this.filteredTasks.Clear();
            bool hasSearch = !string.IsNullOrEmpty(this.searchString);

            foreach (var t in this.allTasks)
            {
                if (!hasSearch || t.ToString().IndexOf(this.searchString, System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    this.filteredTasks.Add(t);
                }
            }

            // ゼロアロケーションでのインプレース・ソート
            this.filteredTasks.Sort((a, b) =>
            {
                var result = 0;
                var offsetA = 0L;
                var offsetB = 0L;
                DevForEditor.TaskMonitorAPI.GetOrder(ref offsetA, a);
                DevForEditor.TaskMonitorAPI.GetOrder(ref offsetB, b);
                result = offsetA.CompareTo(offsetB);
                return this.sortAscending ? result : -result;
            });

            this.listView.RefreshItems();
        }
    }
}
#endif
